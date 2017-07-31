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
    public class DocketSecretConfigurationProvider : ConfigurationProvider
    {
        private const string WindowsSecretPath = @"C:\ProgramData\Docker\secrets\";
        private const string UnixSecretPath = "/run/docker/secrets/";
        
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
        
        public override void Load()
        {
            Data = Directory.EnumerateFiles(_path, $"{_prefix}*")
                .ToDictionary(
                    f => Path.GetFileName(f).Substring(_prefix.Length),
                    f => File.ReadLines(f).First()
                );
        }
    }
}
