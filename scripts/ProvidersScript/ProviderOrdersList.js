$(document).ready(function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNCreateProductFormSurface/GetAllOrdersOfParticularProvider",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#OrderDetailsSection").html(result);
        },
        error: function (result) {
        }
    });
    GetOrderDetails = function (orderIdValue) {
        $.ajax({
            type: "get",
            url: "/umbraco/Surface/USNCreateProductFormSurface/GetOrderDetails/",
            data: { orderId: orderIdValue },
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                $("#order-list").hide();
                $("#Order-details").show();
                $("#Order-details").html(result);
            },
            error: function (result) {
            }
        });
    }
});