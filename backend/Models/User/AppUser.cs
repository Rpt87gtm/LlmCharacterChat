using llmChat.Models.Chat;
using Microsoft.AspNetCore.Identity;

namespace api.Models.User
{
    public class AppUser : IdentityUser
    {
        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
