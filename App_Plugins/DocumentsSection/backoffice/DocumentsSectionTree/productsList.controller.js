angular.module("umbraco").controller("ProductsListController",
    function ($scope, $routeParams, ProductsListResource, notificationsService, navigationService) {
        $scope.loaded = false;
        $scope.AnyRecord = true;
        $scope.ProductsList = [];
        ProductsListResource.getAllProducts().then(function (response) {
            $scope.ProductsList = response.data;
            if (response.data[0].Value.length === 0 && response.data[1].Value.length === 0) {
                $scope.AnyRecord = false;
            }
        });
    });