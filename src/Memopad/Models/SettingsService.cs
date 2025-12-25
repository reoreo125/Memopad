using R3;

namespace Reoreo125.Memopad.Models;

public interface ISettingsService
{
    public Settings Settings {get;}
}
public class SettingsService : ISettingsService, IDisposable
{
    public Settings Settings { get; }

    private DisposableBag _disposableCollection = new();

    public SettingsService()
    {
        Settings = Load();

        Settings.Changed
            .Debounce(TimeSpan.FromMilliseconds(Defaults.SettingsSaveInterval))
            .Subscribe(_ => Save())
            .AddTo(ref _disposableCollection);
    }

    Settings Load()
    {
        return new Settings();
    }
    void Save()
    {

    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
