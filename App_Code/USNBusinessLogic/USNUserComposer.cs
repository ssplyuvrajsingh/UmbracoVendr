using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Sections;
using USNStarterKit.Services;
using USNStarterKit.Interfaces;
using USNWebsite.USNControllers;
using USNStarterKit.BusinessLogic;
using Umbraco.Web.Tour;
using System.Text.RegularExpressions;
using Umbraco.Web.Dashboards;

namespace USNWebsite.USNBusinessLogic
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class USNUserComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.SetDefaultRenderMvcController(typeof(USNBaseController));
            composition.Components().Append<USNSubscribeToEvents>();
            composition.Sections().Append<USNSection>();
            composition.Sections().InsertBefore<PackagesSection, USNSection>();
            composition.Register<IUSNThemeService, USNThemeService>();
            composition.Register<IUSNBlogService, USNBlogService>();
            composition.TourFilters()
               .AddFilter(new BackOfficeTourFilter(pluginName: null, tourFileName: null, tourAlias: new Regex("^umbIntro", RegexOptions.IgnoreCase)));
            composition.Dashboards().Remove<ContentDashboard>();

        }
    }
}