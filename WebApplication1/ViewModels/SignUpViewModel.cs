using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "E-posta formatı yanlıştır")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor")]
        [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz")]
        [Display(Name = "Şifre tekrar")]
        public string PasswordConfirm { get; set; }
    }
}
