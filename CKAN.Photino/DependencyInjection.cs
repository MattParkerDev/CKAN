using CKAN.CmdLine;
using CKAN.Games.KerbalSpaceProgram;
using Microsoft.Extensions.DependencyInjection;

namespace CKAN.Photino;

public static class DependencyInjection
{
    public static IServiceCollection AddKsp(this IServiceCollection services)
    {
        var user = new ConsoleUser(true);
        var game = new KerbalSpaceProgram();
        var gameInstance = new GameInstance(game, @"C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program", "KSP", user);
        var repositoryDataManager = new RepositoryDataManager();
        var registryManager = RegistryManager.Instance(gameInstance, repositoryDataManager);
        
        services.AddSingleton<RegistryManager>(registryManager);
        services.AddSingleton<RepositoryDataManager>();
        return services;
    }
}