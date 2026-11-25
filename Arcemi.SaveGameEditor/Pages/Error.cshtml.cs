using Arcemi.SaveGameEditor.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Arcemi.SaveGameEditor.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string Report { get; private set; }

        public void OnGet() => BuildReport();
        public void OnPost() => BuildReport();

        private void BuildReport()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Report = error is null ? "The editor could not load this page."
                : EditorErrorReports.Describe(error.Error, "Load editor page");
        }
    }
}
