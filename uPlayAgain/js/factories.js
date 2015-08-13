(function() {
  var app = angular.module('gxc.factories', ['ngResource']);

  app.webapi = "/uPlayAgain";
  
  /**
   * Mail factory
   **/
  //var mailService = angular.module('gxcServices', [ 'ngResource' ]);
  app.factory('factories', ['$resource', function($resource) {
    return {
      mail: $resource(app.webapi, {}, {
        incoming: { url: app.webapi + '/api/messages/byUser/:userId', method: 'GET', isArray: true },
        outgoing: { url: app.webapi + '/api/messages/outgoing/:userId', method: 'GET', isArray: true },
        get: { url: app.webapi + '/api/messages', method: 'GET', data: {} }
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
        query: { url: app.webapi + '/api/games/', method: 'GET', isArray: true },
        get : { url: app.webapi + '/api/games/:gameId', method: 'GET', isArray: false },
        search: { url: app.webapi + '/api/search', method: 'GET', isArray: false }
      }),
      library: $resource(app.webapi, {} , {
        byUser: { url: app.webapi + '/api/libraries/byUser/:userId', method: 'GET', isArray: false },
        get: { url: app.webapi + '/api/libraries/:libraryId', method: 'GET', isArray: true },
        add: { url: app.webapi + '/api/librarycomponents', method: 'POST', isArray: false },
        remove: { url: app.webapi + '/api/librarycomponents/:componentId', method: 'DELETE', isArray: false }
      }),
      user: $resource(app.webapi, {}, {
        get : { url: app.webapi + '/api/users/:userId', method: 'GET', isArray: false },
        login: { url: app.webapi + '/api/auth', method: 'POST', isArray: false },
        register: { url: app.webapi + '/api/account/register', method: 'POST', isArray: true }
      })
  }}]);
})();