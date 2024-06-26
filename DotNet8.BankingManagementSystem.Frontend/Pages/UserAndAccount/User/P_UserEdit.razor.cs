﻿namespace DotNet8.BankingManagementSystem.Frontend.Pages.UserAndAccount.User;

public partial class P_UserEdit : ComponentBase
{
    [Parameter] public string userCode { get; set; }
    private StateListResponseModel? _stateListResponseModel;
    private TownshipListResponceModel? _townshipListResponceModel;
    private UserModel _model = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _stateListResponseModel = await ApiService.GetStates();
            await GetUserByCode(userCode);
            StateHasChanged();
        }
    }

    private async Task GetUserByCode(string userCode)
    {
        await InjectService.EnableLoading();
        var result = await ApiService.GetUserByCode(userCode);
        if (result.Response.IsError)
        {
            //
            return;
        }

        _model = result.Data;
        StateHasChanged();
        await InjectService.DisableLoading();
    }

    private async Task ChangeState(string stateCode)
    {
        _model.StateCode = stateCode;
        _townshipListResponceModel = await ApiService.GetTownShipByStateCode(_model.StateCode);
        StateHasChanged();
    }

    private async Task OnValidSubmit()
    {
        var reqModel = new UserRequestModel
        {
            UserName = _model.UserName,
            UserCode = _model.UserCode,
            FullName = _model.FullName,
            MobileNo = _model.MobileNo,
            Email = _model.Email,
            Nrc = _model.Nrc,
            Address = _model.Address,
            StateCode = _model.StateCode,
            TownshipCode = _model.TownshipCode
        };

        var response = await ApiService.UpdateUser(reqModel);
        await InjectService.Go("/general-setup/user");
        await InjectService.SuccessMessage("Updating Successful.");
        StateHasChanged();
    }
}