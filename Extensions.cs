namespace Yolo.Dog.Website
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public static class Extensions
    {
        public static async Task<T> GetCurrentUserAsync<T>(
            this UserManager<T> manager, HttpContext context)
            where T : IdentityUser
        {
            return await manager.GetUserAsync(context.User);
        }

        public static string TrimMultiline(this string str)
        {
            return Regex.Replace(str, @"^\s+", string.Empty);
        }
    }
}
