(function() {
  var COOKIE_ID = '__UPA__LOGIN__COOKIE__';
  var app = angular.module('gxc', ['gxc.factories', 'gxc.services',
      'gxc.directives.games', 'gxc.directives.mail', 'gxc.directives.misc', 'gxc.directives.user',
      'gxc.templates', 'ngRoute', 'ngResource', 'ngCookies', 'ngImgCrop',
      'uiGmapgoogle-maps']);

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
      .when('/mail/compose/:recipientId', {
        template: '<message-new data-exchange=\'0\'></message-new>'
      })
      .when('/mail/exchange/:gameId', {
        template: '<message-new data-exchange=\'{{ gameId }}\'></message-new>'
      })
      .when('/mail/message/:messageId', {
        template: '<message></message>'
      })
      .when('/mail/:direction/:page', {
        template: '<mailbox></mailbox>'
      })
      .when('/library/add', {
        template: '<games-search></games-search>',
      })
      .when('/library', {
        template: '<games-library></games-library>'
      })
      .when('/exchange', {
        template: '<exchange-search></exchange-search>'
      })
      .when('/user/:userId', {
        template: '<user-profile></user-profile>'
      })
      .when('/myprofile', {
        template: '<user-profile></user-profile>'
      })
      .when('/register', {
        template: '<form-register></form-register>'
      })
      .when('/edit-register', {
        template: '<form-edit-register></form-edit-register>'
      })
      .when('/feedback-vote', {
        template: '<feedback-vote></feedback-vote>'
      })
      .when('/regolamento', {
        templateUrl: 'templates/regolamento.html'
      })
      .when('/come-funziona', {
        templateUrl: 'templates/come-funziona-doc.html'
      })
      .when('/feedback', {
        templateUrl: 'templates/feedback-doc.html'
      })
      .when('/spedizioni', {
        templateUrl: 'templates/spedizioni-doc.html'
      })
      .when('/contattaci', {
          templateUrl: 'templates/contattaci.html'
      })
      .when('/test-transaction', {
          template: '<test-transaction></test-transaction>'
      })
      .when('/', {
        templateUrl: 'templates/home.html'
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
          libraries: 'weather,geometry,visualization,places',
          language: 'it'
      });
  });

/* 
redirect su home se non si è loggati 
http://stackoverflow.com/questions/11541695/redirecting-to-a-certain-route-based-on-condition
*/
  app.run([ '$rootScope', '$location', 'user-service', function($rootScope, $location, userSrv) {
      // register listener to watch route changes
      $rootScope.$on("$routeChangeStart", function (event, next, current) {
          if (next.$$route.originalPath != "/register" &&
              next.$$route.originalPath != "/regolamento" &&
              next.$$route.originalPath != "/spedizioni" &&
              next.$$route.originalPath != "/feedback" &&
              next.$$route.originalPath != "/come-funziona" &&
              next.$$route.originalPath != "/contattaci" &&
              next.$$route.originalPath != "/test-transaction" &&
              !userSrv.isLoggedIn()) {
              $location.path("/");
          }

          $('html, body').animate({ scrollTop: 0 }, 'fast');
      });
  }]);

  app.controller('UserController', [ '$scope', '$cookies', 'user-service', function($scope, $cookies, userSrv) {
    $scope.username = undefined;
    $scope.password = undefined;
    
    var _this = this;
    
    this.doLogin = function(username, password) {
      var expiration = new Date();
      expiration.setDate(expiration.getDate() + 10);

      var loginData = { xu: username, xp: password, xd: expiration };
      $cookies.putObject(COOKIE_ID, loginData, { expires: expiration });
      
      userSrv.login(username, password);
    };

    this.loginWithCookie = function() {
      var loginData = $cookies.getObject(COOKIE_ID);
      
      if(loginData !== undefined && loginData.xd > new Date() && !_this.isLoggedIn())
      {
        var user = loginData.xu;
        var pwd = loginData.xp;
        
        _this.doLogin(user, pwd);
      }
    };
    
    this.login = function() {
      _this.doLogin($scope.username, $scope.password);
    };
    
    this.logout = function() {
      userSrv.logout($scope.username, $scope.password);
      
      $scope.username = '';
      $scope.password = '';
    };

    this.isLoggedIn = function() {
      return userSrv.isLoggedIn();
    };

    this.getCurrentUser = function () {
      return userSrv.getCurrentUser();
    };
    
    this.register = function() {
      window.location = '#/register';
    };
    
    _this.loginWithCookie();
  }]);
})();