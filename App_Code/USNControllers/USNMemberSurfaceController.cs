using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Core.Models.PublishedContent;
using USNWebsite.USNModels;
using Umbraco.Web;
using USNStarterKit.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Services;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Configuration;
using USNStarterKit.USNHelpers;


namespace USNWebsite.USNControllers
{
    public class USNMemberSurfaceController : Umbraco.Web.Mvc.SurfaceController
    {
        #region Login Form

        public ActionResult LoginIndex(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColorLabel, string FormBgColorClass, string FormButtonColorLabel, string FormButtonColorClass, string FormButtonHoverColorClass, IPublishedElement Content)
        {
            var model = new USNLoginFormViewModel();
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
            model.FormHideFields = Content.Value<bool>("hideFields");
            model.FormResetPasswordRequestPage = Content.Value<IPublishedContent>("resetPasswordRequestPage");

            if (CurrentPage.Url() != Request.Url.PathAndQuery)
                model.ReturnUrl = Request.Url.PathAndQuery;
            else
                model.ReturnUrl = Content.Value<IPublishedContent>("loginSuccessPage").Url();

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleLoginSubmit(USNLoginFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var globalSettings = Umbraco.Content(model.GlobalSettingsID);

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

                    TempData.Add("LoginFailure", Umbraco.GetDictionaryValue("USN Form reCAPTCHA Error"));
                    return RedirectToCurrentUmbracoPage();
                }
            }

            // Login
            if (Membership.ValidateUser(model.Username, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Username, false);

                return Redirect(model.ReturnUrl);
            }
            else
            {
                TempData.Add("LoginFailure", Umbraco.GetDictionaryValue("USN Login Form Login Error"));
                return RedirectToCurrentUmbracoPage();
            }
        }

        public ActionResult Logout(string LogoutRedirect)
        {
            FormsAuthentication.SignOut();
            return Redirect(LogoutRedirect);
        }

        #endregion

        #region Reset Request Form

        public ActionResult ResetRequestIndex(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColorLabel, string FormBgColorClass, string FormButtonColorLabel, string FormButtonColorClass, string FormButtonHoverColorClass, IPublishedElement Content)
        {
            var model = new USNResetRequestFormViewModel();
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
            model.FormLoginPage = Content.Value<IPublishedContent>("loginFormPage");
            model.FormResetPasswordPage = Content.Value<IPublishedContent>("resetPasswordPage");
            model.FormSubmissionMessage = Content.Value<string>("formSubmissionMessage");
            model.FormResetPasswordPageId = Content.Value<IPublishedContent>("resetPasswordPage").Id;
            TempData.Remove("Email");
            TempData.Add("Email", Content.Value<HtmlString>("resetPasswordEmail"));

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleResetRequestSubmit(USNResetRequestFormViewModel model)
        {
            System.Threading.Thread.Sleep(1000);

            var globalSettings = Umbraco.Content(model.GlobalSettingsID);
            var resetPage = Umbraco.Content(model.FormResetPasswordPageId);

            string recaptchaReset = globalSettings.HasValue("googleReCAPTCHASiteKey") && globalSettings.HasValue("googleReCAPTCHASecretKey") ? "grecaptcha.reset();" : String.Empty;

            if (!ModelState.IsValid)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('{2}');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Required Field Error"))));
            }

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

            try
            {
                IMemberService memberService = Services.MemberService;
                var user = memberService.GetByUsername(model.Username);

                if (user != null)
                {
                  
                    var token = Guid.NewGuid().ToString();
                    user.SetValue("passwordToken", token);
                    user.SetValue("passwordTokenExpires", DateTime.UtcNow.AddHours(24));
                    memberService.Save(user);

                    string email = TempData["Email"] != null ? TempData["Email"].ToString() : String.Empty;
                    string resetUrl = Request.Url.Scheme + "://" + Request.Url.Host + resetPage.Url() + "?id=" + user.Id + "&token=" + token;
                    string resetLink = String.Format("<a href=\"{0}\">{1}</a>", resetUrl, Umbraco.GetDictionaryValue("USN Reset Request Form Email Reset Link"));

                    if (email != String.Empty)
                    {
                        StringBuilder sb = new StringBuilder(email);
                        sb.Replace("[NAME]", user.Name);

                        if (!sb.ToString().Contains("[RESETLINK]"))
                            sb.Append("<p>" + resetLink + "</p>");
                        else
                            sb.Replace("[RESETLINK]", resetLink);

                        email = sb.ToString();
                    }
                    else
                    {
                        email = resetLink;
                    }

                    string errorMessage = String.Empty;

                    if (!SendMail(user.Email, email, Umbraco.GetDictionaryValue("USN Reset Request Form Email Subject"), out errorMessage))
                    {
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Mail Send Error")), HttpUtility.JavaScriptStringEncode(errorMessage)));
                    }
                }

                return JavaScript(String.Format("$('#SubmissionMessage_{0}').show();$('#Form_{0}').hide();$('#Error_{0}').hide();", model.UniqueID));
            }
            catch (Exception ex)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Mail Send Error")), HttpUtility.JavaScriptStringEncode(ex.Message)));
            }
        }

        #endregion


        #region Reset Password Form

        public ActionResult ResetPasswordIndex(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColorLabel, string FormBgColorClass, string FormButtonColorLabel, string FormButtonColorClass, string FormButtonHoverColorClass, IPublishedElement Content)
        {
            var memberService = Services.MemberService;

            var model = new USNResetPasswordFormViewModel();
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

            NameValueCollection queryString = Request.QueryString;

            int integerValue = 0;

            if (queryString.AllKeys.Contains("id"))
            {
                int.TryParse(queryString["id"], out integerValue);
            }

            model.MemberID = integerValue;

            string token = String.Empty;

            if (queryString.AllKeys.Contains("token"))
            {
                token = queryString["token"];
            }

            model.Token = token;

            var member = memberService.GetById(model.MemberID);
            string memberToken = member != null ? member.GetValue<string>("passwordToken") : String.Empty;
            DateTime tokenExpires = member != null ? member.GetValue<DateTime>("passwordTokenExpires") : DateTime.MinValue;

            model.TokenExpired = DateTime.UtcNow > tokenExpires ? true : false;
            model.TokenInvalid = memberToken != model.Token ? true : false;

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleResetPasswordSubmit(USNResetPasswordFormViewModel model)
        {
            System.Threading.Thread.Sleep(1000);

            var globalSettings = Umbraco.Content(model.GlobalSettingsID);

            string recaptchaReset = globalSettings.HasValue("googleReCAPTCHASiteKey") && globalSettings.HasValue("googleReCAPTCHASecretKey") ? "grecaptcha.reset();" : String.Empty;

            if (!ModelState.IsValid)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('{2}');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Required Field Error"))));
            }

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

            try
            {
                IMemberService memberService = Services.MemberService;
                var member = memberService.GetById(model.MemberID);

                if (member != null)
                {
                    string memberToken = member != null ? member.GetValue<string>("passwordToken") : String.Empty;
                    DateTime tokenExpires = member != null ? member.GetValue<DateTime>("passwordTokenExpires") : DateTime.MinValue;

                    bool tokenExpired = DateTime.UtcNow > tokenExpires ? true : false;
                    bool tokenInvalid = memberToken != model.Token ? true : false;

                    if (tokenInvalid)
                    {
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Password Reset Form Token Invalid"))));
                    }
                    else if (tokenExpired)
                    {
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Password Reset Form Token Invalid"))));
                    }

                    memberService.SavePassword(member, model.NewPassword);
                    member.LastPasswordChangeDate = DateTime.Now;
                    member.SetValue("passwordToken", "");
                    member.SetValue("passwordTokenExpires", null);
                    memberService.Save(member);
                }
                else
                {
                    return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Password Reset Form Error"))));
                }

                return JavaScript(String.Format("$('#SubmissionMessage_{0}').show();$('#Form_{0}').hide();$('#Error_{0}').hide();", model.UniqueID));
            }
            catch (Exception ex)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Password Reset Form Error")), HttpUtility.JavaScriptStringEncode(ex.Message)));
            }
        }

        #endregion

        #region Register Form

        public ActionResult RegisterIndex(int GlobalSettingsID, string FullViewPath, string DataSize, string FormColorLabel, string FormBgColorClass, string FormButtonColorLabel, string FormButtonColorClass, string FormButtonHoverColorClass, IPublishedElement Content)
        {
            var model = new USNRegisterFormViewModel();
            bool confirmationSuccess = false;

            bool displayForm = true;
            NameValueCollection queryString = Request.QueryString;

            int memberID = 0;

            if (queryString.AllKeys.Contains("id"))
            {
                int.TryParse(queryString["id"], out memberID);
                displayForm = false;

                string token = String.Empty;

                if (queryString.AllKeys.Contains("token"))
                {
                    token = queryString["token"];
                }

                IMemberService memberService = Services.MemberService;

                var member = memberService.GetById(memberID);

                if (member != null)
                {
                    string memberToken = member != null ? member.GetValue<string>("emailConfirmationToken") : String.Empty;

                    confirmationSuccess = memberToken != token ? false : true;
                    member.SetValue("emailConfirmed", confirmationSuccess);

                    if (confirmationSuccess)
                        member.SetValue("emailConfirmationToken", "");

                    if (confirmationSuccess && !Content.Value<bool>("adminApprovalRequired"))
                        member.IsApproved = true;

                    memberService.Save(member);
                }
            }
            else
            {
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
                model.RegisterModel = Members.CreateRegistrationModel();
                model.CurrentPageId = CurrentPage.Id;
                model.RegisterModel.UsernameIsEmail = Content.Value<bool>("setEmailAddressAsUsername");

                TempData.Remove("EmailConfirmation");
                TempData.Add("EmailConfirmation", Content.Value<HtmlString>("memberConfirmationEmail"));

                //Use this field to store encrypted information regarding form settings, comma separated.
                //1. adminApprovalRequired
                //2. emailConfirmationRequired
                string info = Content.Value<string>("adminApprovalRequired") + "," + Content.Value<string>("emailConfirmationRequired");
                info = StringCipher.Encrypt(info);

                model.Info = info;
            }

            model.DisplayForm = displayForm;
            model.ConfirmationSuccess = confirmationSuccess;

            return PartialView(FullViewPath, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleRegisterSubmit(USNRegisterFormViewModel model)
        {
            System.Threading.Thread.Sleep(1000);

            var globalSettings = Umbraco.Content(model.GlobalSettingsID);

            string recaptchaReset = globalSettings.HasValue("googleReCAPTCHASiteKey") && globalSettings.HasValue("googleReCAPTCHASecretKey") ? "grecaptcha.reset();" : String.Empty;

            if (!ModelState.IsValid)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('{2}');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Required Field Error"))));
            }

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

            try
            {
                //Get encrypted information
                string info = StringCipher.Decrypt(model.Info);
                string[] infoArray = info.Split(',');
                bool adminApprovalRequired = Convert.ToBoolean(infoArray[0]);
                bool emailConfirmationRequired = Convert.ToBoolean(infoArray[1]);

                string username = model.RegisterModel.UsernameIsEmail ? model.RegisterModel.Email : model.RegisterModel.Username;
                string memberTypeAlias = string.IsNullOrEmpty(ConfigurationManager.AppSettings["USN_DefaultMemberType"]) ? "Member" : ConfigurationManager.AppSettings["USN_DefaultMemberType"];
                string memberGroupAlias = string.IsNullOrEmpty(ConfigurationManager.AppSettings["USN_DefaultMemberGroup"]) ? "Main Client" : ConfigurationManager.AppSettings["USN_DefaultMemberGroup"];

                MembershipCreateStatus membershipCreateStatus;
                model.RegisterModel.Name = string.IsNullOrEmpty(model.RegisterModel.Name) ? model.RegisterModel.Email : model.RegisterModel.Name;
                MembershipUser membershipUser = base.Members.RegisterMember(model.RegisterModel, out membershipCreateStatus, false);
                IMemberService memberService = Services.MemberService;

                switch (membershipCreateStatus)
                {
                    case MembershipCreateStatus.Success:
                        var user = memberService.GetByUsername(membershipUser.UserName);
                        user.IsApproved = adminApprovalRequired || emailConfirmationRequired ? false : true;
                        memberService.AssignRole(membershipUser.UserName, memberGroupAlias);
                        user.IsApproved = true;

                        if (emailConfirmationRequired)
                        {
                            var currentPage = Umbraco.Content(model.CurrentPageId);

                            var token = Guid.NewGuid().ToString();
                            user.SetValue("emailConfirmationToken", token);

                            string email = TempData["EmailConfirmation"] != null ? TempData["EmailConfirmation"].ToString() : String.Empty;
                            string confirmUrl = Request.Url.Scheme + "://" + Request.Url.Host + currentPage.Url() + "?id=" + user.Id + "&token=" + token;
                            string confirmLink = String.Format("<a href=\"{0}\">{1}</a>", confirmUrl, Umbraco.GetDictionaryValue("USN Register Form Confirm Email Link"));

                            if (email != String.Empty)
                            {
                                StringBuilder sb = new StringBuilder(email);
                                sb.Replace("[NAME]", user.Name);

                                if (!sb.ToString().Contains("[CONFIRMATIONLINK]"))
                                    sb.Append("<p>" + confirmLink + "</p>");
                                else
                                    sb.Replace("[CONFIRMATIONLINK]", confirmLink);

                                email = sb.ToString();
                            }
                            else
                            {
                                email = confirmLink;
                            }

                            string errorMessage = String.Empty;

                            //if (!SendMail(user.Email, email, Umbraco.GetDictionaryValue("USN Register Form Email Subject"), out errorMessage))
                            //{
                            //    return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Form Mail Send Error")), HttpUtility.JavaScriptStringEncode(errorMessage)));
                            //}
                        }

                        memberService.Save(user);

                        return JavaScript(String.Format("$('#SubmissionMessage_{0}').show();$('#Form_{0}').hide();$('#Error_{0}').hide();", model.UniqueID));
                    case MembershipCreateStatus.InvalidUserName:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Invalid Username"))));
                    case MembershipCreateStatus.InvalidPassword:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Invalid Password"))));
                    case MembershipCreateStatus.InvalidQuestion:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Invalid Question"))));
                    case MembershipCreateStatus.InvalidAnswer:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Invalid Answer"))));
                    case MembershipCreateStatus.InvalidEmail:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Invalid Email"))));
                    case MembershipCreateStatus.DuplicateUserName:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Duplicate Username"))));
                    case MembershipCreateStatus.DuplicateEmail:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Duplicate Email"))));
                    case MembershipCreateStatus.UserRejected:
                    case MembershipCreateStatus.InvalidProviderUserKey:
                    case MembershipCreateStatus.DuplicateProviderUserKey:
                    case MembershipCreateStatus.ProviderError:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Generic"))));
                    default:
                        return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Generic"))));
                }  
            }
            catch (Exception ex)
            {
                return JavaScript(String.Format("{0}$('#Error_{1}').show();$('#Error_{1}').html('<div class=\"info\"><p>{2}</p><p>{3}</p></div>');", recaptchaReset, model.UniqueID, HttpUtility.JavaScriptStringEncode(Umbraco.GetDictionaryValue("USN Register Error Generic")), HttpUtility.JavaScriptStringEncode(ex.Message)));
            }
        }

        #endregion

        public bool SendMail(string mailTo, string email, string subject, out string lsErrorMessage)
        {
            lsErrorMessage = String.Empty;

            try
            {
                //Create MailDefinition 
                MailDefinition md = new MailDefinition();

                //specify the location of template 
                md.BodyFileName = "/usn/emailtemplates/general.html";
                md.IsBodyHtml = true;

                //Build replacement collection to replace fields in template 
                ListDictionary replacements = new ListDictionary();
                replacements.Add("<% mailContent %>", email);

                //now create mail message using the mail definition object 
                System.Net.Mail.MailMessage msg = md.CreateMailMessage(mailTo, replacements, new System.Web.UI.Control());
                msg.Subject = subject;

                //this uses SmtpClient in 2.0 to send email, this can be configured in web.config file.
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                lsErrorMessage = ex.Message;
            }

            return false;
        }
    }


}