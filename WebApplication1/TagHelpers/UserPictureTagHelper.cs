using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentity.Web.TagHelpers
{
    public class UserPictureTagHelper : TagHelper
    {
        public string Picture { get; set; }
        public int Width { get; set; } = 75;
        public int Height { get; set; } = 75;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            string fileName = string.IsNullOrEmpty(Picture) ? "default.png" : Picture;
            string filePath = $"/user-images/{fileName}";

            output.Attributes.SetAttribute("src", filePath);
            output.Attributes.SetAttribute("width", Width);
            output.Attributes.SetAttribute("height", Height);
        }
    }
}
