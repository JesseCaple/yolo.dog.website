namespace Yolo.Dog.Website.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser InvitedBy { get; set; }

        public int InvitesClaimed { get; set; }

        public int InvitesAwarded { get; set; }

        public virtual ICollection<Invite> Invites { get; set; }
    }
}
