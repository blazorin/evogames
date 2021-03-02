using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Client.Components.Bootstrap
{
    public class BootstrapServices : IAsyncDisposable
    {
        private const string Path = "./js/bootstrap-services.js";

        private IJSObjectReference _module;
        private readonly IJSRuntime _jsRuntime;

        public BootstrapServices(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowModalAsync(ElementReference modalWindowElementReference)
        {
            _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", Path);
            await _module.InvokeVoidAsync("bootstrapModal.show", modalWindowElementReference);
        }

        public async Task ShowToastAsync(ElementReference toastElementReference)
        {
            _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", Path);
            await _module.InvokeVoidAsync("bootstrapToast.show", toastElementReference);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await _module.DisposeAsync();
        }
    }
}