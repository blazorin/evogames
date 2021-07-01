using AutoMapper;
using Model.Data;
using Shared.Dto;

namespace Model.Mapping
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// CreateMap
        /// </summary>
        public MappingProfile()
        {
            // User Mapping
            CreateMap<User, UserProfileDto>();
            CreateMap<User, UserDto>();
            CreateMap<User, AdminUserDto>();
            CreateMap<NewUserDto, User>()
                .ForMember(u => u.PasswordHashed, opt => opt.Ignore())
                .ForMember(u => u.IsOwner, opt => opt.Ignore())
                .ForMember(u => u.IsDeveloper, opt => opt.Ignore())
                .ForMember(u => u.IsAdmin, opt => opt.Ignore())
                .ForMember(u => u.IsModerator, opt => opt.Ignore())
                .ForMember(u => u.UserId, opt => opt.Ignore())
                .ForMember(u => u.Bets, opt => opt.Ignore())
                .ForMember(u => u.Balance, opt => opt.Ignore())
                .ForMember(u => u.Logs, opt => opt.Ignore())
                .ForMember(u => u.Perms, opt => opt.Ignore())
                .ForMember(u => u.Transactions, opt => opt.Ignore())
                .ForMember(u => u.UnconfirmedDeposits, opt => opt.Ignore())
                .ForMember(u => u.UnconfirmedWithdraws, opt => opt.Ignore())
                ;

            // Bet Mapping
            CreateMap<Bet, BetDto>();
            CreateMap<Bet, AdminUserDto>();

            // Transaction Mapping
            CreateMap<Transaction, TransactionDto>();
            CreateMap<Transaction, AdminTransactionDto>();
        }
    }
}