using System.ComponentModel.DataAnnotations;
using System.Web;
using Umbraco.Core.Models.PublishedContent;
using USNStarterKit.Models;

namespace USNWebsite.USNModels
{
    public class USNProviderRegisterFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string ProviderType { get; set; }

        public string StreetName { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string TelephoneNumber { get; set; }

        public string ProviderStatus { get; set; }

        [Required]
        public string Email { get; set; }

        public IPublishedContent IdDocument { get; set; }

        public string IdDocumentPath { get; set; }

        public int GlobalSettingsID { get; set; }

        public string CaptchaDataSize { get; set; }

        public string FormColor { get; set; }

        public string FormButtonColor { get; set; }

        public USNHeading FormHeading { get; set; }

        public USNHeading FormSecondaryHeading { get; set; }

        public HtmlString FormIntroduction { get; set; }

        public string FormButtonText { get; set; }

        public bool FormHideFields { get; set; }
    }
}