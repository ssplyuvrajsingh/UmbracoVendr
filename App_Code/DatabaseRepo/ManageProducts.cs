using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using USNWebsite.USNModels;
using System.IO;
using System.Linq;
using Vendr.Core.Api;
using Vendr.Core.Models;
using Vendr.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;
using Vendr.Core;
using FinCMS.App_Code.DatabaseRepo;

namespace USNWebsite.DatabaseRepo
{
    public class ManageProducts
    {
        #region Shop page methods 
        public _ProductPartialViewModel GetAllProductsByProvider(int providerId, MembershipHelper membershipHelper, UmbracoHelper umbracoHelper)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 9),
                new SqlParameter("@ProviderId", providerId)
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);

            return GetProductsModel(ds, membershipHelper, umbracoHelper, providerId);
        }

        public _ProductPartialViewModel GetAllProducts(int categoryId, MembershipHelper membershipHelper, UmbracoHelper umbracoHelper, int providerId)
        {
            int action = 0;
            if (categoryId > 0)
            {
                action = 8;
            }
            else
            {
                action = 14;
            }

            SqlParameter[] param =
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@ProductType", categoryId)
            };

            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetProductsModel(ds, membershipHelper, umbracoHelper, providerId);
        }
        #endregion

        #region Providers methods

        #region Products methods
        public string GetProducts(int memberId, UmbracoHelper umbracoHelper)
        {
            int action = 0;
            if (memberId > 0)
            {
                action = 2;
            }
            else
            {
                action = 6;
            }

            SqlParameter[] param =
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@MemberId", memberId),
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetProductsModel(ds, umbracoHelper);
        }

        public int ProductImageDelete(ProductFiles model)
        {
            SqlParameter[] param =
                {
                    new SqlParameter("@Action", 12),
                    new SqlParameter("@ProductFileId", model.ProductFileId),
                    new SqlParameter("@ProductVersionNumber", model.ProductVersionNumber)
                };
            int res = (int)SqlHelper.ExecuteScalar(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? 1 : 0;
        }

        string spProductsTable = "spProductsTable";
        public int AddUpdateProduct(USNCreateProductFormViewModel model)
        {
            int res = 0;

            SqlParameter[] param =
                {
                    new SqlParameter("@Action", model.Action),
                    new SqlParameter("@ProductId", model.ProductId),
                    new SqlParameter("@MemberId", model.MemberId),
                    new SqlParameter("@ProductName", model.ProductName),
                    new SqlParameter("@ProductType", model.ProductTypeId),
                    new SqlParameter("@ProductPrice", model.ProductPrice),
                    new SqlParameter("@ProductDescription", model.ProductDescription),
                    new SqlParameter("@ProductFiles", model.ProductFiles),
                    new SqlParameter("@ProductCreation", DateTime.Now),
                    new SqlParameter("@ProductLogoLarge", model.ProductLogoLarge),
                    new SqlParameter("@ProductLogoSmall", model.ProductLogoSmall),
                    new SqlParameter("@ProductScreenshot", model.ProductScreenshot),
                    new SqlParameter("@ProductScreenshotDescription", model.ProductScreenshotDescription),
                    new SqlParameter("@ProductProtectionTrialLength", model.ProductProtectionTrialLength),
                    new SqlParameter("@ProductProtectionNumberOfTrials", model.ProductProtectionNumberOfTrials),
                    new SqlParameter("@ProductStatus", model.ProductStatus),
                    new SqlParameter("@ProductVersionNumber", model.ProductNewVersionNumber)
                };

            res = SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? 1 : 0;
        }

        public DataSet GetProductById(int productId)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 3),
                new SqlParameter("@ProductId", productId),
            };

            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);

            return ds;
        }

        public DataSet GetProductByNodeId(int productNodeId)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 10),
                new SqlParameter("@ProductNodeId", productNodeId),
            };

            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);

            return ds;
        }

        public DataSet GetProductFilesByProductId(int productId)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 4),
                new SqlParameter("@ProductId", productId),
            };

            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);

            return ds;
        }

        public DataSet GetAllProductsForProvider(int categoryId)
        {
            int action = 0;
            if (categoryId > 0)
            {
                action = 8;
            }
            else
            {
                action = 6;
            }
            SqlParameter[] param =
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@ProductType", categoryId)
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);

            return ds;
        }

        public bool UpdateProductStatus(int productId, string productStatus, int productNodeId, string productNodeKey = null)
        {
            int res = 0;

            SqlParameter[] param =
                {
                    new SqlParameter("@Action", 7),
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@ProductStatus", productStatus),
                    new SqlParameter("@ProductNodeId", productNodeId),
                    new SqlParameter("@ProductNodeKey", productNodeKey)
                };
            res = SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? true : false;
        }

        public int AddProductNewVersion(USNCreateProductFormViewModel model)
        {
            int res = 0;

            SqlParameter[] param =
                {
                    new SqlParameter("@Action", model.Action),
                    new SqlParameter("@ProductId", model.ProductId),
                    new SqlParameter("@ProductFiles", model.ProductFiles),
                    new SqlParameter("@ProductVersionNumber", model.ProductNewVersionNumber),
                    new SqlParameter("@ProductStatus", model.ProductStatus),
                    new SqlParameter("@CreatedVersionDateTime", DateTime.Now)
                };
            res = SqlHelper.ExecuteNonQuery(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return res > 0 ? 1 : 0;
        }

        public void GetProductFiles(ref USNCreateProductFormViewModel model)
        {
            if (model.ProductFilesList?.Count() > 0)
            {
                SqlParameter[] param =
                {
                    new SqlParameter("@Action", 11),
                    new SqlParameter("@ProductId", model.ProductId)
                };

                DataSet res = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);


                List<ProductVersionFiles> productVersionFilesList = new List<ProductVersionFiles>();

                if (!object.Equals(res.Tables[0], null))
                {
                    if (res.Tables[0].Rows.Count > 0)
                    {

                        for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                        {
                            var rows = res.Tables[0].Rows[i];
                            var obj = new ProductVersionFiles()
                            {
                                ProductFileId = (int)rows["ProductFileId"],
                                FileUrl = rows["ProductFile"].ToString().Substring(1),
                                FileName = Path.GetFileName(rows["ProductFile"].ToString()),
                                ProductVersionNumber = string.IsNullOrEmpty(rows["ProductVersionNumber"].ToString()) ? "0" : rows["ProductVersionNumber"].ToString(),
                                CreatedVersionDateTime = rows["CreatedVersionDateTime"].ToString()
                            };
                            productVersionFilesList.Add(obj);
                        }
                    }
                }
                model.ProductVersionGroupList = productVersionFilesList.GroupBy(x => x.ProductVersionNumber).Select(x => new ProductVersionGroupList
                {
                    ProductVersionNumber = x.Key,
                    productVersionFiles = x.ToList()
                }).ToList();

                model.ProductVersionGroupList.Reverse();
            }
        }
        #endregion

        #region Order methods
        public List<USNProductOrdersViewModel> GetOrdersByProviderId(int memberId)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 15),
                new SqlParameter("@MemberId", memberId),
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetOrdersByProviderIdModel(ds);
        }

        public OrderReadOnly GetOrderDetails(string orderId)
        {
            return VendrApi.Instance.GetOrder(Guid.Parse(orderId));
        }

        public string GetOrderStatus(string orderStatusId, string orderId, string paymentMethodId, IPaymentMethodService _paymentMethodService, string paymentStatus)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", 16),
                new SqlParameter("@OrderId", orderId)
            };
            DataSet ds = SqlHelper.ExecuteDataset(Connection.ConnectionString, CommandType.StoredProcedure, spProductsTable, param);
            return GetOrderStatusModel(ds, orderStatusId, paymentMethodId, _paymentMethodService, paymentStatus);
        }
        #endregion

        #endregion

        #region Convert dataSet to model for shop page
        public _ProductPartialViewModel GetProductsModel(DataSet dataset, MembershipHelper membershipHelper, UmbracoHelper umbracoHelper, int providerId)
        {
            _ProductPartialViewModel result = new _ProductPartialViewModel();
            result.ProductModel = new List<USNCreateProductFormViewModel>();
            if (dataset.Tables[0].Rows?.Count > 0)
            {
                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    var rows = dataset.Tables[0].Rows[i];
                    var member = membershipHelper.GetById(int.Parse(rows["MemberId"].ToString()));
                    var productNodeId = rows["ProductNodeId"].ToString();
                    var productNode = umbracoHelper.Content(productNodeId);
                    if (productNode != null)
                    {
                        var formattedPrice = IPriceExtensions.Formatted(VendrPublishedContentExtensions.CalculatePrice(productNode));

                        result.ProductModel.Add(new USNCreateProductFormViewModel
                        {
                            ProductId = int.Parse(rows["ProductId"].ToString()),
                            ProductName = rows["ProductName"].ToString(),
                            ProductLogoLarge = !string.IsNullOrWhiteSpace(rows["ProductLogoLarge"].ToString()) ? rows["ProductLogoLarge"].ToString().Substring(1) : string.Empty,
                            ProductCreationDate = Convert.ToDateTime(rows["ProductCreation"]).ToString("dd/MM/yyyy HH:mm"),
                            ProductPrice = float.Parse(rows["ProductPrice"].ToString()),
                            FormattedPrice = new FormattedPriceProduct
                            {
                                CurrencyId = formattedPrice.CurrencyId.ToString(),
                                WithoutTax = formattedPrice.WithoutTax,
                                Tax = formattedPrice.Tax,
                                WithTax = formattedPrice.WithTax,
                                WithoutTaxNoSymbol = formattedPrice.WithoutTaxNoSymbol,
                                TaxNoSymbol = formattedPrice.TaxNoSymbol,
                                WithTaxNoSymbol = formattedPrice.WithTaxNoSymbol
                            },
                            ProductStatus = rows["ProductStatus"].ToString(),
                            ProviderName = member.Name,
                            MemberId = int.Parse(rows["MemberId"].ToString()),
                            ProductNodeUrl = productNode != null ? productNode.Url() : string.Empty
                        });
                    }
                }
            }
            result.ProviderId = providerId;
            result.ProviderName = result.ProductModel.Select(x => x.ProviderName).FirstOrDefault();
            return result;
        }
        #endregion

        #region Convert dataset to model for providers

        #region Poducts model
        public string GetProductsModel(DataSet dataset, UmbracoHelper umbracoHelper)
        {
            StringBuilder htmlTable = new StringBuilder();
            htmlTable.Append("<table class='table'><thead class='thead-light'>");
            if (dataset.Tables[0].Rows?.Count > 0)
            {
                htmlTable.Append("<tr><th> Product Name </th><th> Date/Time Added </th><th> Average Rating </th><th> Price </th><th> Number Sold </th><th> Status </th><th> Action </th></tr></thead><tbody>");
                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    var rows = dataset.Tables[0].Rows[i];
                    var productNodeId = rows["ProductNodeId"].ToString();



                    htmlTable.Append("<tr id=" + rows["ProductId"] + ">");
                    htmlTable.Append("<td class='productId d-none'>" + int.Parse(rows["ProductId"].ToString()) + "</td>");
                    htmlTable.Append("<td>" + rows["ProductName"].ToString() + "</td>");
                    htmlTable.Append("<td>" + Convert.ToDateTime(rows["ProductCreation"]).ToString("dd/MMMM/yyyy hh:mm tt") + "</td>");
                    htmlTable.Append("<td>-</td>");
                    var productNode = umbracoHelper.Content(productNodeId);
                    if (productNode != null)
                    {

                        var formattedPrice = IPriceExtensions.Formatted(VendrPublishedContentExtensions.CalculatePrice(productNode));
                        htmlTable.Append("<td>" + formattedPrice.WithTax + "</td>");
                    }
                    else
                    {
                        htmlTable.Append("<td>" + rows["ProductPrice"].ToString() + "</td>");
                    }
                    htmlTable.Append("<td>-</td>");
                    htmlTable.Append("<td>" + rows["ProductStatus"].ToString() + "</td>");
                    htmlTable.Append("<td><a href='/create-product/?productId=" + int.Parse(rows["ProductId"].ToString()) + "' class='btn btn-primary'>Edit Product</a></td>");
                    htmlTable.Append("</tr>");
                }
                htmlTable.Append("</tbody></table>");
            }
            else
            {
                htmlTable.Append("<tr><td style='font-weight:bold;color:white;'>No Products Created!</td></tr></tbody></table>");
            }
            return htmlTable.ToString();
        }
        #endregion

        #region Order list model
        public List<USNProductOrdersViewModel> GetOrdersByProviderIdModel(DataSet dataSet)
        {
            List<USNProductOrdersViewModel> productOrderList = new List<USNProductOrdersViewModel>();
            if (dataSet.Tables[0].Rows?.Count > 0)
            {

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    var rows = dataSet.Tables[0].Rows[i];
                    var obj = new USNProductOrdersViewModel()
                    {
                        FullName = rows["customerFirstName"].ToString() + " " + rows["customerLastName"],
                        OrderStatus = rows["orderStatus"].ToString(),
                        OrderColor = rows["orderColor"].ToString(),
                        PaymentStatus = rows["paymentStatus"].ToString(),
                        Quantity = rows["quantity"].ToString(),
                        Amount = rows["amountAuthorized"].ToString(),
                        OrderId = rows["orderId"].ToString(),
                        CreateDate = (DateTime)rows["createDate"]
                    };
                    productOrderList.Add(obj);
                }
            }
            return productOrderList;
        }

        public string GetOrderStatusModel(DataSet dataSet, string orderStatusId, string paymentMethodId, IPaymentMethodService _paymentMethodService, string paymentStatus)
        {
            StringBuilder htmlDdl = new StringBuilder();
            if (dataSet.Tables[1].Rows.Count > 0)
            {
                htmlDdl.Append("<div id='DdlDiv'>");
                htmlDdl.Append("<select onchange='ChangeOrderStatus($(this))'>");
                for (int i = 0; i < dataSet.Tables[1].Rows.Count; i++)
                {
                    var rows = dataSet.Tables[1].Rows[i];
                    if (orderStatusId == rows["Id"].ToString())
                    {
                        htmlDdl.Append($"<option selected value='{rows["Id"].ToString()}'>{rows["Name"].ToString()}</option>");
                    }
                    else
                    {
                        htmlDdl.Append($"<option value='{rows["Id"].ToString()}'>{rows["Name"].ToString()}</option>");
                    }
                }
                htmlDdl.Append("</select>");
                htmlDdl.Append($"</div>");
            }
            htmlDdl.Append($"<div id='tr-payment'>");
            var pay = _paymentMethodService.GetPaymentMethod(Guid.Parse(paymentMethodId));
            if (paymentStatus == "Authorized" && pay.CanCancelPayments && pay.CanCapturePayments)
            {
                htmlDdl.Append($"<input type='button' value='Cancel Payment' onclick='ChangePaymentStaus(\"Cancelled\")' />");
                htmlDdl.Append($"<input type='button' value='Captur Payment' onclick='ChangePaymentStaus(\"Captured\")' />");
            }
            else if (paymentStatus == "Captured" && pay.CanRefundPayments)
            {
                htmlDdl.Append($"<input type='button' value='Refund Payment' onclick='ChangePaymentStaus(\"Refunded\")' />");
            }
            else if (paymentStatus == "Captured")
            {
                htmlDdl.Append($"<span>Payment Captured</span>");
            }
            else if (paymentStatus == "Cancelled")
            {
                htmlDdl.Append($"<span>Payment Cancelled</span>");
            }
            else if (paymentStatus == "Refunded")
            {
                htmlDdl.Append($"<span>Payment Refunded</span>");
            }
            else if (paymentStatus == "PendingExternalSystem")
            {
                htmlDdl.Append($"<span>Payment Pending</span>");
            }
            else if (paymentStatus == "Error")
            {
                htmlDdl.Append($"<span>Payment Error</span>");
            }
            htmlDdl.Append("</div>");
            return htmlDdl.ToString();
        }
        #endregion

        #endregion
    }
}
