using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }
        
        [Display(Name ="E-posta")]
        public string Email { get; set; }
        
        [Display(Name ="Şifre")]
        public string Password { get; set; }

        [Display(Name = "Şifre tekrar")]
        public string PasswordConfirm { get; set; }
    }
}
