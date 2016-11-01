using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TubesWebsite.Models;

namespace TubesWebsite
{
    public static class Extensions
    {
        public async static Task<T> GetCurrentUserAsync<T>(
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
