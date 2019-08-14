using Chop9ja.API.Models;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Services.Interfaces
{
    public interface IPaymentService
    {
        PaystackOptions PaystackOptions { get; }

        Task<PaymentResult> UsePaystack(User user, decimal amount);
        Task<bool> VerifyPaystack(User user, Transaction transaction);
        Task<bool> ValidatePaystackSignature(string signature, string request);
        Task<bool> ConcludePaystack(PaystackTransactionData data);

        Task<PaymentResult> UseBank(User user, decimal amount, BankAccount userAccount, BankAccount platformAccount);
        Task BankWithdrawal(User user, decimal amount, BankAccount account);

        Task<bool> CreateTransaction(Wallet wallet, decimal amount, TransactionType type, ChannelType channelType);
    }
}
