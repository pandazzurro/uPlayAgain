app.directive('mailbox', ['factories', 'user-service', 'games-service', '$location', function (gxcFct, userSrv, gameSrv, $location) {
    return {
        restrict: 'E',
        templateUrl: 'templates/mail-mailbox.html',
        controller: function ($routeParams, $location) {
            var _this = this;

            _this.params = $routeParams;
            _this.messages = [];
            _this.transactions = [];
            _this.mustLoadMessageCounter = true;
            _this.messagesCount = { in: 0, out: 0, trn: 0 };
            _this.currentPage = 1;
            _this.showTransactions = false;
            _this.currentUserId = userSrv.getCurrentUser().id;

            this.getMessages = function (direction, page) {
                var queryParameters = {
                    userId: _this.currentUserId,
                    page: page
                };
                _this.showTransactions = (direction === 'transaction');
                _this.messagesCount.trn = 0;

                _this.messages = [];
                _this.transactions = [];

                if (direction === 'transaction') {
                    gxcFct.mail.transactions(queryParameters).$promise
                    .then(function (trnSuccess) {
                        _this.transactions = trnSuccess;
                        _this.messagesCount.trn = _this.transactions.length;
                        angular.forEach(_this.transactions, function (tran) {
                            angular.forEach(tran.myItems, function (item) {
                                item.game = {};
                                item.game.gameId = item.gameId;
                                gameSrv.fillGameData(item.game);
                            });

                            angular.forEach(tran.theirItems, function (item) {
                                item.game = {};
                                item.game.gameId = item.gameId;
                                gameSrv.fillGameData(item.game);
                            });

                        });
                    });
                }
                else if (direction === 'in') {
                    gxcFct.mail.incoming(queryParameters).$promise
                    .then(function (mailSuccess) {
                        _this.messages = mailSuccess;
                        if (_this.messages.length > 0) {
                            _this.messagesCount.in = mailSuccess.length;
                        }
                    });
                }
                else { //direction === out
                    gxcFct.mail.outgoing(queryParameters).$promise
                   .then(function (mailSuccess) {
                       _this.messages = mailSuccess;
                       _this.messagesCount.in = mailSuccess.length;
                   });
                }

                _this.currentPage = page;
            };

            this.hoverIn = function (mail) {
                mail.hovered = true;
            }

            this.hoverOut = function (mail) {
                mail.hovered = false;
            }

            this.open = function (mail) {
                $location.path('/mail/message/' + mail.messageId);
            }

            this.changeMyTranStatus = function (tran, newState) {
                if (tran.myStatus != newState) {

                    if (tran.userOwnerId == _this.currentUserId)
                        tran.proposal.userProponent_ProposalStatus = newState;
                    else
                        tran.proposal.userReceiving_ProposalStatus = newState;

                    gxcFct.proposal.update({ propId: tran.proposal.proposalId }, tran.proposal,
                    function (success) {
                        UIkit.notify('Proposta aggiornata', { status: 'success', timeout: 5000 });

                        for (var i = _this.transactions.length - 1; i >= 0; i--) {
                            // Aggiornamento stato proposta
                            if (_this.transactions[i].proposal.proposalId === tran.proposal.proposalId) {
                                _this.transactions[i].myStatus = newState;

                                if (tran.userOwnerId == _this.currentUserId)
                                    _this.transactions[i].proposal.userProponent_ProposalStatus = newState;
                                else
                                    _this.transactions[i].proposal.userReceiving_ProposalStatus = newState;

                                // Se annullo o rifiuto la proposta, o se entrambi hanno accettato, rimuovo tutto dalla lista di visualizzazione
                                if (newState == 2 || newState == 3) {
                                    _this.transactions.splice(i, 1);
                                    // rifiuto
                                    if (newState == 2) {
                                        userSrv.getInfoUser(_this.currentUserId)
                                        .then(function (userToChangeInfo) {
                                            var queryParams = {
                                                messageText: "Ciao, Lo scambio da te proposto con l'utente " + userToChangeInfo.username + " è stato rifiutato.",
                                                messageObject: "Scambio rifiutato",
                                                messageDate: new Date().toISOString(),
                                                userProponent_Id: _this.currentUserId,
                                                userReceiving_Id: tran.userId
                                            };

                                            gxcFct.mail.send(queryParams).$promise
                                            .then(function (success) {
                                                UIkit.notify('Scambio rifiutato. Messaggio inviato a: ' + userToChangeInfo.username, { status: 'success', timeout: 5000 });
                                            });
                                        });
                                    }
                                    // annullo : non faccio nulla?
                                }
                                else if (_this.transactions[i].proposal.userProponent_ProposalStatus == 1 && _this.transactions[i].proposal.userReceiving_ProposalStatus == 1) {
                                    _this.transactions.splice(i, 1);
                                    var queryParams = {
                                        messageText: "Ciao, abbiamo il piacere di informarti che lo scambio è avvenuto con successo. Ora puoi accordarti sulle modalità di trasferimento del gioco. Rispondi al messaggio, accordatevi sulla modalità di scambio. Grazie",
                                        messageObject: "Scambio concluso con successo",
                                        messageDate: new Date().toISOString(),
                                        userProponent_Id: _this.currentUserId,
                                        userReceiving_Id: tran.userId
                                    };

                                    gxcFct.mail.send(queryParams).$promise
                                    .then(function (success) {
                                        userSrv.getInfoUser(tran.userId)
                                        .then(function (userToChangeInfo) {
                                            UIkit.notify('Scambio concluso. Messaggio inviato a: ' + userToChangeInfo.username, { status: 'success', timeout: 5000 });
                                        });

                                        var myLibrary = userSrv.getCurrentUser().LibraryId;
                                        var theirLibrary = tran.theirLibraryId;

                                        angular.forEach(tran.myItems, function (item) {
                                            item.IsExchangeable = false;
                                            item.libraryId = theirLibrary;
                                            item.isDeleted = false;

                                            gxcFct.library.update({ componentId: item.libraryComponentId }, item, function () {

                                            });
                                        });

                                        angular.forEach(tran.theirItems, function (item) {
                                            item.IsExchangeable = false;
                                            item.libraryId = myLibrary;
                                            item.isDeleted = false;

                                            gxcFct.library.update({ componentId: item.libraryComponentId }, item, function () {

                                            });
                                        });
                                        $location.path('/mail/in/1');
                                    },
                                    function (error) {
                                        UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                                    });
                                }
                            }
                        }

                        _this.transactions.forEach(function (trn) {
                            if (trn.proposal.proposalId === tran.proposal.proposalId) {
                                trn.myStatus = newState;
                            }
                            if (tran.userOwnerId == _this.currentUserId)
                                trn.proposal.userProponent_ProposalStatus = newState;
                            else
                                trn.proposal.userReceiving_ProposalStatus = newState;
                        });
                    }, function (reason) {
                        UIkit.notify('Errore nella modifica dello stato della proposta', { status: 'success', timeout: 5000 });
                    });
                };
            }

            this.canRaise = function (tran) {
                // Can Raise
                if ((tran.userOwnerId != _this.currentUserId && tran.direction) || (tran.userOwnerId == _this.currentUserId && !tran.direction))
                    return true;
                // Can Cancel
                return false;
            }

            this.myStatusVisibilityButton = function (tran, status) {
                switch (status) {
                    //da Approvare
                    case 0:
                        return false;
                        //Accetta
                    case 1:
                        return (tran.myStatus == 0);
                        //Rifiuta
                    case 2:
                        return this.canRaise(tran) && (tran.myStatus == 0);
                        //Annulla
                    case 3:
                        return !this.canRaise(tran) && (tran.myStatus == 1);
                        // Rilancio (posso rilanciare solo se non sono l'utente proprietario!
                    case 4:
                        return this.canRaise(tran) && (tran.myStatus == 1 || tran.myStatus == 0);
                }
            }

            this.raiseOffer = function (tran) {
                var newLocation = '/mail/compose/' + tran.userId + '/' + tran.proposal.proposalId;
                $location.path(newLocation);
            }

            this.loadCounter = function () {
                userSrv.updateUserData()
                .then(function () {
                    var counterSuccess = userSrv.getCounterMessages(true);
                    _this.messagesCount.in = counterSuccess.In;
                    _this.messagesCount.out = counterSuccess.Out;
                    _this.messagesCount.trn = counterSuccess.Trn;
                });
            }

            this.isAlreadyReadMail = function (message) {
                if ((message.userReceiving_Id == _this.currentUserId && message.isAlreadyReadReceiving) ||
                    (message.userProponent_Id == _this.currentUserId && message.isAlreadyReadProponent))
                    return true;
                return false;
            }

            this.massiveCheckMail = function () {
                angular.forEach(_this.messages, function (message, index) {
                    message.check = true;                    
                });
            }

            this.massiveUnCheckMail = function () {
                angular.forEach(_this.messages, function (message, index) {
                    message.check = false;
                });
            }

            this.massiveUnReadMail = function () {
                var lastIndexToUnRead = -1;
                angular.forEach(_this.messages, function (message, index) {
                    if (message.check)
                        lastIndexToUnRead = index;
                });
                angular.forEach(_this.messages, function (message, index) {
                    if (message.check) {
                        if (message.userReceiving_Id == _this.currentUserId)
                            message.isAlreadyReadReceiving = false;

                        if (message.userProponent_Id == _this.currentUserId)
                            message.isAlreadyReadProponent = false;

                        gxcFct.mail.update({ messageId: message.messageId }, message, function () {
                            if (index == lastIndexToUnRead) {
                                _this.getMessages(_this.params.direction, _this.params.page);
                                _this.loadCounter();
                            }
                        }, function () { });
                    }
                });
                _this.getMessages(_this.params.direction, _this.params.page);
                _this.loadCounter();
                UIkit.notify('Aggiornati i messaggi NON letti', { status: 'success', timeout: 5000 });
            }

            this.massiveReadMail = function () {
                var lastIndexToRead = -1;
                angular.forEach(_this.messages, function (message, index) {
                    if (message.check)
                        lastIndexToRead = index;
                });

                angular.forEach(_this.messages, function (message, index) {
                    if (message.check) {
                        if (message.userReceiving_Id == _this.currentUserId)
                            message.isAlreadyReadReceiving = true;

                        if (message.userProponent_Id == _this.currentUserId)
                            message.isAlreadyReadProponent = true;

                        gxcFct.mail.update({ messageId: message.messageId }, message, function () {
                            if (index == lastIndexToRead) {
                                _this.getMessages(_this.params.direction, _this.params.page);
                                _this.loadCounter();
                            }
                        }, function () { });
                    }
                });
                _this.getMessages(_this.params.direction, _this.params.page);
                _this.loadCounter();
                UIkit.notify('Aggiornati i messaggi letti', { status: 'success', timeout: 5000 });
            }

            this.massiveDeleteMail = function () {
                UIkit.modal.confirm("Sei sicuro di rimuovere i messaggi selezionati?", function () {
                    var lastIndexToDelete = -1;
                    angular.forEach(_this.messages, function (message, index) {
                        if (message.check)
                            lastIndexToDelete = index;
                    });
                    angular.forEach(_this.messages, function (message, index) {
                        if (message.check) {
                            if (message.userReceiving_Id == _this.currentUserId) {
                                message.isAlreadyDeleteReceiving = true;
                                message.isAlreadyReadReceiving = true;
                            }   

                            if (message.userProponent_Id == _this.currentUserId) {
                                message.isAlreadyDeleteProponent = true;
                                message.isAlreadyReadProponent = true;
                            }   

                            gxcFct.mail.update({ messageId: message.messageId }, message, function ()
                            {
                                if (index == lastIndexToDelete) {
                                    _this.getMessages(_this.params.direction, _this.params.page);
                                    _this.loadCounter();
                                }
                            }, function () { });
                        }
                    });
                    UIkit.notify('Aggiornati i messaggi rimossi', { status: 'success', timeout: 5000 });
                });
            }

            this.initPage = function () {                
                _this.getMessages(_this.params.direction, _this.params.page);
                _this.loadCounter();
            }

            //init
            _this.initPage();
        },
        controllerAs: 'mailbox'
    }
}]);

app.directive('message', ['factories', 'user-service', '$location', function (gxcFct, userSrv, $location) {
    return {
        restrict: 'E',
        templateUrl: 'templates/mail-message.html',
        controller: function ($routeParams) {
            var _this = this;
            _this.message = {};
            _this.transactions = {};
            _this.msgId = $routeParams.messageId;
            _this.segnalazione = "";

            this.backToMailbox = function () {
                $location.path('/mail/in/1');
            };

            this.reply = function (message) {
                var newLocation = '/mail/compose/' + message.sender.id;
                $location.path(newLocation);
            }

            this.alertAdmin = function (message) {
                var messageToSend = "Mittente: " + message.sender.id + ' - ' + message.sender.username + '\n' +
                    "Ricevente: " + message.receiver.id + ' - ' + message.receiver.username + '\n' +
                    message.messageObject + '\n' + message.messageText + '\n\n\n' +
                    "Ulteriori note: " + _this.segnalazione;
                gxcFct.alertAdmin.send({ message: messageToSend }).$promise
                .then(function (success) {
                    UIkit.notify('L\' amministratore del sito è stato avvertito. Prenderà provvedimenti al più presto!', { status: 'success', timeout: 5000 });
                })
            }

            gxcFct.mail.get({ messageId: _this.msgId }).$promise
              .then(function (success) {
                  _this.message = success;
                  _this.message.sender = gxcFct.user.byId({ userId: success.userProponent_Id });
                  gxcFct.user.byId({ userId: success.userReceiving_Id }).$promise
                  .then(function (receiverSuccess) {
                      _this.message.receiver = receiverSuccess;
                      _this.message.isIncoming = receiverSuccess.id == userSrv.getCurrentUser().id;

                      _this.message.isAlreadyReadReceiving = _this.message.isIncoming || _this.message.isAlreadyReadReceiving;
                      _this.message.isAlreadyReadProponent = !_this.message.isIncoming || _this.message.isAlreadyReadProponent;
                      gxcFct.mail.update({ messageId: _this.msgId }, _this.message, function () { }, function () { });
                  });
              });
        },
        controllerAs: 'mail'
    };
}]);

app.directive('messageNew', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
    return {
        restrict: 'E',
        scope: {
            exchange: '@exchange'
        },
        templateUrl: 'templates/mail-message-new.html',
        controller: function ($scope, $routeParams, $location) {
            var _this = this;
            _this.message = { myItems: [], hisItems: [] };
            _this.hisUserId = undefined;
            _this.isRaiseOffer = undefined;
            _this.recipientId = $routeParams.recipientId;
            _this.proposalId = $routeParams.proposalId;
            _this.libraryComponentId = $routeParams.libraryComponentId;
            _this.userProponent_ProposalStatus = 1; // L'utente che crea la proposta la accetta automaticamente.
            _this.userReceiving_ProposalStatus = 0; // da approvare
            _this.directionProposal = true;
            if (_this.proposalId != undefined) {
                _this.isRaiseOffer = true;
                _this.currentTransaction = {}; // setto la transazione, poi la recupero
            }

            var start = new Date();
            var end = new Date();
            end.setDate(end.getDate() + 30);

            var addProposal = function () {
                var queryParams = {
                    dateStart: start.toISOString(),
                    dateEnd: end.toISOString(),
                    proposalText: _this.message.text,
                    proposalObject: _this.message.titolo,
                    proposalDate: new Date().toISOString(),
                    direction: _this.directionProposal,
                    transactionId: _this.currentTransaction.transactionId,
                    userLastChanges_Id: userSrv.getCurrentUser().id,
                    userProponent_ProposalStatus: _this.userProponent_ProposalStatus,
                    userReceiving_ProposalStatus: _this.userReceiving_ProposalStatus,
                    proposalComponents: []
                };

                for (i in _this.message.myItems) {
                    queryParams.proposalComponents.push({
                        libraryComponentId: _this.message.myItems[i].libraryComponents.libraryComponentId,
                        userOwnerId: userSrv.getCurrentUser().id
                    });
                }
                for (i in _this.message.hisItems) {
                    queryParams.proposalComponents.push({
                        libraryComponentId: _this.message.hisItems[i].libraryComponents.libraryComponentId,
                        userOwnerId: _this.hisUserId
                    });
                }

                gxcFct.proposal.add(queryParams).$promise
                .then(function (success) {
                    UIkit.notify('Nuovo scambio creato', { status: 'success', timeout: 5000 });
                    // mando l'utente in pagina di ricerca
                    $location.path('/exchange');
                }, function (reason) {
                    UIkit.notify('Errore in creazione scambio.', { status: 'success', timeout: 5000 });
                });
            }
            
            this.send = function () {
                if (_this.exchange) {
                    if (_this.currentTransaction === undefined) {
                        var queryParams = {
                            userProponent_Id: userSrv.getCurrentUser().id,
                            userReceiving_Id: _this.hisUserId
                        }

                        gxcFct.transaction.add(queryParams).$promise
                        .then(function (success) {
                            _this.currentTransaction = success;
                            addProposal();
                        });
                    }
                    else {
                        if (_this.isRaiseOffer) {
                            gxcFct.proposal.get({ propId: _this.proposalId }).$promise
                            .then(function (proposal) {
                                if (userSrv.getCurrentUser().id == proposal.transaction.userProponent_Id)
                                    proposal.userProponent_ProposalStatus = 3; // annullata
                                else
                                    proposal.userReceiving_ProposalStatus = 3; // annullata

                                gxcFct.proposal.update({ propId: _this.proposalId }, proposal,
                                    function (success) {
                                        _this.currentTransaction.transactionId = success.transactionId;
                                        _this.directionProposal = !success.direction;
                                        if (!_this.directionProposal) {
                                            _this.userProponent_ProposalStatus = 0; // da approvare
                                            _this.userReceiving_ProposalStatus = 1; // L'utente che crea la proposta la accetta automaticamente.
                                        }
                                        addProposal();
                                    });
                            });
                        }
                        else
                            addProposal();
                    }
                }
                else {
                    var queryParams = {
                        messageText: _this.message.text,
                        messageObject: _this.message.titolo,
                        messageDate: new Date().toISOString(),
                        userProponent_Id: userSrv.getCurrentUser().id,
                        userReceiving_Id: _this.hisUserId
                    };

                    gxcFct.mail.send(queryParams).$promise
                    .then(function (success) {
                        UIkit.notify('Messaggio inviato', { status: 'success', timeout: 5000 });
                        $location.path('/mail/in/1');
                    },
                    function (error) {
                        UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                    });
                }
            };

            this.addItem = function (item, isMine) {
                if (isMine)
                    _this.message.myItems.push(item);
                else
                    _this.message.hisItems.push(item);
            };

            this.isItemAssigned = function (item, isMine) {
                var result = false;

                if (isMine)
                    result = _this.message.myItems.indexOf(item) >= 0;
                else
                    result = _this.message.hisItems.indexOf(item) >= 0;

                return result;
            };

            this.removeItem = function (item, isMine) {
                if (isMine) {
                    var i = _this.message.myItems.indexOf(item);
                    _this.message.myItems.splice(i, 1);
                }
                else {
                    var i = _this.message.hisItems.indexOf(item);
                    _this.message.hisItems.splice(i, 1);
                }
            };

            this.disableSend = function () {
                if (_this.exchange && _this.message.myItems.length > 0 && _this.message.hisItems.length > 0)
                    return false;
                if (!_this.exchange)
                    return false;
                return true;
            }

            // My Library Components
            gxcFct.game.byUserWithComponent({ userId: userSrv.getCurrentUser().id }).$promise
            .then(function (gamesIds) {
                var game = [];
                angular.forEach(gamesIds, function (element) {
                    game.push(element);
                });
                _this.myLibrary = game;
            });

            // His Library Components
            gxcFct.game.byUserWithComponent({ userId: _this.recipientId }).$promise
            .then(function (gamesIds) {
                var game = [];
                angular.forEach(gamesIds, function (element) {
                    game.push(element);
                });
                _this.hisLibrary = game;
                _this.hisUserId = _this.recipientId;

                if (_this.libraryComponentId != undefined) {
                    _this.exchange = true;
                    angular.forEach(gamesIds, function (item) {
                        if (item.libraryComponents.libraryComponentId == _this.libraryComponentId) {
                            _this.message.hisItems.push(item);
                            _this.message.titolo = "Proposta per: " + item.games.title;
                        }   
                    });
                }
            });


        },
        controllerAs: 'mail'
    };
}]);

app.directive('feedbackVote', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
    return {
        restrict: 'E',
        templateUrl: 'templates/feedback-vote.html',
        controller: function ($routeParams) {
            var _this = this;
            _this.currentUserId = userSrv.getCurrentUser().id;
            _this.pendingTransactions = [];
            _this.currentPage = 1;

            _this.GetPendingTransactionFeedback = function () {

                var queryParameters = {
                    userId: _this.currentUserId//,
                    //page: _this.currentPage
                };

                gxcFct.feedback.pending(queryParameters).$promise
                    .then(function (trnSuccess) {
                        _this.pendingTransactions = trnSuccess;

                        _this.pendingTransactions.forEach(function (tran) {
                            tran.myItems.forEach(function (item) {
                                item.game = {};
                                item.game.gameId = item.gameId;
                                gameSrv.fillGameData(item.game);
                            });

                            tran.theirItems.forEach(function (item) {
                                item.game = {};
                                item.game.gameId = item.gameId;
                                gameSrv.fillGameData(item.game);
                            })
                        });

                    });
            }

            // Carico le transazioni da assegnare con un feedback all'avvio
            _this.GetPendingTransactionFeedback();

            _this.AddFeedback = function (transactionId, vote) {
                // In caso di transazione positiva assegnare un +1, in caso di transazione negativa un -3
                var tranFilter = undefined;
                for (i = _this.pendingTransactions.length - 1; i >= 0; i--) {
                    if (_this.pendingTransactions[i].transactionId === transactionId) {
                        var queryParameters = {
                            transactionId: transactionId,
                            userId: _this.pendingTransactions[i].userId, //userToFeedback
                            rate: vote
                        };

                        gxcFct.feedback.add(queryParameters).$promise
                        .then(function (success) {
                            // Rimuovo la transazione che ha avuto il feedback
                            _this.pendingTransactions.splice(i, 1);

                            UIkit.notify('Hai votato con successo. Grazie!', { status: 'success', timeout: 5000 });
                        },
                        function (error) {
                            UIkit.notify('Errore creazione Feedback', { status: 'success', timeout: 5000 });
                        });
                        break;
                    }
                }
            }

            _this.hasFeedback = function () {
                if (_this.pendingTransactions.length > 0)
                    return true;
                return false;
            }
        },
        controllerAs: 'feedbackVote'
    };
}]);
