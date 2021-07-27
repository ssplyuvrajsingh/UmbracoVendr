using Newtonsoft.Json;
using Providers.Services;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;
using Umbraco.Web;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;
using USNWebsite.DatabaseRepo;
using USNWebsite.USNModels;

public class DummyDataSurfaceServices : IDummyDataSurfaceServices
{
    ManageProducts manageProducts = new ManageProducts();

    private readonly IMemberService _memberService;
    private readonly UmbracoHelper _umbracoHelper;
    private readonly IProductSurfaceServices _productSurfaceServices;
    private readonly IGeneralService _generalService;
    private readonly INpocoService _npocoService;

    public DummyDataSurfaceServices(IMemberService memberService, UmbracoHelper umbracoHelper, IProductSurfaceServices productSurfaceServices, IGeneralService generalService, INpocoService npocoService)
    {
        _memberService = memberService;
        _umbracoHelper = umbracoHelper;
        _productSurfaceServices = productSurfaceServices;
        _generalService = generalService;
        _npocoService = npocoService;
    }

    #region Create Demo Data
    public void CreateDummyData()
    {
        CreateMembers();
        CreateProducts();
    }
    #endregion

    #region Create Demo Members Services
    public void CreateMembers()
    {
        string json = _generalService.GetJsonData("App_Code/DatabaseRepo/DatabaseMigration/DemoData/ConfigJson/ProviderModel.json");
        ProviderRootModel provider = JsonConvert.DeserializeObject<ProviderRootModel>(json);

        MemberSurfaceController uSNMember = new MemberSurfaceController(_memberService,_generalService);
        uSNMember.HandleRegisterSubmit(provider);
    }
    #endregion

    #region Create Demo Products Services
    public void CreateProducts()
    {
        string json = _generalService.GetJsonData("App_Code/DatabaseRepo/DatabaseMigration/DemoData/ConfigJson/ProductModel.json");
        ProductRootModel products = JsonConvert.DeserializeObject<ProductRootModel>(json);

        var homeNode = _umbracoHelper.ContentAtRoot().Where(x => x.IsDocumentType("providers")).FirstOrDefault();
        var productCategories = homeNode?.Children(x => x.IsDocumentType("productsPage"))?.FirstOrDefault()?.Children(y => y.IsDocumentType("productCategory"))?.ToList();

        var count = 0;
        foreach (var item in products.ProductModel)
        {
            var productCategory = productCategories?[count];
            if (count > 1)
            {
                item.MemberId = _memberService.GetByEmail("dummysecond@gmail.com").Id;
            }
            else
            {
                item.MemberId = _memberService.GetByEmail("dummyfirst@gmail.com").Id;
            }

            //Copy product files
            productFiles(item.ProductName, item.ProductFiles);

            //Copy logo files
            item.ProductLogoLarge = CopyFiles(item.ProductLogoLarge);
            item.ProductLogoSmall = CopyFiles(item.ProductLogoSmall);
            item.ProductScreenshot = CopyFiles(item.ProductScreenshot);

            var model = new USNCreateProductFormViewModel
            {
                Action = item.Action,
                MemberId = item.MemberId,
                ProductName = item.ProductName,
                ProductTypeId = productCategory?.Id.ToString(),
                ProductTypeName = productCategory?.Name,
                ProductPrice = item.ProductPrice,
                ProductDescription = item.ProductDescription,
                ProductFiles = item.ProductFiles,
                ProductLogoLarge = item.ProductLogoLarge,
                ProductLogoSmall = item.ProductLogoSmall,
                ProductScreenshot = item.ProductScreenshot,
                ProductScreenshotDescription = item.ProductScreenshotDescription,
                ProductStatus = item.ProductStatus
            };
            manageProducts.AddUpdateProduct(model);
            count++;
        }
        UpdateProductDetails();
    }

    public void UpdateProductDetails()
    {
        var products = _npocoService.Database().Fetch<ProductsTableSchema>().ToList();
        foreach (var item in products)
        {
            _productSurfaceServices.UpdateProductDetails(item.ProductId.ToString(), "Active");
        }
    }

    public void productFiles(string productName, string productFiles)
    {
        string productFileFolderPath = HttpContext.Current.Server.MapPath(@"~/UploadedFiles/" + productName + "/");
        if (!Directory.Exists(productFileFolderPath))
        {
            Directory.CreateDirectory(productFileFolderPath);
        }

        var files = productFiles.Split(',');
        foreach (var file in files)
        {
             CopyFiles(file);
        }
    }

    public string CopyFiles(string file)
    {
        string result = file;
        var destinationPath = HttpContext.Current.Server.MapPath(file);
        var fileName = Path.GetFileName(file);
        var sourcePath = HttpContext.Current.Server.MapPath(@"~/SourceFiles/" + fileName);

        if (!File.Exists(destinationPath))
        {
            File.Copy(sourcePath, destinationPath);
        }
        else
        {
            result = @"~/UploadedFiles/" + Path.GetFileNameWithoutExtension(file) + Path.GetRandomFileName() + Path.GetExtension(file);
            destinationPath = HttpContext.Current.Server.MapPath(result);
            File.Copy(sourcePath, destinationPath);
        }
        return result;
    }
    #endregion
}