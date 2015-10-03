(function () {
    var app = angular.module('gxc.directives.misc', []);

    app.directive('regolamento', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/regolamento.html'            
        };
    }]);
    app.directive('come-funziona', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/come-funziona-doc.html'
        };
    }]);
    app.directive('feedback', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/feedback-doc.html'
        };
    }]);
    app.directive('spedizioni', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/spedizioni-doc.html'
        };
    }]);
    app.directive('contattaci', ['factories', function (gxcFct) {
        return {
            restrict: 'E',
            templateUrl: 'templates/contattaci.html'
        };
    }]);
})();
