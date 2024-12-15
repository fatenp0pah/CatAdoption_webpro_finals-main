using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CatAdoption.Models
{
    public class Cat
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; 

        [Required]
        public string Breed { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Description { get; set; } = string.Empty;

        // ImageURL Property added
        [Url]
        public string ImageUrl { get; set; } = string.Empty; // Ensures the ImageURL is valid if provided.

        public bool AvailableForAdoption { get; set; } = true;

        // Navigation property for Adoptions relationship
        public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}
