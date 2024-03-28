using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "E-posta formatı yanlıştır")]
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Display(Name = "E-posta")]
        public string City { get; set; }

        [Display(Name = "Fotoğraf")]
        public string Picture { get; set; }

        [Display(Name = "Doğum Tarihi")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Cinsiyet")]
        public int? Gender { get; set; }
    }
}
