using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor")]
        [Required(ErrorMessage = "Yeni şifre tekrar alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre tekrar")]
        public string PasswordConfirm { get; set; }
    }
}
