(function () {
    var app = angular.module('gxc.services', ['ngRoute', 'ngCookies']);

    /**
     * Route service
     **/
    app.service('route-service', ['$routeProvider', function ($route) {
        var _this = this;
        _this.route = $route;
        return _this;
    }]);

    /**
     * Mail service
     **/
    app.service('mail-service', ['factories', function (gxcFct) {
        var _this = this;
        this.getMessages = function (userId, incoming, page) {
            return gxcFct.mail.query();
        };

        this.getMessagesCount = function (userId) {
            return { in: 2, out: 3 };
        };

        return _this;
    }]);


    /**
     * User service
     **/
    app.service('user-service', ['factories', '$scope', '$location', 'authService', 'ngAuthSettings', function (gxcFct, $scope, $location, authService, ngAuthSettings) {
        var _this = this;
        var user = {};
        $scope.loginData = {
            userName: "",
            password: "",
            useRefreshTokens: false
        };
        $scope.message = "";

        /*login*/
        this.login = function (username, password) {
            var queryParameters = {
                Username: username,
                Password: password
            };

            authService.login($scope.loginData).then(function (response) {
                //TODO: redirect
                //$location.path('/orders');
            },
            function (err) {
                $scope.message = err.error_description;
            });
        }
        /*
          gxcFct.user.login(queryParameters).$promise
            .then(function(userSuccess) {
              user = userSuccess;
            
              _this.updateUserData();
              },
            function(error) {
              UIkit.notify('Username o password non validi', { status: 'warning', timeout: 5000 });
              user = {};
            });
        };
        */

        /*login esterni. TODO*/
        this.authExternalProvider = function (provider) {
            var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
            var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                        + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                        + "&redirect_uri=" + redirectUri;
            window.$windowScope = $scope;
            var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
        };

        /*TODO*/
        this.authCompletedCB = function (fragment) {
            $scope.$apply(function () {
                if (fragment.haslocalaccount == 'False') {
                    authService.logOut();
                    authService.externalAuthData = {
                        provider: fragment.provider,
                        userName: fragment.external_user_name,
                        externalAccessToken: fragment.external_access_token
                    };
                    $location.path('/associate');
                }
                else {
                    //Obtain access token and redirect to orders
                    var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                    authService.obtainAccessToken(externalData).then(function (response) {
                        $location.path('/orders');
                    },
                 function (err) {
                     $scope.message = err.error_description;
                 });
                }

            });
        };

        this.updateUserData = function () {
            user.Games = 0;
            user.Messages = 0;
            user.LibraryId = undefined;

            gxcFct.library.byUser({ userId: user.userId }).$promise
              .then(function (libSuccess) {
                  user.LibraryId = libSuccess[0].libraries[0].libraryId;
                  for (lc in libSuccess[0].libraries[0].libraryComponents) {
                      user.Games++;
                  }
              },
              function (error) {
                  UIkit.notify('Si &egrave; verificato un errore nel recupero dei dati utente.', { status: 'warning', timeout: 5000 });
              }); // library.byUser     

            gxcFct.mail.byUser({ userId: user.userId }).$promise
              .then(function (mailSuccess) {
                  user.Messages = mailSuccess.incoming;
              },
              function (error) {
                  UIkit.notify('Si &egrave; verificato un errore nel recupero dei dati utente.', { status: 'warning', timeout: 5000 });
              }); // mail.byUser     
        };

        this.logout = function (username, password) {
            var queryParameters = {
                Username: username,
                Password: password
            };

            gxcFct.user.logout(queryParameters).$promise
              .then(function (userSuccess) {
                  user = userSuccess;
              },
              function (error) {
                  UIkit.notify('Errore di logout', { status: 'warning', timeout: 5000 });
                  user = {};
              });
        };

        this.isLoggedIn = function () {
            return user.userId !== undefined;
        }

        this.getCurrentUser = function () {
            return user;
        }

        return _this;

    }]);

    /**
     * Games service
     **/
    app.service('games-service', ['factories', function (gxcFct) {
        this.genres = gxcFct.genre.query();
        this.platforms = gxcFct.platform.query();
        this.languages = gxcFct.language.query();
        this.statuses = gxcFct.status.query();
        this.distances = [5, 10, 20, 50, 100];

        var _this = this;

        this.getGenreById = function (id) {
            var result = undefined;

            for (i in _this.genres) {
                if (_this.genres[i].IdGenre == id) {
                    result = _this.genres[i];
                    break;
                }
            }

            return result;
        }

        this.getPlatformById = function (id) {
            var result = undefined;

            for (i in _this.platforms) {
                if (_this.platforms[i].IdPlatform == id) {
                    result = _this.platforms[i];
                    break;
                }
            }

            return result;
        }

        this.getStatusById = function (id) {
            var result = undefined;

            for (i in _this.statuses) {
                if (_this.statuses[i].statusId == id) {
                    result = _this.statuses[i];
                    break;
                }
            }

            return result;
        }

        this.getLanguageById = function (id) {
            var result = undefined;

            for (i in _this.languages) {
                if (_this.languages[i].gameLanguageId == id) {
                    result = _this.languages[i];
                    break;
                }
            }

            return result;
        }

        this.fillGameData = function (game) {
            gxcFct.game.get({ gameId: game.gameId }).$promise
            .then(function (gameSuccess) {
                game.gameData = gameSuccess;
                game.gameData.status = _this.getStatusById(game.statusId);
                game.gameData.language = _this.getLanguageById(game.gameLanguageId);
                game.gameData.note = game.note;
            });
        };

        this.searchGames = function (params) {
            return gxcFct.game.query(params).$promise;
        }

        return _this;
    }]);

})();
