using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace USNFabricV8Web.App_Code.USNHostingControllers
{

    public class AzureImageProcessorSettings
    {
        public string StorageAccount { get; set; }
        public string MediaContainer { get; set; }
        public string CacheContainer { get; set; }
        public string CDNRootUrl { get; set; }
        public string StorageRootUrl { get; set; }
        public string SiteID { get; set; }
    }

    public class AzureImageProcessorWebConfigSettings
    {
        public string SiteID { get; set; }
    }

    public class AzureHostingController: Umbraco.Web.WebApi.UmbracoApiController
    {
        private const string ImageProcessorSecurityPath = "~/Config/imageprocessor/securitySiteAzure.config";
        private const string ImageProcessorCachePath = "~/Config/imageprocessor/cacheSiteAzure.config";
        private const string WebConfigPath = "~/web.config";

        private const string ImageProcessorWebConfigMasterSecuritySetting = @"config\imageprocessor\securityMasterSiteAzure.config";
        private const string ImageProcessorWebConfigMasterCacheSetting = @"config\imageprocessor\cacheMasterSiteAzure.config";
        private const string ImageProcessorWebConfigSiteAzureSecuritySetting = @"config\imageprocessor\securitySiteAzure.config";
        private const string ImageProcessorWebConfigSiteAzureCacheSetting = @"config\imageprocessor\cacheSiteAzure.config";

        private const string MEDIACONTAINER = "##MEDIACONTAINER##";
        private const string STORAGECONTAINERROOTURL = "##STORAGECONTAINERROOTURL##";
        private const string CACHECONTAINER = "##CACHECONTAINER##";
        private const string CDNROOTURL = "##CDNROOTURL##";
        private const string STORAGEACCOUNT = "##STORAGEACCOUNT##";

        [HttpPost]
        public bool SetImageProcessorSettings(AzureImageProcessorSettings settings)
        {
            if (string.IsNullOrEmpty(settings.SiteID) || settings.SiteID != ConfigurationManager.AppSettings["SiteID"])
            {
                return false;
            }

            string path = HostingEnvironment.MapPath(ImageProcessorSecurityPath);
            string configText;

            if (File.Exists(path))
            {
                configText = File.ReadAllText(path);

                configText = configText.Replace(MEDIACONTAINER, settings.MediaContainer);
                configText = configText.Replace(STORAGECONTAINERROOTURL, settings.StorageRootUrl);

                File.WriteAllText(path, configText);
            }
            else
            {
                return false;
            }


            path = HostingEnvironment.MapPath(ImageProcessorCachePath);

            if (File.Exists(path))
            {
                configText = File.ReadAllText(path);

                configText = configText.Replace(STORAGEACCOUNT, settings.StorageAccount);
                configText = configText.Replace(CACHECONTAINER, settings.CacheContainer);
                configText = configText.Replace(CDNROOTURL, settings.CDNRootUrl);
                configText = configText.Replace(MEDIACONTAINER, settings.MediaContainer);

                File.WriteAllText(path, configText);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Change the impage processor settings in the web.config file
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        [HttpPost]
        public bool SetImageProcessorWebConfigSettings(AzureImageProcessorWebConfigSettings settings)
        {
            if (string.IsNullOrEmpty(settings.SiteID) || settings.SiteID != ConfigurationManager.AppSettings["SiteID"])
            {
                return false;
            }

            string path = HostingEnvironment.MapPath(WebConfigPath);
            string configText;

            if (File.Exists(path))
            {
                configText = File.ReadAllText(path);

                if (configText.Contains(ImageProcessorWebConfigMasterSecuritySetting))
                {
                    configText = configText.Replace(ImageProcessorWebConfigMasterSecuritySetting, ImageProcessorWebConfigSiteAzureSecuritySetting);
                }
                else
                {
                    return false;
                }

                if (configText.Contains(ImageProcessorWebConfigMasterCacheSetting))
                {
                    configText = configText.Replace(ImageProcessorWebConfigMasterCacheSetting, ImageProcessorWebConfigSiteAzureCacheSetting);
                }
                else
                {
                    return false;
                }

                File.WriteAllText(path, configText);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the current uskinned version number
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetuSkinnedVersion()
        {
            return ConfigurationManager.AppSettings["uSkinned-Version"].ToString();
        }

        /// <summary>
        /// Get the umbraco version number
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetUmbracoVersion()
        {
            return ConfigurationManager.AppSettings["Umbraco.Core.ConfigurationStatus"].ToString();
        }
    }
}