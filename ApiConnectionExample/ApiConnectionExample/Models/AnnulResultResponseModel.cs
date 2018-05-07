using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiConnectionExample.Models
{
    public class AnnulResultResponseModel
    {
        public long Id { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string ClientTransactionId { get; set; }
        public string Message { get; set; }
        public SaleModel Sale { get; set; }
    }

    public class SaleModel
    {
        public long Id { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string ClientTransactionId { get; set; }
        public string Message { get; set; }
    }
}
