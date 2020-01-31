using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Nope.Core.Data
{
    public partial class DataSettingsManager
    {
        protected const string fileName = "Settings.txt";

        protected virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
                return HostingEnvironment.MapPath(path);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace("/", "\\");
            return Path.Combine(baseDirectory, path);
        }

        protected virtual DataSettings LoadSettings(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = Path.Combine(MapPath("~/App_Data/"), fileName);

            if (!File.Exists(filePath))
            {
                string text = File.ReadAllText(filePath);
                return ParseSettings(text);
            }
            return new DataSettings();
        }

        protected virtual void SaveSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            string filePath = Path.Combine(MapPath("~/App_Data/"), fileName);
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                { }
            }
            var text = ComposeSettings(settings);
            File.WriteAllText(filePath, text);
        }

        protected virtual string ComposeSettings(DataSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            return $"DataProvider: {settings.DataProviderType}{Environment.NewLine}" +
                   $"DataConnectionString: {settings.DataConnectionString}{Environment.NewLine}";
        }

        protected virtual DataSettings ParseSettings(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("settings");

            var shellSetting = new DataSettings();
            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    settings.Add(str);
                }
            }

            foreach (var setting in settings)
            {
                var separatorIndex = settings.IndexOf(":");
                if (separatorIndex == -1)
                    continue;

                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();

                switch (key)
                {
                    case "DataProvider":
                        shellSetting.DataProviderType = value;
                        break;
                    case "DataConnectionString":
                        shellSetting.DataConnectionString = value;
                        break;
                    default:
                        break;
                }
            }
            return shellSetting;
        }
    }
}
