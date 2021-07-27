using System;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using USNWebsite.USNModels;
using Umbraco.Web;
using USNStarterKit.Models;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Services;
using System.IO;
using System.Collections.Generic;
using USNWebsite.DatabaseRepo;
using System.Data;
using System.Linq;
using Vendr.Core.Services;
using Vendr.Web.Controllers;
using Vendr.Web.Models;
using System.Text;
using FinCMS.App_Code.DatabaseRepo;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;
using Umbraco.Core.Models;
using Providers.Services;

namespace USNWebsite.USNControllers
{
    public class USNCreateProductFormSurfaceController : Umbraco.Web.Mvc.SurfaceController
    {
        private readonly IMemberService _memberService;
        private readonly IContentService _contentService;
        private readonly ICurrencyService _currencyService;
        private readonly OrderController _orderController;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly TaxController _taxController;
        private readonly IGeneralService _generalService;
        private readonly INpocoService _npocoService;
        private readonly IMediaService _mediaService;

        ManageProducts manageProducts = new ManageProducts();

        public USNCreateProductFormSurfaceController(IMemberService memberService, IContentService contentService, ICurrencyService currencyService, OrderController orderController, IPaymentMethodService paymentMethodService, TaxController taxController, IGeneralService generalService, INpocoService npocoService, IMediaService mediaService)
        {
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _orderController = orderController;
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            _taxController = taxController ?? throw new ArgumentNullException(nameof(taxController));
            _generalService = generalService;
            _npocoService = npocoService;
            _mediaService = mediaService;
        }

        #region Providers methods

        #region Products methods
        // Method to display all products to particular provider after login
        [HttpGet]
        public string GetAllProductsOfParticularProvider()
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                var member = Members.GetCurrentMember();
                var result = string.Empty;
                if (member != null)
                {
                    result = manageProducts.GetProducts(member != null ? member.Id : 0, Umbraco);
                }
                return result;
            }
            else
            {
                StringBuilder htmlTable = new StringBuilder();
                htmlTable.Append("<h5>Your session has timed out. Please login again.</h5>" +
                    "<a href = '/login/' class='btn btn-primary ml-3'><i class='fa fa-arrow-circle-left'></i>Click Here To Login</a>");
                return htmlTable.ToString();
            }
        }

        [Authorize]
        public ActionResult Index(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColor, string FormButtonColor, IPublishedElement Content, List<IPublishedContent> ProductCategories = null)
        {
            TempData["ProductCreationSuccess"] = null;
            TempData["ProductCreationFailure"] = null;
            var productId = int.Parse(Request.QueryString["productId"]);
            var model = new USNCreateProductFormViewModel();
            if (productId > 0)
            {
                var productsListController = new ProductsListController(_memberService, _contentService, _currencyService, _generalService);
                model = productsListController.GetProductDetailsById(productId.ToString(), "frontend");
                manageProducts.GetProductFiles(ref model);
            }
            var store = Umbraco.ContentAtRoot().FirstOrDefault().GetStore();
            model.PricesIncludeTax = store.PricesIncludeTax;

            var taxData = _taxController.GetTaxClass((System.Guid)store?.DefaultTaxClassId);
            model.CountryRegionTaxRateDto = taxData?.CountryRegionTaxRates?.FirstOrDefault(x => x?.CountryId == store.DefaultCountryId);

            model.GlobalSettingsID = GlobalSettingsID;
            model.FormColor = FormColor;
            model.CaptchaDataSize = DataSize;
            model.FormButtonColor = FormButtonColor;
            model.FormHeading = Content.Value<USNHeading>("formHeading");
            model.FormSecondaryHeading = Content.Value<USNHeading>("formSecondaryHeading");
            model.FormIntroduction = Content.Value<HtmlString>("formIntroduction");
            model.FormButtonText = Content.Value<string>("formButtonText");
            model.FormHideFields = Content.Value<bool>("hideFields");
            if (ProductCategories?.Count > 0)
            {
                model.ProductCategories = ProductCategories.Select(x => new ProductCategory
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }

            return PartialView(FullViewPath, model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult HandleProductCreationSubmit(USNCreateProductFormViewModel model, HttpPostedFileBase logoLarge, HttpPostedFileBase logoSmall, HttpPostedFileBase screenShot, HttpPostedFileBase[] productFiles)
        {
            model.MemberId = Members.GetCurrentMember().Id;
            string redirectPath = "/products/";
            string productFileFolderPath = string.Empty;
            List<string> productFilesPath = new List<string>();
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (!string.IsNullOrEmpty(model.ProductNewVersionNumber))
            {
                productFileFolderPath = @"~/UploadedFiles/" + model.ProductName + "_" + model.ProductNewVersionNumber + "/";
                // If directory does not exist, create it. 
                if (!Directory.Exists(Server.MapPath(productFileFolderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(productFileFolderPath));
                }

                foreach (HttpPostedFileBase file in productFiles)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);
                        var ServerSavePath = productFileFolderPath + InputFileName;

                        //Save file to server folder
                        file.SaveAs(Server.MapPath(ServerSavePath));
                        productFilesPath.Add(ServerSavePath);
                    }
                }
                if (productFilesPath.Count > 0)
                {
                    model.ProductFiles = String.Join(",", productFilesPath);
                }
                model.Action = 13;
                int result = manageProducts.AddProductNewVersion(model);
                return Redirect(redirectPath);
            }
            else
            {

                //Create Folder to save Product Files
                if (productFiles?.Length > 0 && !string.IsNullOrEmpty(model.ProductName))
                {
                    if (model.LastVersionProductName == "0" || string.IsNullOrEmpty(model.LastVersionProductName))
                    {
                        productFileFolderPath = @"~/UploadedFiles/" + model.ProductName + "/";
                    }
                    else
                    {
                        productFileFolderPath = @"~/UploadedFiles/" + model.ProductName + "_" + model.LastVersionProductName + "/";
                        model.ProductNewVersionNumber = model.LastVersionProductName;
                    }
                    // If directory does not exist, create it. 
                    if (!Directory.Exists(Server.MapPath(productFileFolderPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(productFileFolderPath));
                    }
                    if (productFiles != null)
                    {
                        foreach (HttpPostedFileBase file in productFiles)
                        {
                            //Checking file is available to save.  
                            if (file != null)
                            {
                                var InputFileName = Path.GetFileName(file.FileName);
                                var pathFolder = productFileFolderPath.Split('/');
                                var physicalPath = Path.Combine(Server.MapPath("~/UploadedFiles/" + pathFolder[2] + pathFolder[3]), InputFileName);
                                if (System.IO.File.Exists(physicalPath))
                                {
                                    var fileNameModify = Path.GetFileNameWithoutExtension(InputFileName);
                                    var extension = Path.GetExtension(InputFileName);
                                    InputFileName = fileNameModify + DateTime.Now.Ticks + extension;
                                }

                                var ServerSavePath = productFileFolderPath + InputFileName;

                                //Save file to server folder
                                file.SaveAs(Server.MapPath(ServerSavePath));
                                productFilesPath.Add(ServerSavePath);
                            }
                        }

                        if (productFilesPath.Count > 0)
                        {
                            model.ProductFiles = String.Join(",", productFilesPath);
                        }
                    }
                }

                var productDetails = new ProductsTableSchema();
                IContent content = null;
                if ((logoLarge != null || logoSmall != null || screenShot != null) && model?.ProductId > 0)
                {
                    productDetails = _npocoService.Database().Fetch<ProductsTableSchema>()
                                       .Where(x => x.ProductId == model.ProductId)
                                       .Select(x => new ProductsTableSchema()
                                       {
                                           ProductNodeId = x.ProductNodeId,
                                           ProductNodeKey = x.ProductNodeKey,
                                           ProductLogoLarge = x.ProductLogoLarge,
                                           ProductLogoSmall = x.ProductLogoSmall,
                                           ProductScreenshot = x.ProductScreenshot
                                       }).FirstOrDefault();

                    content = _contentService.GetById(productDetails.ProductNodeId);
                }

                if (logoLarge != null)
                {
                    model.ProductLogoLarge = _generalService.ReUploadLogo("productLogoLarge", productDetails, content, logoLarge);
                }

                if (logoSmall != null)
                {
                    model.ProductLogoSmall = _generalService.ReUploadLogo("productLogoSmall", productDetails, content, logoSmall);
                }

                if (screenShot != null)
                {
                    model.ProductScreenshot = _generalService.ReUploadLogo("productScreenshot", productDetails, content, screenShot);
                }

                if (model.ProductId > 0)
                {
                    model.Action = 5;

                    if (productFilesPath?.Count > 0)
                    {
                        model.ProductFiles = String.Join(",", productFilesPath);
                    }
                    if (model.ProductStatus == "Save for Later")
                    {
                        redirectPath = "/create-product/?productId=" + model.ProductId;
                    }
                }
                else
                {
                    model.Action = 1;
                }
                int result = manageProducts.AddUpdateProduct(model);
                if (result >= 1)
                {
                    TempData.Add("ProductCreationSuccess", /*Umbraco.GetDictionaryValue("USN Register Form User Already Exists Error")*/"Product Created Successfully.");
                }
                else
                {
                    TempData.Add("ProductCreationFailure", /*Umbraco.GetDictionaryValue("USN Register Form User Already Exists Error")*/"Error occured while creating product.");
                }

                return Redirect(redirectPath);
            }
        }

        public int ProductImageDelete(ProductFiles model)
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                var productId = _npocoService.Database().Fetch<ProductFilesTableSchema>()
                                   .Where(x => x.ProductFileId == model.ProductFileId)
                                   .Select(x => x.ProductId).FirstOrDefault();

                var productDetails = _npocoService.Database().Fetch<ProductsTableSchema>()
                                        .Where(x => x.ProductId == productId)
                                        .Select(x => new ProductsTableSchema()
                                        {
                                            ProductNodeId = x.ProductNodeId,
                                            ProductNodeKey = x.ProductNodeKey
                                        }).FirstOrDefault();

                var content = _contentService.GetById(productDetails.ProductNodeId);
                model.ProductFileName = Path.GetFileName(model.ProductFile);
                if (content != null)
                {
                    var mediaImages = content.GetValue<string>("productFiles").Split(',');
                    List<string> productFiles = new List<string>();
                    foreach (var item in mediaImages)
                    {
                        var arry = item.Split('/');
                        Guid Id = new Guid(arry[arry.Length - 1]);
                        var media = _mediaService.GetById(new Guid(arry[arry.Length - 1]));

                        if (Path.GetFileNameWithoutExtension(media?.Name) == Path.GetFileNameWithoutExtension(model.ProductFileName))
                        {
                            _mediaService.Delete(media);
                        }
                        else if (media != null)
                        {
                            productFiles.Add(item);
                        }
                    }
                    content.SetValue("productFiles", String.Join(",", productFiles));
                    _contentService.Save(content);
                }

                var physicalPath = Server.MapPath("~/" + model.ProductFile);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                var res = manageProducts.ProductImageDelete(model);

                if (res == 0)
                {
                    var pathFolder = model.ProductFile.Split('/');
                    physicalPath = Server.MapPath(@"~/UploadedFiles/" + pathFolder[2] + "/");
                    // If directory does not exist
                    if (Directory.Exists(physicalPath) && _generalService.IsDirectoryEmpty(physicalPath))
                    {
                        Directory.Delete(physicalPath);
                    }
                }
                return 1;
            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region Orders methods
        [HttpGet]
        public ActionResult GetAllOrdersOfParticularProvider()
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                var member = Members.GetCurrentMember();
                var result = manageProducts.GetOrdersByProviderId(member != null ? member.Id : 0);
                return PartialView("/Views/Partials/Providers/_ProviderOrderList.cshtml", result);
            }
            else
            {
                return PartialView("/Views/Partials/Providers/_ProviderOrderList.cshtml", null);
            }
        }

        [HttpGet]
        public ActionResult GetOrderDetails(string orderId)
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                return PartialView("/Views/Partials/Providers/_OrderDetails.cshtml", manageProducts.GetOrderDetails(orderId));
            }
            else
            {
                return PartialView("/Views/Partials/Providers/_OrderDetails.cshtml", null);
            }
        }

        [HttpGet]
        public string GetOrderStatus(string orderStatusId, string orderId, string paymentMethodId, string paymentStatus)
        {
            return manageProducts.GetOrderStatus(orderStatusId, orderId, paymentMethodId, _paymentMethodService, paymentStatus);
        }

        [HttpGet]
        public int UpdateOrderStatusByOrderId(string orderStatusId, string orderId)
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                _orderController.ChangeOrderStatus(Guid.Parse(orderId), Guid.Parse(orderStatusId));
                return 1;
            }
            else
            {
                return 2;
            }
        }

        [HttpGet]
        public string ChangePaymentStaus(string orderId, string paymentStatus)
        {
            if (Members.GetCurrentLoginStatus().IsLoggedIn)
            {
                StringBuilder htmlMessage = new StringBuilder();
                OrderBasicDto basicDto = new OrderBasicDto();
                switch (paymentStatus)
                {
                    case "Cancelled":
                        basicDto = _orderController.CancelPayment(Guid.Parse(orderId));
                        htmlMessage.Append($"<span>Payment Cancelled</span>");
                        break;
                    case "Captured":
                        basicDto = _orderController.CapturePayment(Guid.Parse(orderId));
                        htmlMessage.Append($"<span>Payment Captured</span>");
                        break;
                    case "Refunded":
                        basicDto = _orderController.RefundPayment(Guid.Parse(orderId));
                        htmlMessage.Append($"<span>Payment Refunded</span>");
                        break;
                }
                return htmlMessage.ToString();
            }
            else
            {
                return "logout";
            }
        }

        #endregion

        #endregion

        #region Shop page method
        // Method to display the products on Shop Page of frontend.
        [HttpGet]
        public ActionResult GetAllProducts(int categoryId = 0, int providerId = 0)
        {
            try
            {
                _ProductPartialViewModel result = new _ProductPartialViewModel();
                if (providerId > 0)
                {
                    result = manageProducts.GetAllProductsByProvider(providerId, Members, Umbraco);
                }
                else
                {
                    result = manageProducts.GetAllProducts(categoryId, Members, Umbraco, providerId);
                }

                return PartialView("~/Views/Partials/Providers/_ProductsList.cshtml", result);
            }
            catch
            {
                return PartialView("~/Views/Partials/Providers/_ProductsList.cshtml", null);
            }
        }
        #endregion
    }
}