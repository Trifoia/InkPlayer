using MudBlazor;
using Oqtane.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trifoia.Module.InkPlayer
{
    internal class SettingsViewModel
    {
        public SettingsViewModel(ISettingService settingService, Dictionary<string, string> moduleSettings)
        {
            Value = settingService.GetSetting(moduleSettings, nameof(Value), Value);
        }

        public string Value { get; set; } = string.Empty;
   
        public void SetSettings(ISettingService settingService, Dictionary<string, string> moduleSettings) {

            settingService.SetSetting(moduleSettings, nameof(Value), Value);
       
        }
    }
}
