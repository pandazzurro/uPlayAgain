(function() {
  var app = angular.module('gxc.factories', ['ngResource']);

  app.webapi = "";
  
  /**
   * Mail factory
   **/
  //var mailService = angular.module('gxcServices', [ 'ngResource' ]);
  app.factory('factories', ['$resource', function($resource) {
    return {
      mail: $resource(app.webapi, {}, {
          byUser: { url: app.webapi + '/api/messages/byUser/:userId', method: 'GET', isArray: false },
          get: { url: app.webapi + '/api/messages/:messageId', method: 'GET', isArray: false },
          send: { url: app.webapi + '/api/messages', method: 'POST', isArray: false },
      }),
      genre: $resource(app.webapi + '/api/genres', {}, {
        query: { method: 'GET', isArray: true }
      }),
      platform: $resource(app.webapi + '/api/platforms', {}, {
        query: { method: 'GET', isArray: true }
      }),
      language: $resource(app.webapi + '/api/gamelanguages', {}, {
        query: { method: 'GET', isArray: true }
      }),
      status: $resource(app.webapi + '/api/status', {}, {
        query: { method: 'GET', isArray: true }
      }),
      game: $resource(app.webapi, {}, {
        query: { url: app.webapi + '/api/games/search', method: 'GET', isArray: true },
        get : { url: app.webapi + '/api/games/:gameId', method: 'GET', isArray: false },
        search: { url: app.webapi + '/api/search', method: 'GET', isArray: false }
      }),
      library: $resource(app.webapi, {} , {
        byUser: { url: app.webapi + '/api/libraries/byUser/:userId', method: 'GET', isArray: true },
        get: { url: app.webapi + '/api/libraries/:libraryId', method: 'GET', isArray: false },
        add: { url: app.webapi + '/api/librarycomponents', method: 'POST', isArray: false },
        remove: { url: app.webapi + '/api/librarycomponents/:componentId', method: 'DELETE', isArray: false },
        update: { url: app.webapi + '/api/librarycomponents/:componentId', method: 'PUT', isArray: false },
      }),
      user: $resource(app.webapi, {}, {
        get: { url: app.webapi + '/api/users/:userId', method: 'GET', isArray: false },
        checkUsername: { url: app.webapi + '/api/users/Exists/:username', method: 'GET', isArray: false },
        byId: { url: app.webapi + '/api/users/identity/:userId', method: 'GET', isArray: true },
        profile: { url: app.webapi + '/api/users/profile/:userId', method: 'GET', isArray: false },
        login: { url: app.webapi + '/api/account/login', method: 'POST', isArray: false },
        logout: { url: app.webapi + '/api/account/logout', method: 'POST', isArray: false },
        register: { url: app.webapi + '/api/account/register', method: 'POST', isArray: true },
        update: { url: app.webapi + '/api/users/:userId', method: 'PUT', isArray: false },
        remove: { url: app.webapi + '/api/users/:userId', method: 'DELETE', isArray: false }
      }),
      transaction: $resource(app.webapi, {}, {
          byUser: { url: app.webapi + '/api/transactions/byUser/:userId', method: 'GET', isArray: true },
          get: { url: app.webapi + '/api/transactions/:tranId', method: 'GET', isArray: false },
          add: { url: app.webapi + '/api/transactions', method: 'POST', isArray: false },
          update: { url: app.webapi + '/api/transactions/:tranId', method: 'PUT', isArray: false }
      }),
      proposal: $resource(app.webapi, {}, {
          get: { url: app.webapi + '/api/proposals/:propId', method: 'GET', isArray: false },
          add: { url: app.webapi + '/api/proposals', method: 'POST', isArray: false },
          update: { url: app.webapi + '/api/proposals/:propId', method: 'PUT', isArray: false },
          remove: { url: app.webapi + '/api/proposalComponents/:propCompId', method: 'DELETE', isArray: false }
      }),
      proposalComponents: $resource(app.webapi, {}, {
          get: { url: app.webapi + '/api/proposalComponents/:propCompId', method: 'GET', isArray: false },
          add: { url: app.webapi + '/api/proposalComponents', method: 'POST', isArray: false },
          update: { url: app.webapi + '/api/proposalComponents/:propCompId', method: 'PUT', isArray: false },
          remove: { url: app.webapi + '/api/proposalComponents/:propCompId', method: 'DELETE', isArray: false }
      }),
      feedback: $resource(app.webapi, {}, {
          byUser: { url: app.webapi + '/api/feedbacks/byUser/:userId', method: 'GET', isArray: true },
          get: { url: app.webapi + '/api/feedbacks/:tranId', method: 'GET', isArray: false }, // mi faccio ritornare il feedback di una specifica transazione
          add: { url: app.webapi + '/api/feedbacks', method: 'POST', isArray: false },
          rate: { url: app.webapi + '/api/feedbacks/rate/:userId', method: 'GET', isArray: false },
          pending: { url: app.webapi + '/api/feedbacks/pending/:userId', method: 'GET', isArray: true },
          update: { url: app.webapi + '/api/feedbacks/:tranId', method: 'PUT', isArray: false }
      }),
    }
  }]);

  app.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService', function ($q, $injector, $location, localStorageService) {

      var authInterceptorServiceFactory = {};
      var _request = function (config) {
          config.headers = config.headers || {};
          var authData = localStorageService.get('authorizationData');
          if (authData) {
              config.headers.Authorization = 'Bearer ' + authData.token;
          }
          return config;
      }

      var _responseError = function (rejection) {
          if (rejection.status === 401) {
              var authService = $injector.get('authService');
              var authData = localStorageService.get('authorizationData');

              if (authData) {
                  if (authData.useRefreshTokens) {
                      $location.path('/refresh');
                      return $q.reject(rejection);
                  }
              }
              authService.logOut();
              $location.path('/login');
          }
          return $q.reject(rejection);
      }

      authInterceptorServiceFactory.request = _request;
      authInterceptorServiceFactory.responseError = _responseError;

      return authInterceptorServiceFactory;
  }]);

  app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {
      var serviceBase = ngAuthSettings.apiServiceBaseUri;
      var authServiceFactory = {};

      var _authentication = {
          isAuth: false,
          userName: "",
          useRefreshTokens: false
      };

      var _externalAuthData = {
          provider: "",
          userName: "",
          externalAccessToken: ""
      };

      /* La registrazione è già gestita. Non bisogna rifarla
      var _saveRegistration = function (registration) {
          _logOut();
          return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
              return response;
          });
  
      };
      */

      var _login = function (loginData) {
          var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;
          if (loginData.useRefreshTokens) {
              data = data + "&client_id=" + ngAuthSettings.clientId;
          }
          var deferred = $q.defer();
          $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
              if (loginData.useRefreshTokens) {
                  localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
              }
              else {
                  localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
              }
              _authentication.isAuth = true;
              _authentication.userName = loginData.userName;
              _authentication.useRefreshTokens = loginData.useRefreshTokens;

              deferred.resolve(response);

          }).error(function (err, status) {
              _logOut();
              deferred.reject(err);
          });

          return deferred.promise;

      };

      var _logOut = function () {
          localStorageService.remove('authorizationData');

          _authentication.isAuth = false;
          _authentication.userName = "";
          _authentication.useRefreshTokens = false;
      };

      var _fillAuthData = function () {
          var authData = localStorageService.get('authorizationData');
          if (authData) {
              _authentication.isAuth = true;
              _authentication.userName = authData.userName;
              _authentication.useRefreshTokens = authData.useRefreshTokens;
          }
      };

      var _refreshToken = function () {
          var deferred = $q.defer();
          var authData = localStorageService.get('authorizationData');
          if (authData) {
              if (authData.useRefreshTokens) {
                  var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + ngAuthSettings.clientId;
                  localStorageService.remove('authorizationData');
                  $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
                      localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
                      deferred.resolve(response);
                  }).error(function (err, status) {
                      _logOut();
                      deferred.reject(err);
                  });
              }
          }
          return deferred.promise;
      };

      /*TODO*/
      var _obtainAccessToken = function (externalData) {
          var deferred = $q.defer();
          $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {
              localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });
              _authentication.isAuth = true;
              _authentication.userName = response.userName;
              _authentication.useRefreshTokens = false;
              deferred.resolve(response);
          }).error(function (err, status) {
              _logOut();
              deferred.reject(err);
          });
          return deferred.promise;
      };

      /*TODO*/
      var _registerExternal = function (registerExternalData) {
          var deferred = $q.defer();
          $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {
              localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

              _authentication.isAuth = true;
              _authentication.userName = response.userName;
              _authentication.useRefreshTokens = false;

              deferred.resolve(response);
          }).error(function (err, status) {
              _logOut();
              deferred.reject(err);
          });

          return deferred.promise;

      };

      //authServiceFactory.saveRegistration = _saveRegistration;
      authServiceFactory.login = _login;
      authServiceFactory.logOut = _logOut;
      authServiceFactory.fillAuthData = _fillAuthData;
      authServiceFactory.authentication = _authentication;
      authServiceFactory.refreshToken = _refreshToken;

      authServiceFactory.obtainAccessToken = _obtainAccessToken;
      authServiceFactory.externalAuthData = _externalAuthData;
      authServiceFactory.registerExternal = _registerExternal;

      return authServiceFactory;
  }]);

})();