using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;
using USNWebsite.USNModels;
using Vendr.Core.Services;

namespace Providers.Services
{
    public class RemoveProviderServices : IRemoveProviderService
    {
        private readonly IMemberService _memberService;
        private readonly IOrderService _orderService;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly INpocoService _npocoService;
        private readonly IGeneralService _generalService;

        public RemoveProviderServices(IMemberService memberService, IOrderService orderService, IContentService contentService, IMediaService mediaService, INpocoService npocoService, IGeneralService generalService)
        {
            _memberService = memberService;
            _orderService = orderService;
            _contentService = contentService;
            _mediaService = mediaService;
            _npocoService = npocoService;
            _generalService = generalService;
        }
        public bool RemoveProvider(int Id)
        {
            //Get Products List
            var products = _npocoService.Database().Fetch<ProductsTableSchema>()
                .Where(x => x.MemberId == Id)
                .Select(x => new ProductsTableSchema()
                {
                    ProductId = x.ProductId,
                    ProductNodeKey = x.ProductNodeKey,
                    ProductNodeId = x.ProductNodeId,
                    ProductName = x.ProductName,
                    ProductLogoLarge = x.ProductLogoLarge,
                    ProductLogoSmall = x.ProductLogoSmall,
                    ProductScreenshot = x.ProductScreenshot
                })
                .ToList();

            //Delete Provider Orders
            DeleteProviderOrders(products);

            //Delete Product Files
            DeleteProductFiles(String.Join(",", products.Select(x => x.ProductId)));

            //Delete Product Logos
            foreach (var item in products)
            {
                _generalService.ProductLogoDeleteLocalFolder(item.ProductLogoLarge);
                _generalService.ProductLogoDeleteLocalFolder(item.ProductLogoSmall);
                _generalService.ProductLogoDeleteLocalFolder(item.ProductScreenshot);
            }

            //Delete Provider Wallet Information
            DeleteProviderWalletInfo(Id);

            //Delete Publishe content for product
            DeleteProductContentNode(products.Select(x => x.ProductNodeId).ToList());

            //Delete Products
            _npocoService.Database().Execute($"delete ProductsTable where MemberId = '{Id}'");

            //Dlete Provider
            DeleteUser(Id);
            return true;
        }
        public void DeleteUser(int Id)
        {
            var member = _memberService.GetById(Id);
            if (member != null)
            {
                DeleteMediaProfile("iDDocument", member);
                _memberService.Delete(member);
            }
        }
        public void DeleteProductFiles(string productIds)
        {
            var Ids = productIds.Split(',');
            foreach (var item in Ids)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var productFileList = _npocoService.Database().Fetch<ProductFilesTableSchema>()
                                            .Where(x => x.ProductId.ToString() == item)
                                            .Select(x => new ProductVersionFiles()
                                            {
                                                ProductFileId = x.ProductId,
                                                FileUrl = x.ProductFile,
                                                ProductVersionNumber = x.ProductVersionNumber != null ? x.ProductVersionNumber : "0"
                                            })
                                            .ToList();


                    var productVersionGroupList = productFileList.GroupBy(x => x.ProductVersionNumber).Select(x => new
                    {
                        ProductVersionNumber = x.Key,
                        productVersionFiles = x.FirstOrDefault()
                    }).ToList();

                    foreach (var product in productVersionGroupList)
                    {
                        if (!string.IsNullOrWhiteSpace(product.productVersionFiles.FileUrl))
                        {
                            string productDirectoryPath = HttpContext.Current.Server.MapPath(Path.GetDirectoryName(product.productVersionFiles.FileUrl));
                            if (Directory.Exists(productDirectoryPath))
                            {
                                DirectoryInfo dir = new DirectoryInfo(productDirectoryPath);
                                dir.GetFiles("*", SearchOption.AllDirectories).ToList().ForEach(file => file.Delete());
                                Directory.Delete(productDirectoryPath);
                            }
                        }
                    }
                }
            }

            productIds = productIds.Replace(",", "','");
            _npocoService.Database().Execute($"delete ProductFilesTable where ProductId in ('{productIds}')");
        }
        public void DeleteProviderOrders(List<ProductsTableSchema> products)
        {
            var productReference = String.Join(",", products.Select(x => x.ProductNodeKey)).Replace(",", "','");
            var vendrOrderLineModel = _npocoService.Database().Fetch<VendrOrderLine>($"select * from vendrOrderLine where productReference in ('{productReference}')");
            foreach (var item in vendrOrderLineModel)
            {
                _orderService.DeleteOrder(item.OrderId);
            }
        }
        public void DeleteProviderWalletInfo(int Id)
        {
            _npocoService.Database().Execute($"delete ProviderBankDetails where ProviderId = {Id}");
            _npocoService.Database().Execute($"delete ProviderPaymentLogs where ProviderId = {Id}");
            _npocoService.Database().Execute($"delete ProviderWallet where ProviderId = {Id}");
        }
        public void DeleteProductContentNode(List<int> nodeIds)
        {
            foreach (var Id in nodeIds)
            {
                var content = _contentService.GetById(Id);
                if (content != null)
                {
                    //Delete Media File
                    DeleteMediaProductFiles("productFiles", content);
                    DeleteMediaLogoFiles("productLogoLarge", content);
                    DeleteMediaLogoFiles("productLogoSmall", content);
                    DeleteMediaLogoFiles("productScreenshot", content);

                    // here content getting deleted
                    _contentService.Delete(content);
                }
            }
        }
        public void DeleteMediaLogoFiles(string productImageType, IContent content)
        {
            var mediaImage = content.GetValue<string>(productImageType);
            var arry = mediaImage?.Split('/');
            if (arry != null)
            {
                var media = _mediaService.GetById(new Guid(arry[arry.Length - 1]));
                if (media != null)
                {
                    _mediaService.Delete(media);
                }
            }
        }
        public void DeleteMediaProductFiles(string productImageType, IContent content)
        {
            var mediaImages = content.GetValue<string>(productImageType)?.Split(',');
            if (mediaImages != null)
            {
                foreach (var item in mediaImages)
                {
                    var arry = item.Split('/');
                    var media = _mediaService.GetById(new Guid(arry[arry.Length - 1]));
                    if (media != null)
                    {
                        _mediaService.Delete(media);
                    }
                }
            }
        }
        public void DeleteMediaProfile(string imageAlias, IMember member)
        {
            var mediaImage = member.GetValue<string>(imageAlias);
            var arry = mediaImage?.Split('/');
            if (arry != null)
            {
                var media = _mediaService.GetById(new Guid(arry[arry.Length - 1]));
                if (media != null)
                {
                    _mediaService.Delete(media);
                }
            }
        }
    }
}