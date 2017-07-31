using Microsoft.Extensions.Configuration;

namespace DockerSecretConfigurationProvider
{
    public class DockerSecretConfigurationSource : IConfigurationSource
    {
        public string Prefix { get; set; }
        public string Path { get; set; }
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DocketSecretConfigurationProvider(Prefix, Path);
        }
    }
}