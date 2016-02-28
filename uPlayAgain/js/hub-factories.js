app.factory('Messages', ['$rootScope', 'Hub', function ($rootScope, Hub) {
    var Messages = this;

    //Hub setup
    Messages.createHub = function (userID) {
        var hub = new Hub('MessageHub', {
            listeners: {
                'newConnection': function (id) {
                    Messages.connected.push(id.ConnectionID);
                    $rootScope.$apply();
                    console.log("New Login: " + id.UserId);
                },
                'removeConnection': function (id) {
                    Messages.connected.splice(Messages.connected.indexOf(id), 1);
                    console.log("Disconnected: " + id);
                    $rootScope.$apply();
                },
                'sendMessageHub': function (message, id) {
                    var user = find(id);
                    console.log(message);
                    $rootScope.$apply();
                },
                'sendProposalHub': function (proposal, id) {
                    var user = find(id);
                    console.log(proposal);
                    $rootScope.$apply();
                },
                'sendFeedbackHub': function (feedback, tran, id) {
                    var user = find(id);
                    console.log(feedback);
                    console.log(tran);
                    $rootScope.$apply();
                }
            },
            queryParams : {
                'userId': userID
            },
            methods: [],
            errorHandler: function (error) {
                console.error(error);
            }
        });
    }

    //Helpers
    var find = function (id) {
        for (var i = 0; i < Messages.all.length; i++) {
            if (Messages.all[i].Id == id) return Messages.all[i];
        }
        return null;
    };

    //Variables
    Messages.all = [];
    Messages.connected = [];
    Messages.loading = true;

    return Messages;
}]);