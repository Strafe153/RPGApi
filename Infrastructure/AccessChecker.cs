using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure;

public class AccessChecker(IHttpContextAccessor httpContextAccessor) : IAccessChecker
{
	public void VerifyAccessRights(Player player)
	{
		var claims = httpContextAccessor.HttpContext.User.Claims;

		var performerName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
		var performerRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

		if (player.Name != performerName && performerRole != nameof(PlayerRole.Admin))
		{
			throw new NotEnoughRightsException("Not enough rights to perform the operation");
		}
	}
}