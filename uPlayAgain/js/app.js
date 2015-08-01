(function() {
  var app = angular.module('gxc', ['gxc.factories', 'gxc.services', 'gxc.directives', 'gxc.templates', 'ngRoute', 'ngResource', 'ngImgCrop', 'uiGmapgoogle-maps']);

  // http://stackoverflow.com/questions/20506360/angular-repeat-span-n-times
  app.filter('range', function() {
  return function(val, range) {
    range = parseInt(range);
    
    for (var i = 0; i < range; i++)
      val.push(i + 1);
    
    return val;
  }});
    
  app.config(['$routeProvider',
    function($routeProvider) {
      $routeProvider
      .when('/mail', {
        redirectTo: '/mail/in/1'
      })
      .when('/mail/compose', {
        template: '<message-new></message-new>'
      })
      .when('/mail/message/:messageId', {
        template: '<message></message>'
      })
      .when('/mail/:direction/:page', {
        template: '<mailbox></mailbox>'
      })
/*    .when('/library', {
        redirectTo: '/library/1'
      })*/
      .when('/library/add', {
        template: '<games-search></games-search>',
      })
      .when('/library', {
        template: '<games-library></games-library>'
      })
      .when('/exchange', {
        template: '<exchange-search></exchange-search>'
      })
      .when('/register', {
        template: '<form-register></form-register>'
      })
      .otherwise({
        redirectTo: '/'
      });
    }]);

  /*maps*/
  app.config(function(uiGmapGoogleMapApiProvider) {
      uiGmapGoogleMapApiProvider.configure({
          //    key: 'your api key',
          v: '3.17',
          libraries: 'weather,geometry,visualization'
      });
  })
  
  app.controller('UserController', [ '$scope', 'user-service', function($scope, userSrv) {
    $scope.username = undefined;
    $scope.password = undefined;
    
    this.login = function() {
      userSrv.login($scope.username, $scope.password);
    };
    
    this.logout = function() {
      userSrv.logout();
    };

    this.isLoggedIn = function() {
      return userSrv.isLoggedIn();
    }

    this.getUser = function() {
      return userSrv.getUser();
    }
    
    this.register = function() {
      window.location = '#/register';
    }
  }]);
})();