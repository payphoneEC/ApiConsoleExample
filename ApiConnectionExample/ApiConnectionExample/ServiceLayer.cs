using ApiConnectionExample.Models;
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
        private static long _reimbursmentId;
        public static bool Approved;
        private static string _extTxId = "";
        private static string _ruc = "0190166260001"; //0190409759001

        //Creaate a dummy data for send to PayPhone
        private static readonly TransactionRequestModel Data = new TransactionRequestModel()
        {
            //Total amount to send
            Amount = 65241,
            //Total amount of products that charge taxes without taxes
            AmountWithTax = 0,
            //Total amount of product that not charge taxes
            AmountWithoutTax = 65241,
            //Total taxes generated for AmountWithTax
            Tax = 0,
            //Identifier of transaction for this example (is id of transaction in ower application) 
            ClientTransactionId = Guid.NewGuid().ToString(),
            //Time zone for application
            TimeZone = -5,
            //Latitude fr store
            Lat = "-0.170315",
            //Longitude for store
            Lng = "-78.489632",
            //Store id
            //StoreId = "d8383302-7afe-4f45-8f91-df65995ed28a"
            StoreId = "f4781cf6-af17-46ac-ad22-ecbac1805836"
        };

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
                Console.Write($"{e.StatusCode} - ");
                foreach (var errorResponseModel in e.ErrorList)
                {
                    Console.WriteLine(errorResponseModel.Message);
                }
            }
        }

        /// <summary>
        /// Send the transaction and notifies the customer of a new charge
        /// </summary>
        /// <param name="phoneNumber">Phone number of customer</param>
        /// <param name="regionCode">Phone number region code</param>
        public static bool SetAndSendTransaction(string phoneNumber, string regionCode)
        {
            try
            {
                Data.PhoneNumber = phoneNumber;
                Data.CountryCode = regionCode;

                var tx = connection.PostCall<TransactionResponseModel>("/api/Sale", Data, Configurations.Lang);
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
                    return SetAndSendTransaction(phoneNumber, regionCode);
                }

                Console.Write($"{e.StatusCode} - ");
                foreach (var errorResponseModel in e.ErrorList)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorResponseModel.Message);
                    Console.ResetColor();
                }

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
        public static bool SetAndSendTransactionByNickName(string documentId, string regionCode)
        {
            try
            {
                Data.ChargeByNickName = true;
                Data.CountryCode = regionCode;
                Data.NickName = documentId;

                //Send trasnaction but in this case we use the document id insert by user when he registered in PayPhone 
                var tx = connection.PostCall<TransactionResponseModel>("/api/Sale", Data, Configurations.Lang);
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
                    return SetAndSendTransactionByNickName(documentId, regionCode);
                }

                Console.Write($"{e.StatusCode} - ");
                foreach (var errorResponseModel in e.ErrorList)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorResponseModel.Message);
                    Console.ResetColor();
                }

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
                    var token = connection.GetToken(_ruc);
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
        /// Send transaction to PayPhone by phone number but not notify the user
        /// This method return the identity of client in PayPhone for validate
        /// </summary>
        /// <param name="phoneNumber">Phone number of client</param>
        /// <param name="regionCode">Region phone number code</param>
        public static void SendTransaction(string phoneNumber, string regionCode)
        {

            try
            {
                //Submit transaction but not notify customer
                //Only the user data and transaction ID generated are returned
                var tx = connection.SetTransaction(Data, phoneNumber, regionCode);
                Id = tx.TransactionId;
                Console.Clear();
                Console.WriteLine("Name " + tx.Name + " " + tx.LastName);
                Console.WriteLine("Client transaction id: " + tx.ClientTransactionId);
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    SendTransaction(phoneNumber, regionCode);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Send transaction to PayPhone by document id but not notify the user
        /// This method return the identity of client in PayPhone for validate
        /// </summary>
        /// <param name="documentId">Document id register for the customer</param>
        /// <param name="regionCode">Region phone number code</param>
        public static bool SendTransactionByNickname(string documentId, string regionCode)
        {
            try
            {
                //Submit transaction but not notify customer
                //Only the user data and transaction ID generated are returned
                var tx = connection.SetTransaction(Data, "", regionCode, true, documentId);
                Id = tx.TransactionId;
                Console.Clear();
                Console.WriteLine("Nombre " + tx.Name + " " + tx.LastName);
                Console.WriteLine("Client transaction id: " + tx.ClientTransactionId);
                return true;
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    return SendTransactionByNickname(documentId, regionCode);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
                return false;
            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                return false;
            }
        }

        /// <summary>
        /// Set the previus generate transaction and send notification tu client
        /// </summary>
        public static void SetTransaction()
        {
            try
            {
                //Establish the transaction with the identifier received in the previous method
                var tx = connection.DoTransaction(Id);
                Console.WriteLine(tx.Message);
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    SetTransaction();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error has ocurred " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        /// Get transaction status by trasaction id get from PayPhone
        /// </summary>
        /// <param name="txId">Trasaction id get from PayPhone</param>
        public static void StatusTransaction(long txId)
        {
            try
            {
                //Get transaction status by id
                var tx = connection.GetTransactionById(txId);
                _status = tx.Status;
                //check the status of transaction
                if (_status == (int)TransactionStatus.Approved)
                {
                    Approved = true;
                    //in these case i will send a custom bill if transaction was approved
                    SendCustomBill();
                }
                if (tx.Status == (int)TransactionStatus.Approved || tx.Status == (int)TransactionStatus.Canceled)
                {
                    //If transaction was approved or cancelled cast the object ResultResponseModel for access to transaction data
                    var response = (ResultResponseModel)tx;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Codigo autorizacion {response.Message.AuthorizationCode}");
                    Console.WriteLine($"Mensaje {response.Message.Message}");
                    Console.WriteLine($"Telefono {response.Message.PhoneNumber}");
                    Console.WriteLine($"Bin {response.Message.Bin}");
                    Console.WriteLine($"Deferred {response.Message.Deferred}");
                    Console.WriteLine($"Deferred Message {response.Message.DeferredMessage}");
                    Console.WriteLine($"Amount {response.Message.Amount}");
                    Console.WriteLine($"Deferred Message {response.Message.DeferredMessage}");
                    Console.WriteLine($"Deferred Code {response.Message.DeferredCode}");
                    Console.WriteLine($"Card Brand {response.Message.CardBrand}");
                    Console.WriteLine($"Card Type {response.Message.CardType}");
                    Console.WriteLine($"Email {response.Message.Email}");
                    Console.WriteLine($"Document {response.Message.Document}");
                    //Check if transaction have taxes 
                    if (response.Message.Taxes.Any())
                    {
                        //SUM all store applicable taxes
                        var tariff = response.Message.Taxes.Sum(tax => decimal.Divide(tax.Amount, 100));
                        Console.WriteLine($"Tarifa {tariff}");
                    }

                    Console.ResetColor();
                }
                else
                {
                    //otherwise access to transaction message which describes what happened
                    var pending = (StatusResponseModel)tx;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(pending.Message);
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
                    StatusTransaction(Id);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error has ocurred " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        /// Annull transaction by transction id get from PayPHone
        /// </summary>
        /// <param name="txId">Transction id get from PayPHone</param>
        public static void Annulment(long txId)
        {
            //Prepare the objet to send for the annulment
            var annulment = new AnnulmentRequestModel
            {
                //Transaction if to annul get from PayPhone
                TransactionId = txId,
                //Time zone for place where the annulment is being done
                TimeZone = -5,
                //Latitude for place where the annulment is being done
                Latitude = "-2.56646",
                //Longitude for place where the annulment is being done
                Longitude = "-78.1231313",
            };
            try
            {
                //Send annulment request with object created above
                var response = connection.DoAnnulment(annulment);
                //Save the annulment id for get status later
                AnnulmentId = response.AnnulmentId;
                Console.WriteLine($"Identificador de la anulación: {AnnulmentId}");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Annulment(Id);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
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
                var response = connection.GetAnnulmentById(idAnnul);
                //Check annulment status
                if (response.Status == (int)TransactionStatus.Pending ||
                    response.Status == (int)TransactionStatus.Canceled)
                {
                    Console.WriteLine((response as StatusResponseModel).Message);
                }
                else
                {
                    //if annulment was approbed cast the object ResultResponseModel and access to transaction a annulled
                    var result = (ResultResponseModel)response;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Estado de la anulacion {((TransactionStatus)result.Status)}");
                    Console.WriteLine($"Estado de la transaccion a anular {result.Message.TransactionStatus}");
                    Console.WriteLine($"Codigo de autorizacion {result.Message.AuthorizationCode}");
                    Console.WriteLine($"Phone Number {result.Message.PhoneNumber}");
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
                    GetAnnulmentStatus(AnnulmentId);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
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
        public static void Reimbursemet(string idTx)
        {
            try
            {
                //Send reimbursement by transaction id request
                var response = connection.DoReimbursement(Convert.ToInt64(idTx));
                //save the reimbursement id returned
                _reimbursmentId = response.ReimbursementId;
                Console.WriteLine($"Identificador de reverso {_reimbursmentId}");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Reimbursemet(idTx);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
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
        public static void ReimbursemetByClientTransactionId()
        {
            try
            {
                //Send reimbursement by client transaction id request
                var response = connection.DoReimbursementByClientId(_extTxId);
                //save the reimbursement id returned
                _reimbursmentId = response.ReimbursementId;
                Console.WriteLine($"Identificador de reverso {_reimbursmentId}");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    ReimbursemetByClientTransactionId();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.StatusCode } - {e.ErrorList.ElementAt(0).Message}");
                Console.ResetColor();
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
        /// Get reimbursement status 
        /// </summary>
        public static void ReimbursemetStatus()
        {
            try
            {
                //Get reimbursement status by reimbursement id obteined whe request a reimbursement
                var response = connection.GetReimbursementById(_reimbursmentId);

                //check for status of reimbursement
                if (response.Status == (int)TransactionStatus.Pending || response.Status == (int)TransactionStatus.Canceled)
                {
                    Console.WriteLine((response as StatusResponseModel).Message);
                }
                else
                {
                    //if reimbursement was approved make a cast to ResultResponseModel to get transaction refunded 
                    var result = (ResultResponseModel)response;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Estado del reverso {((TransactionStatus)result.Status)}");
                    Console.WriteLine($"Estado de la transaccion a anular {result.Message.TransactionStatus}");
                    Console.WriteLine($"Codigo de autorizacion {result.Message.AuthorizationCode}");
                    Console.WriteLine($"Phone Number {result.Message.PhoneNumber}");
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
                    ReimbursemetStatus();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        public static void GetStatusByClientTransactionId()
        {
            try
            {
                //Request for transaction status by client transaction id
                var response = connection.GetTransactionByClientId(_extTxId);
                //Check status of transaction
                if (response.Status == (int)TransactionStatus.Pending)
                {
                    Console.WriteLine((response as StatusResponseModel).Message);
                }
                else
                {
                    //if transaction was approved or canceled make a cast to ResultResponseModel 
                    //and access to transaction result
                    var tx = (ResultResponseModel)response;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Codigo autorizacion {tx.Message.AuthorizationCode}");
                    Console.WriteLine($"Mensaje {tx.Message.Message}");
                    Console.WriteLine($"Telefono {tx.Message.PhoneNumber}");
                    Console.WriteLine($"Bin {tx.Message.Bin}");
                    Console.WriteLine($"Deferred {tx.Message.Deferred}");
                    Console.WriteLine($"Deferred Message {tx.Message.DeferredMessage}");
                    Console.WriteLine($"Amount {tx.Message.Amount}");
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
                    GetStatusByClientTransactionId();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        /// Send a custom bill which will be shown to the user
        /// </summary>
        public static void SendCustomBill()
        {
            try
            {
                //Create the custom bill
                var bill = new CustomBillRequestModel()
                {
                    //Transaction id
                    TransactionId = Id,
                    //Your document number correspond to your bill identifier
                    DocumentNumber = "number12",
                    //Name of the company issuing the bill
                    CompanyName = "Empresa electrica quito",
                    //Name of owner bill
                    DocumentOwner = "Eduardo Gomez",
                };
                //Send request for save a custom bill
                var response = connection.SendCustomBill(bill);
                if (response)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Invoice was received");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The invoice was not received");
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
                    SendCustomBill();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        public static void Cancel()
        {
            try
            {
                //Send request for cancel transaction using transaction id
                var response = connection.Cancel(Id);
                Console.WriteLine(response ? "Transaccion cancelada" : "La transaccion no pudo ser cancelada");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Cancel();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        /// Cancel or reimbursement the transaction 
        /// if transaction was approved then refund if not the transaction will cancel
        /// </summary>
        public static void CancelAndReimbursement()
        {
            try
            {
                //Request to CancelAndReimbursement
                var response = connection.CancelAndReimbursement(_extTxId);
                Console.WriteLine(response ? "Transaccion cancelada" : "La transaccion no pudo ser cancelada");
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                //Verify if error returned was Unhautorized and get new token
                if (RefreshToken(e))
                {
                    //Make the same call again after restore the invalid token for new one
                    Cancel();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
                ConfigurationManager.Instance.Token = response.Access_Token;
                ConfigurationManager.Instance.RefreshToken = response.Refresh_Token;

                Console.WriteLine("token " + response.Access_Token);
                Console.WriteLine("token " + response.Access_Token);
                Console.WriteLine("refresh token " + response.Refresh_Token);
                Console.ResetColor();
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
        /// In these case we use the refresh token intance of client secret
        /// </summary>
        /// <param name="ruc">RUC of company associated to application</param>
        public static void GetTokenByRefreshToken(string ruc)
        {
            try
            {
                //Calling to GetTokenByRefreshToken to retrive the new access token and refresh token
                var response = connection.GetTokenByRefreshToken(ruc);
                //Replace old tokens by new one
                ConfigurationManager.Instance.Token = response.Access_Token;
                ConfigurationManager.Instance.RefreshToken = response.Refresh_Token;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("token " + response.Access_Token);
                Console.WriteLine("refresh token " + response.Refresh_Token);
                Console.ResetColor();
            }
            catch (PayPhoneWebException e)
            {
                //All call make to PayPhone need to be inside a try catch
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();
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
                var request = new UserDataRequestModel()
                {
                    ChargeByNickName = byNickName,
                    CountryCode = regionCode
                };
                if (byNickName)
                {
                    request.NickName = identifier;
                }
                else
                {
                    request.PhoneNumber = identifier;
                }

                var response = connection.GetUserData(request);
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();

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
        /// Get currencies availables for these application
        /// </summary>
        public static void GetCurrency()
        {
            try
            {
                //Get currencies
                var response = connection.GetCurrencies();
                foreach (var currency in response)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"{currency.Name } - {currency.CatId}");
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
                    GetCurrency();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.ErrorList.ElementAt(0).Message);
                Console.ResetColor();

            }
            catch (Exception e)
            {
                //Always is recomended have generic exception for possibles errors generate from your code
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception " + e.Message);
                Console.ResetColor();
            }
        }
    }
}
