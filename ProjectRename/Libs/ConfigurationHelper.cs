using System;
using System.Collections.Generic;
using System.Configuration;

namespace MFGLib
{
	class ConfigurationHelper
	{
		public Configuration Config { get; set; }
		
		public ConfigurationHelper()
		{
			Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}		

		public string AppSettings(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return null;
			}

			KeyValueConfigurationElement kv = Config.AppSettings.Settings[key];
			if (kv == null)
			{
				return null;
			}

			return kv.Value;
		}

		public void AppSettings(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return;
			}

			KeyValueConfigurationElement kv = Config.AppSettings.Settings[key];

			if (string.IsNullOrWhiteSpace(value))
			{
				if (kv != null)
				{
					kv.Value = null;
				}
			}
			else
			{
				if (kv != null)
				{
					kv.Value = value;
				}
				else
				{
					Config.AppSettings.Settings.Add(key, value);
				}
			}			
		}

		public void Save()
		{
			Config.Save();
		}
	}
}
