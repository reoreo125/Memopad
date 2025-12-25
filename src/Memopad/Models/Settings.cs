using System.Drawing;
using R3;

namespace Reoreo125.Memopad.Models;

public class Settings
{
    public Observable<Unit> Changed { get; }

    public BindableReactiveProperty<string> LastOpenedFolderPath { get; }
    public BindableReactiveProperty<string> FontFamilyName { get; }
    public BindableReactiveProperty<int> FontSize { get; }
    public BindableReactiveProperty<bool> IsWordWrap { get; }
    public ReactiveProperty<bool> ShowStatusBar { get; }
    public ReactiveProperty<double> ZoomLevel { get; }


    public Settings()
    {
        LastOpenedFolderPath = new BindableReactiveProperty<string>(Defaults.LastOpenedFolderPath);
        FontFamilyName = new BindableReactiveProperty<string>(Defaults.FontFamilyName);
        FontSize = new BindableReactiveProperty<int>(Defaults.FontSize);
        IsWordWrap = new BindableReactiveProperty<bool>(Defaults.IsWrapping);
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
