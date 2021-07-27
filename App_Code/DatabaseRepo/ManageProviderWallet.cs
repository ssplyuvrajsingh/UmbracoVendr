using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Umbraco.Core.Services;
using USNWebsite.USNModels;

namespace FinCMS.App_Code.DatabaseRepo
{
    public class ManageProviderWallet
    {
        string spProductsTable = "spProductWallet";

        #region Provider methods
        public USNProviderWalletModel GetProviderWallet(int providerId)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 1),
                new SqlParameter("@ProviderId", providerId)
            };

            DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetProviderWalletModel(res);
        }

        public int PostWithdrawalRequest(int providerId, decimal amount)
        {
            var data = GetProviderWallet(providerId);
            var balance = Convert.ToDecimal(data.ProviderBalance);
            if (amount == 0)
            {
                return 2;
            }
            else if (balance >= amount)
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Action", 2),
                    new SqlParameter("@ProviderId", providerId),
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@OrderId", Guid.NewGuid().ToString())
                };
                int res = (int)SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
                return res > 0 ? 1 : 0;
            }
            else
            {
                return 3;
            }
        }
        public List<ProviderPaymentLogsModel> GetPaymentHistoryByProvider(int providerId)
        {
            SqlParameter[] param =
            {
              new SqlParameter("@Action", 5),
              new SqlParameter("@ProviderId", providerId)
            };

            DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetPaymentHistoryModelByProvider(res);
        }

        public int EditProviderBankDetails(ProviderBankDetailsModel model)
        {
            SqlParameter[] param =
                {
                    new SqlParameter("@Action", 7),
                    new SqlParameter("@ProviderId", model.ProviderId),
                    new SqlParameter("@BankAccountNumber", model.BankAccountNumber),
                    new SqlParameter("@IFSC", model.IFSC),
                    new SqlParameter("@AccountHolderName", model.AccountHolderName)
                };
            int res = (int)SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? 1 : 0;
        }

        public ProviderBankDetailsModel GetProviderBankDetails(int providerId)
        {
            SqlParameter[] param =
                {
                    new SqlParameter("@Action", 8),
                    new SqlParameter("@ProviderId", providerId)
                };
            DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetProviderBankDetailsModel(res);
        }
        #endregion

        #region Convert DataSet To Model for Provider
        public USNProviderWalletModel GetProviderWalletModel(DataSet res)
        {
            USNProviderWalletModel obj = new USNProviderWalletModel();
            if (res.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    var rows = res.Tables[0].Rows[i];
                    obj = new USNProviderWalletModel()
                    {
                        ProviderId = (int)rows["ProviderId"],
                        ProviderBalance = rows["ProviderBalance"].ToString(),
                        WithdrawalPending = rows["WithdrawalPending"].ToString(),
                        TotalPaid = rows["TotalPaid"].ToString()
                    };
                }
            }
            obj.ProviderBalance = string.IsNullOrEmpty(obj.ProviderBalance) ? "0" : String.Format("{0:0.##}", Convert.ToDecimal(obj.ProviderBalance));
            obj.WithdrawalPending = string.IsNullOrEmpty(obj.WithdrawalPending) ? "0" : String.Format("{0:0.##}", Convert.ToDecimal(obj.WithdrawalPending));
            obj.TotalPaid = string.IsNullOrEmpty(obj.TotalPaid) ? "0" : String.Format("{0:0.##}", Convert.ToDecimal(obj.TotalPaid));
            return obj;
        }
        public List<ProviderPaymentLogsModel> GetPaymentHistoryModelByProvider(DataSet res)
        {
            List<ProviderPaymentLogsModel> providerPaymentLogsModels = new List<ProviderPaymentLogsModel>();
            if (res.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    var rows = res.Tables[0].Rows[i];
                    var obj = new ProviderPaymentLogsModel()
                    {
                        Description = rows["Description"].ToString(),
                        Amount = rows["Amount"].ToString(),
                        LogDate = (DateTime)rows["LogDate"],
                        PaymentStatus = rows["PaymentStatus"].ToString(),
                        OrderId = rows["OrderId"].ToString()
                    };
                    obj.Amount = String.Format("{0:0.##}", Convert.ToDecimal(obj.Amount));
                    obj.LogDateTime = obj.LogDate?.ToString("dddd, dd MMMM yyyy hh:mm tt");
                    obj.GroupByDate = obj.LogDate?.ToString("MMMM yyyy");

                    providerPaymentLogsModels.Add(obj);
                }
            }
            return providerPaymentLogsModels;
        }

        public ProviderBankDetailsModel GetProviderBankDetailsModel(DataSet res)
        {
            ProviderBankDetailsModel obj = new ProviderBankDetailsModel();
            if (res.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    var rows = res.Tables[0].Rows[i];
                    obj = new ProviderBankDetailsModel()
                    {
                        ProviderId = (int)rows["ProviderId"],
                        BankAccountNumber = rows["BankAccountNumber"].ToString(),
                        IFSC = rows["IFSC"].ToString(),
                        AccountHolderName = rows["AccountHolderName"].ToString()
                    };
                }
            }
            return obj;
        }
        #endregion

        #region Admin methods
        public List<ProviderPaymentLogsModel> GetPaymentHistoryByAdmin(IMemberService memberService, int action)
        {
            SqlParameter[] param =
            {
              new SqlParameter("@Action", action)
            };

            DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetPaymentHistoryModelByAdmin(res, memberService);
        }

        public string UpdateWithdrawalRequestByAdmin(int LogId, string paymentStatus)
        {
            SqlParameter[] param =
            {
                    new SqlParameter("@Action", 4),
                    new SqlParameter("@LogId", LogId),
                    new SqlParameter("@PaymentStatus", paymentStatus)
            };

            int res = (int)SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? paymentStatus : "Please try again!";
        }

        public UserDetailsModel GetProviderWithdrawalDetailsByAdmin(int LogId, int ProviderId,IMemberService memberService)
        {
            SqlParameter[] param =
            {
                    new SqlParameter("@Action", 9),
                    new SqlParameter("@LogId", LogId),
                    new SqlParameter("@ProviderId", ProviderId)
            };

            DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetProviderWithdrawalDetailsModelByAdmin(res, memberService);
        }
        #endregion

        #region Convert DataSet To Model for Admin
        public List<ProviderPaymentLogsModel> GetPaymentHistoryModelByAdmin(DataSet res, IMemberService memberService)
        {
            List<ProviderPaymentLogsModel> providerPaymentLogsModels = new List<ProviderPaymentLogsModel>();
            if (res.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    var rows = res.Tables[0].Rows[i];
                    var obj = new ProviderPaymentLogsModel()
                    {
                        LogId = (int)rows["LogId"],
                        ProviderId = (int)rows["ProviderId"],
                        Description = rows["Description"].ToString(),
                        Amount = rows["Amount"].ToString(),
                        LogDate = (DateTime)rows["LogDate"],
                        PaymentStatus = rows["PaymentStatus"].ToString(),
                        OrderId = rows["OrderId"].ToString()
                    };

                    var member = memberService.GetById(obj.ProviderId);
                    obj.ProviderName = member.Name;
                    obj.Amount = String.Format("{0:0.##}", Convert.ToDecimal(obj.Amount));
                    obj.LogDateTime = obj.LogDate?.ToString("dddd, dd MMMM yyyy hh:mm tt");

                    providerPaymentLogsModels.Add(obj);
                }
            }
            return providerPaymentLogsModels.OrderByDescending(x => x.LogDate).ToList();
        }

        public UserDetailsModel GetProviderWithdrawalDetailsModelByAdmin(DataSet res, IMemberService memberService)
        {
            var obj = new UserDetailsModel();
            if (res.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    var rows = res.Tables[0].Rows[i];
                    obj = new UserDetailsModel
                    {
                        LogId = (int)rows["LogId"],
                        ProviderId = (int)rows["ProviderId"],
                        ProviderName = string.Empty,
                        Amount = rows["Amount"].ToString(),
                        OrderId = rows["OrderId"].ToString(),
                        BankAccountNumber = rows["BankAccountNumber"].ToString(),
                        IFSC = rows["IFSC"].ToString(),
                        AccountHolderName = rows["AccountHolderName"].ToString()
                    };

                    var member = memberService.GetById(obj.ProviderId);
                    obj.ProviderName = member.Name;
                    obj.Amount = String.Format("{0:0.##}", Convert.ToDecimal(obj.Amount));

                }
            }
            return obj;
        }
        #endregion
    }
}