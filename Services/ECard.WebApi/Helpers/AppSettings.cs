using Project.Framework.Configuration;

namespace ECard.WebApi.Helpers
{
    public class AppSettings : BaseAppSetting
    {
        public string Secret { get; set; }
        public string ApplicationName { get; set; }
        public int Version { get; set; }
    }
}
