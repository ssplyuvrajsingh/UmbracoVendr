using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Vendr.Core;
using Vendr.Core.Api;
using Vendr.Core.Models;

namespace FinCMS.App_Code.DatabaseRepo
{
    public static class VendrPublishedContentExtensions
    {
        public static string GetProductReference(this IPublishedContent content)
        {
            return content.Key.ToString();
        }

        public static IProductSnapshot AsVendrProduct(this IPublishedContent content)
        {
            var temp = VendrApi.Instance.GetProduct(content.GetProductReference(), null);
            return VendrApi.Instance.GetProduct(content.GetProductReference(), null);
        }

        public static Price CalculatePrice(this IPublishedContent content)
        {
            var temp = content.AsVendrProduct()?.CalculatePrice();
            return content.AsVendrProduct()?.CalculatePrice();
        }

        public static StoreReadOnly GetStore(this IPublishedContent content)
        {
            return content.Root().Value<StoreReadOnly>("store");
        }

        public static OrderReadOnly GetCurrentOrder(this IPublishedContent content)
        {
            return VendrApi.Instance.GetCurrentOrder(content.GetStore().Id);
        }
    }
}