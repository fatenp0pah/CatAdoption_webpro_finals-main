namespace CatAdoption.Models
{
    public class Adopter
    {
        public int Id { get; set; }  // Primary Key (Auto-Incremented)
        public string username { get; set; }  // Adopter's Username
        public string catname { get; set; }  // Adopted Cat's Name (Updated property)
    }
}
