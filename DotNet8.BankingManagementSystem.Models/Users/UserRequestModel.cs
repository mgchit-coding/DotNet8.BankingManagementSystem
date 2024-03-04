﻿namespace DotNet8.BankingManagementSystem.Models.Users;

public class UserRequestModel
{
    public string UserCode { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string MobileNo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Nrc { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string StateCode { get; set; }=null!;
    public string TownshipCode { get; set; } = null!;

}