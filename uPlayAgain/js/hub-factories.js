app.factory('Messages', ['$rootScope', 'Hub', function ($rootScope, Hub) {
    var Messages = this;

    ////Employee ViewModel
    //var Employee = function (employee) {
    //    if (!employee) employee = {};

    //    var Employee = {
    //        Id: employee.Id || null,
    //        Name: employee.Name || 'New',
    //        Email: employee.Email || 'New',
    //        Salary: employee.Salary || 1,
    //        Edit: false,
    //        Locked: employee.Locked || false,
    //        displayMode: function () {
    //            if (this.Locked) return 'lock-template';
    //            return this.Edit ? 'edit-template' : 'read-template';
    //        }
    //    };

    //    return Employee;
    //}

    //Hub setup
    var hub = new Hub('MessageHub', {
        listeners: {
            'newConnection': function (id) {
                Messages.connected.push(id);
                $rootScope.$apply();
            },
            'removeConnection': function (id) {
                Messages.connected.splice(Messages.connected.indexOf(id), 1);
                $rootScope.$apply();
            },
            //'lockEmployee': function (id) {
            //    var employee = find(id);
            //    employee.Locked = true;
            //    $rootScope.$apply();
            //},
            //'unlockEmployee': function (id) {
            //    var employee = find(id);
            //    employee.Locked = false;
            //    $rootScope.$apply();
            //},
            //'updatedEmployee': function (id, key, value) {
            //    var employee = find(id);
            //    employee[key] = value;
            //    $rootScope.$apply();
            //},
            //'addEmployee': function (employee) {
            //    Messages.all.push(new Employee(employee));
            //    $rootScope.$apply();
            //},
            //'removeEmployee': function (id) {
            //    var employee = find(id);
            //    Messages.all.splice(Messages.all.indexOf(employee), 1);
            //    $rootScope.$apply();
            //},
            'sendMessageHub': function (id, message) {
                var user = find(id);
                console.log(message);
            },
        },
        methods: ['login'],
        errorHandler: function (error) {
            console.error(error);
        }
    });

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

    //Methods
    Messages.pushMessage = function () {
        
        hub.promise.done(function () {
            // connection established
            // call your hub method here
            hub.login('ifisdiasd');
        });
        //hub.lock(1);        
    }

    //Messages.edit = function (employee) {
    //    employee.Edit = true;
    //    hub.lock(employee.Id);
    //}
    //Messages.delete = function (employee) {
    //    webApi.remove({ id: employee.Id });
    //}
    //Messages.patch = function (employee, key) {
    //    var payload = {};
    //    payload[key] = employee[key];
    //    webApi.patch({ id: employee.Id }, payload);
    //}
    //Messages.done = function (employee) {
    //    employee.Edit = false;
    //    hub.unlock(employee.Id);
    //}

    ////Load
    //Messages.all = webApi.query(function (data) {
    //    var Messages = [];
    //    angular.forEach(data.value, function (employee) {
    //        Messages.push(new Employee(employee));
    //    });
    //    Messages.all = Messages;
    //    Messages.loading = false;
    //});
    return Messages;
}]);