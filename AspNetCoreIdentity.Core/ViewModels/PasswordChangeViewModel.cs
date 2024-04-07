using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni Şifre alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string NewPasswordConfirm { get; set; }
    }
}
