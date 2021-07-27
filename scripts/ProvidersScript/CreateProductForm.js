$("#saveForLater").click(function () {
    $("#ProductStatus").val("Drafted");
});

$("#submitForApprocal").click(function () {
    $("#ProductStatus").val("Waiting For Approval");
});
function deleteFile(fileId, fileUrl, productVersionNumber) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage) {

        var model = {
            ProductFileId: fileId,
            ProductFile: fileUrl,
            ProductVersionNumber: productVersionNumber
        };
        $.ajax({
            type: "get",
            url: "/umbraco/Surface/USNCreateProductFormSurface/ProductImageDelete/",
            data: model,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result === "1") {
                    $('#tr_' + fileId).remove();

                    var version = productVersionNumber.replace('.', '_');
                    var rowLength = $('#table_' + version + ' tbody tr').length;
                    if (rowLength === 0) {
                        var data = $('#divTable_' + version).next('div').find("#hdn_nextVersion").val();
                        $('#divTable_' + version).remove();
                        $('#LastVersionProductName').val(data);
                    }
                }
                else if (result === "2") {
                    alert("Your session has timed out. Please login again.");
                    window.location.replace("/login/");
                }
            },
            error: function (result) {
                $("#productsDetailsSection").html(result);
            }
        });

    }
}

function showTableAccordingVersionNumber(id) {
    if ($("#" + id).is(":visible")) {
        $("#" + id).hide(500)
    } else {
        $("#" + id).show(500);
    }
}

$("#ProductNewVersionNumber").keyup(function () {
    var versionNumber = $(this).val();

    if (versionNumber === 0 || versionNumber === '') {
        $("#product_files").attr("disabled", "disabled");
    }
    else {
        $('#product_files').removeAttr("disabled");
    }
});