using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using USNStarterKit.Models;

namespace USNWebsite.USNModels
{
    public class USNContactFormViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string Email { get; set; }

        public string Telephone { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public Boolean NewsletterSignup { get; set; }

        public string UniqueID { get; set; }

        public int GlobalSettingsID { get; set; }

        public string CaptchaDataSize { get; set; }

        public string FormColorLabel { get; set; }
        public string FormBgColorClass { get; set; }
        public string FormButtonColorLabel { get; set; }
        public string FormButtonColorClass { get; set; }
        public string FormButtonHoverColorClass { get; set; }

        public string ContactRecipientEmailAddress { get; set; }

        public USNHeading FormHeading { get; set; }

        public USNHeading FormSecondaryHeading { get; set; }

        public HtmlString FormIntroduction { get; set; }

        public string FormButtonText { get; set; }

        public string FormSubmissionMessage { get; set; }

        public string FormSubscriberListID { get; set; }

        public string PageName { get; set; }
    }
}