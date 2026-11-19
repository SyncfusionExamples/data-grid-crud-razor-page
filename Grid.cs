using Microsoft.AspNetCore.Localization;

namespace Grid.CustomLocalization
{
    public class MyCustomRequestCultureProvider : RequestCultureProvider
    {
        
        public MyCustomRequestCultureProvider()
        {
        }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            // Simplified version of my code
            await Task.CompletedTask;
            return new ProviderCultureResult("fr-CH");
        }        
    }
}
