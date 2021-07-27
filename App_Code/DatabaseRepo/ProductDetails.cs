using System;
using System.Linq;
using Umbraco.Core.Services;
using USNWebsite.DatabaseRepo;
using USNWebsite.USNModels;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace FinCMS.App_Code.DatabaseRepo
{
    public class ProductDetails
    {
        private readonly IMemberService _memberService;
        private readonly IContentService _contentService;
        ManageProducts manageProducts = new ManageProducts();

        public ProductDetails(IMemberService memberService, IContentService contentService)
        {
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _contentService = contentService;
        }
        public USNCreateProductFormViewModel GetProductDetails(string contentId)
        {
            return GetProductDetailsModel(manageProducts.GetProductByNodeId(int.Parse(contentId)), "frontend");
        }

        public USNCreateProductFormViewModel GetProductDetailsModel(DataSet productDetails, string calledFrom = "frontend")
        {
            USNCreateProductFormViewModel productViewModel = new USNCreateProductFormViewModel();

            if (productDetails.Tables[0].Rows.Count > 0)
            {
                var productTableRow = productDetails.Tables[0].Rows[0];
                var member = _memberService.GetById(int.Parse(productTableRow["MemberId"].ToString()));
                var productCategoryId = !string.IsNullOrWhiteSpace(productTableRow["ProductType"].ToString())
                        ? int.Parse(productTableRow["ProductType"].ToString())
                        : 0;
                var productCategory = productCategoryId > 0 ? _contentService.GetById(productCategoryId) : null;

                productViewModel = new USNCreateProductFormViewModel
                {
                    ProductId = int.Parse(productTableRow["ProductId"].ToString()),
                    ProductName = productTableRow["ProductName"].ToString(),
                    ProductFilesList = !string.IsNullOrWhiteSpace(productTableRow["ProductFiles"].ToString())
                    ? productTableRow["ProductFiles"].ToString().Split(',').Select(x => new ProductFiles
                    {
                        ProductFile = x.Substring(1),
                        ProductFileName = Path.GetFileName(x.Substring(1))
                    }).ToList()
                    : null,
                    ProductTypeId = productCategory?.Id.ToString(),
                    ProductTypeName = productCategory?.Name,
                    ProductPrice = float.Parse(productTableRow["ProductPrice"].ToString()),
                    ProductDescription = productTableRow["ProductDescription"].ToString(),
                    ProductLogoLargeName = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoLarge"].ToString()) ? Path.GetFileName(productTableRow["ProductLogoLarge"].ToString().Substring(1)) : string.Empty,
                    ProductLogoSmallName = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoSmall"].ToString()) ? Path.GetFileName(productTableRow["ProductLogoSmall"].ToString().Substring(1)) : string.Empty,
                    ProductScreenshotName = !string.IsNullOrWhiteSpace(productTableRow["ProductScreenshot"].ToString()) ? Path.GetFileName(productTableRow["ProductScreenshot"].ToString().Substring(1)) : string.Empty,
                    ProductScreenshotDescription = productTableRow["ProductScreenshotDescription"].ToString(),
                    ProductProtectionTrialLength = int.Parse(productTableRow["ProductProtectionTrialLength"].ToString()),
                    ProductProtectionNumberOfTrials = int.Parse(productTableRow["ProductProtectionNumberOfTrials"].ToString()),
                    ProductStatus = productTableRow["ProductStatus"].ToString(),
                    ProductNodeId = !string.IsNullOrWhiteSpace(productTableRow["ProductNodeId"].ToString()) ? int.Parse(productTableRow["ProductNodeId"].ToString()) : 0,
                    ProviderName = member?.Name,
                    MemberId = (int)member?.Id
                };
                if (calledFrom == "frontend")
                {
                    productViewModel.ProductLogoLarge = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoLarge"].ToString()) ? productTableRow["ProductLogoLarge"].ToString() : string.Empty;
                    productViewModel.ProductLogoSmall = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoSmall"].ToString()) ? productTableRow["ProductLogoSmall"].ToString() : string.Empty;
                    productViewModel.ProductScreenshot = !string.IsNullOrWhiteSpace(productTableRow["ProductScreenshot"].ToString()) ? productTableRow["ProductScreenshot"].ToString() : string.Empty;
                }
                else
                {
                    productViewModel.ProductLogoLarge = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoLarge"].ToString()) ? productTableRow["ProductLogoLarge"].ToString().Substring(1) : string.Empty;
                    productViewModel.ProductLogoSmall = !string.IsNullOrWhiteSpace(productTableRow["ProductLogoSmall"].ToString()) ? productTableRow["ProductLogoSmall"].ToString().Substring(1) : string.Empty;
                    productViewModel.ProductScreenshot = !string.IsNullOrWhiteSpace(productTableRow["ProductScreenshot"].ToString()) ? productTableRow["ProductScreenshot"].ToString().Substring(1) : string.Empty;
                }
                productViewModel.UpdatedProductFiles = productViewModel.ProductFilesList != null ? JsonConvert.SerializeObject(productViewModel.ProductFilesList) : string.Empty;
            }

            return productViewModel;
        }

    }
}