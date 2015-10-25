(function () {
    var app = angular.module('gxc.directives.user', []);

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

    app.directive('formEditRegister', ['factories', 'user-service', function (gxcFct, userSrv) {
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
                    userId: userSrv.getCurrentUser().userId
                };
                gxcFct.user.get(queryParameters,
                function (success) {
                    $scope.currentUser = success;
                    $scope.currentImage = "data:image/png;base64," + $scope.currentUser.image;
                    $scope.currentUser.email = $scope.currentUser.mail;
                    $scope.currentUser.userName = $scope.currentUser.username;

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
                    window.location = '#/mail/compose/' + _this.user.id;
                };

                var queryParameters = {
                    userId: $routeParams.userId
                };

                gxcFct.user.profile(queryParameters).$promise
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
            templateUrl: 'templates/user-link.html',            
            controller: function ($scope) {
                var _this = this;
                _this.username = '';
                _this.ranking = 0;
                _this.rankingCount = 0;
                _this.mail = '';
                _this.image = '';
                _this.positionUser = {};

                var queryParameters = {
                    userId: $scope.userId
                };

                gxcFct.user.byId(queryParameters).$promise
                .then(function (success) {
                    _this.username = success.username;
                    _this.ranking = success.feedbackAvg;
                    _this.rankingCount = success.feedbackCount;
                    _this.mail = success.mail;
                    _this.image = success.image;
                    _this.positionUser = success.positionUser
                });
            },
            controllerAs: 'link'
        };
    }]);
})();
