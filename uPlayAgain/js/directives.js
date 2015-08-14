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

          for(i in _this.genres) {
            if(_this.genres[i].genreId == id)
            {
              result = _this.genres[i];
              break;
            }
          }

          return result;
        }
        
        this.getPlatformById = function(id) {
          var result = undefined;

          for(i in _this.platforms) {
            if(_this.platforms[i].platformId == id)
            {
              result =_this.platforms[i];
              break;
            }
          }

          return result;
        }
        
        this.getDistances = function() {
          return gamesSrv.getDistances();
        };
        
        this.makeUserLink = function (userId) {
            return "<user-link data-user-id='" + userId + "'></user-link>";
        };

        this.round = function (value) {
            return Math.round(value * 100) / 100;
        };

        this.startSearch = function() {
          _this.results = [];
          
          var queryParameters = {
            userId: userSrv.getUser().userId,
            gameTitle: _this.params.string === undefined? '' : _this.params.string,
            genreId: _this.params.genre === undefined ? '' : _this.params.genre.genreId,
            platformId: _this.params.platform === undefined ? '' : _this.params.platform.platformId,
            distance: _this.params.distance === undefined? 1000000 : _this.params.distance,
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

          for(i in _this.genres) {
            if(_this.genres[i].genreId == id)
            {
              result = _this.genres[i];
              break;
            }
          }

          return result;
        }
        
        this.getPlatformById = function(id) {
          var result = undefined;

          for(i in _this.platforms) {
            if(_this.platforms[i].platformId == id)
            {
              result =_this. platforms[i];
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
        
        _this.statuses = gxcFct.status.query();
        _this.languages = gxcFct.language.query();

          this.getStatusById = function(id) {
          var result = undefined;

          for(i in _this.statuses) {
            if(_this.statuses[i].statusId == id)
            {
              result = _this.statuses[i];
              break;
            }
          }

          return result;
        }
        
        this.getLanguageById = function(id) {
          var result = undefined;

          for(i in _this.languages) {
            if(_this.languages[i].gameLanguageId == id)
            {
              result =_this.languages[i];
              break;
            }
          }

          return result;
        }
        
                this.setStatus = function(status) {
          _this.editingGame.gameData.status = status;
        };

        this.setLanguage = function(language) {
          _this.editingGame.gameData.language = language;
        };
        
        var getGames = function() {
          var queryParameters = {
            libraryId: userSrv.getUser().LibraryId,
          };
          
          gxcFct.library.get(queryParameters).$promise
          .then(function(success) {
            for(i in success.libraryComponents) {
              var g = success.libraryComponents[i];
              var queryParameters = {
                gameId: g.gameId
              };
          
              g.gameData = gxcFct.game.get(queryParameters);
            }
            
            _this.games = success.libraryComponents;
          },
          function(error) {
            UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
          });
        };
        
        this.addGame = function() {
          window.location = '#/library/add';
        };
        
        this.editGame = function(game) {
          game.gameData.language = _this.getLanguageById(game.gameLanguageId);
          game.gameData.status = _this.getStatusById(game.statusId);
          _this.editingGame = game;
          
          var modal = UIkit.modal("#gameEditor");
          modal.show();
        };
        
        this.removeGame = function(game) {
          UIkit.modal.confirm("Sicuro di voler rimuovere " + game.gameData.title + " dalla tua libreria?", function(){
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
  
  app.directive('userProfile', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      templateUrl: 'templates/user-profile.html',
      controller: function($routeParams) {
        var _this = this;
        
        this.sendMessage = function() {
          window.location = '#/mail/compose/' + _this.user.userId;
        };
        
        var queryParameters = {
          userId: $routeParams.userId
        };
        
        gxcFct.user.get(queryParameters).$promise
        .then(function(success) {
          _this.user = success;
        });
      },
      controllerAs: 'profile'
    };
  }]);
  
  app.directive('userLink', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      scope: {
        userId : '@user'
      },
      template: '<a href=\'#/user/{{ userId }}\'>{{ link.username }} ({{ link.ranking }})</a>',
      controller: function($scope) {
        var _this = this;
        _this.username = '';
        _this.ranking = 0;
        
        var queryParameters = {
          userId: $scope.userId
        };
        
        gxcFct.user.get(queryParameters).$promise
        .then(function(success) {
          _this.username = success.userName;
        });
      },
      controllerAs: 'link'
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
        _this.messagesCount = { in: 0, out: 0 };
        _this.currentPage = 1;
        
        var getMessages = function(incoming, page) {
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
        
        this.hoverIn = function(mail) {
          mail.hovered = true;
        }
        
        this.hoverOut = function(mail) {
          mail.hovered = false;
        }
        
        this.open = function(mail) {
          window.location = '#/mail/message/' + mail.messageId;
        }

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
          _this.message = {};
        
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

        gxcFct.mail.get({ messageId: $routeParams.messageId }).$promise
          .then(function (success) {
              _this.message = success;
          });
      },
      controllerAs: 'mail'
    };
  }]);
  
  app.directive('messageNew', [ 'factories', function(gxcFct) {
    return {
      restrict: 'E',
      scope: {
        exchange: '@exchange'
      },
      templateUrl: 'templates/mail-message-new.html',
      controller: function($scope, $routeParams) {
          var _this = this;
          _this.message = {};
        
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
        
        _this.recipientId = $routeParams.recipientId;
      },
      controllerAs: 'mail'
    };
  }]);
})();
