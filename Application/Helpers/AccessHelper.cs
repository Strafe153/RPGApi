using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Helpers;

public class AccessHelper : IAccessHelper
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AccessHelper(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public void VerifyAccessRights(Player player)
	{
		var context = _httpContextAccessor.HttpContext;
		var claims = context.User.Claims;

		var performerName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
		var performerRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

		if (player.Name != performerName && performerRole != nameof(PlayerRole.Admin))
		{
			throw new NotEnoughRightsException("Not enough rights to perform the operation");
		}
	}
}