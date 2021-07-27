using System.ComponentModel.DataAnnotations;
using System.Web;
using Umbraco.Core.Models.PublishedContent;
using USNStarterKit.Models;

namespace USNWebsite.USNModels
{
    public class USNLoginFormViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public int GlobalSettingsID { get; set; }

        public string CaptchaDataSize { get; set; }

        public string FormColorLabel { get; set; }
        public string FormBgColorClass { get; set; }
        public string FormButtonColorLabel { get; set; }
        public string FormButtonColorClass { get; set; }
        public string FormButtonHoverColorClass { get; set; }

        public USNHeading FormHeading { get; set; }

        public USNHeading FormSecondaryHeading { get; set; }

        public HtmlString FormIntroduction { get; set; }

        public string FormButtonText { get; set; }

        public bool FormHideFields { get; set; }

        public IPublishedContent FormResetPasswordRequestPage { get; set; }
    }
}