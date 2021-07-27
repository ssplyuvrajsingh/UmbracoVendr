
$(document).ready(function () {
    GetProviderWallet();
});

GetProviderWallet = function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNProviderWalletSurface/GetProviderWallet",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#wallet-section").html(result);
            GetPaymentHistoryByProvider();
        },
        error: function (result) {
        }
    });
}

GetWithdrawalRequest = function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNProviderWalletSurface/GetWithdrawalRequest",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result === "logout") {
                alert("Your session has timed out. Please login again.");
                window.location.replace("/login/");
            }
            else if (result != null && result != '') {
                $("#wallet-section").html(result);
            }
            else {
                alert("Please save your bank account details before withdrawal.");
            }
        },
        error: function (result) {
        }
    });
}

PostWithdrawalRequest = function () {
    $.ajax({
        url: "/umbraco/Surface/USNProviderWalletSurface/PostWithdrawalRequest",
        method: "post",
        data: { amount: $("#withdrawalRequest").val() },
        success: function (result) {
            switch (result) {
                case "1": alert("Your withdrawal request now pending.");
                    GetProviderWallet();
                    break;
                case "2": alert("Please enter valid amount!");
                    break;
                case "3": alert("your remaining balance is insufficient for this whithdrawal request.");
                    break;
                case "4": alert("Your session has timed out. Please login again.");
                    window.location.replace("/login/");
                    break;
                default: alert("Please try again!");
            }
        },
        error: function (result) {
        }
    });
}

GetPaymentHistoryByProvider = function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNProviderWalletSurface/GetPaymentHistoryByProvider",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#paymenthistory-section").html(result);
        },
        error: function (result) {
        }
    });
}

EditBankAccountDetails = function () {
    $.ajax({
        type: "get",
        url: "/umbraco/Surface/USNProviderWalletSurface/EditProviderBankDetails",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#wallet-section").html(result);
        },
        error: function (result) {
        }
    });
}

EditSaveBankAccountDetails = function () {
    if ($("#in_BankAccountNumber").val() != '' && $("#in_IFSC").val() != '' && $("#in_AccountHolderName").val() != '') {
        var model = {
            BankAccountNumber: $("#in_BankAccountNumber").val(),
            IFSC: $("#in_IFSC").val(),
            AccountHolderName: $("#in_AccountHolderName").val()
        }
        $.ajax({
            type: "post",
            url: "/umbraco/Surface/USNProviderWalletSurface/EditProviderBankDetails",
            data: model,
            success: function (result) {
                if (result === "1") {
                    alert("Save successfully");
                }
                else if (result === "2") {
                    alert("Your session has timed out. Please login again.");
                    window.location.replace("/login/");
                }
                else {
                    alert("Please try again!")
                }
            },
            error: function (result) {
            }
        });
    }
    else {
        alert("Please fill every bank details then click on save");
    }
}