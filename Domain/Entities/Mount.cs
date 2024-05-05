using Domain.Enums;

namespace Domain.Entities;

public class Mount
{
	public int Id { get; set; }
	public string Name { get; set; } = default!;
	public MountType Type { get; set; }
	public int Speed { get; set; }

	public ICollection<CharacterMount> CharacterMounts { get; set; } = new List<CharacterMount>();
}
