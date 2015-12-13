/// <reference path="C:\Progetti\uPlayAgain\uPlayAgain\uPlayAgain\Scripts/jquery-2.1.4.intellisense.js" />
/// <reference path="C:\Progetti\uPlayAgain\uPlayAgain\uPlayAgain\Scripts/jquery.validate.js" />
var userId;
var token;

function revalidate() {
    $("#resetPassword").valid();
}

function invertModal(modalId) {
    var modal = UIkit.modal("#" + modalId);

    if (modal.isActive()) {
        modal.hide();
    } else {
        modal.show();
    }
}

function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

$("#newPassword").change(revalidate);
$("#newPasswordConfirm").change(revalidate);
$("#resetPassword").validate({
    onclick: true,
    errorClass: "invalid",
    validClass: "success",
    rules: {
        newPassword: {
            required: true,
            minlength: 8
        },
        newPasswordConfirm: {
            required: true,
            minlength: 8,
            equalTo: "#newPasswordConfirm"
        }
    },
    messages: {
        newPassword: {
            required: "Inserisci la password",
            minlength: jQuery.validator.format("Inserire almeno {0} caratteri richiesti!"),            
        },
        newPasswordConfirm: {
            required: "Inserisci la password di conferma",
            minlength: jQuery.validator.format("Inserire almeno {0} caratteri richiesti!"),
            equalTo: "Inserisci entrambe le password uguali"
        }
    },
    submitHandler: function () { confirmChange(); }
});

function confirmChange() {
    var baseAddres = window.location.origin;
    var urlToCall = baseAddres + "/api/account/ValidateResetPassword/"; // + userId + "/" + token + "/" + $("#newPassword").val();
    var queryParameter = {
        userId: userId,
        token: token,
        password: $("#newPassword").val()
    }
    $.ajax({
        type: 'post',
        url: urlToCall,
        data: JSON.stringify(queryParameter),
        contentType: "application/json; charset=utf-8",
        traditional: true,
        success: function (data) {
            invertModal("modalSpinner");
            UIkit.modal.success("La password è stata modificata correttamente. Potete procedere al login.");
        }
    })
    .fail(function (data) {
        invertModal("modalSpinner");
        UIkit.modal.alert("Errore nella modifica della password. Gli errori sono i seguenti:" + data.responseJSON.message);
    });
}

// Carico i dati da queryString
$(document).ready(function () {
    userId = getUrlParameter('userId');
    token = getUrlParameter('token');    
});


