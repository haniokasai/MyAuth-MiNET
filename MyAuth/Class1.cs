﻿using log4net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{

    [Plugin(PluginName = "MyAuth", Description = "Mysql Auth Plugin for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        Dictionary<string, Boolean> lged = new Dictionary<string, Boolean>();
        Dictionary<string, Boolean> prerg = new Dictionary<string, Boolean>();
        Dictionary<string, int> ct = new Dictionary<string, int>();

        protected static ILog _log = LogManager.GetLogger("MyAuth");

        private PluginContext _context;

        [DataContract]
        public class MyPluginConfiguration
        {
            [DataMember]
            public string ip { get; set; }

            [DataMember]
            public List<string> Rules { get; set; }
        }

        protected override void OnEnable()
        {

            _log.Warn("Loaded");
            mysql.load();
        }
    }
}