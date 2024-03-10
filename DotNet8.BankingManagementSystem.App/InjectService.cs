﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DotNet8.BankingManagementSystem.App
{
    public class InjectService 
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly NavigationManager _navigationManager;
        public InjectService(IJSRuntime jSRuntime, NavigationManager navigationManager)
        {
            _jSRuntime = jSRuntime;
            _navigationManager = navigationManager;
        }

        public async Task InvokeVoidAsync(string identifier, params object[] args)
        {
            await _jSRuntime.InvokeVoidAsync(identifier, args);
        }

        public async Task EnableLoading()
        {
            await _jSRuntime.InvokeVoidAsync("enableLoading", true);
        }

        public async Task DisableLoading()
        {
            await _jSRuntime.InvokeVoidAsync("enableLoading", false);
        }

        public async Task IntervalLoading()
        {
            await _jSRuntime.InvokeVoidAsync("intervalLoading", true);
        }

        public async Task SuccessMessage(string message)
        {
            await _jSRuntime.InvokeVoidAsync("successMessage",message);
        } 
        
        public async Task ErrorMessage(string message)
        {
            await _jSRuntime.InvokeVoidAsync("errorMessage", message);
        }

        public async Task IsConfirmed()
        {
            await _jSRuntime.InvokeVoidAsync("isConfirmed");
        }
        public async Task Go(string url)
        {
            _navigationManager.NavigateTo(url);
        }
    }
}
