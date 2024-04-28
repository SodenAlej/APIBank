using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BankAPI.Data.BankModels
{
    public partial class Client
    {
        public Client()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime RegDate { get; set; }
        [JsonIgnore]
        public string Pwd { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
