(function () {
    var app = angular.module('gxc.services', ['ngRoute', 'ngCookies']);

    /**
     * Route service
     **/
    app.service('route-service', ['$routeProvider', function ($route) {
        this.route = $route;
    }]);

    /**
     * Mail service
     **/
    app.service('mail-service', ['factories', function (gxcFct) {
        this.getMessages = function (userId, incoming, page) {
            return gxcFct.mail.query();
        };

        this.getMessagesCount = function (userId) {
            return { in: 2, out: 3 };
        };
    }]);


    /**
     * User service
     **/
    app.service('user-service', ['factories', function (gxcFct) {
        var _this = this;
        var user = {};

        this.login = function (username, password) {
            var queryParameters = {
                Username: username,
                Password: password
            };

            gxcFct.user.login(queryParameters).$promise
              .then(function (userSuccess) {
                  user = userSuccess;
                  user.LibraryId = user.librariesId[0];

                  _this.updateUserData();
              },
              function (error) {
                  UIkit.notify('Username o password non validi', { status: 'warning', timeout: 5000 });
                  user = {};
              });
        };
        this.getCounterMessages = function (reload) {           
            return user.Messages;
        }

        this.updateUserData = function () {
            user.Games = 0;
            user.Messages = {};
            
            return gxcFct.counter.byUser({ userId: user.id }).$promise
              .then(function (counterSuccess) {                  
                  user.Messages.In = counterSuccess.incoming;
                  user.Messages.Out = counterSuccess.outgoing;
                  user.Messages.Trn = counterSuccess.transactions;
                  user.Messages.All = user.Messages.In + user.Messages.Out + user.Messages.Trn;

                  user.Games = counterSuccess.librariesComponents;
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
                  $location.path('/');
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
            return gxcFct.game.get({ gameId: game.gameId }).$promise
                .then(function (gameSuccess) {
                    game.gameData = gameSuccess;
                    game.gameData.status = _this.getStatusById(game.statusId);
                    game.gameData.language = _this.getLanguageById(game.gameLanguageId);
                    game.gameData.note = game.note;
                    game.gameData.edit = game.edit;
                    game.gameData.gameId = gameSuccess.gameId;
                    game.gameData.genreId = gameSuccess.genreId;
                    game.gameData.image = gameSuccess.image;
                    game.gameData.platformId = gameSuccess.platformId;
                    game.gameData.registrationDate = gameSuccess.registrationDate;
                    game.gameData.shortName = gameSuccess.shortName;
                    game.gameData.isExchangeable = game.isExchangeable;
                    game.gameData.libraryComponents = game.libraryComponents;
                    game.gameData.librayId = game.librayId;
                });
        };

        this.searchGames = function (params) {
            return gxcFct.game.query(params).$promise;
        }
    }]);

})();
