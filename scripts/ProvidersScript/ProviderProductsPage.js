$(document).ready(function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNCreateProductFormSurface/GetAllProductsOfParticularProvider",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#productsDetailsSection").html(result);
        },
        error: function (result) {
            $("#productsDetailsSection").html(result);
        }
    });
});

function getAllProductsOfProvider(providerId) {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNCreateProductFormSurface/GetAllProducts?providerId=" + providerId,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#product_Details").hide();
            $("#productsSection").show();
            $("#productsDetailsSection").html(result);
            $('html, body').animate({
                scrollTop: $("#productsSection").offset().top
            }, 500);
        },
        error: function (result) {
        }
    });
}

function SetImage(imgUrl) {
    $("#Product_Img").attr("src", imgUrl);
}