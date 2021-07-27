using System.ComponentModel.DataAnnotations;
using System.Web;
using Umbraco.Web.Models;
using USNStarterKit.Models;

namespace USNWebsite.USNModels
{
    public class USNRegisterFormViewModel
    {
        public RegisterModel RegisterModel { get; set; }

        public int GlobalSettingsID { get; set; }

        public string CaptchaDataSize { get; set; }
        public string UniqueID { get; set; }

        public string FormColorLabel { get; set; }
        public string FormBgColorClass { get; set; }
        public string FormButtonColorLabel { get; set; }
        public string FormButtonColorClass { get; set; }
        public string FormButtonHoverColorClass { get; set; }

        public USNHeading FormHeading { get; set; }

        public USNHeading FormSecondaryHeading { get; set; }

        public HtmlString FormIntroduction { get; set; }

        public string FormButtonText { get; set; }

        public string FormSubmissionMessage { get; set; }

        public string Info { get; set; }

        public int CurrentPageId { get; set; }

        public bool DisplayForm { get; set; }

        public bool ConfirmationSuccess { get; set; }
    }
}