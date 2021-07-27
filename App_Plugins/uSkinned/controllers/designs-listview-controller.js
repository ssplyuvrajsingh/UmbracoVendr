// Author: uSkinned
// License: uSkinned Commercial License (https://uSkinned.net/license)

angular.module("umbraco").controller("USN.DesignListViewController", function ($scope, listViewHelper, $location) {

    $scope.viewItem = function (item) {

        $location.url(item.editPath);

    }

});