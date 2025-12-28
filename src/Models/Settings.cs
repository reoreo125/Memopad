using System.Printing;
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

    // PageSettings
    [JsonProperty]
    public PageSettings Page { get; } = new();

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
            )
            .Merge(Page.Changed);
    }
}
[JsonObject(MemberSerialization.OptIn)]
public class PageSettings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty]
    public ReactiveProperty<PageMediaSizeName> PaperSizeName { get; } = new(PageMediaSizeName.ISOA4);
    [JsonProperty]
    public ReactiveProperty<PageOrientation> Orientation { get; } = new(PageOrientation.Portrait);
    [JsonProperty]
    public ReactiveProperty<double> MarginLeft { get; } = new(20.0);
    [JsonProperty]
    public ReactiveProperty<double> MarginTop { get; } = new(25.0);
    [JsonProperty]
    public ReactiveProperty<double> MarginRight { get; } = new(20.0);
    [JsonProperty]
    public ReactiveProperty<double> MarginBottom { get; } = new(25.0);
    [JsonProperty]
    public ReactiveProperty<string> Header { get; } = new("&f");  // &f はファイル名のマクロ
    [JsonProperty]
    public ReactiveProperty<string> Footer { get; } = new("Page &p"); // &p はページ番号のマクロ

    [JsonConstructor]
    public PageSettings()
    {
        Changed = Observable.Merge
            (
                PaperSizeName.AsUnitObservable(),
                Orientation.AsUnitObservable(),
                MarginLeft.AsUnitObservable(),
                MarginTop.AsUnitObservable(),
                MarginRight.AsUnitObservable(),
                MarginBottom.AsUnitObservable(),
                Header.AsUnitObservable(),
                Footer.AsUnitObservable()
            );
    }
}
