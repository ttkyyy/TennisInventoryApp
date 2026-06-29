using System;

namespace TennisInventoryApp.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisteredAt { get; set; }
        public decimal Rating { get; set; }
    }
}