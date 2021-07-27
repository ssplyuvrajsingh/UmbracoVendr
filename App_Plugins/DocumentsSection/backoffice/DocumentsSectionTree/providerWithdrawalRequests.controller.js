angular.module("umbraco").controller("ProviderWithdrawalRequestsController",
    function ($scope, $routeParams, ProviderWalletResource, notificationsService, navigationService) {
        $scope.loaded = false;
        $scope.AnyRecord = true;
        $scope.WalletUserList = [];
        $scope.UserDetail = null;

        ProviderWalletResource.getWithdrawalRequestsList().then(function (response) {
            $scope.WalletUserList = response.data;
            if (response.data.length === 0 && response.data.length === 0) {
                $scope.AnyRecord = false;
            }
        });

        providerWithdrawalDetails = function (ids) {
            var values = ids.split("_");
            logId = values[0];
            providerId = values[1];

            
            $("#div_providerWithdrawalRequest").hide();
            $("#div_ProviderDetails").show();
            $("#btn_approved").removeAttr('disabled');
            $("#btn_disapproved").removeAttr('disabled');

            ProviderWalletResource.getProviderWithdrawalDetails(logId, providerId).then(function (response) {
                
                $scope.UserDetail = response.data;
                console.log($scope.UserDetail.LogId);
                $("#sp_ProviderName").text($scope.UserDetail.ProviderName);
                $("#sp_OrderId").text($scope.UserDetail.OrderId);
                $("#sp_WithdrawalAmount").text($scope.UserDetail.Amount);
                $("#sp_AccountNumber").text($scope.UserDetail.BankAccountNumber);
                $("#sp_IFSC").text($scope.UserDetail.IFSC);
                $("#sp_AccountHolderName").text($scope.UserDetail.AccountHolderName);
                $("#btn_approved").val($scope.UserDetail.LogId);
                $("#btn_disapproved").val($scope.UserDetail.LogId);
            });
        }

        updatePaymentStatus = function (logId, paymentStatus) {
            if (confirm('Are you sure you want to payment is ' + paymentStatus + ".")) {
                $("#btn_approved").attr('disabled', 'disabled');
                $("#btn_disapproved").attr('disabled', 'disabled');
                ProviderWalletResource.updatePaymentStatus(logId, paymentStatus).then(function (response) {
                    if (response.data === "approved" || response.data === "disapproved") {
                        notificationsService.success("Payment " + response.data + " successfully.");
                        ProviderWalletResource.getWithdrawalRequestsList().then(function (response) {
                            $scope.WalletUserList = response.data;
                            if (response.data.length === 0 && response.data.length === 0) {
                                $scope.AnyRecord = false;
                            }
                        });
                    }
                    else {
                        notificationsService.error("Error");
                        $("#btn_approved").removeAttr('disabled');
                        $("#btn_disapproved").removeAttr('disabled');
                    }

                });
            }
        }
    });