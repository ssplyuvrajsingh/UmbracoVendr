angular.module("umbraco.resources")
    .factory("RequestedUserListResource", function ($http) {
        return {
            getUsersList: function (memberType) {
                return $http.get("Surface/RequestedUsersList/GetAllRequestedUsers?memberType=" + memberType);
            },
            updateProviderStatus: function (memberId, status) {
                return $http.post("Surface/RequestedUsersList/UpdateProviderStatus?memberId=" + memberId + "&status=" + status);
            },
            DeleteProvider: function (memberId) {
                return $http.post("Surface/RequestedUsersList/DeleteProvider?memberId=" + memberId);
            }
        };
    });