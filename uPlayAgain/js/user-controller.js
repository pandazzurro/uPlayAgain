﻿/// <reference path="services/authService.js" />
/// <reference path="services/baseServices.js" />

app.controller('UserController', ['$scope', '$cookies', 'user-service', 'authService', 'Messages', '$rootScope', function ($scope, $cookies, userSrv, authService, Messages, $rootScope) {
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
    
    this.login = function () {
        $scope.loginData.userName = $scope.username,
        $scope.loginData.password = $scope.password;
        authService.login($scope.loginData).then(function (response) {
            // Redirect verso una pagina particolare?
            UIkit.notify('Benvenuto ' + response.userName, { status: 'success', timeout: 1500 });
            Messages.createHub(userSrv.getCurrentUser().id);
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
                Messages.createHub(authService.userId());
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

    $rootScope.$on('newMessage', function (event, message) {
        if(message !== undefined)
            UIkit.notify("<i class='uk-icon-envelope-o'></i> Nuovo messaggio. Controlla la posta", { pos: 'top-center', status: 'info' });
    });

    $rootScope.$on('newProposal', function (event, proposal) {
        if (proposal !== undefined)
            UIkit.notify("<i class='uk-icon-exchange'></i> Nuovo scambio. Controlla la posta", { pos: 'top-center', status: 'info' });
    });

    $rootScope.$on('newFeedback', function (event, value) {
        if (value !== undefined)
            UIkit.notify("<i class='uk-icon-star-half-o'></i> Nuovo feedback. Controlla la posta", { pos: 'top-center', status: 'info' });
    });

}]);