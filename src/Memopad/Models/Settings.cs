using System.Drawing;
using R3;

namespace Reoreo125.Memopad.Models;

public class Settings
{
    public Observable<Unit> Changed { get; }

    public ReactiveProperty<string> LastOpenedFolderPath { get; }
    public ReactiveProperty<string> FontFamilyName { get; }
    public ReactiveProperty<int> FontSize { get; }
    public ReactiveProperty<bool> IsWordWrap { get; }
    public ReactiveProperty<bool> ShowStatusBar { get; }
    public ReactiveProperty<double> ZoomLevel { get; }


    public Settings()
    {
        LastOpenedFolderPath = new ReactiveProperty<string>(Defaults.LastOpenedFolderPath);
        FontFamilyName = new ReactiveProperty<string>(Defaults.FontFamilyName);
        FontSize = new ReactiveProperty<int>(Defaults.FontSize);
        IsWordWrap = new ReactiveProperty<bool>(Defaults.IsWrapping);
        ShowStatusBar = new ReactiveProperty<bool> (Defaults.ShowStatusBar);
        ZoomLevel = new ReactiveProperty<double> (Defaults.ZoomLevel);

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
