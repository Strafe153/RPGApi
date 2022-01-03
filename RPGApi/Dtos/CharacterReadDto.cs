using RPGApi.Data;

namespace RPGApi.Dtos
{
    public class CharacterReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CharacterRace Race { get; set; }
        public int Health { get; set; }

        public Guid PlayerId { get; set; }
    }
}
