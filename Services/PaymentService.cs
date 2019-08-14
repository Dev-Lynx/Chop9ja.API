using Chop9ja.API.Data;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.Responses;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chop9ja.API.Services
{
    [AutoBuild]
    public class PaymentService : IPaymentService
    {
        #region Properties

        #region Services

        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IPayStackApi Paystack { get; }

        #endregion

        #region Options
        public PaystackOptions PaystackOptions => PaystackWrapper.Value;
        #endregion

        #region Internals
        [DeepDependency]
        IOptions<PaystackOptions> PaystackWrapper { get; }
        #endregion

        #endregion

        #region Methods

        #region Paystack
        public async Task<PaymentResult> UsePaystack(User user, decimal amount)
        {
            bool customerExists = false;
            try
            {
                customerExists = Paystack.Customers.Fetch(user.Email).Status;
            }
            catch { }

            if (!customerExists)
            {
                Paystack.Customers.Create(new CustomerCreateRequest()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.PhoneNumber
                });
            }

            PaymentResult result = new PaymentResult();

            bool isRecurring = !string.IsNullOrWhiteSpace(user.Wallet.PaystackAuthorization);

            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == ChannelType.Paystack);

            // Convert to kobo
            int total = (int)Math.Round(amount * channel.ConversionRate);
            int fee = (int)Math.Round((total * channel.FeePercentage) + channel.FixedFee);

            // Handle Recurring payments
            if (isRecurring)
            {
                var success = Paystack.Transactions
                    .ChargeAuthorization(user.Wallet.PaystackAuthorization, user.Email, total, user.WalletId.ToString(), false);

                if (success.Status)
                {
                    result.Status = PaymentStatus.Success;
                    return result;
                }
            }

            var request = new TransactionInitializeRequest()
            {
                Email = user.Email,
                AmountInKobo = total,
                Reference = Guid.NewGuid().ToString(),
                MetadataObject = new Dictionary<string, object>()
                {
                    { "walletId", user.WalletId.ToString() }
                }
            };



            
            var response = Paystack.Transactions.Initialize(request);

            result.Status = response.Status ? PaymentStatus.Redirected : PaymentStatus.Failed;
            result.Message = response.Data.AuthorizationUrl;

            if (result.Status == PaymentStatus.Failed)
            {
                Logger.LogError("A Paystack Transaction appears to have failed. \n{0}", response.Message);
                result.Message = response.Message;
            }

            return result;
        }

        public Task<bool> ValidatePaystackSignature(string signature, string request)
        {
            byte[] key = Encoding.UTF8.GetBytes(PaystackOptions.SecretKey);
            byte[] input = Encoding.UTF8.GetBytes(request);
            string paystackSignature = string.Empty;

            using (var hmac = new HMACSHA512(key))
            {
                byte[] hash = hmac.ComputeHash(input);
                paystackSignature = BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            bool valid = paystackSignature.ToLower() == signature.ToLower();
            return Task.FromResult(valid);
        }

        public Task<bool> VerifyPaystack(User user, Transaction transaction)
        {
            var response = Paystack.Transactions.Verify(transaction.Id.ToString());
            if (!response.Status)
                Logger.LogError("An error occured while attempting to verify a transaction. \n{0}", response.Message);

            return Task.FromResult(response.Status);
        }

        public async Task<bool> ConcludePaystack(PaystackTransactionData data)
        {
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == ChannelType.Paystack);

            if (!decimal.TryParse(data.Amount, out decimal transactionAmount)) return false;

            if (!Guid.TryParse((string)data.Metadata["walletId"], out Guid walletId)) return false;
            if (!Guid.TryParse(data.Reference, out Guid transactionId)) return false;


            User platformAccount = DataContext.PlatformAccount;
            Wallet wallet = await DataContext.Store.GetByIdAsync<Wallet>(walletId);

            decimal amount = transactionAmount / channel.ConversionRate;
            await CreateTransaction(wallet, amount, TransactionType.Deposit, ChannelType.Paystack);

            if (data.Authorization.Reusable && string.IsNullOrWhiteSpace(wallet.PaystackAuthorization))
            {
                wallet.PaystackAuthorization = data.Authorization.AuthorizationCode;
                await DataContext.Store.UpdateOneAsync(wallet.User);
            }
            
            return true;
        }
        #endregion

        #region Bank
        public async Task<PaymentResult> UseBank(User user, decimal amount, BankAccount userAccount, BankAccount platformAccount)
        {
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == ChannelType.Bank);

            DepositPaymentRequest request = new DepositPaymentRequest()
            {
                AddedAtUtc = DateTime.UtcNow,
                User = user,
                Amount = amount,
                PaymentChannel = channel,
                Status = RequestStatus.Pending,
                TransactionType = TransactionType.Deposit,
                UserBankAccountId = userAccount.Id,
                PlatformBankAccountId = platformAccount.Id
            };

            await user.AddPaymentRequestAsync(request);

            return new PaymentResult() { Status = PaymentStatus.Pending };
        }

        public async Task BankWithdrawal(User user, decimal amount, BankAccount account)
        {
            PaymentChannel channel = DataContext.Store.GetOne<PaymentChannel>(p => p.Type == ChannelType.Bank);

            PaymentRequest request = new PaymentRequest()
            {
                AddedAtUtc = DateTime.UtcNow,
                User = user,
                Amount = amount,
                PaymentChannel = channel,
                Status = RequestStatus.Pending,
                TransactionType = TransactionType.Withdrawal,
                UserBankAccountId = account.Id
            };

            user.Wallet.AvailableBalance -= amount;

            await user.AddPaymentRequestAsync(request);
        }
        #endregion

        #region Transactions
        public async Task<bool> CreateTransaction(Wallet wallet, decimal amount, TransactionType type, ChannelType channelType)
        {
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel>(p => p.Type == channelType);

            User platformAccount = DataContext.PlatformAccount;

            Transaction transaction = new Transaction()
            {
                AddedAtUtc = DateTime.UtcNow,
                Amount = amount,
                AuxilaryUser = platformAccount,
                PaymentChannel = channel,
                Type = type,
            };

            await wallet.AddTransactionAsync(transaction);
            await platformAccount.Wallet.AddTransactionAsync(new Transaction(transaction) { AuxilaryUser = wallet.User });

            return true;
        }
        #endregion

        #endregion
    }
}
