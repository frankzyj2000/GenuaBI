﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GenuinaBI.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace GenuinaBI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
