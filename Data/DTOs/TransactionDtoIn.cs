using Microsoft.AspNetCore.Mvc;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Data.DTOs
{
    public partial class TransactionDtoIn
    {
        public int Id { get ; set; }
        public int AccountId { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int? ExternalAccount { get; set; }

    }
}