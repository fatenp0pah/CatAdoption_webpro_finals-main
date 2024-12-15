using System;
using System.Collections.Generic;

namespace CatAdoption.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }= string.Empty;

        public string Password { get; set; }= string.Empty;

        public string Email { get; set; }= string.Empty;

        public string Role { get; set; } = "Customer"; // Default role
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Adoption> Adoptions { get; set; }= new List<Adoption>();
    }
}
