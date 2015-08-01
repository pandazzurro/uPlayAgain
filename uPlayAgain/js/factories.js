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
        incoming: { url: '/api/messages/incoming/:userId', method: 'GET', isArray: false },
        outgoing: { url: '/api/messages/outgoing/:userId', method: 'GET', isArray: false },
        get: { url: '/api/messages', method: 'GET', data: {} }
      }),
      genre: $resource('/api/genres', {}, {
        query: { method: 'GET', isArray: false }
      }),
      platform: $resource('/api/platforms', {}, {
        query: { method: 'GET', isArray: false }
      }),
      game: $resource('/api/games', {}, {
        query: { method: 'GET', isArray: false },
        get : { url: '/api/games/:gameId', method: 'GET', isArray: false },
        search: { url: '/api/search', method: 'GET', isArray: false }
      }),
      library: $resource(app.webapi, {} , {
        byUser: { url: '/api/libraries/byUser/:userId', method: 'GET', isArray: false },
        get: { url: '/api/libraries/:libraryId', method: 'GET', isArray: false },
        add: { url: '/api/librarycomponents', method: 'POST', isArray: false },
        remove: { url: '/api/librarycomponents/:componentId', method: 'DELETE', isArray: false }
      }),
      user: $resource(app.webapi, {}, {
        login: { url: '/api/auth', method: 'POST', isArray: false },
        register: { url: '/api/account/register', method: 'POST', isArray: true }
      })
  }}]);
})();