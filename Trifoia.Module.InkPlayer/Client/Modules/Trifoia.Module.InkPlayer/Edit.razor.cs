using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.Services;

using Trifoia.Module.InkPlayer.Services;


namespace Trifoia.Module.InkPlayer
{
    public partial class Edit: ModuleBase
    {
		[Inject] public InkPlayerService InkPlayerService { get; set; }
		[Inject] public NavigationManager NavigationManager { get; set; }
		[Inject] public IStringLocalizer<Edit> Localizer { get; set; }		
        [Inject] public ISettingService SettingService { get; set; }

        private MudForm mudform;
        private bool success = false;
        private SettingsViewModel _settingsVM;
        private bool IsLoaded = false;
        private Models.InkPlayer _item { get; set; } = new();
        private int _id;

		public override SecurityAccessLevel SecurityAccessLevel => SecurityAccessLevel.Edit;

		public override string Actions => "Add,Edit";

		public override string Title => "Manage InkPlayer";

        public override List<Resource> Resources => new List<Resource>()
        {
            new Resource { ResourceType = ResourceType.Stylesheet,  Url = "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" },
            new Resource { ResourceType = ResourceType.Stylesheet,  Url = "_content/MudBlazor/MudBlazor.min.css" },
            new Resource { ResourceType = ResourceType.Stylesheet,  Url = ModulePath() + "Module.css" },
            new Resource { ResourceType = ResourceType.Script,     Url = "_content/MudBlazor/MudBlazor.min.js", Location = ResourceLocation.Body, Level = ResourceLevel.Site },
            new Resource { ResourceType = ResourceType.Script,      Url = ModulePath() + "Module.js" },
        };

        protected override async Task OnInitializedAsync()
	    {
		    try
		    {
                var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
                _settingsVM = new SettingsViewModel(SettingService, moduleSettings);
			    if (PageState.Action == "Edit")
			    {
				    _id = Int32.Parse(PageState.QueryString["id"]);
                    (_item, var code) = await InkPlayerService.GetInkPlayerAsync(_id);
                    if (!IsSuccessStatusCode(code)) {
                        throw new HttpRequestException($"Error loading InkPlayer. Code: {code}");
                    }
			    }
                IsLoaded = true;
            }
		    catch (Exception ex)
		    {
			    await logger.LogError(ex, "Error Loading InkPlayer {InkPlayerId} {Error}", _id, ex.Message);
			    AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
		    }
	    }

		private async Task Save()
		{
            try
            {
				await mudform.Validate();
				
                if (mudform.IsValid)
                {
                    if (PageState.Action == "Add")
                    {
                        _item.ModuleId = ModuleState.ModuleId;
                        (var item, var code) = await InkPlayerService.AddInkPlayerAsync(_item);
                        if (code is not HttpStatusCode.OK) {
                            throw new HttpRequestException($"Error Adding {_item}. Code: {code}");
                        }    
                        _item = item; 
                        await logger.LogInformation("InkPlayer Added {item}", item);
                    }
                    else
                    {
                        (var latest, var code) = await InkPlayerService.GetInkPlayerAsync(_id);
                        if (code is not HttpStatusCode.OK) {
                            throw new HttpRequestException($"Error loading InkPlayer. Code: {code}");
                        }
                    
                        // update values from the local version of InkPlayer
                        latest.Name = _item.Name;
                        // update Database with the latest version of InkPlayer
                        (var item, code) = await InkPlayerService.UpdateInkPlayerAsync(latest);
                        if (code is not HttpStatusCode.OK) {
                            throw new HttpRequestException($"Error Adding {_item}. Code: {code}");
                        }         
                        _item = item; 
                        await logger.LogInformation("InkPlayer Updated {latest}", latest);
                    }
                    NavigationManager.NavigateTo(NavigateUrl());
                }
                else
                {
                    AddModuleMessage(Localizer["Message.SaveValidation"], MessageType.Warning);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError(ex, "Error Saving InkPlayer {Error}", ex.Message);
                AddModuleMessage(Localizer["Message.SaveError"], MessageType.Error);
            }
		}
    
        static bool IsSuccessStatusCode(HttpStatusCode statusCode) { 
            return (int)statusCode >= 200 && (int)statusCode <= 299; 
        }
    }
}
