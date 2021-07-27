function getProducts(categoryId) {
    console.log(categoryId);
    $("#hdn_categoryId").val(categoryId);
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNCreateProductFormSurface/GetAllProducts?categoryId=" + categoryId,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#productsDetailsSection").html(result);
        },
        error: function (result) {
            $("#productsDetailsSection").html(result);
        }
    });
}

$(document).ready(function () {
    $(".productCategory").first().css("color", "#75c9d6");
    getProducts(0);
});

$(".productCategory").click(function () {
    const self = $(this);
    $(".productCategory").each(function () {
        $(this).css("color", "#19233a");
    });
    self.css("color", "#75c9d6");
});