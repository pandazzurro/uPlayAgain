var getUrlParameter = function getUrlParameter(sParam) {
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

$(document).ready(function () {
    var userId = getUrlParameter('userId');
    var token = getUrlParameter('token');
    var baseAddres = window.location.origin;
    var urlToCall = baseAddres + "/api/account/ValidateMail/" + userId + "/"+ token;
    $.get(urlToCall, function (data) {
        $("#success").addClass("visible").removeClass("hidden");
        $("#failed").addClass("hidden").removeClass("visible");
    })
    .fail(function (data) {
        $("#success").addClass("hidden").removeClass("visible");
        $("#failed").addClass("visible").removeClass("hidden");
    });
});

