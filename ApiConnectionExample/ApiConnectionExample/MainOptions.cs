using System;

namespace ApiConnectionExample
{
    public static class MainOptions
    {
        public static bool NewTransaction { get; set; }
        public static void Options()
        {
            NewTransaction = false;
            //Set descriptions and options to start using the example
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("This is PayPhone example that show how extensible and flexible is use his API.");
            Console.WriteLine("To start using this example, select any option shown below");
            Console.ResetColor();

            //Options for use this api
            Console.WriteLine("\t 1) For set and send transaction");
            Console.WriteLine("\t 2) For get token");
            Console.WriteLine("\t 3) For get token by refresh token");
            Console.WriteLine("\t 4) For set and send transaction by NickName");
            Console.WriteLine("\t 5) For set transaction by NickName");
            Console.WriteLine("\t 6) For annulment transaction by id");
            Console.WriteLine("\t 7) For get annulment status");
            Console.WriteLine("\t 8) For get transaction status");
            Console.WriteLine("\t 9) For get user data by phone number");
            Console.WriteLine("\t 0) For get user data by nickname");
            Console.WriteLine("\t A) For get currency");

            ConsoleKeyInfo key;
            bool condition = false;

            do
            {
                Console.Write("Select one option: ");
                key = Console.ReadKey(false);
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        //Get regions
                        ServiceLayer.GetRegions();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Region code of phone number is listed next to the name of each country");
                        Console.ResetColor();
                        Console.Write("Enter the region code above: ");
                        var regionCode = Console.ReadLine();
                        //Asigno la transaccion
                        Console.Write("Insert your phone number: ");
                        var phoneNumber = Console.ReadLine();
                        condition = ServiceLayer.Sale(phoneNumber, regionCode);

                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        //Request the company RUC for get token
                        Console.Write("Insert RUC: ");
                        var ruc = Console.ReadLine();

                        //Call to method GetToken with ruc inserted
                        ServiceLayer.GetToken(ruc);
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        //Request the company RUC for get token
                        Console.Write("Insert RUC: ");
                        var ruc1 = Console.ReadLine();

                        //Call to method GetTokenByRefreshToken with ruc inserted
                        ServiceLayer.GetTokenByRefreshToken(ruc1);
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        //Get regions
                        ServiceLayer.GetRegions();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Region code of phone number is listed next to the name of each country");
                        Console.ResetColor();
                        Console.Write("Enter the region code above: ");
                        regionCode = Console.ReadLine();
                        //Set the transaction with document id
                        Console.Write("Insert your document id: ");
                        var nickname = Console.ReadLine();
                        condition = ServiceLayer.SaleByNickName(nickname, regionCode);
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        string idTx;
                        //Annulment one transaction by id
                        do
                        {
                            Console.Write("Insert transaction id for annulment: ");
                            idTx = Console.ReadLine();

                        } while (string.IsNullOrEmpty(idTx));

                        //Send annulment request
                        ServiceLayer.Annulment(long.Parse(idTx));
                        break;
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        //Get annulment status by annulmet id
                        do
                        {
                            Console.Write("Insert annulment id for get status: ");
                            idTx = Console.ReadLine();

                        } while (string.IsNullOrEmpty(idTx));

                        //Request the annulment status
                        ServiceLayer.GetAnnulmentStatus(long.Parse(idTx));
                        break;
                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        //Get transaction status by id
                        do
                        {
                            Console.Write("Get status by transaction id: ");
                            idTx = Console.ReadLine();

                        } while (string.IsNullOrEmpty(idTx));

                        //Request transaction status
                        ServiceLayer.GetStatusTransaction(long.Parse(idTx));
                        break;
                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                        ServiceLayer.GetRegions();
                        string phone;
                        //Insert phone number of user that we need get data
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Region code of phone number is listed next to the name of each country");
                            Console.ResetColor();
                            Console.Write("Enter the region code above: ");
                            regionCode = Console.ReadLine();
                            Console.Write("Insert phone number: ");
                            phone = Console.ReadLine();

                        } while (string.IsNullOrEmpty(phone));

                        //Request user data of the given phone number
                        ServiceLayer.GetUserData(phone, regionCode, false);
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        ServiceLayer.GetRegions();
                        string nickName;
                        //Get user data by user document id
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Region code of phone number is listed next to the name of each country");
                            Console.ResetColor();
                            Console.Write("Enter the region code above: ");
                            regionCode = Console.ReadLine();
                            Console.Write("Insert user document id: ");
                            nickName = Console.ReadLine();

                        } while (string.IsNullOrEmpty(nickName));

                        //Request user data
                        ServiceLayer.GetUserData(nickName, regionCode, true);
                        break;

                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.A:
                        ServiceLayer.GetCurrency();
                        break;
                }
                condition = !condition && ((key.Key != ConsoleKey.D1 && key.Key != ConsoleKey.NumPad1)
                                || (key.Key != ConsoleKey.D2 && key.Key != ConsoleKey.NumPad2)
                                || (key.Key != ConsoleKey.D3 && key.Key != ConsoleKey.NumPad3)
                                || (key.Key != ConsoleKey.Escape)
                                || (key.Key != ConsoleKey.D4 && key.Key != ConsoleKey.NumPad4)
                                || (key.Key != ConsoleKey.D5 && key.Key != ConsoleKey.NumPad5)
                                || (key.Key != ConsoleKey.D6 && key.Key != ConsoleKey.NumPad6)
                                || (key.Key != ConsoleKey.D7 && key.Key != ConsoleKey.NumPad7)
                                || (key.Key != ConsoleKey.D8 && key.Key != ConsoleKey.NumPad8)
                                || (key.Key != ConsoleKey.D9 && key.Key != ConsoleKey.NumPad9)
                                || (key.Key != ConsoleKey.D0 && key.Key != ConsoleKey.NumPad0)
                                || (key.Key != ConsoleKey.A));
            } while (condition);

            SecondaryOtions();

            if (NewTransaction)
            {
                Console.Clear();
                Options();
            }
        }

        public static void SecondaryOtions()
        {
            if (ServiceLayer.Id != 0)
            {
                //Add options to use this example
                Console.WriteLine("\t 1) For get status of transaction");
                //To use these options you must first obtain the status of the transaction and that it be approved
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\t To use these options you must firts press the option one for get transaction status");
                Console.WriteLine("\t 2) For annulment transaction");
                Console.WriteLine("\t 3) For get annulment status");
                Console.WriteLine("\t 4) For reimbursement transaction");
                Console.WriteLine("\t 5) For get reimbursement status");
                Console.WriteLine("\t 6) For reimbursement transaction by client id");
                Console.ResetColor();
                //End
                Console.WriteLine("\t 7) For transaction status by appId");
                Console.WriteLine("\t 8) For cancel transaction");
                Console.WriteLine("\t 9) For cancel and reimbursement transaction by client id");
                Console.WriteLine("\t Ctrl + N) For new transaction press Ctrl + N");

                ConsoleKeyInfo cky;


                do
                {
                    cky = Console.ReadKey(false);

                    switch (cky.Key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            //Get transaction status by transaction id
                            ServiceLayer.GetStatusTransaction(ServiceLayer.Id);
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            //Validate if transacction was approved
                            if (ServiceLayer.Approved)
                            {
                                //If transaction was approved i can annulment
                                ServiceLayer.Annulment(ServiceLayer.Id);
                            }
                            else
                            {
                                //otherwise show a message
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("The transaction must be approved");
                                Console.ResetColor();
                            }

                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            //Check if transaction was approved
                            if (ServiceLayer.Approved)
                            {
                                //Get annulment status by annulment id
                                ServiceLayer.GetAnnulmentStatus(ServiceLayer.AnnulmentId);
                            }
                            else
                            {
                                //Otherwise show message
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("The transaction must be approved");
                                Console.ResetColor();
                            }
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            //Get transaction id for refund
                            Console.Write("Enter transaction ID for refund: ");
                            var idTx = Console.ReadLine();
                            //Send refund
                            ServiceLayer.Reimbursemet(idTx);
                            break;
                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            //Check if transaction was approved
                            if (ServiceLayer.Approved)
                            {
                                //Get reimbursement status
                                ServiceLayer.ReimbursemetStatus();
                            }
                            else
                            {
                                //Otherwise show a message
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("The transaction must be approved");
                                Console.ResetColor();
                            }
                            break;
                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:
                            //Request reimbursement by client transaction id
                            ServiceLayer.ReimbursemetByClientTransactionId();
                            break;
                        case ConsoleKey.D7:
                        case ConsoleKey.NumPad7:
                            //Get status of transaction by client transaction id
                            ServiceLayer.GetStatusByClientTransactionId();
                            break;
                        case ConsoleKey.D8:
                        case ConsoleKey.NumPad8:
                            //Request Cancel transaction
                            ServiceLayer.Cancel();
                            break;
                        case ConsoleKey.D9:
                        case ConsoleKey.NumPad9:
                            //Request cancel or reimbursement the transaction
                            //If the transaction was approved it is reversed but canceled
                            ServiceLayer.CancelAndReimbursement();
                            break;
                        case ConsoleKey.Escape:
                            Environment.Exit(0);
                            break;
                        case ConsoleKey.N:
                            if ((cky.Modifiers & ConsoleModifiers.Control) != 0)
                            {
                                NewTransaction = true;
                            }
                            break;


                    }


                } while (cky.Key != ConsoleKey.Escape && !NewTransaction);


            }
        }
    }
}
