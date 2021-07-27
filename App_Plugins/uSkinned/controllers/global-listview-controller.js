// Author: uSkinned
// License: uSkinned Commercial License (https://uSkinned.net/license)

angular.module("umbraco").controller("USN.GlobalListViewController", function ($scope, $location) {

    $scope.viewItem = function (item) {

        $location.url(item.editPath);

    }

});