using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Worlds;

namespace MyAuth
{
    public class events : Plugin
    {

        protected override void OnEnable()
        {
            Class1._log.Info("Event Handler");
        }


    }
}
