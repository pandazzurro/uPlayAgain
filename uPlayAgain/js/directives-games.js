//(function () {
//    var app = angular.module('gxc.directives.games', []);

    app.directive('exchangeSearch', ['factories', 'user-service', 'games-service', '$location', function (gxcFct, userSrv, gamesSrv, $location) {
        return {
            restrict: 'E',
            templateUrl: 'templates/exchange-search.html',
            controller: function () {
                var _this = this;

                _this.GENRES_ALL = "Tutti";
                _this.PLATFORMS_ALL = "Tutte";
                _this.DISTANCES_ALL = "Nessun limite";

                _this.genres = gamesSrv.genres;
                _this.platforms = gamesSrv.platforms;
                _this.distances = gamesSrv.distances;

                _this.searchPerformed = false;
                _this.params = {};
                _this.results = [];

                this.setGenre = function (genre) {
                    _this.params.genre = genre;
                };

                this.setPlatform = function (platform) {
                    _this.params.platform = platform;
                };

                this.setDistance = function (distance) {
                    _this.params.distance = distance;
                }

                this.reset = function () {
                    _this.params = {};
                };

                this.getGenreById = function (id) {
                    var result = undefined;

                    for (i in _this.genres) {
                        if (_this.genres[i].genreId == id) {
                            result = _this.genres[i];
                            break;
                        }
                    }

                    return result;
                }

                this.getPlatformById = function (id) {
                    var result = undefined;

                    for (i in _this.platforms) {
                        if (_this.platforms[i].platformId == id) {
                            result = _this.platforms[i];
                            break;
                        }
                    }

                    return result;
                }

                this.round = function (value) {
                    return Math.round(value * 100) / 100;
                };

                this.startSearch = function () {
                    _this.results = [];

                    var queryParameters = {
                        userId: userSrv.getCurrentUser().userId,
                        gameTitle: _this.params.string === undefined ? '' : _this.params.string,
                        genreId: _this.params.genre === undefined ? '' : _this.params.genre.genreId,
                        platformId: _this.params.platform === undefined ? '' : _this.params.platform.platformId,
                        distance: _this.params.distance === undefined ? 1000000 : _this.params.distance,
                        skip: 0,
                        take: 10000
                    };

                    _this.results = gxcFct.game.search(queryParameters, function (success) {
                        _this.searchPerformed = true;
                    });
                };

                this.showDetails = function (game) {
                    _this.details = game;
                    gxcFct.user.get({ userId: game.user.userId }).$promise
                    .then(function (success) {
                        _this.detailsUser = success;
                    });
                };

                this.sendProposal = function (details) {
                    var newLocation = '/mail/compose/' + details.user.id + '//' + details.libraryComponent.libraryComponentId;
                    $location.path(newLocation);
                }
            },
            controllerAs: 'search'
        };
    }]);

    app.directive('gamesSearch', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gamesSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/games-search.html',
            controller: function () {
                var _this = this;

                _this.GENRES_ALL = "Tutti";
                _this.PLATFORMS_ALL = "Tutte";

                _this.genres = gamesSrv.genres;
                _this.platforms = gamesSrv.platforms;

                _this.searchPerformed = false;
                _this.params = {};
                _this.results = [];

                this.setGenre = function (genre) {
                    _this.params.genre = genre;
                };

                this.setPlatform = function (platform) {
                    _this.params.platform = platform;
                };

                this.reset = function () {
                    _this.params = {};
                };

                this.getGenreById = function (id) {
                    var result = undefined;

                    for (i in _this.genres) {
                        if (_this.genres[i].genreId == id) {
                            result = _this.genres[i];
                            break;
                        }
                    }

                    return result;
                }

                this.getPlatformById = function (id) {
                    var result = undefined;

                    for (i in _this.platforms) {
                        if (_this.platforms[i].platformId == id) {
                            result = _this.platforms[i];
                            break;
                        }
                    }

                    return result;
                }

                this.startSearch = function () {
                    _this.results = [];

                    var queryParameters = {
                        gameTitle: _this.params.string,
                        genreId: _this.params.genre === undefined ? undefined : _this.params.genre.genreId,
                        platformId: _this.params.platform === undefined ? undefined : _this.params.platform.platformId
                    };

                    _this.results = gxcFct.game.query(queryParameters, function (success) {
                        _this.searchPerformed = true;
                    });
                }

                this.addToLibrary = function (game) {
                    var queryParameters = {
                        LibraryId: userSrv.getCurrentUser().LibraryId,
                        GameId: game.gameId,
                        GameLanguageId: 3, //per ora fisso
                        StatusId: 1
                    };

                    gxcFct.library.add(queryParameters).$promise
                      .then(function (success) {
                          userSrv.updateUserData();
                          UIkit.notify(game.title + ' &grave; stato aggiunto alla libreria', { status: 'success', timeout: 5000 });
                      },
                      function (error) {
                          UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                      });
                }
            },
            controllerAs: 'search'
        };
    }]);

    app.directive('gamesLibrary', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/games-library.html',
            controller: function () {
                var _this = this;
                _this.games = [];
                _this.gameSrv = gameSrv;
                _this.currentUser = userSrv.getCurrentUser().id;

                _this.statuses = gameSrv.statuses;
                _this.languages = gameSrv.languages;

                this.setStatus = function (status) {
                    _this.editingGame.gameData.status = status;
                };

                this.setLanguage = function (language) {
                    _this.editingGame.gameData.language = language;
                };

                var getGames = function (libraryId) {
                    gxcFct.library.get({ libraryId: libraryId }).$promise
                    .then(function (success) {
                        _this.libraryOwner = success.userId;

                        for (i in success.libraryComponents) {
                            var g = success.libraryComponents[i];

                            g.canEdit = _this.libraryOwner == _this.currentUser;
                            gameSrv.fillGameData(g);
                        }

                        _this.games = success.libraryComponents;
                    },
                    function (error) {
                        UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                    });
                };

                this.addGame = function () {
                    window.location = '#/library/add';
                };

                this.editGame = function (game) {
                    _this.editingGame = game;

                    var modal = UIkit.modal("#gameEditor");
                    modal.show();
                };

                this.toggleTrade = function () {
                    _this.editingGame.gameData.isExchangeable = !_this.editingGame.gameData.isExchangeable;
                };

                this.getRemainingChars = function () {
                    var result = 200;

                    if (_this.editingGame !== undefined && _this.editingGame.gameData.note !== undefined && _this.editingGame.gameData.note != null)
                    {
                        result = 200 - _this.editingGame.gameData.note.length;
                    }

                    return result;
                }

                this.removeGame = function (game) {
                    UIkit.modal.confirm("Sicuro di voler rimuovere " + game.gameData.title + " dalla tua libreria?", function () {
                        var queryParameters = {
                            componentId: game.libraryComponentId,
                        };

                        gxcFct.library.remove(queryParameters).$promise
                        .then(function (success) {
                            getGames(userSrv.getCurrentUser().LibraryId);
                            userSrv.updateUserData();
                        });
                    });
                };

                this.saveChanges = function (gameEdit) {
                    var queryParameters = {
                        LibraryComponentId: gameEdit.libraryComponentId,
                        GameId: gameEdit.gameData.gameId,
                        Note: gameEdit.gameData.note,
                        IsExchangeable: gameEdit.gameData.isExchangeable,
                        LibraryId: gameEdit.libraryId,
                        StatusId: gameEdit.statusId,
                        GameLanguageId: gameEdit.gameData.language.gameLanguageId
                    };

                    gxcFct.library.update({ componentId: gameEdit.libraryComponentId }, queryParameters).$promise
                    .then(function (success) {
                        getGames(userSrv.getCurrentUser().LibraryId);
                        var modal = UIkit.modal("#gameEditor");
                        modal.hide();
                    });
                }

                getGames(userSrv.getCurrentUser().LibraryId);
            },
            controllerAs: 'library'
        };
    }]);
//})();
