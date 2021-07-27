angular.module('umbraco').controller('DocumentsSectionController', ['$scope', 'notificationsService', '$http', function ($scope, notificationsService, $http) {
    $scope.Name = 'Hello world';

    $scope.AnyMember = true;
    $http.get("Surface/RequestedUsersList/MembersUser")
        .then(function (response) {
            if (response.data.length > 0) {
                if (response.data === "0") {
                    $scope.AnyMember = false;
                }
            }
            else {
                notificationsService.error("Error");
            }
        });

    createDummyData = function () {
        $(".dummyData").attr('disabled', 'disabled');
        if (confirm('Are you sure you want to create dummy data?')) {

            $http.get("Surface/RequestedUsersList/CreateDummyData")
                .then(function (response) {
                    if (response.data === "Success") {
                        notificationsService.success("Created successfully");
                    }
                    else {
                        notificationsService.error("Error");
                    }
          });
        }
    }

}]);