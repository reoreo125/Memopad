using Newtonsoft.Json;
using R3;

namespace Reoreo125.Memopad.Models;

[JsonObject(MemberSerialization.OptIn)]
public class Settings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty]
    public ReactiveProperty<string> LastOpenedFolderPath { get; } = new(Defaults.LastOpenedFolderPath);
    [JsonProperty]
    public ReactiveProperty<string> FontFamilyName { get; } = new(Defaults.FontFamilyName);
    [JsonProperty]
    public ReactiveProperty<int> FontSize { get; } = new(Defaults.FontSize);
    [JsonProperty]
    public ReactiveProperty<bool> IsWordWrap { get; } = new(Defaults.IsWrapping);
    [JsonProperty]
    public ReactiveProperty<bool> ShowStatusBar { get; } = new(Defaults.ShowStatusBar);
    [JsonProperty]
    public ReactiveProperty<double> ZoomLevel { get; } = new(Defaults.ZoomLevel);

    [JsonConstructor]
    public Settings()
    {
        Changed = Observable.Merge
            (
                LastOpenedFolderPath.Skip(1).AsUnitObservable(),
                FontFamilyName.Skip(1).AsUnitObservable(),
                FontSize.Skip(1).AsUnitObservable(),
                IsWordWrap.Skip(1).AsUnitObservable(),
                ShowStatusBar.Skip(1).AsUnitObservable(),
                ZoomLevel.Skip(1).AsUnitObservable()
            );
    }
}
