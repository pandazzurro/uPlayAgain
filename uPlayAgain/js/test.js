(function() {
  var srv = angular.module('test.service', ['ngResource']);

  srv.service('testSrv', [ '$resource', function($resource) {
    
    this.testGet = function(api, data) {
      var res = $resource(api, {}, {
        test: { method: 'GET', isArray: true }
      });
      
      return res.test(data).$promise;
    };
    this.testPost = function(api, data) {
      var res = $resource(api, {}, {
        test: { method: 'POST', isArray: true }
      });
      
      return res.test(data).$promise;
    };
    this.testPut = function(api, data) {
      var res = $resource(api, {}, {
        test: { method: 'PUT', isArray: true }
      });
      
      return res.test(data).$promise;
    };
    this.testDelete = function(api, data) {
      var res = $resource(api, {}, {
        test: { method: 'DELETE', isArray: true }
      });
      
      return res.test(data).$promise;
    };
  }]);
  
var dir = angular.module('test.directive', []);
dir.directive('testDir', [ 'testSrv', function(testSrv) {
  return {
    restrict: 'E',
    templateUrl: 'templates/test.html',
    controller: function() {
      var _this = this;

      _this.api = undefined;
      _this.data = undefined;
      _this.result = undefined;
      _this.success = true;

      this.get = function() {
        testSrv.testGet(_this.api, JSON.parse(_this.data))
          .then(function(success) {
            _this.success = true;
            _this.result = success;
          },
          function(error) {
            _this.success = false;
            _this.result = error;
          });
        };

      this.put = function() {
        testSrv.testPut(_this.api, JSON.parse(_this.data))
          .then(function(success) {
            _this.success = true;
            _this.result = success;
          },
          function(error) {
            _this.success = false;
            _this.result = error;
          });
        };

      this.post = function() {
        testSrv.testPost(_this.api, JSON.parse(_this.data))
          .then(function(success) {
            _this.success = true;
            _this.result = success;
          },
          function(error) {
            _this.success = false;
            _this.result = error;
          });

        };

      this.delete = function() {
        testSrv.testDelete(_this.api, JSON.parse(_this.data))
          .then(function(success) {
            _this.success = true;
            _this.result = success;
          },
          function(error) {
            _this.success = false;
            _this.result = error;
          });

      };
    },
    controllerAs: 'testCtrl'
  };
}]);
  
var app = angular.module('test.app', ['test.service', 'test.directive', 'ngResource']);
})();