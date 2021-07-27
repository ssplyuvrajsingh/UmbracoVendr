angular.module("umbraco").controller("RequestedUserList.RequestedUserListDashboardController",
    function ($scope, $routeParams, RequestedUserListResource, notificationsService, navigationService) {
        $scope.loaded = false;
        $scope.RequestedUserList = [];
        RequestedUserListResource.getRequestedUserList().then(function (response) {
            $scope.RequestedUserList = response.data;
        });
    });