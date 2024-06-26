﻿namespace DotNet8.BankingManagementSystem.Models;

public class PageSettingModel
{
    public PageSettingModel() { }
    public PageSettingModel(int pageNo, int pageSize, int pageCount)
    {
        PageNo = pageNo;
        PageSize = pageSize;
        PageCount = pageCount;
    }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
}