using System;
using Umbraco.Web;
using USNWebsite.USNModels;
using USNStarterKit.Extensions;

namespace USNWebsite.USNBusinessLogic
{
    public static class USNUmbracoHelperExtensions
    {
        public static string GetFlag(this UmbracoHelper umbracoHelper, string culture)
        {
            if (culture.Contains("-"))
            {
                culture = culture.Split(new char[] { '-' })[1];
            }
            return string.Concat("/images/flags/", culture.ToLower(), ".svg");
        }

        public static string GetSplitDesktopTabletOrderClass(this UmbracoHelper umbracoHelper, string splitSide, string desktopTabletOrder)
        {
            string output = String.Empty;

            if (splitSide == "First" && desktopTabletOrder == "desktop_One")
            {
                output = "1";
            }
            else if (splitSide == "First" && desktopTabletOrder == "desktop_Two")
            {
                output = "2";
            }
            else if (splitSide == "Second" && desktopTabletOrder == "desktop_One")
            {
                output = "2";
            }
            else if (splitSide == "Second" && desktopTabletOrder == "desktop_Two")
            {
                output = "1";
            }
            else if (splitSide == "First" && !string.IsNullOrEmpty(desktopTabletOrder))
            {
                output = "1";
            }
            else if (splitSide == "Second" && !string.IsNullOrEmpty(desktopTabletOrder))
            {
                output = "2";
            }

            return output;
        }

        public static string GetSplitMobileOrderClass(this UmbracoHelper umbracoHelper, string splitSide, string mobileOrder)
        {
            string output = String.Empty;

            if (splitSide == "First" && mobileOrder == "mobile_One")
            {
                output = "order-1";
            }
            else if (splitSide == "First" && mobileOrder == "mobile_Two")
            {
                output = "order-2";
            }
            else if (splitSide == "Second" && mobileOrder == "mobile_One")
            {
                output = "order-2";
            }
            else if (splitSide == "Second" && mobileOrder == "mobile_Two")
            {
                output = "order-1";
            }
            else if (splitSide == "First" && !string.IsNullOrEmpty(mobileOrder))
            {
                output = "order-1";
            }
            else if (splitSide == "Second" && !string.IsNullOrEmpty(mobileOrder))
            {
                output = "order-2";
            }

            return output;
        }

        public static USNPageLayoutSettings GetPageLayoutSettings(this UmbracoHelper umbracoHelper, string pageLayout)
        {
            USNPageLayoutSettings output = new USNPageLayoutSettings();

            output.PageLayout = pageLayout;

            switch (pageLayout)
            {
                case "pageLayoutLeft":
                    output.FirstDivColumnClass = "content-col left-col col-lg-8 col-12 order-1";
                    output.SecondDivColumnClass = "right-col col-xl-3 col-lg-4 col-12 order-2";
                    break;
                case "pageLayoutRight":
                    output.FirstDivColumnClass = "content-col right-col col-lg-8 col-12 order-lg-2 order-1";
                    output.SecondDivColumnClass = "left-col col-xl-3 col-lg-4 col-12 order-lg-1 order-2";
                    break;
                case "pageLayoutCenterNavRight":
                    output.FirstDivColumnClass = "content-col center-col col-lg-6 col-12 order-lg-2 order-1";
                    output.SecondDivColumnClass = "left-col col-lg-3 col-12 order-lg-1 order-3";
                    output.ThirdDivColumnClass = "right-col col-lg-3 col-12 order-lg-3 order-2";
                    break;
                case "pageLayoutCenterNavLeft":
                    output.FirstDivColumnClass = "content-col center-col col-lg-6 col-12 order-lg-2 order-1";
                    output.SecondDivColumnClass = "left-col col-lg-3 col-12 order-lg-1 order-2";
                    output.ThirdDivColumnClass = "right-col col-lg-3 col-12 order-lg-3 order-3";
                    break;
                case "pageLayoutWide":
                    output.FirstDivColumnClass = "content-col wide-col col-12";
                    break;
                default:
                    output.PageLayout = "pageLayoutFull";
                    break;
            }

            return output;
        }

        public static USNImageSettings GetImageSettings(this UmbracoHelper umbracoHelper, string imageStyle, bool boxPad = false)
        {
            USNImageSettings output = new USNImageSettings();

            output.BoxPad = boxPad;

            switch (imageStyle)
            {
                case "image1by1Square":
                    output.WidthSmall = 80;
                    output.HeightSmall = 80;
                    output.Width = 800;
                    output.Height = 800;
                    output.CropAlias = "1:1 Square";
                    output.CircleClass = String.Empty;
                    break;
                case "image1by1Circle":
                    output.WidthSmall = 80;
                    output.HeightSmall = 80;
                    output.Width = 800;
                    output.Height = 800;
                    output.CropAlias = "1:1 Circle";
                    output.CircleClass = "rounded-circle";
                    break;
                case "image2by1":
                    output.WidthSmall = 80;
                    output.HeightSmall = 40;
                    output.Width = 800;
                    output.Height = 400;
                    output.CropAlias = "2:1";
                    output.CircleClass = String.Empty;
                    break;
                case "image1by2":
                    output.WidthSmall = 40;
                    output.HeightSmall = 80;
                    output.Width = 400;
                    output.Height = 800;
                    output.CropAlias = "1:2";
                    output.CircleClass = String.Empty;
                    break;
                case "image3by2":
                    output.WidthSmall = 80;
                    output.HeightSmall = 53;
                    output.Width = 800;
                    output.Height = 530;
                    output.CropAlias = "3:2";
                    output.CircleClass = String.Empty;
                    break;
                case "image2by3":
                    output.WidthSmall = 53;
                    output.HeightSmall = 80;
                    output.Width = 530;
                    output.Height = 800;
                    output.CropAlias = "2:3";
                    output.CircleClass = String.Empty;
                    break;
                case "image4by3":
                    output.WidthSmall = 80;
                    output.HeightSmall = 60;
                    output.Width = 800;
                    output.Height = 600;
                    output.CropAlias = "4:3";
                    output.CircleClass = String.Empty;
                    break;
                case "image3by4":
                    output.WidthSmall = 60;
                    output.HeightSmall = 80;
                    output.Width = 600;
                    output.Height = 800;
                    output.CropAlias = "3:4";
                    output.CircleClass = String.Empty;
                    break;
                case "image16by9":
                    output.WidthSmall = 80;
                    output.HeightSmall = 45;
                    output.Width = 800;
                    output.Height = 450;
                    output.CropAlias = "16:9";
                    output.CircleClass = String.Empty;
                    break;
                case "image9by16":
                    output.WidthSmall = 45;
                    output.HeightSmall = 80;
                    output.Width = 450;
                    output.Height = 800;
                    output.CropAlias = "9:16";
                    output.CircleClass = String.Empty;
                    break;
                case "image16by10":
                    output.WidthSmall = 80;
                    output.HeightSmall = 50;
                    output.Width = 800;
                    output.Height = 500;
                    output.CropAlias = "16:10";
                    output.CircleClass = String.Empty;
                    break;
                case "image10by16":
                    output.WidthSmall = 50;
                    output.HeightSmall = 80;
                    output.Width = 500;
                    output.Height = 800;
                    output.CropAlias = "10:16";
                    output.CircleClass = String.Empty;
                    break;
                default:
                    output.WidthSmall = 80;
                    output.HeightSmall = 60;
                    output.Width = 800;
                    output.Height = 600;
                    output.CropAlias = "4:3";
                    output.CircleClass = String.Empty;
                    break;
            }

            return output;
        }

        public static USNCarouselSettings GetCarouselSettings(this UmbracoHelper umbracoHelper, string itemsPerRow, bool enableCarousel, decimal? autoRotateSpeed, bool showDots, bool showArrows)
        {
            USNCarouselSettings output = new USNCarouselSettings();

            string slickAttributes = enableCarousel ? "data-slick='{{{0}{1}{2}{3}}}'" : "";
            string autoRotateAttribute = String.Empty;
            string carouselRowClass = String.Empty;
            string slidesToShowAttribute = String.Empty;
            string dotsAttribute = showDots ? "\"dots\": true " : "\"dots\": false ";
            string arrowsAttribute = showArrows ? "\"arrows\": true " : "\"arrows\": false ";

            if (enableCarousel)
            {
                carouselRowClass = "slides";

                if (autoRotateSpeed.HasValue() && autoRotateSpeed > 0)
                {
                    autoRotateSpeed = autoRotateSpeed * 1000;
                    autoRotateAttribute = "\"autoplay\": true, \"autoplaySpeed\": " + Convert.ToInt32(autoRotateSpeed);
                }
            }

            output.CarouselRowClass = carouselRowClass;

            switch (itemsPerRow)
            {
                case "items-1":
                    output.ItemsClass = enableCarousel ? "items-1" : "items-1 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 1";
                    break;
                case "items-2":
                    output.ItemsClass = enableCarousel ? "items-2" : "items-2 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 2, \"responsive\": [{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-3":
                    output.ItemsClass = enableCarousel ? "items-3" : "items-3 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 3, \"responsive\": [{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 2}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-4":
                    output.ItemsClass = enableCarousel ? "items-4" : "items-4 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 4, \"responsive\": [{\"breakpoint\": 992,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-5":
                    output.ItemsClass = enableCarousel ? "items-5" : "items-5 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 5, \"responsive\": [{\"breakpoint\": 1200,\"settings\": {\"slidesToShow\": 4}},{\"breakpoint\": 992,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 2}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-6":
                    output.ItemsClass = enableCarousel ? "items-6" : "items-6 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 6, \"responsive\": [{\"breakpoint\": 1200,\"settings\": {\"slidesToShow\": 5}},{\"breakpoint\": 992,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-7":
                    output.ItemsClass = enableCarousel ? "items-7" : "items-7 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 7, \"responsive\": [{\"breakpoint\": 1200,\"settings\": {\"slidesToShow\": 6}},{\"breakpoint\": 992,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 2}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                case "items-8":
                    output.ItemsClass = enableCarousel ? "items-8" : "items-8 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 8, \"responsive\": [{\"breakpoint\": 1200,\"settings\": {\"slidesToShow\": 7}},{\"breakpoint\": 992,\"settings\": {\"slidesToShow\": 3}},{\"breakpoint\": 768,\"settings\": {\"slidesToShow\": 2}},{\"breakpoint\": 575,\"settings\": {\"slidesToShow\": 1}}]";
                    break;
                default:
                    output.ItemsClass = enableCarousel ? "items-1" : "items-1 col-12";
                    slidesToShowAttribute = "\"slidesToShow\": 1";
                    break;
            }

            //Add commas
            dotsAttribute = autoRotateAttribute != String.Empty ? "," + dotsAttribute : dotsAttribute;
            arrowsAttribute = autoRotateAttribute != String.Empty || dotsAttribute != String.Empty ? "," + arrowsAttribute : arrowsAttribute;
            slidesToShowAttribute = autoRotateAttribute != String.Empty || dotsAttribute != String.Empty || arrowsAttribute != String.Empty ? "," + slidesToShowAttribute : slidesToShowAttribute;

            slickAttributes = enableCarousel ? String.Format(slickAttributes, autoRotateAttribute, dotsAttribute, arrowsAttribute, slidesToShowAttribute) : String.Empty;

            output.SlickAttributes = slickAttributes;

            return output;
        }

        public static USNAnimation GetAnimationSettings(this UmbracoHelper umbracoHelper, bool animate, decimal animationDelay, decimal animationDuration, object animationStyle)
        {
            USNAnimation output = new USNAnimation();

            output.AnimationClass = animate ? "os-animation" : String.Empty;
            output.AnimationDelay = animationDelay.ToString() + "s";
            output.AnimationDuration = animationDuration.ToString() + "s";
            output.AnimationStyle = animationStyle.HasValue() ? animationStyle.ToString() : "fadeIn";

            return output;
        }
    }
}