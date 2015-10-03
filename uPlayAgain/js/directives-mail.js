(function () {
    var app = angular.module('gxc.directives.mail', []);

    app.directive('mailbox', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/mail-mailbox.html',
            controller: function ($routeParams) {
                var _this = this;

                _this.params = $routeParams;
                _this.messages = [];
                _this.transactions = [];
                _this.messagesCount = { in: 0, out: 0, trn: 0 };
                _this.currentPage = 1;
                _this.showTransactions = false;

                var getMessages = function (direction, page) {
                    var queryParameters = {
                        userId: userSrv.getUser().userId,
                    };
                    var incoming = (direction === 'in');
                    _this.showTransactions = (direction === 'transactions');

                    _this.messages = [];
                    _this.transactions = [];

                    gxcFct.mail.byUser(queryParameters).$promise
                      .then(function (mailSuccess) {
                          _this.messages = incoming ? mailSuccess[0].messagesIn : mailSuccess[0].messagesOut;

                          for (msg in _this.messages) {
                              _this.messages[msg].userId = incoming ? _this.messages[msg].userProponent.userId : _this.messages[msg].userReceiving.userId;
                          }

                          _this.messagesCount.in = mailSuccess[0].messagesIn.length;
                          _this.messagesCount.out = mailSuccess[0].messagesOut.length;
                      }); // mail.byUser     

                    gxcFct.transaction.byUser(queryParameters).$promise
                    .then(function (trnSuccess) {
                        for (trn in trnSuccess[0].transactionsProponent) {
                            _this.transactions.push(trnSuccess[0].transactionsProponent[trn]);
                        }
                        for (trn in trnSuccess[0].transactionsReceiving) {
                            _this.transactions.push(trnSuccess[0].transactionsReceiving[trn]);
                        }

                        _this.messagesCount.trn += trnSuccess[0].transactionsProponent.length;
                        _this.messagesCount.trn += trnSuccess[0].transactionsReceiving.length;
                    });

                    _this.currentPage = page;
                };

                this.hoverIn = function (mail) {
                    mail.hovered = true;
                }

                this.hoverOut = function (mail) {
                    mail.hovered = false;
                }

                this.open = function (mail) {
                    window.location = '#/mail/message/' + mail.messageId;
                }

                getMessages(_this.params.direction, _this.params.page);
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
                          _this.message.isIncoming = receiverSuccess.id == userSrv.getUser().id;
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
                _this.recipientData = undefined;

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
                        direction: (_this.currentProposal === undefined ? true : !_this.currentProposal.direction),
                        transactionId: _this.currentTransaction.transactionId,
                        userLastChanges_Id: userSrv.getUser().id,
                        proposalComponents: []
                    };

                    for (i in _this.message.myItems) {
                        queryParams.proposalComponents.push({ 
                            libraryComponentId: _this.message.myItems[i].libraryComponentId,
                            userOwnerId: userSrv.getUser().id
                        });
                    }
                    for (i in _this.message.hisItems) {
                        queryParams.proposalComponents.push({
                            libraryComponentId: _this.message.hisItems[i].libraryComponentId,
                            userOwnerId: _this.recipientData.id
                        });
                    }

                    gxcFct.proposal.add(queryParams);

                    /*
                    gxcFct.proposal.add(queryParams).$promise
                    .then(function (success) {
                        var proposal = success;

                        for (i in _this.message.myItems) {
                            var queryParams = {
                                libraryComponentId: _this.message.myItems[i].gameId,
                                proposalId: success.proposalId
                            }
                            gxcFct.proposalComponents.add(queryParams);
                        }
                        for (i in _this.message.hisItems) {
                            var queryParams = {
                                libraryComponentId: _this.message.hisItems[i].gameId,
                                proposalId: success.proposalId
                            }
                            gxcFct.proposalComponents.add(queryParams);
                        }
                    });
                    */
                }

                this.send = function () {
                    if (_this.exchange)
                    {
                        if (_this.currentTransaction === undefined) {
                            var queryParams = {
                                userProponent_Id: userSrv.getUser().id,
                                userReceiving_Id: _this.recipientData.id
                            }

                            gxcFct.transaction.add(queryParams).$promise
                            .then(function (success) {
                                _this.currentTransaction = success;
                                addProposal();
                            });
                        }
                        else
                        {
                            addProposal();
                        }
                    }
                    else {
                        var queryParams = {
                            messageText: _this.message.text,
                            messageObject: _this.message.titolo,
                            messageDate: new Date().toISOString(),
                            userProponent_Id: userSrv.getUser().id,
                            userReceiving_Id: _this.recipientData.id
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

                gxcFct.library.get({ libraryId: userSrv.getUser().LibraryId }).$promise
                .then(function (librarySuccess) {

                    for (i in librarySuccess.libraryComponents) {
                        var g = librarySuccess.libraryComponents[i];
                        gameSrv.fillGameData(g);
                    }

                    _this.myLibrary = librarySuccess.libraryComponents;
                });

                gxcFct.user.byId({ userId: _this.recipientId }).$promise
                .then(function (userSuccess) {
                    gxcFct.library.byUser({ userId: userSuccess.userId }).$promise
                    .then(function(ulSuccess) {
                    _this.recipientData = ulSuccess[0];
                        gxcFct.library.get({ libraryId: ulSuccess[0].libraries[0].libraryId }).$promise
                        .then(function (librarySuccess) {

                            for (i in librarySuccess.libraryComponents) {
                                var g = librarySuccess.libraryComponents[i];
                                gameSrv.fillGameData(g);
                            }

                            _this.hisLibrary = librarySuccess.libraryComponents;
                        });
                    });
                });
                
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
                    userLastChanges_Id: 'b692ce4a-f114-473d-a754-1e30173fb4cb', //userSrv.getUser().id, // utente Proponente
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
                    gxcFct.library.get({ libraryId: userSrv.getUser().LibraryId }).$promise
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
                        userProponent_Id: userSrv.getUser().id,
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
                        userId: userSrv.getUser().userId,
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
                    newProposal.userLastChanges_Id = userSrv.getUser().id;
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
                        userId: userSrv.getUser().id,
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
                        userId: 'b692ce4a-f114-473d-a754-1e30173fb4cb'//userSrv.getUser().id,
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
    
    app.directive('feedbackVote', ['factories', 'user-service', function (gxcFct, userSrv) {
        return {
            restrict: 'E',
            templateUrl: 'templates/feedback-vote.html',
            controller: function ($routeParams) {
                var _this = this;
                _this.currentUserId = userSrv.getUser().id;
                _this.currentTransactionToVote = [];                

                _this.GetPendingTransactionFeedback = function () {
                    gxcFct.feedback.pending({ userId: userSrv.getUser().id }).$promise
                    .then(function (success) {
                        success.forEach(function (tran) {
                            gxcFct.transaction.get({ tranId: tran }).$promise
                            .then(function (successTran) {
                                // Carico solo la proposta conclusa e i rispettivi componenti.
                                for (i = successTran.proposals.length - 1; i >= 0; i--) {
                                    // rimuovo le proposte non accettate da entrambi gli utenti
                                    if (successTran.proposals[i].userReceiving_ProposalStatus != 1 && successTran.proposals[i].userProponent_ProposalStatus != 1) {
                                        array.splice(i, 1);
                                    }
                                    //Aggiungo i componenti alla proposta
                                    else {
                                        // 
                                        _this.currentTransactionToVote.push(successTran);
                                        gxcFct.proposal.get({ propId: successTran.proposals[i].proposalId }).$promise
                                        .then(function (successComponents) {
                                            // Aggancio la proposta con le componenti alla transazione
                                            for (var i = 0; i < _this.currentTransactionToVote.length; i++) {                                                
                                                if (_this.currentTransactionToVote[i].proposals[0].proposalId === successComponents.proposalId) {
                                                    _this.currentTransactionToVote[i].proposals[0] = successComponents;
                                                    break;
                                                }
                                            }
                                        },
                                        function (error) {
                                            UIkit.notify('Errore lettura componenti della proposata', { status: 'success', timeout: 5000 });
                                        });                                         
                                    }
                                }

                                // TODO -> Caricare il feedback dell'utente presente nelle transazioni.                                
                            },
                            function (error) {
                                UIkit.notify('Errore lettura transazioni da votare', { status: 'success', timeout: 5000 });
                            });
                        });
                    },
                    function (error) {
                        UIkit.notify('Errore lettura feedback da votare', { status: 'success', timeout: 5000 });
                    });
                }
                
                // Carico le transazioni da assegnare con un feedback all'avvio
                _this.GetPendingTransactionFeedback();
                
                _this.AddFeedback = function (transactionId, vote) {
                    // In caso di transazione positiva assegnare un +1, in caso di transazione negativa un -3
                    var tranFilter = undefined;
                    for (i = 0; i < _this.currentTransactionToVote.length; i++) {
                        if (_this.currentTransactionToVote[i].transactionId === transactionId) {
                            // Reperisco l'utente corretto al quale rilasciare il feedback.
                            var userToFeedback = undefined;
                            if (_this.currentTransactionToVote[i].userProponent_Id != userSrv.getUser().id && _this.currentTransactionToVote[i].userReceiving_Id == userSrv.getUser().id)
                                userToFeedback = _this.currentTransactionToVote[i].userProponent_Id;
                            if (_this.currentTransactionToVote[i].userProponent_Id == userSrv.getUser().id && _this.currentTransactionToVote[i].userReceiving_Id != userSrv.getUser().id)
                                userToFeedback = _this.currentTransactionToVote[i].userReceiving_Id;

                            _this.Feedback = {
                                transactionId: transactionId,
                                userId: userToFeedback,
                                rate: vote
                            };

                            gxcFct.feedback.add(_this.Feedback).$promise
                            .then(function (success) {
                                // Rimuovo la transazione che ha avuto il feedback
                                _this.currentTransactionToVote[i].splice(i, 1);
                                UIkit.notify('Hai votato con successo. Grazie!', { status: 'success', timeout: 5000 });
                            },
                            function (error) {
                                UIkit.notify('Errore creazione Feedback', { status: 'success', timeout: 5000 });
                            });
                            break;
                        }                            
                    }
                    
                    
                }                
            },
            controllerAs: 'feedbackVote'
        };
    }]);
})();
