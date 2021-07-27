using System.IO;
using System.Web;
using Umbraco.Core.Models;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;


namespace Providers.Services
{
    public interface IGeneralService
    {
        string GetFileGuid(string filePath);
        string GetJsonData(string filePath);
        string UploadFiles(Stream stream, string name, string fileType);
        IMedia CreateAndSaveMediaFolder(string folderName, int parentFolderId);
        IMedia GetParentFolder(string folderName);
        bool IsDirectoryEmpty(string path);
        void ProductLogoDelete(IContent content, string fileType);
        void ProductLogoDeleteLocalFolder(string filePath);
        string ReUploadLogo(string logoType, ProductsTableSchema productDetails, IContent content, HttpPostedFileBase logoFile);
    }
}