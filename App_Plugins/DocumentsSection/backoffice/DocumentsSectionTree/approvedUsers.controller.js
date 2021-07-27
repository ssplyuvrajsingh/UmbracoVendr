angular.module("umbraco").controller("RequestedUserList.ApprovedUsersController",
    function ($scope, $routeParams, RequestedUserListResource, notificationsService, navigationService) {
        $scope.loaded = false;
        $scope.AnyRecord = true;
        $scope.RequestedUserList = [];
        RequestedUserListResource.getUsersList("approvedUsers").then(function (response) {
            $scope.RequestedUserList = response.data;
            if (response.data.length === 0) {
                $scope.AnyRecord = false;
            }
            setTimeout(function () {
                bindHtml();
            }, 1000);
        });

        $("#updateMemberStatus").click(function () {
            var memberId = $(this).parent().find("#updateMemberId").val();
            var status = $("#providerStatusDDL").val();

            RequestedUserListResource.updateProviderStatus(memberId, status).then(function (response) {
                if (response.data === "Success") {
                    notificationsService.success("Saved");
                    $("#providerStatusDDL").val("Waiting for Approval");
                }
                else {
                    notificationsService.error("Error");
                    $("#providerStatusDDL").val("Waiting for Approval");
                }

                $("#documentsSection").addClass("d-none");
                RequestedUserListResource.getUsersList("approvedUsers").then(function (response) {
                    $scope.RequestedUserList = response.data;
                    setTimeout(function () {
                        bindHtml();
                    }, 1000);
                    if (response.data.length === 0) {
                        $scope.AnyRecord = false;
                    }
                });
            });
        });

        $("#deleteMember").click(function () {
            if (confirm('Are you sure you want to delete this provider?')) {

                var memberId = $(this).parent().find("#updateMemberId").val();
                RequestedUserListResource.DeleteProvider(memberId).then(function (response) {
                    if (response.data === "Success") {
                        notificationsService.success("Deleted successfully");
                    }
                    else {
                        notificationsService.error("Error");
                    }

                    $("#documentsSection").addClass("d-none");
                    RequestedUserListResource.getUsersList("approvedUsers").then(function (response) {
                        $scope.RequestedUserList = response.data;
                        setTimeout(function () {
                            bindHtml();
                        }, 1000);
                        if (response.data.length === 0) {
                            $scope.AnyRecord = false;
                        }
                    });
                });
            }
        });

        function bindHtml() {
            $(".userIdLink").click(function () {
                var userId = $(this).text();
                $("#updateMemberId").val(userId);
                var fileName = $(this).closest(".umb-table-row").find(".fileName").val();
                var fileType = $(this).closest(".umb-table-row").find(".fileType").val();
                var fileUrl = $(this).closest(".umb-table-row").find(".fileUrl").val();

                if (fileType === "jpg" || fileType === "png" || fileType === "jpeg") {
                    var dynamicHtml = "<div class='mb-4'><label class='mr-4'><strong>File : </strong></label><img src=" + fileUrl + " style='height: 300px; width: 300px;' class='ml-4' /></div>";
                    dynamicHtml += "<div><label><strong>File Name : </strong>" + fileName + "</label><div>";
                    $("#fileSection").html(dynamicHtml);
                }
                else if (fileType === "pdf") {
                    var dynamicHtml = "<div class='mb-4'><label class='mr-4'><strong>File : </strong></label><a href='" + fileUrl + "' target='_blank'><img src='/images/pdficon.png' style='height: 300px; width: 300px;' class='ml-4' /></a></div>";
                    dynamicHtml += "<div><label><strong>File Name : </strong>" + fileName + "</label><div>";
                    $("#fileSection").html(dynamicHtml);
                }
                $("#documentsSection").removeClass("d-none");
            });
        }
    });