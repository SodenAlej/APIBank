namespace TestBankAPI.Data.DTOs;

public class TransactionDtoOut
{
    public int? ClientId { get; set; }
    public int Id { get; set; }
    public string AccountName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public decimal Balance { get; set; }
    public DateTime RegDate { get; set; }
}