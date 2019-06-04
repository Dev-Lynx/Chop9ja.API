using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    public class CreditCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Number { get; set; }
        public string VerificationCode { get; set; }
        public DateTime Expiration { get; set; }
        public User User { get; set; }
    }
}
