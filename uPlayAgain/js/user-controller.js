/// <reference path="services/authService.js" />
/// <reference path="services/baseServices.js" />

app.controller('UserController', ['$scope', '$cookies', 'user-service', 'authService', 'Messages', function ($scope, $cookies, userSrv, authService, Messages) {
    $scope.loginData = {
        userName: "",
        password: "",
        useRefreshTokens: false
    };    
    $scope.username = undefined;
    $scope.password = undefined;
    $scope.message = "";

    //$scope.authExternalProvider = function (provider) {
    //    var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';

    //    var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
    //                                                                + "&response_type=token&client_id=" + ngAuthSettings.clientId
    //                                                                + "&redirect_uri=" + redirectUri;
    //    window.$windowScope = $scope;

    //    var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
    //};

    //$scope.authCompletedCB = function (fragment) {

    //    $scope.$apply(function () {

    //        if (fragment.haslocalaccount == 'False') {

    //            authService.logOut();

    //            authService.externalAuthData = {
    //                provider: fragment.provider,
    //                userName: fragment.external_user_name,
    //                externalAccessToken: fragment.external_access_token
    //            };

    //            $location.path('/associate');

    //        }
    //        else {
    //            //Obtain access token and redirect to orders
    //            var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
    //            authService.obtainAccessToken(externalData).then(function (response) {

    //                $location.path('/orders');

    //            },
    //         function (err) {
    //             $scope.message = err.error_description;
    //         });
    //        }

    //    });
    //}

    var _this = this;
    //$scope.Messages = Messages;
    //$scope.Messages.pushMessage();

    this.login = function () {
        $scope.loginData.userName = $scope.username,
        $scope.loginData.password = $scope.password;
        authService.login($scope.loginData).then(function (response) {
            // Redirect verso una pagina particolare?
            UIkit.notify('Benvenuto ' + response.userName, { status: 'success', timeout: 1500 });
            
            //MessageHubProxy.client.connection = function (name, message) {
            //    console.log(name + ' ' + message);
            //};

            $.connection.hub.start().done(function () {
                // Wire up Send button to call NewContosoChatMessage on the server.
                $('#newContosoChatMessage').click(function () {
                    contosoChatHubProxy.server.newContosoChatMessage($('#displayname').val(), $('#message').val());
                    $('#message').val('').focus();
                });
            });
        },
         function (err) {
             $scope.message = err.error_description;
             console.log($scope.message);
             UIkit.notify('Username o password non corretti', { status: 'danger', timeout: 5000 });
         });
    };

    this.logout = function () {
        authService.logOut();
    };

    this.isLoggedIn = function () {
        if (authService.isAuth() && userSrv.getCurrentUser().id === undefined && userSrv.isFirsReadData()) {            
            if (authService.userId() != undefined) {
                userSrv.loadData(authService.userId());
                requestaLoginData = false;
            }
        }
        return authService.isAuth();
    };

    this.getCurrentUser = function () {
        return userSrv.getCurrentUser();
    };

    this.register = function () {
        window.location = '#/register';
    };

    this.resetPassword = function () {
        userSrv.resetPassword($scope.mailReset);
    }
}]);