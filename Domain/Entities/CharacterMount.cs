namespace Domain.Entities;

public class CharacterMount
{
	public int CharacterId { get; set; }
	public Character Character { get; set; } = default!;

	public int MountId { get; set; }
	public Mount Mount { get; set; } = default!;
}
