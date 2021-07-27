using System.Collections.Generic;
using Umbraco.Core.Models;
using UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration;

namespace Providers.Services
{
    public interface IRemoveProviderService
    {
        bool RemoveProvider(int Id);
        void DeleteUser(int Id);
        void DeleteProductFiles(string productIds);
        void DeleteProviderOrders(List<ProductsTableSchema> products);
        void DeleteProviderWalletInfo(int Id);
        void DeleteProductContentNode(List<int> nodeIds);
        void DeleteMediaLogoFiles(string productImageType, IContent content);
        void DeleteMediaProductFiles(string productImageType, IContent content);
    }
}