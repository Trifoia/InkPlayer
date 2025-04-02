using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Shared;
using Oqtane.Services;
using InkRun = global::Ink.Runtime;
using global::Ink;
using Trifoia.Module.InkPlayer.Services;
using MudBlazor;
using System.ComponentModel;
using System.Linq;
using Markdig;

namespace Trifoia.Module.InkPlayer;

public partial class Index : ModuleBase, IDisposable
{

    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected ISettingService SettingService { get; set; }
    [Inject]
    protected ISnackbar Snackbar { get; set; }

    bool loading;

    // properties we listed for
    private const string UserStateProperty = "UserState";

    MarkupString _currentLine = new MarkupString();
    List<CustomInkChoice> _currentChoices = new();
    List<string> _inkState = new();

    bool _hasNext = false;
    bool _hasPrevious = false;
    bool _hasFinish = false;
    int _pageCount = 0;
    SettingsViewModel _settingsVM;

    protected InkRun.Story _story;
    private bool disposedValue;
    private string settingsUrl;
    private string returnUrl;

    protected override void OnInitialized()
    {
        // listen for changes to sitestate
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged += PropertyChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!ShouldRender()) return;

        loading = true;

        var moduleSettings = await SettingService.GetModuleSettingsAsync(ModuleState.ModuleId);
        _settingsVM = new SettingsViewModel(SettingService, moduleSettings);

        returnUrl = WebUtility.UrlEncode(PageState.Uri.AbsolutePath.ToString());
        settingsUrl = EditUrl("Settings", $"returnurl={returnUrl}&tab=ModuleSettings");

        CompileStory();

        if (PageState.EditMode || _story == null)
            return;

        try
        {
            _inkState = new();
            if (_story.canContinue)
            {
                Next();
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            await logger.LogError(ex, "Error Loading Content {Error}", ex.Message);
        }

        loading = false;
    }

    protected void CompileStory()
    {

        try
        {
            if (String.IsNullOrEmpty(_settingsVM.Ink))
            {
                _story = null;
            }

            // add headers to the ink
            var headers = InkFunctions.GetHeaders();
            var ink = $"{headers}\n\n{_settingsVM.Ink}";

            // compile the story
            var compiler = new Compiler(ink);
            var compiledStory = compiler.Compile();
            _story = compiledStory;

            // bind external functions
            InkFunctions.BindExternalFunctions(_story, SiteState, this);




        }
        catch (Exception ex)
        {
            _story = null;
            logger.LogError(ex, "Error Loading story {message}", ex.Message);
            AddModuleMessage("Error Loading Story", MessageType.Error);
        }

    }


    protected void ChoiceSelected(CustomInkChoice choice)
    {
        var choiceIndex = choice.Index;
        _story.ChooseChoiceIndex(choiceIndex);
        Next();
    }

    private void Next()
    {
        if (_story != null)
        {
            if (_story.canContinue)
            {
                string nextLine = _story.ContinueMaximally();
                _currentLine = ProcessStoryText(nextLine);
                _inkState.Add(_story.state.ToJson());
            }

            ProcessTags();
        }
    }

    private void Previous()
    {
        if (_story != null && _inkState.Any())
        {
            // remove the current state and load the last one
            _inkState.RemoveAt(_inkState.Count - 1); // could be a pop?

            var state = _inkState.LastOrDefault();
            _story.state.LoadJson(state);

            _currentLine = ProcessStoryText(_story.currentText);
            ProcessTags();
        }

    }

    MarkupString ProcessStoryText(string text)
    {
        // because the Ink authoring tool may not support "\n" line breaks, process "<br>" to linebreaks to allow Markdown to parse them
        text = text.Replace("\\", "\n");

        var output = Markdig.Markdown.ToHtml(text);

        return new MarkupString(output);
    }

    private void ProcessTags()
    {
        if (_story == null)
            return;

        if (_story.currentTags.Any())
        {
            // look for a lottie tag on the current story section
            var lottieTag = _story.currentTags.FirstOrDefault(t => t.Contains("lottie:", StringComparison.InvariantCultureIgnoreCase));
            if (lottieTag != null)
            {
                string lottieUrl = lottieTag.Substring(7); // remove the lottie: prefix and add https://

                // update the sitestate with the new lottie url
                if (!lottieUrl.Contains("~"))
                {
                    // tags can't have : or // so we need to add it back in
                    lottieUrl = $"https://{lottieUrl}"; // remove the lottie: prefix and add https://
                }
                SiteState.Properties.Lottie = lottieUrl;  // this will be used by the Lottie module
            }

            // look for a lottie tag on the current story section
            var imageTag = _story.currentTags.FirstOrDefault(t => t.Contains("image:", StringComparison.InvariantCultureIgnoreCase));
            if (lottieTag != null)
            {
                string imageUrl = lottieTag.Substring(6); // remove the lottie: prefix and add https://

                // update the sitestate with the new lottie url
                if (!imageUrl.Contains("~"))
                {
                    // tags can't have : or // so we need to add it back in
                    imageUrl = $"https://{imageUrl}"; // remove the lottie: prefix and add https://
                }
                SiteState.Properties.Lottie = imageUrl;  // this will be used by the Lottie module
            }
        }

        _currentChoices = _story.currentChoices
                                        .Select(choice => new CustomInkChoice
                                        {
                                            Text = choice.text,
                                            Tags = choice.tags,
                                            Index = choice.index,
                                            PathStringOnChoice = choice.pathStringOnChoice
                                        })
                                        .ToList();

        _hasNext = _story.canContinue;
        _hasPrevious = _inkState.Count > 1;
        _hasFinish = !_hasNext && _currentChoices.Count == 0;

        if (string.IsNullOrEmpty(_currentLine.Value) && !_hasNext && _currentChoices.Count == 0)
        {
            // if there's no more text and no more choices, we're at the end of the story
            return;
        }

        StateHasChanged();
    }


    async void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {

        // listen for changes to siteState.Properties.InkVariable
        if (e.PropertyName == UserStateProperty)
        {
            // sync the user state with any ink variables
            InkFunctions.SyncUserState(_story, SiteState, PageState);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        ((INotifyPropertyChanged)SiteState.Properties).PropertyChanged -= PropertyChanged;
    }
}