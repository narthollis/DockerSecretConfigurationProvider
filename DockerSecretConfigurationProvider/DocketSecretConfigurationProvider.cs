using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace DockerSecretConfigurationProvider
{
    public class DocketSecretConfigurationProvider : IConfigurationProvider
    {
        private const string WindowsSecretPath = @"C:\ProgramData\Docker\secrets\";
        private const string UnixSecretPath = "/run/docker/secrets/";

        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        protected List<string> Keys = new List<string>();

        private readonly string _prefix;
        private readonly string _path;

        public DocketSecretConfigurationProvider() : this(string.Empty, GetBaseDirectoryByOperatingSystem())
        { }

        public DocketSecretConfigurationProvider(string prefix) : this(prefix, GetBaseDirectoryByOperatingSystem())
        { }

        public DocketSecretConfigurationProvider(string prefix, string path)
        {
            _prefix = prefix;
            _path = path;
        }

        private static string GetBaseDirectoryByOperatingSystem()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WindowsSecretPath : UnixSecretPath;
        }

        public bool TryGet(string key, out string value)
        {
            if (Keys.Contains(key))
            {
                value = File.ReadLines(Path.Combine(_path, $"{_prefix}{key}")).First();
                return true;
            }

            value = null;
            return false;
        }

        public void Set(string key, string value)
        {
            // Given that this is a secrets provider and reads the file, every try get, I don't think implmenting a Set is a great idea
            // I am also not sure of the reprocutions, hence I am leaving it as a throw, rather than silently failing
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        public void Load()
        {
            Keys = Directory.EnumerateFiles(_path, $"{_prefix}*").Select(Path.GetFileName).ToList();
        }

        // Copied from https://github.com/aspnet/Configuration/blob/master/src/Microsoft.Extensions.Configuration/ConfigurationProvider.cs#L66
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return Keys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(k => Segment(k, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        // Copied from https://github.com/aspnet/Configuration/blob/master/src/Microsoft.Extensions.Configuration/ConfigurationProvider.cs#L79
        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        protected void OnReload()
        {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }
    }
}
