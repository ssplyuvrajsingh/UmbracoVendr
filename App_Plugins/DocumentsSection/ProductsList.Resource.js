angular.module("umbraco.resources")
    .factory("ProductsListResource", function ($http) {
        return {
            getAllProducts: function () {
                return $http.get("Surface/ProductsList/GetAllProducts");
            },
            getProductDetails: function (productId) {
                return $http.get("Surface/ProductsList/GetProductDetails?productId=" + productId);
            },
            updateProductDetails: function (productId, productStatus) {
                return $http.post("Surface/ProductsList/UpdateProductDetails?productId=" + productId + "&productStatus=" + productStatus);
            }
        };
    });