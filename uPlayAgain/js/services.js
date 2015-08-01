(function() {
  var app = angular.module('gxc.services', ['ngRoute']);

  /**
   * Route service
   **/
  app.service('route-service', [ '$routeProvider', function($route) {
    this.route = $route;
  }]);
  
  /**
   * Mail service
   **/
  app.service('mail-service', [ 'factories', function(gxcFct) {
    this.getMessages = function(userId, incoming, page) {
      return gxcFct.mail.query();
    };

    this.getMessagesCount = function(userId) {
      return {in: 2, out: 3};
    };
  }]);

  
  /**
   * User service
   **/
  app.service('user-service', [ 'factories', function(gxcFct) {
    var _this = this;
    var user = {};
    
    this.login = function(username, password) {
      var queryParameters = {
          Username: username,
          Password: password,
          ConfirmPassword: password
      };
      
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
    
    this.updateUserData = function() {
      user.Games = 0;
      user.Messages = 0;
      user.LibraryId = undefined;

      gxcFct.library.byUser({ userId: user.userId }).$promise
        .then(function(libSuccess) {
          for(lib in libSuccess.$values) {
            user.LibraryId =libSuccess.$values[lib].libraryId;
            user.Games += libSuccess.$values[lib].libraryComponents.$values.length;
          }
        }, 
        function(error) {
          UIkit.notify('Si &egrave; verificato un errore nel recupero dei dati utente.', { status: 'warning', timeout: 5000 });
        }); // library.byUser     

      gxcFct.mail.incoming({ userId: user.userId }).$promise
        .then(function(mailSuccess) {
          for(mail in mailSuccess.$values) {
            user.Messages += mailSuccess.$values[mail].length;
          }
        }, 
        function(error) {
          UIkit.notify('Si &egrave; verificato un errore nel recupero dei dati utente.', { status: 'warning', timeout: 5000 });
        }); // mail.byUser     
    };
    
    this.logout = function() {
      user = {};
    };
    
    this.isLoggedIn = function() {
      return user.userId !== undefined;
    }
    
    this.getUser = function() {
      return user;
    }
  }]);
  
  /**
   * Games service
   **/
  app.service('games-service', [ 'factories', function(gxcFct) {
    //var genres = gxcFct.genre.query();
    //var platforms = gxcFct.platform.query();
    //var distances = [5, 10, 20, 50, 100];
    
    this.getGenreById = function(id) {
      var result = undefined;
      
      for(i in genres) {
        if(genres[i].IdGenre == id)
        {
          result = genres[i];
          break;
        }
      }
      
      return result;
    }
    
    this.getGenres = function() {
      return genres;
    }

    this.getPlatformById = function(id) {
      var result = undefined;
      
      for(i in platforms) {
        if(platforms[i].IdPlatform == id)
        {
          result = platforms[i];
          break;
        }
      }
      
      return result;
    }
          
    this.getPlatforms = function() {
      return platforms;
    }
    
    this.getDistances = function() {
      return distances;
    }
    
    //this.searchGames = function(title, genre, platform, maxDistance) {
    this.searchGames = function(params) {
      return gxcFct.game.query(params).$promise; /*{},
        function(data) {
          var results = [];
        
          for(i in data) {
            var game = data[i];

            if(title !== undefined && title.length > 0 && game.Title.toLowerCase().indexOf(title.toLowerCase()) >= 0)
            {
              results.push(game);
              continue;
            }
            if(genre !== undefined && game.Genre == genre.IdGenre)
            {
              results.push(game);
              continue;
            }
            if(platform !== undefined && game.Platform == platform.IdPlatform)
            {
              results.push(game);
              continue;
            }
      });*/
    }
  }]);

  /**
   * Library service
   **/
  app.service('library-service', [ 'factories', function(gxcFct) {
  }]);

})();
