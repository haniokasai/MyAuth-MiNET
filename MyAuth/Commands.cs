using MiNET;
using MiNET.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{
    class Commands : Class1
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        [Command(Name = "register", Description = "Register your account", Permission = "com.haniokasai.myauth.register")]
        public void register(Player player, string passwd)
        {
        }

        [Command(Name = "login", Description = "Login your account", Permission = "com.haniokasai.myauth.login")]
        public void login(Player player, string passwd)
        {
        }

        [Command(Name = "changepasswd", Description = "changepasswd your account", Permission = "com.haniokasai.myauth.changepasswd")]
        public void changepasswd(Player player, string oldpasswd, string newpasswd)
        {
        }

        [Command(Name = "unregister", Description = "UnRegister your account", Permission = "com.haniokasai.myauth.unregister")]
        public void unregister(Player player, string playername, string unregisterpasswd)
        {
        }
    }
}
