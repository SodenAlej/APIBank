using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using BankAPI.Data.DTOs;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Controllers;

[Authorize(Policy = "Client")]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{   
    private readonly AccountService accountService;
    private readonly TransactionService transactionService;
    public TransactionController(TransactionService transaction,
                            AccountService accountService)
    {
        transactionService = transaction;
        this.accountService = accountService;
    }

     [HttpGet("consulteAccount/{id}")]
     public async Task<IEnumerable<TransactionDtoOut?>?> Get(int id, TransactionDtoIn transaction)
     { 
        if (id != transaction.AccountId){  
            return null;
        }
        var account = await transactionService.GetDtoById(id);

        return account;
     }

    [HttpPost("deposito/{id}")]
    public async Task<IActionResult> Deposito(TransactionDtoIn transaction, int id)
    {
        if( id != transaction.AccountId)
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({transaction.Id}) del cuerpo de la solicitud"});

        if ( 1 != transaction.TransactionType)
            return BadRequest(new { message = $"El tipo de transaccion deseada no concuerda con el tipo de transaccion de la solicitud({transaction.TransactionType}) -- " +
                "1) Deposito en efectivo"});
        
        if ( transaction.ExternalAccount != null )
            return BadRequest(new { message = "No puede tener cuenta asociada. Ya que es deposito en efectivo"});

        decimal newBalance =  await accountService.ValidarDeposito(transaction);

        await accountService.UpdateBalance(transaction, newBalance);
         await transactionService.Create(transaction);
        return Ok(); 
    }

    [HttpPost("retiro/{id}")]
    public async Task<IActionResult> Retiro(TransactionDtoIn transaction, int id)
    {
        if( id != transaction.AccountId)
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({transaction.AccountId}) del cuerpo de la solicitud"});
            
        if ( 2 != transaction.TransactionType && 4 != transaction.TransactionType)
            return BadRequest(new { message = $"El tipo de transaccion deseada no concuerda con el tipo de transaccion de la solicitud({transaction.TransactionType}) -- " +
                "2) Retiro en efectivo  -- 4) Retiro via transferencia"});
        
        if( 4 == transaction.TransactionType && transaction.ExternalAccount == null)
            return BadRequest(new { message = "Para los retiros via transferencia es necesario especificar una cuenta, ya sea interna o externa"});
        
        decimal newBalance = await accountService.ValidarRetiro(transaction);

        if ( newBalance < 0 )
            return BadRequest( new { message = "No cuenta con suficiente dinero en la cuenta"});

        await accountService.UpdateBalance(transaction, newBalance);
        await transactionService.Create(transaction);
        return Ok(); 
    }
    
    [HttpDelete("deleteAccount/{id}")]
    public async Task<IActionResult> DeleteAccount(TransactionDtoIn transaction, int id)
    {
        if( id != transaction.AccountId)
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({transaction.AccountId}) del cuerpo de la solicitud"});
        
        var accountToDelete = await accountService.GetById(id);

        if(accountToDelete is not null)
        {
            if (accountToDelete.Balance == 0)
            {
                await accountService.Delete(transaction.AccountId);
                return Ok(new { message = "Las cuenta se elimino correctamente."});
            }
            else
            {
                return BadRequest( new { message= "El Balance de la cuenta no es 0."});
            }
        }
        else
        {
            return AccountNotFound(id);
        }

    }
    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"El cuenta con ID = {id} no existe."});
    }
}