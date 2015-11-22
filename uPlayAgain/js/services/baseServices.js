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
    var isFirsReadData = true;

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

    this.loadData = function (userId) {
        if (isFirsReadData)
            isFirsReadData = false;

        gxcFct.user.load({id : userId}).$promise
            .then(function (userSuccess) {
                user = userSuccess;
                user.LibraryId = user.librariesId[0];
                _this.updateUserData();

                isFirsReadData = true;
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
                // Modifica: viene richiesto il conteggio solo dei messaggi in ingresso e transazioni
                user.Messages.All = user.Messages.In + /*user.Messages.Out +*/ user.Messages.Trn;

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

    this.isFirsReadData = function () {        
        return isFirsReadData;
    }

    this.getInfoUser = function (userId) {
        return gxcFct.user.profile({ userId: userId }).$promise
    }
}]);

/**
* Games service
**/
app.service('games-service', ['factories', '$q', function (gxcFct, $q) {
    this.genres = gxcFct.genre.query();
    this.platforms = gxcFct.platform.query();
    this.languages = gxcFct.language.query();
    this.statuses = gxcFct.status.query();
    this.distances = [5, 10, 20, 50, 100];
    this.gameLargeImages = [];
    this.gameDataStored = [];

    var _this = this;
    _this.defaultImage = "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";

    this.getDefaultImage = function () {
        return _this.defaultImage;
    }

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
                game.gameData.image = gameSuccess.image == '' || gameSuccess.image == undefined || gameSuccess.image == null ? _this.defaultImage : gameSuccess.image;
                game.gameData.platformId = gameSuccess.platformId;
                game.gameData.registrationDate = gameSuccess.registrationDate;
                game.gameData.shortName = gameSuccess.shortName;
                game.gameData.isExchangeable = game.isExchangeable;
                game.gameData.libraryComponents = game.libraryComponents;
                game.gameData.librayId = game.librayId;
            });
    };

    this.loadImage = function (gameId) {
        var result = undefined;
        var deferred = $q.defer();

        _this.gameLargeImages.forEach(function (g) {
            if (result === undefined && g.gameId == gameId) {
                result = g.largeImage;
            }
        });
        if (result != undefined) {
            deferred.resolve(result);            
        }
        else {
            gxcFct.game.largeImage({ gameId: gameId }).$promise
            .then(function (gameSuccess) {
                result = gameSuccess.image == '' || gameSuccess.image == undefined || gameSuccess.image == null ? _this.defaultImage : gameSuccess.image;
                _this.gameLargeImages.push({
                    gameId: gameId,
                    largeImage: result
                });
                deferred.resolve(result);
            });
        }
        return deferred.promise;
    }

    this.searchGames = function (params) {
        return gxcFct.game.query(params).$promise;
    }
}]);
