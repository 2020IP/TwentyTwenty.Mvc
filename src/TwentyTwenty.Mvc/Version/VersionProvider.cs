using System.Reflection;

namespace TwentyTwenty.Mvc.Version
{
    public interface IVersionProvider
    {
        string GetVersion();
    }

    public class VersionProvider : IVersionProvider
    {
        private string _version = null;
        public string Version => _version;

        public string GetVersion()
        {
            if (_version == null)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                
                _version = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                    ?? entryAssembly.GetName().Version.ToString();
            }
            return _version;
        }
    }
}