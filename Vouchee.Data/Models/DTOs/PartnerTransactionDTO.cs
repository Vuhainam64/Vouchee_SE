using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.DTOs
{
    public class SePayTransactionDTO
    {
        public int id { get; set; }

        public string? gateway { get; set; }
        public DateTime? transactionDate { get; set; }
        public string? accountNumber { get; set; }
        public string? code { get; set; }
        public string? content { get; set; }
        public string? transferType { get; set; }
        public int? transferAmount { get; set; }
        public int? accumulated { get; set; }
        public string? subAccount { get; set; }
        public string? referenceCode { get; set; }
        public string? description { get; set; }
    }

    public class CreateSePayPartnerInTransactionDTO : SePayTransactionDTO
    {
        public DateTime createdAt = DateTime.Now;
    }
}
