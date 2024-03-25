using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "E-posta formatı yanlıştır")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
    }
}