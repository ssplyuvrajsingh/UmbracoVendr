angular.module("umbraco.resources")
    .factory("ProviderWalletResource", function ($http) {
        return {
            getWithdrawalRequestsList: function () {
                return $http.get("Surface/ProviderWallet/GetWithdrawalRequestsList");
            },
            updatePaymentStatus: function (logId, paymentStatus) {
                return $http.get("Surface/ProviderWallet/UpdatePaymentStatus?logId=" + logId + "&paymentStatus=" + paymentStatus);
            },
            getPaymentHistoryByAdmin: function () {
                return $http.get("Surface/ProviderWallet/GetPaymentHistoryByAdmin");
            },
            getProviderWithdrawalDetails: function (logId,providerId) {
                return $http.get("Surface/ProviderWallet/GetProviderWithdrawalDetailsByAdmin?logId=" + logId + "&providerId=" + providerId);
            }
        };
    });