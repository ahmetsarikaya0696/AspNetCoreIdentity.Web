using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = "DublicateUserName", Description = $"{userName} kullanıcı adı daha önce bir kullanıcı tarafından alınmıştır" }
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = "DublicateEmail", Description = $"{email} e-posta adresi daha önce bir kullanıcı tarafından alınmıştır" }
        }

        public override IdentityError PasswordTooShort(int length)
        {

            return new IdentityError { Code = "PasswordTooShort", Description = "Şifre en az 6 karakter olmalıdır!" };
        }
    }
}
