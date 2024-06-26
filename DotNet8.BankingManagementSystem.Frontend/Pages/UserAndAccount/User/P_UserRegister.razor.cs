﻿namespace DotNet8.BankingManagementSystem.Frontend.Pages.UserAndAccount.User;

public partial class P_UserRegister : ComponentBase
{
    private UserRequestModel _model = new();
    private StateListResponseModel? _stateListResponseModel;
    private TownshipListResponceModel? _townshipListResponceModel;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _stateListResponseModel = await ApiService.GetStates();
            StateHasChanged();
        }
    }

    private async Task OnValidSubmit()
    {
        try
        {
            var response = await ApiService.CreateUser(_model);
            await InjectService.Go("/user-and-account/user");
            await InjectService.SuccessMessage("Registration Successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task ChangeState(string stateCode)
    {
        _model.StateCode = stateCode;
        _townshipListResponceModel = await ApiService.GetTownShipByStateCode(_model.StateCode);
        StateHasChanged();
    }
}