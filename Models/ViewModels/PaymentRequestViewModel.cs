using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels
{
    public class UserPaymentRequestViewModel
    {
        public DateTime Created { get; set; }
        public decimal Amount { get; set; }

        public virtual TransactionType TransactionType { get; set; }
        public RequestStatus Status { get; set; }

        public PaymentChannelViewModel PaymentChannel { get; set; }

        public BankAccountViewModel UserBankAccount { get; set; }
    }

    public class UserDepositPaymentRequestViewModel : UserPaymentRequestViewModel
    {
        public override TransactionType TransactionType { get => TransactionType.Deposit; set { } }
        public BankAccountViewModel PlatformBankAccount { get; set; }
    }

    public class PaymentRequestUpdateViewModel
    {
        public Guid Id { get; set; }
        public RequestStatus Status { get; set; }
    }
}
