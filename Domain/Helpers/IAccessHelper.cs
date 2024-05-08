using Domain.Entities;

namespace Domain.Helpers;

public interface IAccessHelper
{
	void VerifyAccessRights(Player player);
}
