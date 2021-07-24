using Microsoft.AspNetCore.Components;

namespace Client.Pages.Account.Authentication
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject] protected NavigationManager NavManager { get; set; }

        protected override void OnInitialized()
        {
            var uri = NavManager.ToBaseRelativePath(NavManager.Uri);
            NavManager.NavigateTo("login?returnUrl=" + uri);
        }
    }
}