using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.Mappers.MapingExtensions
{
    public static class MappingExtensions
    {

        public static User ToEntity(this AccountRegisterDto model)
        {
            return model.MapTo<AccountRegisterDto, User>();
        }
        public static User ToEntity(this ChangePasswordDto model)
        {
            return model.MapTo<ChangePasswordDto, User>();
        }
        public static User ToEntity(this UpdateAccountDto model)
        {
            return model.MapTo<UpdateAccountDto, User>();
        }
    }
}
