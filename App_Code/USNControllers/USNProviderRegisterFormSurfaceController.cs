using System;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using USNWebsite.USNModels;
using Umbraco.Web;
using USNStarterKit.Models;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using System.Linq;
using System.IO;
using Providers.Services;

namespace USNWebsite.USNControllers
{
    public class USNProviderRegisterFormSurfaceController : Umbraco.Web.Mvc.SurfaceController
    {
        private readonly IMemberService _memberService;
        private readonly IGeneralService _generalService;

        public USNProviderRegisterFormSurfaceController(IMemberService memberService, IGeneralService generalService)
        {
            _memberService = memberService;
            _generalService = generalService;
        }

        #region Login Form for provider
        [Authorize]
        public ActionResult Index(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColor, string FormButtonColor, IPublishedElement Content)
        {
            var currentMember = Members.GetCurrentMember();
            var model = new USNProviderRegisterFormViewModel();
            if(currentMember != null)
            {
                model.FirstName = currentMember.GetProperty("firstName")?.Value<string>();
                model.LastName = currentMember.GetProperty("lastName")?.Value<string>();
                model.Email = currentMember.GetProperty("email")?.Value<string>();
                model.ProviderType = currentMember.GetProperty("providerType")?.Value<string>();
                model.StreetName = currentMember.GetProperty("streetName")?.Value<string>();
                model.City = currentMember.GetProperty("city")?.Value<string>();
                model.ZipCode = currentMember.GetProperty("zipCode")?.Value<string>();
                model.Country = currentMember.GetProperty("country")?.Value<string>();
                model.TelephoneNumber = currentMember.GetProperty("telephoneNumber")?.Value<string>();
                model.Email = currentMember.GetProperty("emailAddress").Value<string>() == "" ? currentMember.GetProperty("email").Value<string>() : currentMember.GetProperty("emailAddress").Value<string>();
                model.IdDocument = currentMember.GetProperty("iDDocument")?.Value<IPublishedContent>();
                model.ProviderStatus = currentMember.GetProperty("providerStatus")?.Value<string>();
                if (!string.IsNullOrEmpty(model.ProviderStatus))
                {
                    TempData["ProviderRegistrationSuccess"] =  null;
                    TempData["ProviderRegistrationFailure"] = null;
                }
            }
            model.GlobalSettingsID = GlobalSettingsID;
            model.FormColor = FormColor;
            model.CaptchaDataSize = DataSize;
            model.FormButtonColor = FormButtonColor;
            model.FormHeading = Content.Value<USNHeading>("formHeading");
            model.FormSecondaryHeading = Content.Value<USNHeading>("formSecondaryHeading");
            model.FormIntroduction = Content.Value<HtmlString>("formIntroduction");
            model.FormButtonText = Content.Value<string>("formButtonText");
            model.FormHideFields = Content.Value<bool>("hideFields");

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult HandleProviderRegisterSubmit(USNProviderRegisterFormViewModel model, HttpPostedFileBase iDDocument)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            else
            {
                var imageGUID = string.Empty;
                var member = _memberService.GetById(Members.GetCurrentMember().Id);

                if (iDDocument?.ContentLength > 0)
                {
                    string fileType = Path.GetExtension(iDDocument.FileName);
                    imageGUID = _generalService.UploadFiles(iDDocument.InputStream, iDDocument.FileName, fileType);
                }
                else
                {
                    imageGUID = member.Properties?.Where(x => x.Alias == "iDDocument").FirstOrDefault()?.GetValue()?.ToString();
                }

                try
                {
                    member.SetValue("firstName", model.FirstName);
                    member.SetValue("lastName", model.LastName);
                    member.SetValue("providerType", model.ProviderType);
                    member.SetValue("streetName", model.StreetName);
                    member.SetValue("city", model.City);
                    member.SetValue("zipCode", model.ZipCode);
                    member.SetValue("country", model.Country);
                    member.SetValue("telephoneNumber", model.TelephoneNumber);
                    member.SetValue("emailAddress", model.Email);
                    member.SetValue("iDDocument", imageGUID);
                    member.SetValue("providerStatus", "Waiting for Approval");

                    if (SaveMember(member))
                    {
                        TempData.Add("ProviderRegistrationSuccess", /*Umbraco.GetDictionaryValue("USN Register Form Registration Error")*/"Provider Successfully Registered.");
                        return RedirectToCurrentUmbracoPage();
                    }
                    else
                    {
                        TempData.Add("ProviderRegistrationFailure", Umbraco.GetDictionaryValue("USN Register Form Registration Error"));
                        return RedirectToCurrentUmbracoPage();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(typeof(USNProviderRegisterFormSurfaceController), "failed to Register Provider.", ex);
                    return RedirectToCurrentUmbracoPage();
                }
            }
        }

        protected bool SaveMember(IMember member)
        {
            try
            {
                _memberService.Save(member);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(USNMemberSurfaceController), "Failed to save member. Member email: " + member.Email, ex);
            }

            return false;
        }
        #endregion
    }
}