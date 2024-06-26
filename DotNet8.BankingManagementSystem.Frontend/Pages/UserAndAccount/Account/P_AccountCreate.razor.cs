﻿namespace DotNet8.BankingManagementSystem.Frontend.Pages.UserAndAccount.Account;

public partial class P_AccountCreate : ComponentBase
{
    private AccountRequestModel _model = new();
    private async Task OnValidSubmit()
    {
        try
        {
            var response = await ApiService.CreateAccount(_model);
            await InjectService.Go("/user-and-account/account");
            await InjectService.SuccessMessage("Creating Successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}