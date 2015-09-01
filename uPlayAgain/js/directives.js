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
                            latitude: location.lat(),
                            longitude: location.lng()
                        };
                        $scope.map.zoom = 18;
                        $scope.marker.coords = {
                            latitude: location.lat(),
                            longitude: location.lng()
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
                _this.recipientData = undefined;

                var addProposal = function () {
                    var queryParams = {
                        proposalText: _this.message.text,
                        proposalObject: _this.message.titolo,
                        proposalDate: new Date().toISOString(),
                        direction: (_this.currentProposal === undefined ? true : !_this.currentProposal.direction),
                        transactionId: _this.currentTransaction.id,
                        userLastChanges_Id: userSrv.getUser().id
                    };

                    gxcFct.proposal.add(queryParams).$promise
                    .then(function (success) {
                        var proposal = success;

                        for (i in _this.message.myItems) {
                            var queryParams = {
                                libraryComponentId: _this.message.myItems[i].id,
                                proposalId: success.id
                            }
                            gxcFct.proposalComponents.add(queryParams);
                        }
                        for (i in _this.message.hisItems) {
                            var queryParams = {
                                libraryComponentId: _this.message.hisItems[i].id,
                                proposalId: success.id
                            }
                            gxcFct.proposalComponents.add(queryParams);
                        }
                    });
                }

                this.send = function () {
                    if (_this.exchange)
                    {
                        if (_this.currentTransaction === undefined) {
                            var queryParams = {
                                userProponent_Id: userSrv.getUser().id,
                                userReceiving_Id: _this.recipientData.id
                            }

                            gxcFct.transaction.add(queryParams).$promise
                            .then(function (success) {
                                _this.currentTransaction = success;
                                addProposal();
                            });
                        }
                        else
                        {
                            addProposal();
                        }
                    }
                    else {
                        var queryParams = {
                            messageText: _this.message.text,
                            messageObject: _this.message.titolo,
                            messageDate: new Date().toISOString(),
                            userProponent_Id: userSrv.getUser().id,
                            userReceiving_Id: _this.recipientData.id
                        };
                        
                        gxcFct.mail.send(queryParams).$promise
                        .then(function (success) {
                            UIkit.notify('Messaggio inviato', { status: 'success', timeout: 5000 });
                            window.location = '#/mail/in/1';
                        },
                        function (error) {
                            UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                        });
                    }
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
                .then(function (userSuccess) {
                    _this.recipientData = userSuccess[0];
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

    app.directive('testTransaction', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            //scope: {                
            //},
            templateUrl: 'templates/test-transaction.html',
            controller: function ($routeParams, $scope) {
                var _this = this;
                _this.transactionStatus = ['Aperta', 'InAttesa', 'Conclusa'];
                _this.proposalStatus = ['DaApprovare','Accettata','Rifiutata'];
                _this.userReceiving_Id = 'b692ce4a-f114-473d-a754-1e30173fb4cd'; //alessandro.pilati
                _this.selectedLibraryGames = []; // Array di giochi presenti nella libreria dell'utente che riceve la proposta.
                _this.proposalText = 'Ciao sono il testo della proposta';
                _this.proposalObject = 'Ciao sono l\'oggetto della proposta';

                var currentDate = new Date();
                var futureDate = new Date(); //Aggiungere 1 anno

                // Oggetto contenente una proposta
                _this.currentProposal = [{
                    dateStart: currentDate.toISOString(),
                    dateEnd: futureDate.toISOString(),
                    direction: true, //la transazione iniziale ha sempre il verso PROPONENTE -> RICEVENTE
                    proposalText: _this.proposalText,
                    proposalObject: _this.proposalObject,
                    transactionId: undefined, // La transazione all'inizio non è ancora stata creata
                    userLastChanges_Id: 'b692ce4a-f114-473d-a754-1e30173fb4cb', //userSrv.getUser().id, // utente Proponente
                    userProponent_ProposalStatus: _this.proposalStatus[1], // Stato della proposta corrente per l'utente proponente. Se la propone ovviamente significa che l'accetta
                    userReceiving_ProposalStatus: _this.proposalStatus[0], // Stato della proposta corrente per l'utente ricevente
                    proposalComponents: []
                }];

                /*
                Carica dei dati di esempio. In questo caso aggiungo alla proposta tutti i giochi della mia libreria.
                */
                $scope.LoadData = function () {
                    // Carico dei componenti nella proposta di scambio. 
                    // I giochi verranno selezionati dall'utente Proponente. 
                    // I giochi selezionati saranno presenti nella libreria dell'utente ricevente e nella libreria dell'utente proponente.

                    // TODO: sistemare la libreria di lettura!
                    gxcFct.library.get({ libraryId: userSrv.getUser().LibraryId }).$promise
                    .then(function (librarySuccess) {
                        for (i in librarySuccess.libraryComponents) {
                            var g = librarySuccess.libraryComponents[i];
                            _this.selectedLibraryGames.push({libraryComponentId: g.libraryComponentId});
                        }

                        // Aggiunta dei componenti alla proposta di scambio
                        _this.currentProposal[0].proposalComponents = _this.selectedLibraryGames;
                    });

                }

                $scope.createInitialProposal = function () {
                    var queryParams = {
                        userProponent_Id: userSrv.getUser().id,
                        userReceiving_Id: _this.userReceiving_Id,
                        transactionStatus: _this.transactionStatus[0],
                        feedbacks: undefined,
                        proposals: _this.currentProposal
                    };

                    gxcFct.transaction.add(queryParams).$promise
                    .then(function (success) {
                        UIkit.notify('Transazione creata', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione transazione', { status: 'success', timeout: 5000 });
                    });
                };

                /*
                Carica tutte le transazioni per un utente.
                Da studiare:
                1) Quando considerare una transazione conclusa? Usiamo le date o il campo TransactionStatus?
                2) Servono ancora le date della transazione? Oppure le usiamo per stabilire la durata temporale (DataStar -> Inizio transazione; DataEnd -> Transazione conclusa/annullata)
                2b) Servono ancora le date della proposte? Oppure le usiamo per stabilire la durata temporale (DataStar -> Inizio proposta; DataEnd -> Proposta conclusa/annullata)
                3) Una volta che la transazione è conclusa si potrà generare il feedback.
                4) Una volta che la transazione è annullata NON si potrà generare nessun feedback.
                */
                $scope.LoadTransactionByUser = function () {
                    var queryParameters = {
                        userId: userSrv.getUser().userId,
                    };
                    gxcFct.transaction.byUser(queryParameters).$promise
                      .then(function (transSuccess) {
                          _this.tranProponent = transSuccess[0].transactionsProponent;
                          _this.tranReceiving = transSuccess[0].transactionsReceiving;
                      }); // transaction.byUser    
                }

                /*
                Aggiungo una nuova proposta alla transazione già creata in precedenza.
                Questa funzione serve per:
                1) Aggiungere una nuova proposta alla transazione attuale
                */
                $scope.AddProposal = function () {
                    var newProposal = _this.currentProposal[0];
                    // Prelevo una transazione a caso da quelle ricevute
                    newProposal.transactionId = _this.tranReceiving[0].transactionId;
                    // metto un pò di dati casuali
                    newProposal.proposalComponents = [_this.currentProposal[0].proposalComponents[1]];
                    newProposal.proposalText = 'Rilancio';
                    newProposal.proposalObject = 'Oggetto del rilancio';
                    newProposal.userLastChanges_Id = userSrv.getUser().id;
                    newProposal.direction = false; // Rilancio del ricevente
                    newProposal.userProponent_ProposalStatus = _this.proposalStatus[0], // Stato della proposta corrente per l'utente proponente.
                    newProposal.userReceiving_ProposalStatus = _this.proposalStatus[1], // Stato della proposta corrente per l'utente ricevente. Se la propone ovviamente significa che l'accetta

                    gxcFct.proposal.add(newProposal).$promise
                    .then(function (success) {
                        UIkit.notify('Proposta creata', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione Proposta', { status: 'success', timeout: 5000 });
                    });
                }


                /*
                Aggiornamento della proposta
                */
                $scope.UpdateProposal = function () {

                    var randomProposalId = _this.tranReceiving[0].proposals[0].proposalId;

                    gxcFct.proposal.get({ propId: randomProposalId }).$promise
                   .then(function (success) {
                       var oldProposal = success;
                       oldProposal.proposalObject = 'ProposataAggiornata';

                       gxcFct.proposal.update({ propId: randomProposalId }, oldProposal,
                        function (success) {
                            UIkit.notify('Utente aggiornato. Ora puoi accedere alle funzionalit&agrave; del sito', { status: 'success', timeout: 5000 });
                            window.location = '#/';
                        },
                        function (error) {
                            UIkit.notify('Si &egrave; verificato un errore durante la registrazione. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
                        });
                   },
                   function (error) {
                       UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                   });

                    
                    
                }












                /*Sezione feedback*/
                _this.Feedback = {
                    transactionId: undefined,
                    userId: undefined,
                    rate: undefined
                };
                // In caso di transazione positiva assegnare un +1, in caso di transazione negativa un -3
                _this.RateVote = [1,-3];

                $scope.AddFeedback = function () {
                    // Prelevo una transazione a caso da quelle ricevute
                    _this.Feedback.transactionId = _this.tranReceiving[0].transactionId;
                    // Prelevo l'utente corrente o l'utente destinatario della transazione
                    _this.Feedback.userId = _this.tranReceiving[0].userReceiving_Id;
                    _this.Feedback.rate = _this.RateVote[0];

                    gxcFct.feedback.add(_this.Feedback).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback creato', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione Feedback', { status: 'success', timeout: 5000 });
                    });

                }

                $scope.GetRate = function () {
                    // Ritorno il rate dell'utente corrente
                    var queryParameters = {
                        userId: userSrv.getUser().id,
                    };

                    gxcFct.feedback.rate(queryParameters).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback rate: ' + success.rate + "% su " + success.counter + "feedback ricevuto", { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore rate Feedback', { status: 'success', timeout: 5000 });
                    });
                }
                
                // Ritorna tutti gli ID delle transazioni senza feedback per l'utente
                $scope.GetPendingTransactionFeedback = function () {
                    // Ritorno il rate dell'utente corrente
                    var queryParameters = {
                        userId: 'b692ce4a-f114-473d-a754-1e30173fb4cb'//userSrv.getUser().id,
                    };

                    gxcFct.feedback.pending(queryParameters).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback pending: ' + success, { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore pending Feedback', { status: 'success', timeout: 5000 });
                    });
                }
            },
            controllerAs: 'testTransaction'
        };
    }]);
    
    app.directive('feedback-vote', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/feedback-vote.html',
            controller: function ($routeParams, $scope) {
                var _this = this;
                _this.currentUserId = userSrv.getUser().id;
                _this.currentTransactionToVote = [];                
                                
                _this.GetPendingTransactionFeedback = function () {
                    gxcFct.feedback.pending({ userId: userSrv.getUser().id }).$promise
                    .then(function (success) {
                        success.forEach(function (tran) {
                            gxcFct.transaction.get({ tranId: tran }).$promise
                            .then(function (successTran) {
                                // Carico solo la proposta conclusa e i rispettivi componenti.
                                for (i = successTran.proposals.length - 1; i >= 0; i--) {
                                    // rimuovo le proposte non accettate da entrambi gli utenti
                                    if (successTran.proposals[i].userReceiving_ProposalStatus != 1 && successTran.proposals[i].userProponent_ProposalStatus != 1) {
                                        array.splice(i, 1);
                                    }
                                    //Aggiungo i componenti alla proposta
                                    else {
                                        gxcFct.proposal.get({ propId: successTran.proposals[i].proposalId }).$promise
                                        .then(function (successComponents) {
                                            successTran.proposals[i].proposalComponents = successComponents;
                                        },
                                        function (error) {
                                            UIkit.notify('Errore lettura componenti della proposata', { status: 'success', timeout: 5000 });
                                        });                                         
                                    }
                                }

                                // TODO -> Caricare il feedback dell'utente presente nelle transazioni.
                                _this.currentTransactionToVote.push(successTran);
                            },
                            function (error) {
                                UIkit.notify('Errore lettura transazioni da votare', { status: 'success', timeout: 5000 });
                            });
                        });
                    },
                    function (error) {
                        UIkit.notify('Errore lettura feedback da votare', { status: 'success', timeout: 5000 });
                    });
                }
                // Carico le transazioni da assegnare con un feedback all'avvio
                _this.GetPendingTransactionFeedback();
                
                _this.AddFeedback = function (transactionId, vote) {
                    // In caso di transazione positiva assegnare un +1, in caso di transazione negativa un -3

                    var tranFilter = array.filter(function (tranFilter) {
                        if (tranFilter.transactionId === transactionId) return tranFilter;
                    })[0];

                    var userToFeedback = undefined;
                    if (tranFilter.userProponent_Id != userSrv.getUser().id && tranFilter.userReceiving_Id == userSrv.getUser().id) userToFeedback = tranFilter.userProponent_Id;
                    if (tranFilter.userProponent_Id == userSrv.getUser().id && tranFilter.userReceiving_Id != userSrv.getUser().id) userToFeedback = tranFilter.userReceiving_Id;

                    _this.Feedback = {
                        transactionId: transactionId,
                        userId: userToFeedback,
                        rate: vote
                    };                    

                    gxcFct.feedback.add(_this.Feedback).$promise
                    .then(function (success) {
                        UIkit.notify('Hai votato con successo!', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione Feedback', { status: 'success', timeout: 5000 });
                    });
                }                
            },
            controllerAs: 'feedback-vote'
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
