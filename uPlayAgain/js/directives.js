(function() {
  var app = angular.module('gxc.directives', []);

  app.directive('formRegister', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      templateUrl: 'templates/form-register.html',
      controller: function ($routeParams, $scope, uiGmapGoogleMapApi) {
        var _this = this;
        _this.params = {};
        _this.TOSagreed = false;
        
     
        // gestore delle mappe
        $scope.map = { center: { latitude: 45.4300, longitude: 10.9880 }, zoom: 8 };
        $scope.options = { scrollwheel: false };
        $scope.marker = {
            id: 0,
            coords: {
                latitude: 45.4300,
                longitude: 10.9880
            },
            options: { draggable: true },
            events: {
                dragend: function (marker, eventName, args) {
                    var lat = marker.getPosition().lat();
                    var lon = marker.getPosition().lng();
                    $scope.marker.options = {
                        draggable: true,
                        labelContent: "lat: " + $scope.marker.coords.latitude + ' ' + 'lon: ' + $scope.marker.coords.longitude,
                        labelAnchor: "100 0",
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
        
        this.checkUsername = function() {
          //verifica se esiste già un utente con questo nome
        };
        
        this.findLocation = function() {
          //geolocator
        };
            
        this.toggleAgreement = function() {
          _this.TOSagreed = !_this.TOSagreed;
        };
        
        this.register = function() {
          if(!_this.TOSagreed)
          {
            UIkit.notify('Devi accettare le condizioni d\'uso del sito per poterti registrare.', { status: 'warning', timeout: 5000 });
          }
          else
          {
            var queryParameters = {
                userName: _this.params.username,
                password: _this.params.password,
                confirmPassword: _this.params.confirmPassword,
                positionUser: _this.params.location,
                email: _this.params.email,
                image: $scope.currentCroppedImage,
                // TODO: controllare che non sia la coordinata di default!
                positionUser: $scope.coordinateSelected
            }
            gxcFct.user.register(queryParameters,
            function(success) {
              UIkit.notify('Utente registrato. Ora puoi accedere alle funzionalit&agrave; del sito', { status: 'success', timeout: 5000 });
              window.location = '#/';
            },
            function(error){
              UIkit.notify('Si &egrave; verificato un errore durante la registrazione. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
            }
            );
          }
        };
      },
      controllerAs: 'newuser'
    };
  }]);

  app.directive('exchangeSearch', [ 'factories', 'user-service', 'games-service', function(gxcFct, userSrv, gamesSrv) {
    return {
      restrict: 'E',
      templateUrl: 'templates/exchange-search.html',
      controller: function() {
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
        
        this.setGenre = function(genre) {
          _this.params.genre = genre;
        };

        this.setPlatform = function(platform) {
          _this.params.platform = platform;
        };
        
        this.setDistance = function(distance) {
          _this.params.distance = distance;
        }

        this.reset = function() {
          _this.params = {};
        };
        
        this.getGenreById = function(id) {
          var result = undefined;

          for(i in _this.genres.$values) {
            if(_this.genres.$values[i].genreId == id)
            {
              result = _this.genres.$values[i];
              break;
            }
          }

          return result;
        }
        
        this.getPlatformById = function(id) {
          var result = undefined;

          for(i in _this.platforms.$values) {
            if(_this.platforms.$values[i].platformId == id)
            {
              result =_this.platforms.$values[i];
              break;
            }
          }

          return result;
        }
        
        this.getDistances = function() {
          return gamesSrv.getDistances();
        };
        
        this.startSearch = function() {
          _this.results = [];
          
          var queryParameters = {
            userId: userSrv.getUser().userId,
            gameTitle: _this.params.string === undefined? '' : _this.params.string,
            genreId: _this.params.genre === undefined ? '' : _this.params.genre.genreId,
            platformId: _this.params.platform === undefined ? '' : _this.params.platform.platformId,
            distance: _this.params.distance === undefined? 1000000 : _this.params.distance
          };
          
          _this.results = gxcFct.game.search(queryParameters, function(success) {
            _this.searchPerformed = true;
          });
        }
      },
      controllerAs: 'search'
    };
  }]);
  
  app.directive('gamesSearch', [ 'factories', 'user-service', 'games-service', function(gxcFct, userSrv, gamesSrv) {
    return {
      restrict: 'E',
      templateUrl: 'templates/games-search.html',
      controller: function() {
        var _this = this;
        
        _this.GENRES_ALL = "Tutti";
        _this.PLATFORMS_ALL = "Tutte";
        
        _this.genres = gxcFct.genre.query();
        _this.platforms = gxcFct.platform.query();
        
        _this.searchPerformed = false;
        _this.params = {};
        _this.results = [];
        
        this.setGenre = function(genre) {
          _this.params.genre = genre;
        };

        this.setPlatform = function(platform) {
          _this.params.platform = platform;
        };
        
        this.reset = function() {
          _this.params = {};
        };
        
        this.getGenreById = function(id) {
          var result = undefined;

          for(i in _this.genres.$values) {
            if(_this.genres.$values[i].genreId == id)
            {
              result = _this.genres.$values[i];
              break;
            }
          }

          return result;
        }
        
        this.getPlatformById = function(id) {
          var result = undefined;

          for(i in _this.platforms.$values) {
            if(_this.platforms.$values[i].platformId == id)
            {
              result =_this. platforms.$values[i];
              break;
            }
          }

          return result;
        }
        
        this.startSearch = function() {
          _this.results = [];
          
          var queryParameters = {
            name: _this.params.string,
            genre: _this.params.genre === undefined ? undefined : _this.params.genre.genreId,
            platform: _this.params.platform === undefined ? undefined : _this.params.platform.platformId
          };
          
          _this.results = gxcFct.game.query(queryParameters, function(success) {
            _this.searchPerformed = true;
          });
        }
        
        this.addToLibrary = function(game) {
          var queryParameters = {
            LibraryId: userSrv.getUser().LibraryId,
            GameId: game.gameId,
            GameLanguageId: 3, //per ora fisso
            StatusId: 1
          };
          
          gxcFct.library.add(queryParameters).$promise
            .then(function(success) {
              userSrv.updateUserData();
              UIkit.notify(game.title + ' &grave; stato aggiunto alla libreria', { status: 'success', timeout: 5000 });
            },
            function(error) {
              UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
            });
        }
      },
      controllerAs: 'search'
    };
  }]);
  
  app.directive('gamesLibrary', [ 'factories', 'user-service', function(gxcFct, userSrv) {
    return {
      restrict: 'E',
      templateUrl: 'templates/games-library.html',
      controller: function() {
        var _this = this;
        _this.games = [];
        
        var getGames = function() {
          var queryParameters = {
            libraryId: userSrv.getUser().LibraryId,
          };
          
          gxcFct.library.get(queryParameters).$promise
          .then(function(success) {
            for(i in success.libraryComponents.$values) {
              var g = success.libraryComponents.$values[i];
              var queryParameters = {
                gameId: g.gameId
              };
          
              g.gameData = gxcFct.game.get(queryParameters);
            }
            
            _this.games = success.libraryComponents.$values;
          },
          function(error) {
            UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
          });
        };
        
        this.addGame = function() {
          window.location = '#/library/add';
        };
        
        this.editGame = function(game) {
          window.location = '#/edit';
        };
        
        this.removeGame = function(game) {
          UIkit.modal.confirm("Sicuro di voler rimuovere " + game.title + " dalla tua libreria?", function(){
            var queryParameters = {
              componentId: game.libraryComponentId,
            };
            
            gxcFct.library.remove(queryParameters).$promise
            .then(function(success) {
              getGames();
              userSrv.updateUserData();
            });
          });
        };
        
        getGames();
      },
      controllerAs: 'library'
    };
  }]);
  
  app.directive('userProfile', [ 'user-service', function(userSrv) {
    return {
      restrict: 'E',
      templateUrl: 'user-profile.html',
      controller: function() {
        var _this = this;
        
        
      },
      controllerAs: 'profile'
    };
  }]);
  
  app.directive('mailbox', [ 'factories', 'user-service', function(gxcFct, userSrv) {
    return {
      restrict: 'E',
      templateUrl: 'templates/mail-mailbox.html',
      controller: function($routeParams) {
        var _this = this;
        
        _this.params = $routeParams;
        _this.messages = [];
        _this.messagesCount = {in: 0, out: 0};
        _this.currentPage = 1;
        
        var getMessages = function(incoming, page) {
          var queryParameters = {
            userId: userSrv.getUser().userId,
          };
          
          _this.messages = [];
          if(incoming) {
            _this.messages = gxcFct.mail.incoming(queryParameters);
          }
          else {
            _this.messages = gxcFct.mail.outgoing(queryParameters);
          }
          
          _this.currentPage = page;
        };
        
        var getMessagesCount = function() {
          var queryParameters = {
            userId: userSrv.getUser().userId,
          };
          
          gxcFct.mail.incoming(queryParameters).$promise
            .then(function(success) {
              _this.messagesCount.in = success.$values.length;
            });
          gxcFct.mail.outgoing(queryParameters).$promise
            .then(function(success) {
              _this.messagesCount.out = success.$values.length;
            });
        };

        this.hoverIn = function(mail) {
          mail.hovered = true;
        }
        
        this.hoverOut = function(mail) {
          mail.hovered = false;
        }
        
        this.open = function(mail) {
          window.location = '#/mail/message/' + mail.MessageId;
        }
        
        getMessagesCount();
        getMessages(_this.params.direction === 'in', _this.params.page);
      },
      controllerAs: 'mailbox'
    }
  }]);
  
  app.directive('message', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      templateUrl: 'templates/mail-message.html',
      controller: function($routeParams) {
        var _this = this;
        
        this.reply = function() {
          
        };
        
        this.toggleImportant = function() {
          
        };
        
        this.archive = function() {
          
        };
        
        this.notify = function() {
          
        };
        
        this.backToMailbox = function() {
          window.location = '#/mail/in/1';
        };
        
        //_this.message = gxcFct.mail.get(
        _this.msgId = $routeParams;
      },
      controllerAs: 'mail'
    };
  }]);
  
  app.directive('messageNew', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      templateUrl: 'templates/mail-message-new.html',
      controller: function($routeParams) {
        var _this = this;
        
        this.reply = function() {
          
        };
        
        this.toggleImportant = function() {
          
        };
        
        this.archive = function() {
          
        };
        
        this.notify = function() {
          
        };
        
        this.send = function() {
          window.location = '#/mail/in/1';
        };
        
        //_this.message = gxcFct.mail.get(
        _this.msgId = $routeParams;
      },
      controllerAs: 'mail'
    };
  }]);
})();
