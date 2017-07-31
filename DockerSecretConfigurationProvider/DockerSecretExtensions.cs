using Microsoft.Extensions.Configuration;

namespace DockerSecretConfigurationProvider
{
    public static class DockerSecretExtensions
    {
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new DockerSecretConfigrationSource());
            return configurationBuilder;
        }

        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder configurationBuilder, string prefix)
        {
            configurationBuilder.Add(new DockerSecretConfigrationSource { Prefix = prefix });
            return configurationBuilder;
        }

        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder configurationBuilder, string prefix, string path)
        {
            configurationBuilder.Add(new DockerSecretConfigrationSource { Prefix = prefix, Path = path });
            return configurationBuilder;
        }
    }
}