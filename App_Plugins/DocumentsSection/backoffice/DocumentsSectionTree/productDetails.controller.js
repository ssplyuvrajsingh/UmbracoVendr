angular.module("umbraco").controller("ProductDetailsController",
    function ($scope, $routeParams, ProductsListResource, notificationsService, navigationService, $location) {
        $scope.loaded = false;
        $scope.AnyRecord = true;
        $scope.ProductDetails = null;
        $productId = $location.search().productId;

        ProductsListResource.getProductDetails($productId).then(function (response) {
            $scope.ProductDetails = response.data;
            console.log($scope.ProductDetails.IsFormattedPrice);
            if (response.data.length === 0) {
                $scope.AnyRecord = false;
            }

            setTimeout(function () {
                const innerHtml = $(".product_description").attr("data-innerhtml");
                $(".product_description").html(innerHtml).removeAttr("data-innerhtml");

                $(".productStatusDDL").val($(".productStatus").val());
            }, 500);

            $(".updateProductStatus").click(function () {
                var productId = $(".productId").val();
                var productStatus = $(".productStatusDDL").val();

                ProductsListResource.updateProductDetails(productId, productStatus).then(function (response) {
                    if (response.data === "True") {
                        notificationsService.success("Saved");
                    }
                    else {
                        notificationsService.error("Error");
                    }
                });
            });
        });
    });