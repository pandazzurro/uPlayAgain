(function () {
    var app = angular.module('gxc.directives.mail', []);

    app.directive('mailbox', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/mail-mailbox.html',
            controller: function ($routeParams, $location) {
                var _this = this;

                _this.params = $routeParams;
                _this.messages = [];
                _this.messagesIncoming = [];
                _this.messagesOutgoing = [];
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
                    //var incoming = (direction === 'in');
                    _this.showTransactions = (direction === 'transaction');
                    _this.messagesCount.trn = 0;

                    _this.messages = [];
                    _this.transactions = [];

                    if (direction === 'transaction') {
                        gxcFct.mail.transactions(queryParameters).$promise
                        .then(function (trnSuccess) {
                            _this.transactions = trnSuccess;
                            _this.messagesCount.trn = _this.transactions.length;
                            _this.transactions.forEach(function (tran) {
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
                    }else   if (direction === 'in') {
                                gxcFct.mail.incoming(queryParameters).$promise
                                .then(function (mailSuccess) {
                                    _this.messagesIncoming = mailSuccess;
                                    if (_this.messagesIncoming.length > 0) {                                        
                                        _this.messagesCount.in = mailSuccess.length;
                                    }
                                });
                            }
                            else {
                                gxcFct.mail.outgoing(queryParameters).$promise
                               .then(function (mailSuccess) {
                                   _this.messagesOutgoing = mailSuccess;                                   
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
                    $location.path('#/mail/message/' + mail.messageId);
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
                                    }
                                    if (_this.transactions[i].proposal.userProponent_ProposalStatus == 1 && _this.transactions[i].proposal.userReceiving_ProposalStatus == 1) {
                                        _this.transactions.splice(i, 1);
                                        // TODO: mandare un messaggio di accetazione della proposta!
                                        // TODO: togliere i giochi dalle librerie o metterli (non scambiabili)
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
                    if (tran.userOwnerId != _this.currentUserId)
                        return true;
                    // Can Cancel
                    return false;
                }

                this.myStatusVisibilityButton = function (tran, status) {
                    switch(status)
                    {
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
                    $location.path('#/mail/compose/' + tran.userId + '/' + tran.proposal.proposalId);
                }

                this.getMessages(_this.params.direction, _this.params.page);
                
                this.loadCounter = function () {
                    userSrv.updateUserData()
                    .then(function () {
                        var counterSuccess = userSrv.getCounterMessages(true);
                        _this.messagesCount.in = counterSuccess.In;
                        _this.messagesCount.out = counterSuccess.Out;
                        _this.messagesCount.trn = counterSuccess.Trn;
                    });
                }

                this.loadCounter();                
            },
            controllerAs: 'mailbox'
        }
    }]);

    app.directive('message', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/mail-message.html',
            controller: function ($routeParams) {
                var _this = this;
                _this.message = {};
                _this.transactions = {};

                this.reply = function () {

                };

                this.toggleImportant = function () {

                };

                this.archive = function () {

                };

                this.notify = function () {

                };

                this.backToMailbox = function () {
                    window.location = '#/mail/in/1';
                };

                //_this.message = gxcFct.mail.get(
                _this.msgId = $routeParams;

                gxcFct.mail.get({ messageId: $routeParams.messageId }).$promise
                  .then(function (success) {
                      _this.message = success;
                      _this.message.sender = gxcFct.user.byId({ userId: success.userProponent_Id });
                      gxcFct.user.byId({ userId: success.userReceiving_Id }).$promise
                      .then(function (receiverSuccess) {
                          _this.message.receiver = receiverSuccess;
                          _this.message.isIncoming = receiverSuccess.id == userSrv.getCurrentUser().id;
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
            controller: function ($scope, $routeParams) {
                var _this = this;
                _this.message = { myItems: [], hisItems: [] };
                _this.hisUserId = undefined;
                _this.isRaiseOffer = undefined;

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
                        direction: _this.directionProposal(),
                        transactionId: _this.currentTransaction.transactionId,
                        userLastChanges_Id: userSrv.getCurrentUser().id,
                        userProponent_ProposalStatus: 1, // L'utente che crea la proposta la accetta automaticamente.
                        userReceiving_ProposalStatus: 0, // da approvare
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
                    .then(function(success){
                        UIkit.notify('Nuovo scambio creato', { status: 'success', timeout: 5000 });
                    }, function (reason) {
                        UIkit.notify('Errore in creazione scambio.', { status: 'success', timeout: 5000 });
                    });
                }

                this.directionProposal = function () {
                    // TODO: Se è un rilancio di una transazione e se non solo l'utente owner, la direzione è false!
                    return true;
                }

                this.send = function () {
                    if (_this.exchange)
                    {
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
                        else
                        {
                            if (_this.isRaiseOffer) {
                                // TODO: annulla la proposta precedente
                            }
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
                            window.location = '#/mail/in/1';
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

                _this.recipientId = $routeParams.recipientId;                

                // My Library Components
                gxcFct.game.byUserWithComponent({ userId: userSrv.getCurrentUser().id }).$promise
                .then(function (gamesIds) {
                    var game = [];
                    gamesIds.forEach(function (element) {
                        game.push(element);
                    });
                    _this.myLibrary = game;
                });

                // His Library Components
                gxcFct.game.byUserWithComponent({ userId: _this.recipientId }).$promise
                .then(function (gamesIds) {
                    var game = [];                    
                    gamesIds.forEach(function(element){
                        game.push(element);
                    });
                    _this.hisLibrary = game;
                    _this.hisUserId = _this.recipientId;
                });
                
                _this.transactionId = $routeParams.transactionId;
                _this.proposalId = $routeParams.proposalId;
                if (_this.transactionId != undefined && _this.proposalId != undefined) {
                    _this.currentTransaction.transactionId = _this.transactionId;
                    _this.isRaiseOffer = true;
                }
            },
            controllerAs: 'mail'
        };
    }]);

    app.directive('testTransaction', ['factories', 'user-service', 'games-service', function (gxcFct, userSrv, gameSrv) {
        return {
            restrict: 'E',
            //scope: {                
            //},
            templateUrl: 'templates/test-transaction.html',
            controller: function ($routeParams, $scope) {
                var _this = this;
                _this.transactionStatus = ['Aperta', 'InAttesa', 'Conclusa'];
                _this.proposalStatus = ['DaApprovare','Accettata','Rifiutata'];
                _this.userReceiving_Id = 'b692ce4a-f114-473d-a754-1e30173fb4cd'; //alessandro.pilati
                _this.selectedLibraryGames = []; // Array di giochi presenti nella libreria dell'utente che riceve la proposta.
                _this.proposalText = 'Ciao sono il testo della proposta';
                _this.proposalObject = 'Ciao sono l\'oggetto della proposta';

                var currentDate = new Date();
                var futureDate = new Date(); //Aggiungere 1 anno

                // Oggetto contenente una proposta
                _this.currentProposal = [{
                    dateStart: currentDate.toISOString(),
                    dateEnd: futureDate.toISOString(),
                    direction: true, //la transazione iniziale ha sempre il verso PROPONENTE -> RICEVENTE
                    proposalText: _this.proposalText,
                    proposalObject: _this.proposalObject,
                    transactionId: undefined, // La transazione all'inizio non è ancora stata creata
                    userLastChanges_Id: 'b692ce4a-f114-473d-a754-1e30173fb4cb', //userSrv.getCurrentUser().id, // utente Proponente
                    userProponent_ProposalStatus: _this.proposalStatus[1], // Stato della proposta corrente per l'utente proponente. Se la propone ovviamente significa che l'accetta
                    userReceiving_ProposalStatus: _this.proposalStatus[0], // Stato della proposta corrente per l'utente ricevente
                    proposalComponents: []
                }];

                /*
                Carica dei dati di esempio. In questo caso aggiungo alla proposta tutti i giochi della mia libreria.
                */
                $scope.LoadData = function () {
                    // Carico dei componenti nella proposta di scambio. 
                    // I giochi verranno selezionati dall'utente Proponente. 
                    // I giochi selezionati saranno presenti nella libreria dell'utente ricevente e nella libreria dell'utente proponente.

                    // TODO: sistemare la libreria di lettura!
                    gxcFct.library.get({ libraryId: userSrv.getCurrentUser().LibraryId }).$promise
                    .then(function (librarySuccess) {
                        for (i in librarySuccess.libraryComponents) {
                            var g = librarySuccess.libraryComponents[i];
                            _this.selectedLibraryGames.push({libraryComponentId: g.libraryComponentId});
                        }

                        // Aggiunta dei componenti alla proposta di scambio
                        _this.currentProposal[0].proposalComponents = _this.selectedLibraryGames;
                    });

                }

                $scope.createInitialProposal = function () {
                    var queryParams = {
                        userProponent_Id: userSrv.getCurrentUser().id,
                        userReceiving_Id: _this.userReceiving_Id,
                        transactionStatus: _this.transactionStatus[0],
                        feedbacks: undefined,
                        proposals: _this.currentProposal
                    };

                    gxcFct.transaction.add(queryParams).$promise
                    .then(function (success) {
                        UIkit.notify('Transazione creata', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione transazione', { status: 'success', timeout: 5000 });
                    });
                };

                /*
                Carica tutte le transazioni per un utente.
                Da studiare:
                1) Quando considerare una transazione conclusa? Usiamo le date o il campo TransactionStatus?
                2) Servono ancora le date della transazione? Oppure le usiamo per stabilire la durata temporale (DataStar -> Inizio transazione; DataEnd -> Transazione conclusa/annullata)
                2b) Servono ancora le date della proposte? Oppure le usiamo per stabilire la durata temporale (DataStar -> Inizio proposta; DataEnd -> Proposta conclusa/annullata)
                3) Una volta che la transazione è conclusa si potrà generare il feedback.
                4) Una volta che la transazione è annullata NON si potrà generare nessun feedback.
                */
                $scope.LoadTransactionByUser = function () {
                    var queryParameters = {
                        userId: userSrv.getCurrentUser().userId,
                    };
                    gxcFct.transaction.byUser(queryParameters).$promise
                      .then(function (transSuccess) {
                          _this.tranProponent = transSuccess[0].transactionsProponent;
                          _this.tranReceiving = transSuccess[0].transactionsReceiving;
                      }); // transaction.byUser    
                }

                /*
                Aggiungo una nuova proposta alla transazione già creata in precedenza.
                Questa funzione serve per:
                1) Aggiungere una nuova proposta alla transazione attuale
                */
                $scope.AddProposal = function () {
                    var newProposal = _this.currentProposal[0];
                    // Prelevo una transazione a caso da quelle ricevute
                    newProposal.transactionId = _this.tranReceiving[0].transactionId;
                    // metto un pò di dati casuali
                    newProposal.proposalComponents = [_this.currentProposal[0].proposalComponents[1]];
                    newProposal.proposalText = 'Rilancio';
                    newProposal.proposalObject = 'Oggetto del rilancio';
                    newProposal.userLastChanges_Id = userSrv.getCurrentUser().id;
                    newProposal.direction = false; // Rilancio del ricevente
                    newProposal.userProponent_ProposalStatus = _this.proposalStatus[0], // Stato della proposta corrente per l'utente proponente.
                    newProposal.userReceiving_ProposalStatus = _this.proposalStatus[1], // Stato della proposta corrente per l'utente ricevente. Se la propone ovviamente significa che l'accetta

                    gxcFct.proposal.add(newProposal).$promise
                    .then(function (success) {
                        UIkit.notify('Proposta creata', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione Proposta', { status: 'success', timeout: 5000 });
                    });
                }


                /*
                Aggiornamento della proposta
                */
                $scope.UpdateProposal = function () {

                    var randomProposalId = _this.tranReceiving[0].proposals[0].proposalId;

                    gxcFct.proposal.get({ propId: randomProposalId }).$promise
                   .then(function (success) {
                       var oldProposal = success;
                       oldProposal.proposalObject = 'ProposataAggiornata';

                       gxcFct.proposal.update({ propId: randomProposalId }, oldProposal,
                        function (success) {
                            UIkit.notify('Utente aggiornato. Ora puoi accedere alle funzionalit&agrave; del sito', { status: 'success', timeout: 5000 });
                            window.location = '#/';
                        },
                        function (error) {
                            UIkit.notify('Si &egrave; verificato un errore durante la registrazione. Ci dispiace per l\'inconveniente e ti chiediamo di riprovare più tardi.', { status: 'error', timeout: 5000 });
                        });
                   },
                   function (error) {
                       UIkit.notify('Si &egrave; verificato un errore nell\'operazione. Si prega di riprovare', { status: 'warning', timeout: 5000 });
                   });

                    
                    
                }












                /*Sezione feedback*/
                _this.Feedback = {
                    transactionId: undefined,
                    userId: undefined,
                    rate: undefined
                };
                // In caso di transazione positiva assegnare un +1, in caso di transazione negativa un -3
                _this.RateVote = [1,-3];

                $scope.AddFeedback = function () {
                    // Prelevo una transazione a caso da quelle ricevute
                    _this.Feedback.transactionId = _this.tranReceiving[0].transactionId;
                    // Prelevo l'utente corrente o l'utente destinatario della transazione
                    _this.Feedback.userId = _this.tranReceiving[0].userReceiving_Id;
                    _this.Feedback.rate = _this.RateVote[0];

                    gxcFct.feedback.add(_this.Feedback).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback creato', { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore creazione Feedback', { status: 'success', timeout: 5000 });
                    });

                }

                $scope.GetRate = function () {
                    // Ritorno il rate dell'utente corrente
                    var queryParameters = {
                        userId: userSrv.getCurrentUser().id,
                    };

                    gxcFct.feedback.rate(queryParameters).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback rate: ' + success.rate + "% su " + success.counter + "feedback ricevuto", { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore rate Feedback', { status: 'success', timeout: 5000 });
                    });
                }
                
                // Ritorna tutti gli ID delle transazioni senza feedback per l'utente
                $scope.GetPendingTransactionFeedback = function () {
                    // Ritorno il rate dell'utente corrente
                    var queryParameters = {
                        userId: 'b692ce4a-f114-473d-a754-1e30173fb4cb'//userSrv.getCurrentUser().id,
                    };

                    gxcFct.feedback.pending(queryParameters).$promise
                    .then(function (success) {
                        UIkit.notify('Feedback pending: ' + success, { status: 'success', timeout: 5000 });
                    },
                    function (error) {
                        UIkit.notify('Errore pending Feedback', { status: 'success', timeout: 5000 });
                    });
                }
            },
            controllerAs: 'testTransaction'
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
                        userId: _this.currentUserId,
                        page: _this.currentPage
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

                                //TODO: impostare conclusa la transazione se ho il feedback di entrambi!
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
})();
