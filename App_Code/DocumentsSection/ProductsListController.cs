using FinCMS.App_Code.DatabaseRepo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using USNWebsite.DatabaseRepo;
using USNWebsite.USNModels;
using Vendr.Core;
using Vendr.Core.Services;
using Providers.Services;

namespace USNWebsite.USNControllers
{
    public class ProductsListController : SurfaceController
    {
        private readonly IMemberService _memberService;
        private readonly IContentService _contentService;
        private readonly ICurrencyService _currencyService;
        private readonly IGeneralService _generalService;

        ManageProducts manageProducts = new ManageProducts();

        public ProductsListController(IMemberService memberService, IContentService contentService, ICurrencyService currencyService, IGeneralService generalService)
        {
            _memberService = memberService;
            _contentService = contentService;
            _currencyService = currencyService;
            _generalService = generalService;
        }

        // Method to display all products in Umbraco back-office.
        [HttpGet]
        public ActionResult GetAllProducts()
        {
            dynamic dynObject = new ExpandoObject();
            List<USNCreateProductFormViewModel> result = new List<USNCreateProductFormViewModel>();
            List<int> providersIdList = new List<int>();
            List<ProvidersList> providersList = new List<ProvidersList>();

            DataSet productsData = manageProducts.GetAllProductsForProvider(0);
            if (productsData.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < productsData.Tables[0].Rows.Count; i++)
                {
                    providersIdList.Add(int.Parse(productsData.Tables[0].Rows[i]["MemberId"].ToString()));
                    var member = Members.GetById(int.Parse(productsData.Tables[0].Rows[i]["MemberId"].ToString()));
                    USNCreateProductFormViewModel model = new USNCreateProductFormViewModel();

                    model.ProductId = int.Parse(productsData.Tables[0].Rows[i]["ProductId"].ToString());
                    model.ProductName = productsData.Tables[0].Rows[i]["ProductName"].ToString();
                    model.ProductCreationDate = Convert.ToDateTime(productsData.Tables[0].Rows[i]["ProductCreation"]).ToString("dd/MM/yyyy HH:mm");
                    model.ProductPrice = float.Parse(productsData.Tables[0].Rows[i]["ProductPrice"].ToString());
                    model.ProductStatus = productsData.Tables[0].Rows[i]["ProductStatus"].ToString();
                    model.ProviderName = member?.Name;
                    model.MemberId = int.Parse(productsData.Tables[0].Rows[i]["MemberId"].ToString());

                    string nodeId = productsData.Tables[0].Rows[i]["ProductNodeId"].ToString();
                    var productNode = Umbraco.Content(nodeId);
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
                    result.Add(model);
                }
                providersIdList = providersIdList.Distinct().ToList();
            }

            foreach (var item in providersIdList)
            {
                providersList.Add(new ProvidersList
                {
                    ProviderId = item,
                    ProviderName = Members.GetById(item)?.Name
                });
            }
            dynObject.result = result.OrderByDescending(x => x.ProductCreationDate);
            dynObject.providersList = providersList;

            return Json(dynObject, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProductDetails(string productId)
        {
            return Json(GetProductDetailsById(productId, "backoffice"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public USNCreateProductFormViewModel GetProductDetailsById(string productId, string calledFrom)
        {
            DataSet ds = manageProducts.GetProductById(int.Parse(productId));
            ProductDetails productDetails = new ProductDetails(_memberService, _contentService);
            var model = productDetails.GetProductDetailsModel(ds, calledFrom);
            var productNode = Umbraco.Content(model.ProductNodeId);
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

        [HttpPost]
        public bool UpdateProductDetails(string productId, string productStatus)
        {
            var result = false;
            var productDetails = GetProductDetailsById(productId, "backoffice");
            IContent product = null;

            if (productStatus == "Active")
            {
                if (productDetails.ProductNodeId > 0)
                {
                    product = _contentService.GetById(productDetails.ProductNodeId);
                    _contentService.SaveAndPublish(product);
                    result = manageProducts.UpdateProductStatus(int.Parse(productId), productStatus, product.Id, product.Key.ToString());
                }
                else
                {
                    var productCategory = _contentService.GetById(int.Parse(productDetails.ProductTypeId));
                    if (productCategory != null)
                    {
                        var store = Umbraco.ContentAtRoot().FirstOrDefault().GetStore();
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

                        if (productDetails.ProductFilesList?.Count > 0)
                        {
                            List<string> productFiles = new List<string>();
                            foreach (var productFile in productDetails.ProductFilesList)
                            {
                                productFiles.Add(_generalService.GetFileGuid(productFile.ProductFile));
                            }
                            productDetails.ProductFiles = String.Join(",", productFiles);
                        }
                        product.SetValue("productName", productDetails.ProductName);
                        product.SetValue("price", JsonConvert.SerializeObject(priceData));
                        product.SetValue("productDescription", productDetails.ProductDescription);
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
            }
            else
            {
                if (productDetails.ProductNodeId > 0)
                {
                    product = _contentService.GetById(productDetails.ProductNodeId);
                    _contentService.Unpublish(product);
                    result = manageProducts.UpdateProductStatus(int.Parse(productId), productStatus, productDetails.ProductNodeId);
                }
                else
                {
                    result = manageProducts.UpdateProductStatus(int.Parse(productId), productStatus, 0);
                }
            }

            return result;
        }
    }
}
