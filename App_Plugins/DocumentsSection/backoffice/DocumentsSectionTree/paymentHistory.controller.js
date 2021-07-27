angular.module("umbraco").controller("paymentHistoryController",
    function ($scope, $routeParams, ProviderWalletResource, notificationsService, navigationService) {
        $scope.loaded = false;
        $scope.AnyRecord = true;
        $scope.WalletUserList = [];
        ProviderWalletResource.getPaymentHistoryByAdmin().then(function (response) {
            $scope.WalletUserList = response.data;
            if (response.data.length === 0 && response.data.length === 0) {
                $scope.AnyRecord = false;
            }
        });
    });