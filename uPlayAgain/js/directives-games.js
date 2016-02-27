/// <reference path="services/baseServices.js" />
/// <reference path="app.js" />
/// <reference path="factories.js" />

app.directive('exchangeSearch', ['factories', 'user-service', 'games-service', '$location', function (gxcFct, userSrv, gameSrv, $location) {
    return {
        restrict: 'E',
        templateUrl: 'templates/exchange-search.html',
        controller: function ($cookies) {
            var _this = this;

            _this.GENRES_ALL = "Tutti";
            _this.PLATFORMS_ALL = "Tutte";
            _this.DISTANCES_ALL = "Nessun limite";

            _this.genres = gameSrv.genres;
            _this.platforms = gameSrv.platforms;
            _this.distances = gameSrv.distances;

            _this.searchPerformed = false;
            _this.results = [];
            _this.searchParameter = {};
            _this.itemsOnPage = 10;
            _this.pagination = UIkit.pagination("#gridPagination");
           
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

            this.startSearch = function (skip) {
                if (skip == undefined)
                    skip = 0;
                _this.results = [];                

                var queryParameters = {
                    userId: userSrv.getCurrentUser().userId,
                    gameTitle: _this.params.string === undefined ? '' : _this.params.string,
                    genreId: _this.params.genre === undefined ? '' : _this.params.genre.genreId,
                    platformId: _this.params.platform === undefined ? '' : _this.params.platform.platformId,
                    distance: _this.params.distance === undefined ? 1000000 : _this.params.distance,
                    skip: skip * _this.itemsOnPage,
                    take: _this.itemsOnPage
                };

                $cookies.putObject('exchangeSearch', _this.params);

                _this.results = gxcFct.game.search(queryParameters, function (success) {
                    _this.searchPerformed = true;
                    _this.pagination.options.currentPage = skip + 1;
                    _this.pagination.options.items = success.count;
                    _this.pagination.options.itemsOnPage = _this.itemsOnPage;
                    _this.pagination.options.lblNext = ">";
                    _this.pagination.options.lblPrev = "<";
                    
                    angular.forEach(success.searchGame, function (value, index) {
                        if (value.game.image == undefined || value.game.image == "" || value.game.image == null)
                            value.game.image = gameSrv.getDefaultImage();

                        if (success.searchGame.length == index + 1) {
                            _this.pagination.init();
                        }
                    });                    
                    _this.startPagination = false;
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

            this.loadImage = function (gameId) {
                _this.selectedImage = "";
                gameSrv.loadImage(gameId)
                .then(function (success) {
                    _this.selectedImage = success;
                })
            }

            this.startSearchByCookiesHistory = function () {
                if ($cookies.getObject('exchangeSearch') === undefined)
                    _this.params = {};
                else
                {
                    _this.params = $cookies.getObject('exchangeSearch');
                    _this.startSearch(0);                    
                }
            }

            _this.startSearchByCookiesHistory();
            _this.startPagination = false;
            _this.pagination.options.onSelectPage = function (e, pageIndex) {
                if (!_this.startPagination) {
                    _this.startPagination = true;
                    _this.startSearch(e);                    
                }
            };            
        },
        controllerAs: 'search'
    };
}]);

app.directive('gamesSearch', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
    return {
        restrict: 'E',
        templateUrl: 'templates/games-search.html',
        controller: function () {
            var _this = this;

            _this.GENRES_ALL = "Tutti";
            _this.PLATFORMS_ALL = "Tutte";

            _this.genres = gameSrv.genres;
            _this.platforms = gameSrv.platforms;

            _this.searchPerformed = false;
            _this.params = {};
            _this.results = [];
            _this.addingGame = undefined;
            _this.gameSrv = gameSrv;
            _this.itemsOnPage = 10;
            _this.pagination = UIkit.pagination("#gridPagination");

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

            this.startSearch = function (skip) {
                if (skip == undefined)
                    skip = 0;
                _this.results = [];

                var queryParameters = {
                    gameTitle: _this.params.string === undefined ? ' ' : _this.params.string,
                    genreId: _this.params.genre === undefined ? ' ' : _this.params.genre.genreId,
                    platformId: _this.params.platform === undefined ? ' ' : _this.params.platform.platformId,
                    skip: skip * _this.itemsOnPage,
                    take: _this.itemsOnPage
                };

                _this.results = gxcFct.game.query(queryParameters, function (success) {
                    _this.searchPerformed = true;
                    _this.pagination.options.currentPage = skip + 1;
                    _this.pagination.options.items = success.count;
                    _this.pagination.options.itemsOnPage = _this.itemsOnPage;
                    _this.pagination.options.lblNext = ">";
                    _this.pagination.options.lblPrev = "<";

                    _this.pagination.init();
                    _this.startPagination = false;
                });
            }

            this.setStatus = function (status) {
                _this.addingGame.gameData.status = status;
            };

            this.setLanguage = function (language) {
                _this.addingGame.gameData.language = language;
            };

            this.getRemainingChars = function () {
                var result = 200;

                if (_this.addingGame !== undefined && _this.addingGame.gameData.note !== undefined && _this.addingGame.gameData.note != null) {
                    result = 200 - _this.addingGame.gameData.note.length;
                }

                return result;
            }

            this.populateAddGame = function (game) {
                _this.addingGame = { gameData : {} };
                _this.addingGame.gameData = game;
                _this.addingGame.gameData.language = { gameLanguageId: 1, description: "Italiano" };
                _this.addingGame.gameData.status = { statusId: 1, description: "Eccellente" };
                _this.addingGame.gameData.isExchangeable = true;
            }

            this.toggleTrade = function () {
                _this.addingGame.gameData.isExchangeable = !_this.addingGame.gameData.isExchangeable;
            };

            this.addToLibrary = function () {
                var game = _this.addingGame.gameData;

                var queryParameters = {
                    LibraryId: userSrv.getCurrentUser().LibraryId,
                    GameId: game.gameId,
                    StatusId: game.status.statusId,
                    IsExchangeable: game.isExchangeable,
                    GameLanguageId: game.language.gameLanguageId,
                    Note: game.note
                };

                gxcFct.library.add(queryParameters).$promise
                  .then(function (success) {
                      _this.invertModal("gameAdd");
                      userSrv.updateUserData();
                      UIkit.notify(game.title + ' aggiunto alla libreria', { status: 'success', timeout: 5000 });
                  },
                  function (error) {
                      UIkit.notify('Si è verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                  });
            }

            this.loadImage = function (gameId) {
                _this.selectedImage = "";
                gameSrv.loadImage(gameId)
                .then(function (success) {
                    _this.selectedImage = success;                   
                })
            }

            this.invertModal = function (modalId) {
                var modal = UIkit.modal("#" + modalId);

                if (modal.isActive()) {
                    modal.hide();
                } else {
                    modal.show();
                }
            }

            _this.startPagination = false;
            _this.pagination.options.onSelectPage = function (e, pageIndex) {
                if (!_this.startPagination) {
                    _this.startPagination = true;
                    _this.startSearch(e);
                }
            };            
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
            _this.genres = gameSrv.genres;
            _this.platforms = gameSrv.platforms;

            _this.statuses = gameSrv.statuses;
            _this.languages = gameSrv.languages;
            _this.selectedImage = "";

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

            this.infoGame = function (game) {
                _this.infosGame = game;

                var modal = UIkit.modal("#gameInfo");
                modal.show();
            };

            this.toggleTrade = function () {
                _this.editingGame.gameData.isExchangeable = !_this.editingGame.gameData.isExchangeable;
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

            this.getRemainingChars = function () {
                var result = 200;

                if (_this.editingGame !== undefined && _this.editingGame.gameData.note !== undefined && _this.editingGame.gameData.note != null) {
                    result = 200 - _this.editingGame.gameData.note.length;
                }

                return result;
            }

            this.loadImage = function (gameId) {
                _this.selectedImage = "";
                gameSrv.loadImage(gameId)
                .then(function (success) {
                    _this.selectedImage = success;
                })
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

app.directive('gamesLast', ['factories', 'games-service', function (gxcFct, gameSrv) {
    return {
        restrict: 'E',
        templateUrl: 'templates/game-last.html',
        controller: function () {
            var _this = this;
            _this.games = [];
            _this.lastGameNumber = 10;

            this.loadLastGame = function () {
                var lastGameNumber = 10;
                gxcFct.game.last({ gameCount: _this.lastGameNumber }).$promise
                .then(function (success) {
                    _this.games = success;
                });
            }
            
            _this.loadLastGame();
        },
        controllerAs: 'gamesLast'
    };
}]);