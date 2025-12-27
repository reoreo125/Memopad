using Newtonsoft.Json;
using R3;

namespace Reoreo125.Memopad.Models;

[JsonObject(MemberSerialization.OptIn)]
public class Settings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty]
    public ReactiveProperty<string> LastOpenedFolderPath { get; set; } = new(Defaults.LastOpenedFolderPath);
    [JsonProperty]
    public ReactiveProperty<string> FontFamilyName { get; set; } = new(Defaults.FontFamilyName);
    [JsonProperty]
    public ReactiveProperty<int> FontSize { get; set; } = new(Defaults.FontSize);
    [JsonProperty]
    public ReactiveProperty<bool> IsWordWrap { get; set; } = new(Defaults.IsWrapping);
    [JsonProperty]
    public ReactiveProperty<bool> ShowStatusBar { get; set; } = new(Defaults.ShowStatusBar);
    [JsonProperty]
    public ReactiveProperty<double> ZoomLevel { get; set; } = new(Defaults.ZoomLevel);

    [JsonConstructor]
    public Settings()
    {
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
