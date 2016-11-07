namespace Yolo.Dog.Website.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

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
