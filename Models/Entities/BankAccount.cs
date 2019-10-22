using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Entities
{
    [Owned]
    public class BankAccount : Document
    {
        public int BankId { get; set; } = -1;

        Bank _bank;
        [BsonIgnore]
        public Bank Bank
        {
            get
            {
                if (_bank == null || _bank.Id < 0)
                    _bank = Core.DataContext.Store.GetById<Bank, int>(BankId);
                return _bank;
            }
            set
            {
                if (value != null)
                    BankId = value.Id;
                _bank = value;
            }
        }

        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; }
    }
}


namespace Chop9ja.API.Models.ViewModels
{
    public class BankAccountIdViewModel
    {
        public string Id { get; set; }
    }

    public class BankAccountViewModel
    {
        /// <summary>
        /// ID of the bank on the list. 
        /// List can be found at (https://github.com/tomiiide/nigerian-banks/blob/master/banks.json).
        /// </summary>
        [Required]
        public int BankId { get; set; }
        
        /// <summary>
        /// Registered Name of the bank account
        /// </summary>
        [Required]
        public string AccountName { get; set; }

        /// <summary>
        /// Bank Account Number
        /// </summary>
        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "Only numbers are allowed")]
        public string AccountNumber { get; set; }
    }

    public class UserDefaultBankAccountViewModel
    {
        /// <summary>
        /// Identity for the bank account. It should be stored for easy 
        /// account referencing.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates that the current bank account is most preffered by the user.
        /// </summary>
        [Required]
        public bool IsDefault { get; set; }
    }

    public class UserBankAccountViewModel : BankAccountViewModel
    { 
        /// <summary>
        /// Identity for the bank account. It should be stored for easy 
        /// account referencing.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates that the current bank account is most preffered by the user.
        /// </summary>
        [Required]
        public bool IsDefault { get; set; }
    }


}