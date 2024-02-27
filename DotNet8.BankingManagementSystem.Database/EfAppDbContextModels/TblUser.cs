﻿using System;
using System.Collections.Generic;

namespace DotNet8.BankingManagementSystem.Database.EfAppDbContextModels;

public partial class TblUser
{
    public int UserId { get; set; }

    public string UserCode { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string FullName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Nrc { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string StateCode { get; set; } = null!;

    public string TownshipCode { get; set; } = null!;
}
