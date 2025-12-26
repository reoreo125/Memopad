using Newtonsoft.Json;
using R3;

namespace Reoreo125.Memopad.Models;

[JsonObject(MemberSerialization.OptIn)]
public class Settings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty]
    public ReactiveProperty<string> LastOpenedFolderPath { get; set; }
    [JsonProperty]
    public ReactiveProperty<string> FontFamilyName { get; set; }
    [JsonProperty]
    public ReactiveProperty<int> FontSize { get; set; }
    [JsonProperty]
    public ReactiveProperty<bool> IsWordWrap { get; set; }
    [JsonProperty]
    public ReactiveProperty<bool> ShowStatusBar { get; set; }
    [JsonProperty]
    public ReactiveProperty<double> ZoomLevel { get; set; }

    [JsonConstructor]
    public Settings()
    {
        LastOpenedFolderPath    = new ReactiveProperty<string>(Defaults.LastOpenedFolderPath);
        FontFamilyName          = new ReactiveProperty<string>(Defaults.FontFamilyName);
        FontSize                = new ReactiveProperty<int>(Defaults.FontSize);
        IsWordWrap              = new ReactiveProperty<bool>(Defaults.IsWrapping);
        ShowStatusBar           = new ReactiveProperty<bool> (Defaults.ShowStatusBar);
        ZoomLevel               = new ReactiveProperty<double> (Defaults.ZoomLevel);
        
        Changed = Observable.Merge
            (
                LastOpenedFolderPath.AsUnitObservable(),
                FontFamilyName.AsUnitObservable(),
                FontSize.AsUnitObservable(),
                IsWordWrap.AsUnitObservable(),
                ShowStatusBar.AsUnitObservable(),
                ZoomLevel.AsUnitObservable()
            );
    }
}
