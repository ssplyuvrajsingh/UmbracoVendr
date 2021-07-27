using System.ComponentModel.DataAnnotations;
using System.Web;
using USNStarterKit.Models;

namespace USNWebsite.USNModels
{
    public class USNResetPasswordFormViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        public int MemberID { get; set; }
        public string Token { get; set; }

        public bool TokenExpired { get; set; }

        public bool TokenInvalid { get; set; }

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
    }
}