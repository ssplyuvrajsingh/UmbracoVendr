using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USNWebsite.USNModels
{
    public class USNProviderWalletModel
    {
       public int ProviderId { get; set; }
       public string ProviderBalance { get; set; }
       public string WithdrawalPending { get; set; }
       public string TotalPaid { get; set; }
    }

    public class ProviderPaymentLogsModel
    {
        public int LogId { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public DateTime? LogDate { get; set; }
        public string LogDateTime { get; set; }
        public string GroupByDate { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderId { get; set; }
    }

    public class ProviderBankDetailsModel
    {
        public int ProviderId { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string AccountHolderName { get; set; }
    }

    public class UserDetailsModel
    {
        public int LogId { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string AccountHolderName { get; set; }
    }
}