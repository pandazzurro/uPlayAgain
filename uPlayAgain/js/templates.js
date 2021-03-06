//(function() {
//  var app = angular.module('gxc.templates', []);

  app.directive('navbarTop', function() {
    return {
      restrict: 'E',
      templateUrl: 'templates/navbar-top.html'
    };
  });
  
  app.directive('navbarSide', function() {
    return {
      restrict: 'E',
      templateUrl: 'templates/navbar-side.html'
    };
  });

  app.directive('navbarFooter', function () {
      return {
          restrict: 'E',
          templateUrl: 'templates/navbar-footer.html'
      };
  });

  app.directive('navbarAds', function () {
      return {
          restrict: 'E',
          templateUrl: 'templates/navbar-ads.html'
      };
  });
  
  app.directive('formLogin', function() {
    return {
      restrict: 'E',
      templateUrl: 'templates/form-login.html'
    };
  });

  app.directive('formLoginSide', function() {
    return {
      restrict: 'E',
      templateUrl: 'templates/form-login-side.html'
    };
  });

//})();
