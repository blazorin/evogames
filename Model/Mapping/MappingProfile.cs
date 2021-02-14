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
                .ForMember(u => u.IsAdmin, opt => opt.Ignore())
                .ForMember(u => u.UserId, opt => opt.Ignore());

            // Bet Mapping
            CreateMap<Bet, BetDto>();
            CreateMap<Bet, AdminUserDto>();

            // Transaction Mapping
            CreateMap<Transaction, TransactionDto>();
            CreateMap<Transaction, AdminTransactionDto>();
        }
    }
}