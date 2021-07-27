using FinCMS.App_Code.DatabaseRepo;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using USNWebsite.USNModels;

namespace FinCMS.App_Code.USNControllers
{
    public class USNProviderWalletSurfaceController : SurfaceController
    {
        ManageProviderWallet manageProviderWallet = new ManageProviderWallet();

        [HttpGet]
        public ActionResult GetProviderWallet()
        {
            if (Members.IsLoggedIn())
            {
                return PartialView("/Views/Partials/Providers/_ProviderWallet.cshtml", manageProviderWallet.GetProviderWallet(Members.GetCurrentMemberId()));
            }
            else
            {
                return PartialView("/Views/Partials/Providers/_ProviderWallet.cshtml", null);
            }
        }

        [HttpGet]
        public ActionResult GetWithdrawalRequest()
        {
            if (Members.IsLoggedIn())
            {
                var res = manageProviderWallet.GetProviderBankDetails(Members.GetCurrentMemberId());
                if (res.ProviderId > 0 && !string.IsNullOrEmpty(res.AccountHolderName) && !string.IsNullOrEmpty(res.BankAccountNumber) && !string.IsNullOrEmpty(res.IFSC))
                {
                    return PartialView("/Views/Partials/Providers/_WithdrawalRequest.cshtml");
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Content("logout");
            }
        }

        [HttpPost]
        public int PostWithdrawalRequest(decimal amount)
        {
            if (Members.IsLoggedIn())
            {
                return manageProviderWallet.PostWithdrawalRequest(Members.GetCurrentMemberId(), amount);
            }
            else
            {
                return 4;
            }
        }

        [HttpGet]
        public ActionResult GetPaymentHistoryByProvider()
        {
            return PartialView("/Views/Partials/Providers/_PaymentHistory.cshtml", manageProviderWallet.GetPaymentHistoryByProvider(Members.GetCurrentMemberId()));
        }

        [HttpGet]
        public ActionResult EditProviderBankDetails()
        {
            if (Members.IsLoggedIn())
            {
                return PartialView("/Views/Partials/Providers/_BankAccountDetails.cshtml", manageProviderWallet.GetProviderBankDetails(Members.GetCurrentMemberId()));
            }
            else
            {
                return PartialView("/Views/Partials/Providers/_BankAccountDetails.cshtml", null);
            }
        }

        [HttpPost]
        public int EditProviderBankDetails(ProviderBankDetailsModel model)
        {
            if (Members.IsLoggedIn())
            {
                model.ProviderId = Members.GetCurrentMemberId();
                return manageProviderWallet.EditProviderBankDetails(model);
            }
            else
            {
                return 2;
            }
        }
    }
}