using ApiConnectionExample.Models;
using ApiConnectionExample.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ApiConnectionExample
{
    public static class ServiceLayer
    {
        //Create Transaction class instance for access to PayPhone library methods
        private static Connection connection = new Connection();

        //Properties used for the process in this example 
        //These properties do not necessarily have to be used in your project
        public static long Id;
        private static int _status = 1;
        public static long AnnulmentId;
        public static bool Approved;        

        #region Estos métodos lo pueden usar todos los comercios 

        /// <summary>
        /// Get regions available i PayPhone
        /// This regions needs to be show to user for his choose the one corresponding to his phone number 
        /// </summary>
        public static void GetRegions()
        {
            try
            {
                //When calling this method we receive a RegionsRespnseModel
                //In the list of regions we can find the name and its telephone code
                var regionList = connection.GetCall<List<RegionResponseModel>>("/api/Regions", Configurations.Lang);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                foreach (var regionsResponseModel in regionList)
                {
                    Console.WriteLine($"{regionsResponseModel.Name} - {regionsResponseModel.PrefixNumber}");
                }
                Console.ResetColor();
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetRegions();
                }
                else
                {
                    //All call make to PayPhone need to be inside a try catch
                    PrintErrors(e);
                }
                
            }
        }

        /// <summary>
        /// Send the transaction and notifies the customer of a new charge
        /// </summary>
        /// <param name="phoneNumber">Phone number of customer</param>
        /// <param name="regionCode">Phone number region code</param>
        public static bool Sale(TransactionRequestModel data)
        {
            try
            {
                data.ChargeByNickName = false;
                var tx = connection.PostCall<TransactionResponseModel>("/api/Sale", data, Configurations.Lang);
                Id = tx.TransactionId;
                Console.Clear();
                Console.WriteLine("Transaction Id " + tx.TransactionId);
                return true;
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    return Sale(data);
                }

                PrintErrors(e);

                return false;

            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
                return false;
            }
        }

        /// <summary>
        /// Send the transaction and notifies the customer of a new charge
        /// </summary>
        /// <param name="documentId">Document id of PayPhone user</param>
        /// <param name="regionCode">Region Phone Number Code</param>
        public static bool SaleByNickName(TransactionRequestModel data)
        {
            try
            {
                data.ChargeByNickName = true;
                //Send trasnaction but in this case we use the document id insert by user when he registered in PayPhone 
                var tx = connection.PostCall<TransactionResponseModel>("/api/Sale", data, Configurations.Lang);
                //Save PayPhone transaction id for get status of transaction
                Id = tx.TransactionId;
                Console.Clear();
                Console.WriteLine("Transaction Id " + tx.TransactionId);
                return true;
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    return SaleByNickName(data);
                }

                PrintErrors(e);

                return false;
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
                return false;
            }
        }

        /// <summary>
        /// Get transaction status by trasaction id get from PayPhone
        /// </summary>
        /// <param name="txId">Trasaction id get from PayPhone</param>
        public static void GetStatusTransaction(long txId)
        {
            try
            {
                //Get transaction status by id
                var tx = connection.GetCall<TransactionResultResponseModel>($"/api/Sale/{txId}", Configurations.Lang);
                _status = tx.StatusCode;
                //check the status of transaction
                if (_status == (int)TransactionStatus.Approved)
                {
                    Approved = true;
                }
                if (tx.StatusCode == (int)TransactionStatus.Approved || tx.StatusCode == (int)TransactionStatus.Canceled)
                {
                    //If transaction was approved or cancelled cast the object ResultResponseModel for access to transaction data
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Codigo autorizacion {tx.AuthorizationCode}");
                    Console.WriteLine($"Mensaje {tx.Message}");
                    Console.WriteLine($"Telefono {tx.PhoneNumber}");
                    Console.WriteLine($"Bin {tx.Bin}");
                    Console.WriteLine($"Deferred {tx.Deferred}");
                    Console.WriteLine($"Deferred Message {tx.DeferredMessage}");
                    Console.WriteLine($"Amount {tx.Amount}");
                    Console.WriteLine($"Deferred Message {tx.DeferredMessage}");
                    Console.WriteLine($"Deferred Code {tx.DeferredCode}");
                    Console.WriteLine($"Card Brand {tx.CardBrand}");
                    Console.WriteLine($"Card Type {tx.CardType}");
                    Console.WriteLine($"Email {tx.Email}");
                    Console.WriteLine($"Document {tx.Document}");
                    //Check if transaction have taxes 
                    if (tx.Taxes != null && tx.Taxes.Any())
                    {
                        //SUM all store applicable taxes
                        var tariff = tx.Taxes.Sum(tax => decimal.Divide(tax.Amount, 100));
                        Console.WriteLine($"Tarifa {tariff}");
                    }

                    Console.ResetColor();
                }
                else
                {
                    //otherwise access to transaction message which describes what happened
                    var pending = tx;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pending.TransactionStatus);
                    Console.ResetColor();
                }
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetStatusTransaction(txId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }


        /// <summary>
        /// Get transaction status by client transaction id
        /// </summary>
        public static void GetStatusByClientTransactionId(string clientId)
        {
            try
            {
                //Request for transaction status by client transaction id
                var result = connection.GetCall<List<TransactionResultResponseModel>>($"/api/Sale/client/{clientId}", Configurations.Lang);
                if (_status == (int)TransactionStatus.Approved)
                {
                    Approved = true;
                }

                foreach (var response in result)
                {
                    //Check status of transaction
                    if (response.StatusCode == (int)TransactionStatus.Approved || response.StatusCode == (int)TransactionStatus.Canceled)
                    {
                        //If transaction was approved or cancelled cast the object ResultResponseModel for access to transaction data
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Codigo autorizacion {response.AuthorizationCode}");
                        Console.WriteLine($"Mensaje {response.Message}");
                        Console.WriteLine($"Telefono {response.PhoneNumber}");
                        Console.WriteLine($"Bin {response.Bin}");
                        Console.WriteLine($"Deferred {response.Deferred}");
                        Console.WriteLine($"Deferred Message {response.DeferredMessage}");
                        Console.WriteLine($"Amount {response.Amount}");
                        Console.WriteLine($"Deferred Message {response.DeferredMessage}");
                        Console.WriteLine($"Deferred Code {response.DeferredCode}");
                        Console.WriteLine($"Card Brand {response.CardBrand}");
                        Console.WriteLine($"Card Type {response.CardType}");
                        Console.WriteLine($"Email {response.Email}");
                        Console.WriteLine($"Document {response.Document}");
                        //Check if transaction have taxes 
                        if (response.Taxes != null && response.Taxes.Any())
                        {
                            //SUM all store applicable taxes
                            var tariff = response.Taxes.Sum(tax => decimal.Divide(tax.Amount, 100));
                            Console.WriteLine($"Tarifa {tariff}");
                        }

                        Console.ResetColor();
                    }
                    else
                    {
                        //otherwise access to transaction message which describes what happened
                        var pending = response;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(pending.TransactionStatus);
                        Console.ResetColor();
                    }
                }
                
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetStatusByClientTransactionId(clientId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Update the actual token for new one
        /// Validate if exception receive is of Unauthorized type 
        /// </summary>
        /// <param name="e">Exception returned by PayPhone</param>
        /// <returns></returns>
        private static bool RefreshToken(PayPhoneWebException e)
        {
            try
            {
                //Check status code oofthe response
                if (e.StatusCode.Equals(HttpStatusCode.Unauthorized.ToString()))
                {
                    //If status code is Unauthorized get a new token and replace 
                    var token = connection.GetToken(Configurations.Ruc);
                    Configurations.Token = token.Access_Token;
                    return true;
                }
                return false;
            }
            catch (PayPhoneWebException exc)
            {
                //Remember all call in PayPHone return an exception 
                //Capture the exception and observe what happens
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"401 - Unathorize {exc.Message}");
                Console.ResetColor();
                return false;
            }
            catch (Exception exc)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + exc.Message);
                Console.ResetColor();
                return false;
            }
        }        

        /// <summary>
        /// Annull transaction by transction id get from PayPHone
        /// </summary>
        /// <param name="txId">Transction id get from PayPHone</param>
        public static void Annulment(long txId)
        {
            //Prepare the objet to send for the annulment
            var cancellation = new CancellationRequestModel
            {
                Id = txId
            };
            try
            {
                //Send annulment request with object created above
                var response = connection.PostCall<AnnulResponseModel>("/api/Annul", cancellation, Configurations.Lang);
                //Save the annulment id for get status later
                AnnulmentId = response.Id;
                Console.WriteLine($"Identificador de la anulación: {AnnulmentId}");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Annulment(txId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get annulment status by annulmet id
        /// </summary>
        /// <param name="idAnnul">Annulment id get from PayPhone</param>
        public static void GetAnnulmentStatus(long idAnnul)
        {
            try
            {
                //Get annulment status by annulment id
                var response = connection.GetCall<AnnulResultResponseModel>($"/api/Annul/{idAnnul}", Configurations.Lang);
                //Check annulment status
                if (response.StatusCode == (int)TransactionStatus.Pending ||
                    response.StatusCode == (int)TransactionStatus.Canceled)
                {
                    Console.WriteLine(response.Message);
                }
                else
                {
                    //if annulment was approbed cast the object ResultResponseModel and access to transaction a annulled
                    var result = response;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Estado de la anulacion {result.Status}");
                    Console.WriteLine($"Estado de la Venta {result.Sale.Status}");                    
                    Console.ResetColor();
                }

            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetAnnulmentStatus(idAnnul);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Annull transaction by client transction id
        /// </summary>
        /// <param name="txId">Transction id get from PayPHone</param>
        public static void AnnulmentByClientId(string clientId)
        {
            //Prepare the objet to send for the annulment
            var cancellation = new CancellationByClientRequestModel
            {
                ClientId = clientId
            };
            try
            {
                //Send annulment request with object created above
                var response = connection.PostCall<AnnulResponseModel>("/api/Annul/client", cancellation, Configurations.Lang);
                //Save the annulment id for get status later
                AnnulmentId = response.Id;
                Console.WriteLine($"Identificador de la anulación: {AnnulmentId}");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    AnnulmentByClientId(clientId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get annulment status by client transaction id
        /// </summary>
        /// <param name="idAnnul">Annulment id get from PayPhone</param>
        public static void GetAnnulmentStatusByClientId(string clientId)
        {
            try
            {
                //Get annulment status by annulment id
                var response = connection.GetCall<AnnulResultResponseModel>($"/api/Annul/client/{clientId}", Configurations.Lang);
                //Check annulment status
                if (response.StatusCode == (int)TransactionStatus.Pending ||
                    response.StatusCode == (int)TransactionStatus.Canceled)
                {
                    Console.WriteLine(response.Message);
                }
                else
                {
                    //if annulment was approbed cast the object ResultResponseModel and access to transaction a annulled
                    var result = response;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Estado de la anulacion {result.Status}");
                    Console.WriteLine($"Estado de la Venta {result.Sale.Status}");
                    Console.ResetColor();
                }

            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetAnnulmentStatusByClientId(clientId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Send request for reimbursement by transaction id
        /// </summary>
        /// <param name="idTx">Transaction id get from PayPhone</param>
        public static void Reimbursemet(long txId)
        {
            try
            {
                //Prepare the objet to send for the annulment
                var cancellation = new CancellationRequestModel
                {
                    Id = txId
                };
                //Send reimbursement by transaction id request
                var response = connection.PostCall<bool>("/api/Reverse", cancellation, Configurations.Lang);
                //save the reimbursement id returned
                if (response)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"El reverso ha sido realizado satisfactoriamente");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"La transaccion no pudo ser reversada");
                }
                Console.ResetColor();

            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Reimbursemet(txId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Send request for reimbursement by client transaction id
        /// </summary>
        public static void ReimbursemetByClientTransactionId(string clientId)
        {
            try
            {
                var cancellation = new CancellationByClientRequestModel
                {
                    ClientId = clientId
                };
                //Send reimbursement by client transaction id request
                var response = connection.PostCall<bool>("/api/Reverse/Client", cancellation, Configurations.Lang);
                //save the reimbursement id returned
                if (response)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"El reverso ha sido realizado satisfactoriamente");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"La transaccion no pudo ser reversada");
                }
                Console.ResetColor();
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    ReimbursemetByClientTransactionId(clientId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }


        /// <summary>
        /// Cancel a transaction that has not yet been approved
        /// </summary>
        public static void Cancel(long id)
        {
            try
            {
                //Prepare the objet to send for the annulment
                var cancellation = new CancellationRequestModel
                {
                    Id = id
                };
                //Send request for cancel transaction using transaction id
                var response = connection.PostCall<bool>("/api/Cancel", cancellation, Configurations.Lang);
                Console.WriteLine(response ? "Transaccion cancelada" : "La transaccion no pudo ser cancelada");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Cancel(id);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Cancel a transaction that has not yet been approved by client transaction id
        /// </summary>
        public static void CancelByClientId(string clientId)
        {
            try
            {
                //Prepare the objet to send for the annulment
                var cancellation = new CancellationByClientRequestModel
                {
                    ClientId = clientId
                };
                //Send request for cancel transaction using transaction id
                var response = connection.PostCall<bool>("/api/Cancel/client", cancellation, Configurations.Lang);
                Console.WriteLine(response ? "Transaccion cancelada" : "La transaccion no pudo ser cancelada");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    CancelByClientId(clientId);
                }

                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get user daata if exist in PayPhone
        /// </summary>
        /// <param name="identifier">Identifier off user (Phone number or document id)</param>
        /// <param name="regionCode">Region phone number code</param>
        /// <param name="byNickName">True if the first arameter is document id</param>
        public static void GetUserData(string identifier, string regionCode, bool byNickName)
        {
            try
            {
                var uri = "";
                if (byNickName)
                {
                    uri = $"/api/Users/nickname/{identifier}/region/{regionCode}";
                }
                else
                {
                    uri = $"/api/Users/{identifier}/region/{regionCode}";
                }

                var response = connection.GetCall<UserResponseModel>(uri, Configurations.Lang);
                Console.WriteLine("Nombre de usuario " + response.Name + " " + response.LastName);
                Console.WriteLine("Email " + response.Email);
                foreach (var phoneNumber in response.PhoneNumbers)
                {
                    Console.WriteLine("Phone numbers " + phoneNumber);
                }

            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    GetUserData(identifier, regionCode, byNickName);
                }

                PrintErrors(e);

            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Get new token to access to PayPhone service 
        /// </summary>
        /// <param name="ruc">RUC of company associated to application</param>
        public static void GetToken(string ruc)
        {
            try
            {
                //Calling to GetToken to retrive the new access token and refresh token
                var response = connection.GetToken(ruc);
                //Replace old tokens by new one
                Configurations.Token = response.Access_Token;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("token " + response.Access_Token);
                Console.ResetColor();
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                PrintErrors(e);
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Imprime los mensajes de error retornados por el servicio
        /// </summary>
        /// <param name="e"></param>
        private static void PrintErrors(PayPhoneWebException e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{e.StatusCode} - {e.Error.Message} ");
            if (e.Error.Errors != null && e.Error.Errors.Count > 0)
            {
                foreach (var item in e.Error.Errors)
                {
                    if (item.ErrorDescriptions == null || item.ErrorDescriptions.Length <= 0)
                    {
                        Console.WriteLine($"{item.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"({item.Message} - {string.Join(";", item.ErrorDescriptions)})");
                    }
                    
                }
            }
            
            Console.ResetColor();
        }
        #endregion

        #region Estos metodos para habilitar se debe contactar con PayPhone
        //En esta seccion se consumen metodos que no se libres para todos los comercios
        //Esto no quiere decir que hay que pagar sino que para emplearlos se debe primero ponerse en contacto con PayPhone
        //Estos métodos permite mediante el api realizar cobros con la tarjeta de crédito o débito directamente sin tener usuario PayPhone
        #endregion


    }
}
