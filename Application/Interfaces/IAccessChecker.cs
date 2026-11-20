using Domain.Entities;

namespace Application.Interfaces;

public interface IAccessChecker
{
	void VerifyAccessRights(Player player);
}
