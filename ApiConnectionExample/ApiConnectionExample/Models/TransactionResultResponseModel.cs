using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    public class TransactionResultResponseModel
    {
        public string Email { get; set; }
        public string CardType { get; set; }
        public string ClientUserId { get; set; }
        public string Processor { get; set; }
        public string Bin { get; set; }
        public string DeferredCode { get; set; }
        public string DeferredMessage { get; set; }
        public bool Deferred { get; set; }
        public string CardBrandCode { get; set; }
        public string CardBrand { get; set; }
        public int Amount { get; set; }
        public string ClientTransactionId { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusCode { get; set; }
        public string TransactionStatus { get; set; }
        public string AuthorizationCode { get; set; }
        public string Message { get; set; }
        public int MessageCode { get; set; }
        public long TransactionId { get; set; }
        public string Document { get; set; }
        public string Currency { get; set; }
        public string OptionalParameter1 { get; set; }
        public string OptionalParameter2 { get; set; }
        public string OptionalParameter3 { get; set; }
        public List<TaxModel> Taxes { get; set; }
    }

    public class TaxModel {
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Value { get; set; }
        public int Tax { get; set; }
    }
}
