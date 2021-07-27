using FinCMS.App_Code.DatabaseRepo;
using Newtonsoft.Json;
using Providers.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using USNWebsite.DatabaseRepo;
using USNWebsite.USNModels;
using Vendr.Core;
using Vendr.Core.Services;


public class ProductSurfaceServices : IProductSurfaceServices
{
    private readonly IMemberService _memberService;
    private readonly IContentService _contentService;
    private readonly ICurrencyService _currencyService;
    private readonly UmbracoHelper _umbracoHelper;
    private readonly IGeneralService _generalService;
    ManageProducts manageProducts = new ManageProducts();
     
    
    public ProductSurfaceServices(IMemberService memberService, IContentService contentService, ICurrencyService currencyService, UmbracoHelper umbracoHelper, IGeneralService generalService)
    {
        _memberService = memberService;
        _contentService = contentService;
        _currencyService = currencyService;
        _umbracoHelper = umbracoHelper;
        _generalService = generalService;
    }
    public void UpdateProductDetails(string productId, string productStatus)
    {
        var result = false;
        var productDetails = GetProductDetailsById(productId, "backoffice");
        IContent product = null;

        var productCategory = _contentService.GetById(int.Parse(productDetails.ProductTypeId));
        if (productCategory != null)
        {
            var store = _umbracoHelper.ContentAtRoot().FirstOrDefault().GetStore();
            var currencies = _currencyService.GetCurrencies(store.Id);
            var currencyId = currencies.Select(x => x.Id).FirstOrDefault();

            var priceData = new Dictionary<Guid, string>();
            foreach (var item in currencies)
            {
                priceData.Add(item.Id, productDetails.ProductPrice.ToString());
            }


            if (productDetails.ProductNodeId > 0)
            {
                product = _contentService.GetById(productDetails.ProductNodeId);
            }
            else
            {
                product = _contentService.CreateContent(productDetails.ProductName, productCategory.GetUdi(), "productPage");
            }

            try
            {
                if (productDetails.ProductFilesList?.Count > 0)
                {
                    List<string> productFiles = new List<string>();
                    foreach (var productFile in productDetails.ProductFilesList)
                    {
                        productFiles.Add(_generalService.GetFileGuid(productFile.ProductFile));
                    }
                    productDetails.ProductFiles = String.Join(",", productFiles);
                }

                if (!string.IsNullOrWhiteSpace(productDetails.ProductLogoLarge))
                {
                    product.SetValue("productLogoLarge", _generalService.GetFileGuid(productDetails.ProductLogoLarge));
                }
                if (!string.IsNullOrWhiteSpace(productDetails.ProductLogoSmall))
                {
                    product.SetValue("productLogoSmall", _generalService.GetFileGuid(productDetails.ProductLogoSmall));
                }
                if (!string.IsNullOrWhiteSpace(productDetails.ProductScreenshot))
                {
                    product.SetValue("productScreenshot", _generalService.GetFileGuid(productDetails.ProductScreenshot));
                }
            }
            catch
            {

            }

            product.SetValue("productName", productDetails.ProductName);
            product.SetValue("price", JsonConvert.SerializeObject(priceData));
            product.SetValue("productDescription", productDetails.ProductDescription);
           
            product.SetValue("productScreenshotDescription", productDetails.ProductScreenshotDescription);
            if (!string.IsNullOrWhiteSpace(productDetails.ProductFiles))
            {
                product.SetValue("productFiles", productDetails.ProductFiles);
            }
            product.SetValue("productTrialLength", productDetails.ProductProtectionTrialLength);
            product.SetValue("productNumberOfTrials", productDetails.ProductProtectionNumberOfTrials);
            _contentService.SaveAndPublish(product);
            result = manageProducts.UpdateProductStatus(int.Parse(productId), productStatus, product.Id, product.Key.ToString());
        }
    }
    public USNCreateProductFormViewModel GetProductDetailsById(string productId, string calledFrom)
    {
        DataSet ds = manageProducts.GetProductById(int.Parse(productId));
        ProductDetails productDetails = new ProductDetails(_memberService, _contentService);
        var model = productDetails.GetProductDetailsModel(ds);
        var productNode = _umbracoHelper.Content(model.ProductNodeId);
        if (productNode != null)
        {

            var formattedPrice = IPriceExtensions.Formatted(VendrPublishedContentExtensions.CalculatePrice(productNode));
            model.FormattedPrice = new FormattedPriceProduct
            {
                CurrencyId = formattedPrice.CurrencyId.ToString(),
                WithoutTax = formattedPrice.WithoutTax,
                Tax = formattedPrice.Tax,
                WithTax = formattedPrice.WithTax,
                WithoutTaxNoSymbol = formattedPrice.WithoutTaxNoSymbol,
                TaxNoSymbol = formattedPrice.TaxNoSymbol,
                WithTaxNoSymbol = formattedPrice.WithTaxNoSymbol
            };
            model.IsFormattedPrice = true;
        }
        manageProducts.GetProductFiles(ref model);
        return model;
    }
}