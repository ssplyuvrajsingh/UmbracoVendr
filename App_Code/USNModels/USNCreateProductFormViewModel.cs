using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using USNStarterKit.Models;
using Vendr.Core.Models;
using Vendr.Web.Models;

namespace USNWebsite.USNModels
{
    public class USNCreateProductFormViewModel
    {
        public int Action { get; set; }

        public int MemberId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }

        public float ProductPrice { get; set; }

        public FormattedPriceProduct FormattedPrice { get; set; }

        public bool IsFormattedPrice { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCreationDate { get; set; }

        public string ProductLogoLarge { get; set; }

        public string ProductLogoLargeName { get; set; }

        public string ProductLogoSmall { get; set; }

        public string ProductLogoSmallName { get; set; }

        public string ProductScreenshot { get; set; }

        public string ProductScreenshotName { get; set; }

        public string ProductScreenshotDescription { get; set; }

        public string ProductFiles { get; set; }

        public List<ProductFiles> ProductFilesList { get; set; }

        public List<ProductVersionGroupList> ProductVersionGroupList { get; set; }

        public string UpdatedProductFiles { get; set; }

        public int ProductProtectionTrialLength { get; set; }

        public int ProductProtectionNumberOfTrials { get; set; }

        public string ProductStatus { get; set; }

        public string ProviderName { get; set; }

        public int ProductNodeId { get; set; }
        public string ProductNodeUrl { get; set; }

        public int GlobalSettingsID { get; set; }

        public string CaptchaDataSize { get; set; }

        public string FormColor { get; set; }

        public string FormButtonColor { get; set; }

        public USNHeading FormHeading { get; set; }

        public USNHeading FormSecondaryHeading { get; set; }

        public HtmlString FormIntroduction { get; set; }

        public string FormButtonText { get; set; }

        public bool FormHideFields { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

        public string ProductNewVersionNumber { get; set; }

        public string LastVersionProductName { get; set; }

        public CountryRegionTaxRateDto CountryRegionTaxRateDto { get; set; }

        public bool PricesIncludeTax { get; set; }
    }

    public class _ProductPartialViewModel
    {
        public List<USNCreateProductFormViewModel> ProductModel { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
    }

    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductFiles
    {
        public int ProductFileId { get; set; }
        public string ProductFile { get; set; }
        public string ProductFileName { get; set; }
        public string ProductVersionNumber { get; set; }
    }

    public class ProvidersList
    {
        public int ProviderId { get; set; }

        public string ProviderName { get; set; }
    }

    public class ProductVersionFiles
    {
        public int ProductFileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ProductVersionNumber { get; set; }
        public string CreatedVersionDateTime { get; set; }
    }
    public class ProductVersionGroupList
    {
        public string ProductVersionNumber { get; set; }
        public List<ProductVersionFiles> productVersionFiles { get; set; }
    }

    public class FormattedPriceProduct
    {
        public string CurrencyId { get; set; }
        public string WithoutTax { get; set; }
        public string Tax { get; set; }
        public string WithTax { get; set; }
        public string WithoutTaxNoSymbol { get; set; }
        public string TaxNoSymbol { get; set; }
        public string WithTaxNoSymbol { get; set; }
    }
}