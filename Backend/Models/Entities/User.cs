namespace Models.Entities
{
    public class User : Entity<int>
    {
        public string Name { get; set; } = default!;
        
    }
}
