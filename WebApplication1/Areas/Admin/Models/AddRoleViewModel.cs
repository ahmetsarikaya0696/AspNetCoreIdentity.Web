using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur!")]
        [Display(Name = "Ad")]
        public string Name { get; set; }
    }
}
