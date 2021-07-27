using FinCMS.App_Code.DatabaseRepo;
using System.Web.Mvc;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace USNWebsite.USNControllers
{
    public class ProviderWalletController : SurfaceController
    {
        private readonly IMemberService _memberService;
        ManageProviderWallet manageProviderWallet = new ManageProviderWallet();

        public ProviderWalletController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public ActionResult GetWithdrawalRequestsList()
        {
            var result = manageProviderWallet.GetPaymentHistoryByAdmin(_memberService, 3);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProviderWithdrawalDetailsByAdmin(int logId, int providerId)
        {
            var result = manageProviderWallet.GetProviderWithdrawalDetailsByAdmin(logId, providerId, _memberService);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string UpdatePaymentStatus(int logId, string paymentStatus)
        {
             return manageProviderWallet.UpdateWithdrawalRequestByAdmin(logId, paymentStatus);
        }

        [HttpGet]
        public ActionResult GetPaymentHistoryByAdmin()
        {
            var result = manageProviderWallet.GetPaymentHistoryByAdmin(_memberService, 6);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}