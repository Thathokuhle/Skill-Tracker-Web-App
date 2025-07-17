using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SkillTrackerApp.Pages
{
    public class IndexModel : PageModel
    {

        public IndexModel(ILogger<IndexModel> logger)
        {

        }

        public void OnGetAsync()
        {
            Page();
        }
    }
}