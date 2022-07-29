using AuthenticationAndAuthorizationExercise.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthenticationAndAuthorizationExercise.Pages.Account
{
    public class LoginModel : PageModel
    {
        public const string MyCookie = "MyCookieAuthentication";

        [BindProperty]
        public Credential Credential { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //verify credentials
            if (Credential.UserName != "admin" || Credential.Password != "password")
            {
                return Page(); 
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "admin@gmail.com")
            };
            
            ClaimsIdentity identity = new(claims, "MyCookieAuthentication");
            ClaimsPrincipal claimsPrincipal = new(identity);

            await HttpContext.SignInAsync(MyCookie, claimsPrincipal);

            return RedirectToPage("/Index");
        }
    }
}
