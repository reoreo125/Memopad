using System.ComponentModel;
using System.Printing;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Validators;

namespace Reoreo125.Memopad.Models;

[JsonObject(MemberSerialization.OptIn)]
public class Settings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty, FolderPathValidator, FailbackValue(nameof(Defaults.LastOpenedFolderPath))]
    public ReactiveProperty<string> LastOpenedFolderPath { get; } = new(Defaults.LastOpenedFolderPath);

    [JsonProperty, FontFamilyNameValidator, FailbackValue(nameof(Defaults.FontFamilyName))]
    public ReactiveProperty<string> FontFamilyName { get; } = new(Defaults.FontFamilyName);

    [JsonProperty]
    public ReactiveProperty<string> FontStyleName { get; } = new(Defaults.FontStyleName);
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
                FontStyleName.AsUnitObservable(),
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
    public ReactiveProperty<PageMediaSizeName> PaperSizeName { get; } = new(Defaults.PaperSizeName);
    [JsonProperty]
    public ReactiveProperty<InputBin> InputBin { get; } = new(Defaults.InputBin);
    [JsonProperty]
    public ReactiveProperty<PageOrientation> Orientation { get; } = new(Defaults.PageOrientation);
    [JsonProperty]
    public ReactiveProperty<double> MarginLeft { get; } = new(Defaults.MarginLeft);
    [JsonProperty]
    public ReactiveProperty<double> MarginTop { get; } = new(Defaults.MarginTop);
    [JsonProperty]
    public ReactiveProperty<double> MarginRight { get; } = new(Defaults.MarginRight);
    [JsonProperty]
    public ReactiveProperty<double> MarginBottom { get; } = new(Defaults.MarginBottom);
    [JsonProperty]
    public ReactiveProperty<string> Header { get; } = new(Defaults.Header);  // &f はファイル名のマクロ
    [JsonProperty]
    public ReactiveProperty<string> Footer { get; } = new(Defaults.Footer); // &p はページ番号のマクロ

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
