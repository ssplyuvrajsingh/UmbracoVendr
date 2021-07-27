using System;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;

namespace Providers.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly ServiceContext _serviceContext;
        private readonly IMediaService _mediaService;
        private readonly INpocoService _npocoService;
        public GeneralService(ServiceContext serviceContext, IMediaService mediaService, INpocoService npocoService)
        {
            _serviceContext = serviceContext;
            _mediaService = mediaService;
            _npocoService = npocoService;
        }

        public string GetFileGuid(string filePath)
        {
            var fileGuid = string.Empty;
            filePath = HttpContext.Current.Server.MapPath("/" + filePath.Replace("~", ""));
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader sw = new StreamReader(filePath))
                {
                    var fileType = Path.GetExtension(filePath);
                    var fileName = Path.GetFileName(filePath);
                    fileGuid = UploadFiles(sw.BaseStream, fileName, fileType);
                }
            }
            return fileGuid;
        }

        public string GetJsonData(string filePath)
        {
            var jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, filePath);
            string json = string.Empty;
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                json = r.ReadToEnd();
            }
            return json;
        }

        public string UploadFiles(Stream stream, string name, string fileType)
        {
            try
            {
                var parentFolder = GetParentFolder("Document Section")
                                      ?? CreateAndSaveMediaFolder("Document Section", -1);

                name = name.Replace(" ", "-");
                IMediaService mediaService = _serviceContext.MediaService;
                IMedia media = null;
                if (fileType == ".pdf" || fileType == ".zip")
                {
                    media = mediaService.CreateMedia(name, parentFolder.Id, "File");
                }
                else
                {
                    media = mediaService.CreateMedia(name, parentFolder.Id, "Image");
                }
                media.SetValue(_serviceContext.ContentTypeBaseServices, "umbracoFile", name, stream);
                mediaService.Save(media);
                return string.Concat("umb://media/", media.Key.ToString().Replace("-", ""));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IMedia CreateAndSaveMediaFolder(string folderName, int parentFolderId)
        {
            var folder = _mediaService.CreateMedia(folderName, parentFolderId, "Folder");
            _mediaService.Save(folder);
            return folder;
        }

        public IMedia GetParentFolder(string folderName)
        {
            var folder = _mediaService.GetRootMedia()
                .Where(m => m.ContentType.Name == "Folder" && m.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                .Select(y => y).FirstOrDefault();

            return folder;
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public void ProductLogoDelete(IContent content, string fileType)
        {
            if (content != null)
            {
                var mediaImage = content.GetValue<string>(fileType);
                var arry = mediaImage.Split('/');
                var media = _mediaService.GetById(new Guid(arry[arry.Length - 1]));
                if (media != null)
                {
                    _mediaService.Delete(media);
                }
            }
        }
        public void ProductLogoDeleteLocalFolder(string filePath)
        {
            filePath = HttpContext.Current.Server.MapPath(filePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public string ReUploadLogo(string logoType, ProductsTableSchema productDetails, IContent content, HttpPostedFileBase logoFile)
        {
            string result = string.Empty;
            ProductLogoDelete(content, logoType);
            ProductLogoDeleteLocalFolder(productDetails.ProductLogoLarge);
            var InputFileName = Path.GetFileName(logoFile.FileName);
            var serverSavePath = HttpContext.Current.Server.MapPath(@"~/UploadedFiles/" + InputFileName);

            //Save file to server folder
            if (!System.IO.File.Exists(serverSavePath))
            {
                logoFile.SaveAs(serverSavePath);
                result = @"~/UploadedFiles/" + InputFileName;
            }
            else
            {
                serverSavePath = @"~/UploadedFiles/" + Path.GetFileNameWithoutExtension(InputFileName) + Path.GetRandomFileName() + Path.GetExtension(InputFileName);
                logoFile.SaveAs(HttpContext.Current.Server.MapPath(serverSavePath));
                result = serverSavePath;
            }
            return result;
        }

    }
}