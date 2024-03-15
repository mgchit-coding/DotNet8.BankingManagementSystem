﻿using DotNet8.BankingManagementSystem.Database.EfAppDbContextModels;
using DotNet8.BankingManagementSystem.Mapper;
using DotNet8.BankingManagementSystem.Models;
using DotNet8.BankingManagementSystem.Models.Account;
using DotNet8.BankingManagementSystem.Models.TransactionHistory;
using DotNet8.BankingManagementSystem.Models.Transfer;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.BankingManagementSystem.BackendApi.Features.Transaction;

public class TransactionService
{
    private readonly AppDbContext _dbContext;

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region TransactionHistory

    public async Task<TransactionHistoryListResponseModel> TransactionHistory(int pageNo, int pageSize)
    {
        TransactionHistoryListResponseModel model = new TransactionHistoryListResponseModel();
        var query = _dbContext.TblTransactionHistories.AsNoTracking();
        var result = await query.OrderByDescending(x => x.TransactionDate)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        var count = await query.CountAsync();
        int pageCount = count / pageSize;
        if (count % pageSize > 0) pageCount++;
        var lst = result.Select(x => x.Change()).ToList();
        model = new TransactionHistoryListResponseModel()
        {
            Data = lst,
            PageSetting = new PageSettingModel(pageNo, pageSize, pageCount),
            Response = new MessageResponseModel(true, "Success")
        };
        return model;
    }

    #endregion

    #region Deposit

    public async Task<AccountResponseModel> Deposit(TransactionRequestModel requestModel)
    {
        var query = _dbContext.TblAccounts.AsNoTracking();
        var item = await query.FirstOrDefaultAsync(x => x.AccountNo == requestModel.AccountNo);
        if (item is null)
        {
            throw new Exception("Invalid Account");
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            //decimal newBalance = item.Balance + amount;
            //item.Balance = newBalance;
            item.Balance += requestModel.Amount;
            _dbContext.TblAccounts.Update(item);
            int result = await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Console.WriteLine(ex.Message);
        }

        AccountResponseModel model = new AccountResponseModel()
        {
            Data = item.Change(),
            Response = new MessageResponseModel(true, "Deposit Successfully.")
        };
        return model;
    }

    #endregion

    #region Withdrawl

    public async Task<AccountResponseModel> Withdraw(TransactionRequestModel requestModel)
    {
        var query = _dbContext.TblAccounts.AsNoTracking();
        var item = await query.FirstOrDefaultAsync(x => x.AccountNo == requestModel.AccountNo);
        if (item is null)
        {
            throw new Exception("Account is not found.");
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            //decimal newBalance = item.Balance - amount;
            //item.Balance = newBalance;
            item.Balance -= requestModel.Amount;
            _dbContext.TblAccounts.Update(item);
            int result = await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Console.WriteLine(ex.Message);
        }

        AccountResponseModel model = new AccountResponseModel()
        {
            Data = item.Change(),
            Response = new MessageResponseModel(true, "Deposit Successfully.")
        };
        return model;
    }

    #endregion

    #region Transfer

    public async Task<TransferResponseModel> Transfer(TransferModel requestModel)
    {
        TransferResponseModel model = new TransferResponseModel();
        var query = _dbContext.TblAccounts.AsNoTracking();
        var fromAccount = await query.FirstOrDefaultAsync(x => x.AccountNo == requestModel.FromAccountNo);
        if (fromAccount is null)
        {
            throw new Exception("Invalid From Account.");
        }

        var toAccount = await query.FirstOrDefaultAsync(x => x.AccountNo == requestModel.ToAccountNo);
        if (toAccount is null)
        {
            throw new Exception("Invalid To Account.");
        }

        if (fromAccount.Balance < requestModel.Amount)
        {
            model = new TransferResponseModel()
            {
                Response = new MessageResponseModel(false, "Insufficient Balance.")
            };
            goto result;
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            fromAccount.Balance -= requestModel.Amount;
            _dbContext.TblAccounts.Update(fromAccount);

            toAccount.Balance += requestModel.Amount;
            _dbContext.TblAccounts.Update(toAccount);

            //int result = await _dbContext.SaveChangesAsync();

            TblTransactionHistory creditTransactionHistory = new TblTransactionHistory()
            {
                Amount = requestModel.Amount,
                TransactionDate = DateTime.Now,
                FromAccountNo = fromAccount.AccountNo!,
                ToAccountNo = toAccount.AccountNo!,
                AdminUserCode = "Admin",
                TransactionType = "Credit"
            };

            TblTransactionHistory dedbitTransactionHistory = new TblTransactionHistory()
            {
                Amount = requestModel.Amount,
                TransactionDate = DateTime.Now,
                ToAccountNo = toAccount.AccountNo!,
                FromAccountNo = fromAccount.AccountNo!,
                AdminUserCode = "Admin",
                TransactionType = "Debit"
            };
            await _dbContext.AddRangeAsync(dedbitTransactionHistory, creditTransactionHistory);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }

        model = new TransferResponseModel
        {
            Response = new MessageResponseModel(true, "Balance transfer successful.")
        };

        result:
        return model;
    }

    #endregion

    #region Auto generate account

    public async Task<List<TblAccount>> GenerateAccounts(int count, int year)
    {
        Random random = new Random();
        List<TblAccount> model = new List<TblAccount>();

        for (int i = 0; i < count; i++)
        {
            string customerCode = GenerateCustomerCode();
            decimal balance = (decimal)(random.Next(10000000, 100000000));
            if (balance < 0) balance *= -1;
            int num = balance.ToString().Length == 8 ? 2 : 3;
            balance = Convert.ToDecimal(balance.ToString().Substring(0, num)) * 100000;

            TblAccount item = new TblAccount
            {
                CustomerCode = customerCode,
                Balance = balance
            };

            model.Add(item);
        }

        await _dbContext.TblAccounts.AddRangeAsync(model);
        await _dbContext.SaveChangesAsync();

        List<TblTransactionHistory> transactions = new List<TblTransactionHistory>();
        foreach (var item in model)
        {
            for (DateTime date = new DateTime(DateTime.Now.Year - year, 1, 1);
                 date < DateTime.Now;
                 date = date.AddDays(1))
            {
                string GetDifferentAccountNo(int currentAccountId)
                {
                    Random random = new Random();
                    int randomNumber;
                    do
                    {
                        randomNumber = random.Next(1, 100);
                    } while (randomNumber == currentAccountId);

                    return randomNumber.ToString("D6");
                }

                var fromAccountNo = GetDifferentAccountNo(item.AccountId);
                var toAccountNo = (Convert.ToInt32(fromAccountNo) + 1).ToString("D6");
                var amount = (decimal)random.NextDouble() * 1000000;
                TblTransactionHistory creditTransaction = new TblTransactionHistory
                {
                    FromAccountNo = fromAccountNo,
                    ToAccountNo = toAccountNo,
                    TransactionDate = date,
                    Amount = amount,
                    AdminUserCode = "Admin",
                    TransactionType = "Credit"
                };
                transactions.Add(creditTransaction);
                TblTransactionHistory debitTransaction = new TblTransactionHistory
                {
                    FromAccountNo = fromAccountNo,
                    ToAccountNo = toAccountNo,
                    TransactionDate = date,
                    Amount = amount,
                    AdminUserCode = "Admin",
                    TransactionType = "Debit"
                };
                transactions.Add(debitTransaction);
            }
        }

        await _dbContext.TblTransactionHistories.AddRangeAsync(transactions);
        await _dbContext.SaveChangesAsync();
        return model;
    }

    private string GenerateCustomerCode()
    {
        string randomNumber = new Random().Next(1000000, 9999999).ToString();
        return "C" + randomNumber;
    }

    #endregion
}