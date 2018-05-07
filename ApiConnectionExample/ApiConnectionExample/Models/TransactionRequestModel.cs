using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    public class TransactionRequestModel
    {
        public string NickName { get; set; }
        public bool ChargeByNickName { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode{ get; set; }
        public int TimeZone { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string ClientUserId { get; set; }
        public string DeferredType { get; set; }
        public string Reference { get; set; }
        public int Amount { get; set; }
        public int AmountWithTax { get; set; }
        public int AmountWithoutTax { get; set; }
        public int Tax { get; set; }
        public int Service { get; set; }
        public int Tip { get; set; }
        public string ClientTransactionId { get; set; }
        public string StoreId { get; set; }
        public string TerminalId { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string OptionalParameter1 { get; set; }
        public string OptionalParameter2 { get; set; }
        public string OptionalParameter3 { get; set; }
    }
}
