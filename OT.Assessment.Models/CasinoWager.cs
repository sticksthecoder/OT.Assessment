using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Models
{
    public class CasinoWager
    {
        public string wagerId { get; set; }
        public string theme { get; set; }
        public string provider { get; set; }
        public string gameName { get; set; }
        public string transactionId { get; set; }
        public string brandId { get; set; }
        public string accountId { get; set; }
        public string Username { get; set; }
        public string externalReferenceId { get; set; }
        public string transactionTypeId { get; set; }
        public double amount { get; set; }
        public DateTime createdDateTime { get; set; }
        public int numberOfBets { get; set; }
        public string countryCode { get; set; }
        public string sessionData { get; set; }
        public long duration { get; set; }
    }
}
