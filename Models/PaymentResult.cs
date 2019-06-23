using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models
{
    public enum PaymentStatus
    {
        Success, Redirected, Failed
    }

    public class PaymentResult
    {
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
