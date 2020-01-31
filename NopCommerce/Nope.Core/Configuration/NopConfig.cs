using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nope.Core.Configuration
{
    public partial class NopConfig : IConfigurationSectionHandler
    {
        public bool IgnoreStartupTask { get; set; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new NopConfig();

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTask = Convert.ToBoolean(attribute.Value);
            }
            return config;
        }
    }
}
