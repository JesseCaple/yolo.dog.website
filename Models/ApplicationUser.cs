using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace yolo.dog.website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser InvitedBy { get; set; }
        public int InvitesClaimed { get; set; }
        public int InvitesAwarded { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }
    }
}
