using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TubesWebsite.Models
{
    public class Invite
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public byte[] TokenHash { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }
    }
}
