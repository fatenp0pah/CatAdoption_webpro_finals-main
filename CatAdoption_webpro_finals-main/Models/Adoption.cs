using System;

namespace CatAdoption.Models
{
    public class Adoption
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Cat Cat { get; set; } = new Cat();
        public virtual User User { get; set; } = new User();
    }
}
