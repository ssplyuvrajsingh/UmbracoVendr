using createsend_dotnet;
using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using USNStarterKit.Extensions;
using USNStarterKit.Models;
using USNWebsite.USNModels;

namespace USNWebsite.USNControllers
{
    public class USNNewsletterSignupSurfaceController : Umbraco.Web.Mvc.SurfaceController
    {
        public ActionResult Index(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColorLabel, string FormBgColorClass, string FormButtonColorLabel, string FormButtonColorClass, string FormButtonHoverColorClass, IPublishedElement Content)
        {
            var model = new USNNewsletterFormViewModel();
            model.UniqueID = Content.Key.ToString();
            model.GlobalSettingsID = GlobalSettingsID;
            model.CaptchaDataSize = DataSize;
            model.FormColorLabel = FormColorLabel;
            model.FormBgColorClass = FormBgColorClass;
            model.FormButtonColorLabel = FormButtonColorLabel;
            model.FormButtonColorClass = FormButtonColorClass;
            model.FormButtonHoverColorClass = FormButtonHoverColorClass;
            model.FormHeading = Content.Value<USNHeading>("formHeading");
            model.FormSecondaryHeading = Content.Value<USNHeading>("formSecondaryHeading");
            model.FormIntroduction = Content.Value<HtmlString>("formIntroduction");
            model.FormButtonText = Content.Value<string>("formButtonText");
            model.FormSubmissionMessage = Content.Value<string>("formSubmissionMessage");
            model.FormSubscriberListID = Content.Value<string>("formSubscriberListID");
            model.FormHideFields = Content.Value<bool>("hideFields");

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleNewsletterSubmit(USNNewsletterFormViewModel model)
        {
            System.Threading.Thread.Sleep(1000);

            var globalSettings = Umbraco.Content(model.GlobalSettingsID);

            string recaptchaReset = globalSettings.HasValue("googleReCAPTCHASiteKey") && globalSettings.HasValue("googleReCAPTCHASecretKey") ? "grecaptcha.reset();" : String.Empty;

            string lsReturnValue = String.Empty;

            if (!ModelState.IsValid)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Required Field Error"))));
            }
            try
            {
                if (globalSettings.HasValue("googleReCAPTCHASiteKey") && globalSettings.HasValue("googleReCAPTCHASecretKey"))
                {
                    var response = Request["g-recaptcha-response"];
                    string secretKey = globalSettings.Value<string>("googleReCAPTCHASecretKey");
                    var client = new WebClient();
                    var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                    var obj = JObject.Parse(result);
                    var status = (bool)obj.SelectToken("success");

                    if (!status)
                    {
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('{2}');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form reCAPTCHA Error"))));
                    }
                }

                string firstName = model.FirstName == "***" ? String.Empty : model.FirstName;
                string lastName = model.LastName == "***" ? String.Empty : model.LastName;

                if (globalSettings.Value<string>("emailMarketingPlatform") == "newsletterCM")
                {
                    AuthenticationDetails auth = new ApiKeyAuthenticationDetails(globalSettings.Value<string>("newsletterAPIKey"));

                    string subsciberListID = String.Empty;

                    if (model.FormSubscriberListID.HasValue())
                        subsciberListID = model.FormSubscriberListID;
                    else
                        subsciberListID = globalSettings.Value<string>("defaultNewsletterSubscriberListID");

                    Subscriber loSubscriber = new Subscriber(auth, subsciberListID);

                    List<SubscriberCustomField> customFields = new List<SubscriberCustomField>();

                    string subscriberID = loSubscriber.Add(model.Email, firstName + " " + lastName, customFields, false);
                }
                else if (globalSettings.Value<string>("emailMarketingPlatform") == "newsletterMC")
                {

                    var mc = new MailChimpManager(globalSettings.Value<string>("newsletterAPIKey"));

                    string subsciberListID = String.Empty;

                    if (model.FormSubscriberListID.HasValue())
                        subsciberListID = model.FormSubscriberListID;
                    else
                        subsciberListID = globalSettings.Value<string>("defaultNewsletterSubscriberListID");


                    var email = new EmailParameter()
                    {
                        Email = model.Email
                    };

                    var myMergeVars = new MergeVar();
                    myMergeVars.Add("FNAME", firstName);
                    myMergeVars.Add("LNAME", lastName);

                    EmailParameter results = mc.Subscribe(subsciberListID, email, myMergeVars, "html", false, true, false, false);
                }

                return JavaScript(String.Format("$('#SubmissionMessage_{0}').show();$('#Form_{0}').hide();$('#Error_{0}').hide();", model.UniqueID));
            }
            catch (Exception ex)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Newsletter Form Signup Error")), HttpUtility.JavaScriptStringEncode(ex.Message)));
            }
        }
    }
}