(function () {
    var app = angular.module('gxc.directives', []);

    app.directive('formRegister', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/form-register.html',
            controller: function ($routeParams, $scope, uiGmapGoogleMapApi) {
                var _this = this;
                _this.params = {};
                _this.TOSagreed = false;
                _this.idUsernameAlreadyExists = undefined;

                // gestore delle mappe
                $scope.map = {
                    center: {
                        latitude: 45.4300,
                        longitude: 10.9880
                    },
                    zoom: 12
                };
                $scope.options = {
                    streetViewControl: false,
                    scaleControl: true,
                    panControl: true
                };

                /*search box*/
                var events = {
                    places_changed: function (searchBox) {
                        var places = searchBox.getPlaces();
                        var location = places[0].geometry.location;
                        $scope.map.center = {
                            latitude: location.k,
                            longitude: location.D
                        };
                        $scope.map.zoom = 18;
                        $scope.marker.coords = {
                            latitude: location.k,
                            longitude: location.D
                        };
                    }
                }
                $scope.searchbox = { template: 'searchbox.tpl.html', events: events };
                /*search box*/

                $scope.marker = {
                    id: 0,
                    coords: {
                        latitude: 45.4300,
                        longitude: 10.9880
                    },
                    options: {
                        draggable: true
                    },
                    events: {
                        dragend: function (marker, eventName, args) {
                            var lat = marker.getPosition().lat();
                            var lon = marker.getPosition().lng();
                            $scope.marker.options = {
                                draggable: true,
                                labelContent: "Imposta la tua posizione",
                                labelAnchor: "60 0",
                                labelClass: "marker-labels"
                            };
                        }
                    }
                };
                $scope.coordinateSelected = undefined;
                $scope.$watchCollection("marker.coords", function (newVal, oldVal) {
                    if (_.isEqual(newVal, oldVal))
                        return;
                    $scope.coordinateSelected = {
                        Geography: {
                            CoordinateSystemId: 4326,
                            WellKnownText: "POINT (" + $scope.marker.coords.latitude + " " + $scope.marker.coords.longitude + ")"
                        }
                    };
                });

                // gestore delle immagini caricate.
                $scope.currentImage = '';
                $scope.currentCroppedImage = '';
                var handleFileSelect = function (evt) {
                    var file = evt.currentTarget.files[0];
                    var reader = new FileReader();
                    reader.onload = function (evt) {
                        $scope.$apply(function ($scope) {
                            $scope.currentImage = evt.target.result;
                            $('img-crop>canvas').css({ 'margin-top': 0, 'margin-left': 0 });
                        });
                    };
                    reader.readAsDataURL(file);
                };
                angular.element(document.querySelector('#fileInput')).on('change', handleFileSelect);

                this.checkUsername = function () {
                    //verifica se esiste già un utente con questo nome
                    gxcFct.user.checkUsername({ username: _this.params.username },
                        function (success) {
                            if (!success) _this.idUsernameAlreadyExists = true;
                            else _this.idUsernameAlreadyExists = false;
                        },
                        function (error) { _this.idUsernameAlreadyExists = true; }
                        );
                };
                
                this.toggleAgreement = function () {
                    _this.TOSagreed = !_this.TOSagreed;
                };

                this.register = function () {
                    if ($scope.coordinateSelected == undefined) {
                        UIkit.notify('Devi selezionare una posizione valida.', { status: 'warning', timeout: 5000 });
                        return;
                    }
                    if (_this.isUsernameAlreadyExists) {
                        UIkit.notify('Username già presente.', { status: 'error', timeout: 5000 });
                        return;
                    }
                    if (!_this.TOSagreed) {
                        UIkit.notify('Devi accettare le condizioni d\'uso del sito per poterti registrare.', { status: 'warning', timeout: 5000 });
                        return;
                    }
                    
                    var queryParameters = {
                        userName: _this.params.username,
                        password: _this.params.password,
                        confirmPassword: _this.params.confirmPassword,
                        positionUser: _this.params.location,
                        email: _this.params.email,
                        image: $scope.currentCroppedImage.replace(/^data:image\/(png|jpg);base64,/, ""),
                        positionUser: $scope.coordinateSelected
                    }
                    gxcFct.user.register(queryParameters,
                    function (success) {
                        UIkit.notify('Utente registrato. Ora puoi accedere alle funzionalit&agrave; del sito', { status: 'success', timeout: 5000 });
                        window.location = '#/';
                    },
                    function (error) {
                        UIkit.notify('Si &egrave; verificato un errore durante la registrazione. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
                    }
                    );
                };
            },
            controllerAs: 'newuser'
        };
    }]);

    app.directive('formEditRegister', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/form-edit-register.html',
            controller: function ($routeParams, $scope, uiGmapGoogleMapApi) {
                var _this = this;
                _this.params = {};
                _this.TOSagreed = false;
                $scope.currentUser = undefined;

                // Recupero i dati dell'utente
                var queryParameters = {
                    userId: userSrv.getUser().userId
                };
                gxcFct.user.get(queryParameters,
                function (success) {
                    $scope.currentUser = success;
                    $scope.currentImage = "data:image/png;base64," + $scope.currentUser.image;
                    
                    var coords = $scope.currentUser.positionUser.geography.wellKnownText.replace('POINT (', '').replace(')', '').split(" ");

                    $scope.map = {
                        center: {
                            latitude: coords[0],
                            longitude: coords[1]
                        },
                        zoom: 18
                    };

                    $scope.marker = {
                        id: 0,
                        coords: {
                            latitude: coords[0],
                            longitude: coords[1]
                        },
                        options: {
                            draggable: true
                        },
                        events: {
                            dragend: function (marker, eventName, args) {
                                var lat = marker.getPosition().lat();
                                var lon = marker.getPosition().lng();
                                $scope.marker.options = {
                                    draggable: true,
                                    labelContent: "Imposta la tua posizione",
                                    labelAnchor: "60 0",
                                    labelClass: "marker-labels"
                                };
                            }
                        }
                    };
                },
                function (error) {
                    UIkit.notify('Si &egrave; verificato un errore nel recupero delle informazioni. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
                });
                
                // gestore delle mappe
                $scope.map = undefined;
                $scope.options = {
                    streetViewControl: false,
                    scaleControl: true,
                    panControl: true
                };

                /*search box*/
                var events = {
                    places_changed: function (searchBox) {
                        var places = searchBox.getPlaces();
                        var location = places[0].geometry.location;
                        $scope.map.center = {
                            latitude: location.k,
                            longitude: location.D
                        };
                        $scope.map.zoom = 18;
                        $scope.marker.coords = {
                            latitude: location.k,
                            longitude: location.D
                        };
                    }
                }
                $scope.searchbox = { template: 'searchbox.tpl.html', events: events };
                /*search box*/

                $scope.coordinateSelected = undefined;
                $scope.$watchCollection("marker.coords", function (newVal, oldVal) {
                    if (_.isEqual(newVal, oldVal))
                        return;
                    $scope.coordinateSelected = {
                        Geography: {
                            CoordinateSystemId: 4326,
                            WellKnownText: "POINT (" + $scope.marker.coords.latitude + " " + $scope.marker.coords.longitude + ")"
                        }
                    };
                });

                // gestore delle immagini caricate.
                $scope.currentImage = '';
                $scope.currentCroppedImage = '';
                var handleFileSelect = function (evt) {
                    var file = evt.currentTarget.files[0];
                    var reader = new FileReader();
                    reader.onload = function (evt) {
                        $scope.$apply(function ($scope) {
                            $scope.currentImage = evt.target.result;
                            $('img-crop>canvas').css({ 'margin-top': 0, 'margin-left': 0 });
                        });
                    };
                    reader.readAsDataURL(file);
                };
                angular.element(document.querySelector('#fileInput')).on('change', handleFileSelect);

                this.checkUsername = function () {
                    //verifica se esiste già un utente con questo nome
                    gxcFct.user.checkUsername({ username: $scope.currentUser.userName },
                        function (success) { if(!success) UIkit.notify('Username già presente.', { status: 'error', timeout: 5000 });},
                        function (error) { UIkit.notify('Username già presente.', { status: 'error', timeout: 5000 }); }
                        );
                };

                this.toggleAgreement = function () {
                    _this.TOSagreed = !_this.TOSagreed;
                };

                this.register = function () {
                    var userToSave = {
                        image: $scope.currentCroppedImage.replace(/^data:image\/(png|jpg);base64,/, ""),
                        id: $scope.currentUser.id,
                        userId: $scope.currentUser.userId,
                        positionUser: $scope.coordinateSelected,
                        provider: $scope.currentUser.provider,
                        userName: $scope.currentUser.userName,
                        password: $scope.currentUser.password,
                        confirmPassword: $scope.currentUser.confirmPassword,
                        email: $scope.currentUser.email
                    }
                    
                    gxcFct.user.update({ userId: $scope.currentUser.userId }, userToSave,
                    function (success) {
                        UIkit.notify('Utente aggiornato. Ora puoi accedere alle funzionalit&agrave; del sito', { status: 'success', timeout: 5000 });
                        window.location = '#/';
                    },
                    function (error) {
                        UIkit.notify('Si &egrave; verificato un errore durante la registrazione. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
                    });
                };
            },
            controllerAs: 'newuser'
        };
    }]);

    app.directive('exchangeSearch', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gamesSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/exchange-search.html',
            controller: function () {
                var _this = this;

                _this.GENRES_ALL = "Tutti";
                _this.PLATFORMS_ALL = "Tutte";
                _this.DISTANCES_ALL = "Nessun limite";

                _this.genres = gxcFct.genre.query();
                _this.platforms = gxcFct.platform.query();
                _this.distances = [5, 10, 20, 50, 100];

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

                this.getDistances = function () {
                    return gamesSrv.getDistances();
                };

                this.makeUserLink = function (userId) {
                    return "<user-link data-user-id='" + userId + "'></user-link>";
                };

                this.round = function (value) {
                    return Math.round(value * 100) / 100;
                };

                this.startSearch = function () {
                    _this.results = [];

                    var queryParameters = {
                        userId: userSrv.getUser().userId,
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

                _this.genres = gxcFct.genre.query();
                _this.platforms = gxcFct.platform.query();

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
                        name: _this.params.string,
                        genre: _this.params.genre === undefined ? undefined : _this.params.genre.genreId,
                        platform: _this.params.platform === undefined ? undefined : _this.params.platform.platformId
                    };

                    _this.results = gxcFct.game.query(queryParameters, function (success) {
                        _this.searchPerformed = true;
                    });
                }

                this.addToLibrary = function (game) {
                    var queryParameters = {
                        LibraryId: userSrv.getUser().LibraryId,
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

                _this.statuses = gxcFct.status.query();
                _this.languages = gxcFct.language.query();

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

                            g.canEdit = _this.libraryOwner == userSrv.getUser().id;
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
                            getGames(userSrv.getUser().LibraryId);
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
                        getGames(userSrv.getUser().LibraryId);
                        var modal = UIkit.modal("#gameEditor");
                        modal.hide();
                    });
                }

                getGames(userSrv.getUser().LibraryId);
            },
            controllerAs: 'library'
        };
    }]);

    app.directive('userProfile', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/user-profile.html',
            controller: function ($routeParams, $scope) {
                var _this = this;

                // gestore delle mappe
                $scope.map = {
                    zoom: 15
                };
                $scope.options = {
                    scaleControl: true,
                    panControl: true
                };

                $scope.marker = {
                    id: 0,
                    options: {
                        draggable: false
                    }                    
                };
                
                this.sendMessage = function () {
                    window.location = '#/mail/compose/' + _this.user.userId;
                };

                var queryParameters = {
                    userId: $routeParams.userId
                };

                gxcFct.user.get(queryParameters).$promise
                .then(function (success) {
                    _this.user = success;
                    var coords = _this.user.positionUser.geography.wellKnownText.replace('POINT (', '').replace(')', '').split(" ");                    
                    $scope.map.center = {
                        latitude: coords[0],
                        longitude: coords[1]
                    };;
                    $scope.marker.coords = {
                        latitude: coords[0],
                        longitude: coords[1]
                    };;
                });
            },
            controllerAs: 'profile'
        };
    }]);

    app.directive('userLink', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            scope: {
                userId: '@user'
            },
            template: '<a href=\'#/user/{{ userId }}\'>{{ link.username }} ({{ link.ranking }})</a>',
            controller: function ($scope) {
                var _this = this;
                _this.username = '';
                _this.ranking = 0;

                var queryParameters = {
                    userId: $scope.userId
                };

                gxcFct.user.get(queryParameters).$promise
                .then(function (success) {
                    _this.username = success.userName;
                });
            },
            controllerAs: 'link'
        };
    }]);

    app.directive('mailbox', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/mail-mailbox.html',
            controller: function ($routeParams) {
                var _this = this;

                _this.params = $routeParams;
                _this.messages = [];
                _this.messagesCount = { in: 0, out: 0 };
                _this.currentPage = 1;

                var getMessages = function (incoming, page) {
                    var queryParameters = {
                        userId: userSrv.getUser().userId,
                    };

                    _this.messages = [];
                    gxcFct.mail.byUser(queryParameters).$promise
                      .then(function (mailSuccess) {
                          _this.messages = incoming ? mailSuccess[0].messagesIn : mailSuccess[0].messagesOut;

                          for (msg in _this.messages) {
                              _this.messages[msg].userId = incoming ? _this.messages[msg].userProponent.userId : _this.messages[msg].userReceiving.userId;
                          }

                          _this.messagesCount.in = mailSuccess[0].messagesIn.length;
                          _this.messagesCount.out = mailSuccess[0].messagesOut.length;
                      }); // mail.byUser     

                    _this.currentPage = page;
                };

                this.hoverIn = function (mail) {
                    mail.hovered = true;
                }

                this.hoverOut = function (mail) {
                    mail.hovered = false;
                }

                this.open = function (mail) {
                    window.location = '#/mail/message/' + mail.messageId;
                }

                getMessages(_this.params.direction === 'in', _this.params.page);
            },
            controllerAs: 'mailbox'
        }
    }]);

    app.directive('message', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/mail-message.html',
            controller: function ($routeParams) {
                var _this = this;
                _this.message = {};

                this.reply = function () {

                };

                this.toggleImportant = function () {

                };

                this.archive = function () {

                };

                this.notify = function () {

                };

                this.backToMailbox = function () {
                    window.location = '#/mail/in/1';
                };

                //_this.message = gxcFct.mail.get(
                _this.msgId = $routeParams;

                gxcFct.mail.get({ messageId: $routeParams.messageId }).$promise
                  .then(function (success) {
                      _this.message = success;
                      _this.message.sender = gxcFct.user.byId({ userId: success.userProponent_Id });
                      gxcFct.user.byId({ userId: success.userReceiving_Id }).$promise
                      .then(function (receiverSuccess) {
                          _this.message.receiver = receiverSuccess;
                          _this.message.isIncoming = receiverSuccess.id == userSrv.getUser().id;
                      });
                  });
            },
            controllerAs: 'mail'
        };
    }]);

    app.directive('messageNew', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            scope: {
                exchange: '@exchange'
            },
            templateUrl: 'templates/mail-message-new.html',
            controller: function ($scope, $routeParams) {
                var _this = this;
                _this.message = { myItems: [], hisItems: [] };

                this.send = function () {
                    var queryParams = {
                        messageText: _this.message.text,
                        messageDate: new Date()
                    };

                    gxcFct.mail.send(queryParams).$promise
                    .then(function (success) {
                        UIkit.notify('Messaggio inviato', { status: 'success', timeout: 5000 });
                        window.location = '#/mail/in/1';
                    },
                    function (error) {
                        UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                    });
                };

                this.addItem = function (item, isMine) {
                    if (isMine)
                        _this.message.myItems.push(item);
                    else
                        _this.message.hisItems.push(item);
                };

                this.isItemAssigned = function (item, isMine) {
                    var result = false;

                    if (isMine)
                        result = _this.message.myItems.indexOf(item) >= 0;
                    else
                        result = _this.message.hisItems.indexOf(item) >= 0;

                    return result;
                };

                this.removeItem = function (item, isMine) {
                    if (isMine) {
                        var i = _this.message.myItems.indexOf(item);
                        _this.message.myItems.splice(i, 1);
                    }
                    else {
                        var i = _this.message.hisItems.indexOf(item);
                        _this.message.hisItems.splice(i, 1);
                    }
                };

                _this.recipientId = $routeParams.recipientId;

                gxcFct.library.get({ libraryId: userSrv.getUser().LibraryId }).$promise
                .then(function (librarySuccess) {

                    for (i in librarySuccess.libraryComponents) {
                        var g = librarySuccess.libraryComponents[i];
                        gameSrv.fillGameData(g);
                    }

                    _this.myLibrary = librarySuccess.libraryComponents;
                });

                gxcFct.library.byUser({ userId: _this.recipientId }).$promise
                .then(function(userSuccess) {
                    gxcFct.library.get({ libraryId: userSuccess[0].libraries[0].libraryId }).$promise
                    .then(function (librarySuccess) {

                        for (i in librarySuccess.libraryComponents) {
                            var g = librarySuccess.libraryComponents[i];
                            gameSrv.fillGameData(g);
                        }

                        _this.hisLibrary = librarySuccess.libraryComponents;
                    });
                });
                
            },
            controllerAs: 'mail'
        };
    }]);

    app.directive('transaction', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            scope: {                
            },
            templateUrl: 'templates/testTransaction.html',
            controller: function ($scope, $routeParams) {
                var _this = this;
                _this.transactionStatus = ['Aperta', 'InAttesa', 'Conclusa'];
                _this.userReceiving_Id = 'b692ce4a-f114-473d-a754-1e30173fb4cd'; //alessandro.pilati
                _this.userProponent_Id = 'b692ce4a-f114-473d-a754-1e30173fb4cb'; //andrea.tosato

                _this.currentProposalComponents = [];

                _this.currentProposal = {
                    dateStart: new Date(),
                    dateEnd: undefined,
                    direction: true, //la transazione iniziale ha sempre il verso PROPONENTE -> RICEVENTE
                    proposalText: 'Ciao sono il testo della proposta',
                    proposalObject: 'Ciao sono l\'oggetto della proposta',
                    userLastChanges_Id: _this.userProponent_Id, // utente Proponente
                    proposalComponents: undefined
                }

                this.LoadData = function () {
                    // Carico dei componenti nella proposta di scambio
                    gxcFct.library.get({ libraryId: userSrv.getUser().LibraryId }).$promise
                    .then(function (librarySuccess) {
                        for (i in librarySuccess.libraryComponents) {
                            var g = librarySuccess.libraryComponents[i];
                            _this.currentProposalComponents.push({
                                libraryComponents: g
                            });
                        }

                        // Aggiunta dei componenti alla proposta di scambio
                        _this.currentProposal.proposalComponents = _this.currentProposalComponents;
                    });

                }

                this.createInitialProposal = function () {
                    var queryParams = {
                        userProponent: _this.userReceiving,
                        userReceiving: _this.userProponent,
                        transactionStatus: _this.transactionStatus[0],
                        feedbacks: undefined,
                        proposals: _this.currentProposal
                    };

                    gxcFct.transaction.add(queryParams).$promise
                    .then(function (success) {                        
                    },
                    function (error) {                        
                    });
                };
                
            },
            controllerAs: 'transaction'
        };
    }]);

    app.directive('regolamento', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/regolamento.html'            
        };
    }]);
    app.directive('come-funziona', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/come-funziona-doc.html'
        };
    }]);
    app.directive('feedback', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/feedback-doc.html'
        };
    }]);
    app.directive('spedizioni', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/spedizioni-doc.html'
        };
    }]);
    app.directive('contattaci', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/contattaci.html'
        };
    }]);
})();
