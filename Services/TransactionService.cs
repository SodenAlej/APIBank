using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Services;

public class TransactionService
{
    private readonly BankContext _context;

    public TransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<BankTransaction?> GetById(int id)
    {
        return await _context.BankTransactions.FindAsync(id);
    }

    public async Task<IEnumerable<TransactionDtoOut?>> GetDtoById(int id)
    {
        return await _context.Accounts.
        Where(a => a.ClientId == id).
        Select(a => new TransactionDtoOut
        {
            Id= a.Id,
            ClientId= a.ClientId,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }


    public async Task<BankTransaction> Create(TransactionDtoIn newTransactionDTO)
    {
        var newTransaction = new BankTransaction();

        newTransaction.AccountId = newTransactionDTO.AccountId;
        newTransaction.TransactionType = newTransactionDTO.TransactionType;
        newTransaction.Amount = newTransactionDTO.Amount;
        newTransaction.ExternalAccount = newTransactionDTO.ExternalAccount;

        _context.BankTransactions.Add(newTransaction);
       await _context.SaveChangesAsync();

        return newTransaction;
    }
    


    
}
