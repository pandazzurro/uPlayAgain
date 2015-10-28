(function () {
    var app = angular.module('gxc.factories', ['ngResource']);

    app.webapi = "";

    /**
     * Mail factory
     **/
    //var mailService = angular.module('gxcServices', [ 'ngResource' ]);
    app.factory('factories', ['$resource', function ($resource) {
        return {
            mail: $resource(app.webapi, {}, {
                byUser: { url: app.webapi + '/api/messages/byUser/:userId', method: 'GET', isArray: false },
                get: { url: app.webapi + '/api/messages/:messageId', method: 'GET', isArray: false },
                update: { url: app.webapi + '/api/messages/:messageId', method: 'PUT', isArray: false },
                send: { url: app.webapi + '/api/messages', method: 'POST', isArray: false },
                incoming: { url: app.webapi + 'api/messages/byUser/:userId/incoming/:page', method: 'GET', isArray: true, params: { userId: '@userId', page: '@page' } },
                outgoing: { url: app.webapi + 'api/messages/byUser/:userId/outgoing/:page', method: 'GET', isArray: true, params: { userId: '@userId', page: '@page' } },
                transactions: { url: app.webapi + '/api/messages/byUser/:userId/transactions/:page', method: 'GET', isArray: true, params: { userId: '@userId', page: '@page' } }
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
                get: { url: app.webapi + '/api/games/:gameId', method: 'GET', isArray: false },
                byUser: { url: app.webapi + '/api/games/byUser/:userId', method: 'GET', isArray: true, params: { userId: '@userId' } },                
                search: { url: app.webapi + '/api/search', method: 'GET', isArray: false },
                byUserWithComponent: { url: app.webapi + '/api/GameExchangeable/ByUser/:userId', method: 'GET', isArray: true, params: { userId: '@userId' } }
            }),
            library: $resource(app.webapi, {}, {
                byUser: { url: app.webapi + '/api/libraries/byUser/:userId', method: 'GET', isArray: true, params: { userId: '@userId' } },
                get: { url: app.webapi + '/api/libraries/:libraryId', method: 'GET', isArray: false },
                add: { url: app.webapi + '/api/librarycomponents', method: 'POST', isArray: false },
                remove: { url: app.webapi + '/api/librarycomponents/:componentId', method: 'DELETE', isArray: false },
                update: { url: app.webapi + '/api/librarycomponents/:componentId', method: 'PUT', isArray: false },
            }),            
            user: $resource(app.webapi, {}, {
                get: { url: app.webapi + '/api/users/:userId', method: 'GET', isArray: false },
                checkUsername: { url: app.webapi + '/api/users/Exists/:username', method: 'GET', isArray: false },
                byId: { url: app.webapi + '/api/users/identity/:userId', method: 'GET', isArray: false },
                profile: { url: app.webapi + '/api/users/profile/:userId', method: 'GET', isArray: false },
                login: { url: app.webapi + '/api/account/login', method: 'POST', isArray: false },
                logout: { url: app.webapi + '/api/account/logout', method: 'POST', isArray: false },
                register: { url: app.webapi + '/api/account/register', method: 'POST', isArray: true },
                update: { url: app.webapi + '/api/users/:userId', method: 'PUT', isArray: false },
                remove: { url: app.webapi + '/api/users/:userId', method: 'DELETE', isArray: false }
            }),
            transaction: $resource(app.webapi, {}, {                
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
            counter: $resource(app.webapi, {}, {
                byUser: { url: app.webapi + 'api/counter/byUser/:userId', method: 'GET', isArray: false }
            }),

        }
    }]);
})();