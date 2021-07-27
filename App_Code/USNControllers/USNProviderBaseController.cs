using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using USNStarterKit.Models;

namespace UmbracoVendr.App_Code.USNControllers
{
    public class USNProviderBaseController : Umbraco.Web.Mvc.RenderMvcController
    {
        // GET: USNProviderBase
        public USNBaseViewModel ProviderBaseDetails(USNBaseViewModel model)
        {
           var data =  model.Content.Root().IsDocumentType("providers");
            var homeNode = Umbraco.ContentAtRoot().Where(x => x.IsDocumentType("USNHomepage")).FirstOrDefault();
            USNBaseViewModel customModel = new USNBaseViewModel(model.Content);

            if (homeNode.IsDocumentType("USNHomepage"))
            {
                IPublishedContent configurationNode = homeNode.Value<IPublishedContent>("websiteConfigurationNode", fallback: Fallback.ToAncestors);
                IPublishedContent globalSettings = configurationNode.Children.Where(x => x.IsDocumentType("USNGlobalSettings")).FirstOrDefault();
                IPublishedContent navigation = configurationNode.Children.Where(x => x.IsDocumentType("USNNavigation")).FirstOrDefault();
                IPublishedContent footer = configurationNode.Children.Where(x => x.IsDocumentType("USNFooter")).FirstOrDefault();
                IPublishedContent websiteStyle = model.Content.Value<IPublishedContent>("overrideWebsiteStyle", fallback: Fallback.ToAncestors) != null ? model.Content.Value<IPublishedContent>("overrideWebsiteStyle", fallback: Fallback.ToAncestors) : globalSettings.Value<IPublishedContent>("websiteStyle");

                //If cant find websiteStyle get first available
                websiteStyle = websiteStyle == null ? Umbraco.ContentAtRoot().Where(x => x.IsDocumentType("USNStylesFolder")).FirstOrDefault().Children.FirstOrDefault() : websiteStyle;

                customModel.HomeNode = homeNode;
                customModel.GlobalSettings = globalSettings;
                customModel.Navigation = navigation;
                customModel.Footer = footer;
                customModel.WebsiteStyle = websiteStyle;

                //Get Selected Style
                string themeFolder = websiteStyle != null ? websiteStyle.Value<string>("frontendSource").ToLower().Trim().Replace(" ", "_") : "uskinned";
                customModel.ThemeFolder = themeFolder;
            }
            return customModel;
        }
    }
}