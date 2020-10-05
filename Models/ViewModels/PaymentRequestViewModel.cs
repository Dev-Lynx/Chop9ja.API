using Chop9ja.API.Models.Entities;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class UserPaymentRequestViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }

        public DateTime Created { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }

        [Sieve(CanFilter = true)]
        public virtual TransactionType TransactionType { get; set; }
        [Sieve(CanFilter = true)]
        public RequestStatus Status { get; set; }

        public PaymentChannelViewModel PaymentChannel { get; set; }

        public BankAccountViewModel UserBankAccount { get; set; }

        public string Description { get; set; }
    }

    public class UserDepositPaymentRequestViewModel : UserPaymentRequestViewModel
    {
        //public override TransactionType TransactionType { get; set; }
        public BankAccountViewModel PlatformBankAccount { get; set; }
    }

    public class PaymentRequestUpdateViewModel
    {
        public Guid Id { get; set; }
        public RequestStatus Status { get; set; }
    }
}
