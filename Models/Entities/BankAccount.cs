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
    }
}


namespace Chop9ja.API.Models.ViewModels
{
    public class NewBankAccountViewModel
    {
        public int BankId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }

    public class BankAccountViewModel : NewBankAccountViewModel
    {
        public Guid Id { get; set; }
    }
}