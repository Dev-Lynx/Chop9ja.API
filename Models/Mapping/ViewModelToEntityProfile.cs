using AutoMapper;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.Mapping
{
    public class ViewModelToEntityProfile : Profile
    {
        public ViewModelToEntityProfile()
        {
            CreateMap<UserRegistrationViewModel, User>();

            CreateMap<OneTimePassword, OneTimePasswordViewModel>().
                ForMember(ov => ov.Expires, opt => opt.MapFrom(o => o.Created.Add(o.LifeSpan)));

            CreateMap<User, UserViewModel>();

            CreateMap<PaymentChannel, PaymentChannelViewModel>().
                ForMember(c => c.PaymentRange, opt => opt.MapFrom(o => o.PaymentRange.ToString()));

            CreateMap<Transaction, TransactionViewModel>();
            CreateMap<Wallet, WalletViewModel>();

            CreateMap<BankAccountViewModel, BankAccount>();
            CreateMap<BankAccount, UserBankAccountViewModel>();
        }
    }
}
