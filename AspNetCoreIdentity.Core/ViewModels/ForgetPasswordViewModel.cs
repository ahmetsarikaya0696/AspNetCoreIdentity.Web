using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Core.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "E-posta formatı yanlıştır")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
    }
}