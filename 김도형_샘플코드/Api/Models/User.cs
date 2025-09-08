using System;
using System.ComponentModel.DataAnnotations;

namespace DefenseGameWebServer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nickname { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}