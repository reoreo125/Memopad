using System.ComponentModel.DataAnnotations;
using System.Printing;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Validators;

namespace Reoreo125.Memopad.Models;

public interface ISettings
{
    public Observable<Unit> Changed { get; }

    public ReactiveProperty<string> LastOpenedFolderPath { get; }
    public ReactiveProperty<string> FontFamilyName { get; }
    public ReactiveProperty<string> FontStyleName { get; }
    public ReactiveProperty<int> FontSize { get; }
    public ReactiveProperty<bool> IsWordWrap { get; }
    public ReactiveProperty<bool> ShowStatusBar { get; }
    public ReactiveProperty<int> ZoomLevel { get; }

    public IPageSettings Page { get; }
}
[JsonObject(MemberSerialization.OptIn)]
public class Settings : ISettings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty, FolderPathValidator, FallbackValue(nameof(Defaults.LastOpenedFolderPath))]
    public ReactiveProperty<string> LastOpenedFolderPath { get; } = new(Defaults.LastOpenedFolderPath);

    [JsonProperty, FontFamilyNameValidator, FallbackValue(nameof(Defaults.FontFamilyName))]
    public ReactiveProperty<string> FontFamilyName { get; } = new(Defaults.FontFamilyName);

    [JsonProperty, FontStyleNameValidator, FallbackValueFromMethod(nameof(Defaults.GetFontStyleName), nameof(FontFamilyName))]
    public ReactiveProperty<string> FontStyleName { get; } = new(Defaults.GetFontStyleName(Defaults.FontFamilyName));

    [JsonProperty, Range(Defaults.FontSizeMin, Defaults.FontSizeMax), FallbackValue(nameof(Defaults.FontSize))]
    public ReactiveProperty<int> FontSize { get; } = new(Defaults.FontSize);

    [JsonProperty, FallbackValue(nameof(Defaults.IsWrapping))]
    public ReactiveProperty<bool> IsWordWrap { get; } = new(Defaults.IsWrapping);

    [JsonProperty, FallbackValue(nameof(Defaults.ShowStatusBar))]
    public ReactiveProperty<bool> ShowStatusBar { get; } = new(Defaults.ShowStatusBar);

    [JsonProperty, Range(Defaults.ZoomMin, Defaults.ZoomMax), FallbackValue(nameof(Defaults.ZoomLevel))]
    public ReactiveProperty<int> ZoomLevel { get; } = new(Defaults.ZoomLevel);

    // PageSettings
    [JsonProperty]
    public IPageSettings Page { get; } = new PageSettings();

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

public interface IPageSettings
{
    public Observable<Unit> Changed { get; }

    public ReactiveProperty<PageMediaSizeName> PaperSizeName { get; }
    public ReactiveProperty<InputBin> InputBin { get; }
    public ReactiveProperty<PageOrientation> Orientation { get; }
    public ReactiveProperty<double> MarginLeft { get; }
    public ReactiveProperty<double> MarginTop { get; }
    public ReactiveProperty<double> MarginRight { get; }
    public ReactiveProperty<double> MarginBottom { get; }
    public ReactiveProperty<string> Header { get; }
    public ReactiveProperty<string> Footer { get; }
}

[JsonObject(MemberSerialization.OptIn)]
public class PageSettings : IPageSettings
{
    [JsonIgnore]
    public Observable<Unit> Changed { get; }

    [JsonProperty, PaperSizeNameValidator, FallbackValue(nameof(Defaults.PaperSizeName))]
    public ReactiveProperty<PageMediaSizeName> PaperSizeName { get; } = new(Defaults.PaperSizeName);

    [JsonProperty, InputBinValidator, FallbackValue(nameof(Defaults.InputBin))]
    public ReactiveProperty<InputBin> InputBin { get; } = new(Defaults.InputBin);

    [JsonProperty, PageOrientationValidator, FallbackValue(nameof(Defaults.PageOrientation))]
    public ReactiveProperty<PageOrientation> Orientation { get; } = new(Defaults.PageOrientation);

    [JsonProperty, Range(Defaults.MarginMin, Defaults.MarginMax), FallbackValue(nameof(Defaults.MarginLeft))]
    public ReactiveProperty<double> MarginLeft { get; } = new(Defaults.MarginLeft);

    [JsonProperty, Range(Defaults.MarginMin, Defaults.MarginMax), FallbackValue(nameof(Defaults.MarginTop))]
    public ReactiveProperty<double> MarginTop { get; } = new(Defaults.MarginTop);

    [JsonProperty, Range(Defaults.MarginMin, Defaults.MarginMax), FallbackValue(nameof(Defaults.MarginRight))]
    public ReactiveProperty<double> MarginRight { get; } = new(Defaults.MarginRight);

    [JsonProperty, Range(Defaults.MarginMin, Defaults.MarginMax), FallbackValue(nameof(Defaults.MarginBottom))]
    public ReactiveProperty<double> MarginBottom { get; } = new(Defaults.MarginBottom);

    [JsonProperty]
    [StringLength(Defaults.HeaderMaxLength, ErrorMessage = "{0} is too long.")]
    [RegularExpression(@"^[^\r\n]*$", ErrorMessage = "{0} cannot contain line breaks.")]
    [FallbackValue(nameof(Defaults.Header))]
    public ReactiveProperty<string> Header { get; } = new(Defaults.Header);  // &f はファイル名のマクロ

    [JsonProperty]
    [StringLength(Defaults.FooterMaxLength, ErrorMessage = "{0} is too long.")]
    [RegularExpression(@"^[^\r\n]*$", ErrorMessage = "{0} cannot contain line breaks.")]
    [FallbackValue(nameof(Defaults.Footer))]
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
