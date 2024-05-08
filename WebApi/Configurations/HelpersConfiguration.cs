using Application.Helpers;
using Domain.Helpers;

namespace WebApi.Configurations;

public static class HelpersConfiguration
{
	public static void AddHelpers(this IServiceCollection services) =>
		services
			.AddSingleton<IAccessHelper, AccessHelper>()
			.AddSingleton<ICacheHelper, CacheHelper>()
			.AddSingleton<ITokenHelper, TokenHelper>();
}
