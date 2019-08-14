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

            CreateMap<User, UserViewModel>().ForMember(u => u.Username, opt => opt.MapFrom(o => o.UserName));
            CreateMap<User, UserTokenViewModel>().ForMember(u => u.Username, opt => opt.MapFrom(o => o.UserName));
            CreateMap<User, UserOneTimePasswordModel>().ForMember(u => u.Username, opt => opt.MapFrom(o => o.UserName));
            CreateMap<UserViewModel, User>().ForMember(u => u.UserName, opt => opt.MapFrom(o => o.Username));


            CreateMap<PaymentChannel, PaymentChannelViewModel>().
                ForMember(c => c.PaymentRange, opt => opt.MapFrom(o => o.PaymentRange.ToString()));

            CreateMap<Transaction, TransactionViewModel>();
            CreateMap<Wallet, WalletViewModel>();

            CreateMap<BankAccountViewModel, BankAccount>();
            CreateMap<BankAccount, UserBankAccountViewModel>();

            CreateMap<BetViewModel, Bet>();
            CreateMap<Bet, BetViewModel>()
                .ForMember(s => s.Date, opt => opt.MapFrom(b => b.AddedAt))
                .ForMember(s => s.CashedOut, opt => opt.MapFrom(b => b.CashOutRequested));

            CreateMap<Bet, StagedBetViewModel>()
                .ForMember(s => s.Status, opt => opt.MapFrom(b => b.CashOutStatus))
                .ForMember(s => s.Date, opt => opt.MapFrom(b => b.CashedOutOn))
                .ForMember(s => s.CashedOut, opt => opt.MapFrom(b => b.CashOutRequested));

            CreateMap<Bet, BackOfficeClaimViewModel>()
                .ForMember(s => s.Status, opt => opt.MapFrom(b => b.CashOutStatus))
                .ForMember(s => s.Date, opt => opt.MapFrom(b => b.CashedOutOn))
                .ForMember(s => s.CashedOut, opt => opt.MapFrom(b => b.CashOutRequested));

            CreateMap<DepositPaymentRequest, UserDepositPaymentRequestViewModel>();
            CreateMap<PaymentRequest, UserPaymentRequestViewModel>()
                .ForMember(p => p.Created, opt => opt.MapFrom(r => r.AddedAtUtc.ToLongDateString()))
                .Include<DepositPaymentRequest, UserDepositPaymentRequestViewModel>();
        }
    }
}
