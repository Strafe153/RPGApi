namespace RPGApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Character> Characters { get; set; }
    }
}
