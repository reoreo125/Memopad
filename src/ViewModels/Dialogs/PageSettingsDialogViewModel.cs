using System.Drawing.Printing;
using System.Printing;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;

namespace Reoreo125.Memopad.ViewModels.Dialogs
{
    public class PageSettingsDialogViewModel : BindableBase, IDialogAware, IDisposable
    {
        public string? Title => "ページ設定";

        public DialogCloseListener RequestClose { get; }

        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
        public DelegateCommand OkCommand => new(() =>
        {
            // OKクリックで保存
            SettingsService.Settings.Page.PaperSizeName.Value = PageSizeName.Value;
            SettingsService.Settings.Page.InputBin.Value = InputBin.Value;
            SettingsService.Settings.Page.Orientation.Value = Orientation.Value;
            SettingsService.Settings.Page.MarginLeft.Value = MarginLeft.Value;
            SettingsService.Settings.Page.MarginRight.Value = MarginRight.Value;
            SettingsService.Settings.Page.MarginTop.Value = MarginTop.Value;
            SettingsService.Settings.Page.MarginBottom.Value = MarginBottom.Value;
            SettingsService.Settings.Page.Header.Value = Header.Value;
            SettingsService.Settings.Page.Footer.Value = Footer.Value;
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        });

        public BindableReactiveProperty<PageMediaSizeName> PageSizeName { get; }
        public BindableReactiveProperty<InputBin> InputBin { get; }
        public BindableReactiveProperty<PageOrientation> Orientation { get; }
        public BindableReactiveProperty<double> MarginLeft { get; }
        public BindableReactiveProperty<double> MarginTop { get; }
        public BindableReactiveProperty<double> MarginRight { get; }
        public BindableReactiveProperty<double> MarginBottom { get; }
        public BindableReactiveProperty<string> Header { get; }
        public BindableReactiveProperty<string> Footer { get; }

        public IEnumerable<object> PageSizeOptions { get; } = new[]
        {
            new { Name = "11x17(拡大縮小)", Value = PageMediaSizeName.NorthAmerica11x17 },
            new { Name = "17x22(拡大縮小)", Value = PageMediaSizeName.NorthAmericaArchitectureCSheet }, // 17x22インチ
            new { Name = "2L判", Value = PageMediaSizeName.Japan2LPhoto },
            new { Name = "A2(拡大縮小)", Value = PageMediaSizeName.ISOA2 },
            new { Name = "A3(拡大縮小)", Value = PageMediaSizeName.ISOA3 },
            new { Name = "A3ノビ(拡大縮小)", Value = PageMediaSizeName.OtherMetricA3Plus },
            new { Name = "A4", Value = PageMediaSizeName.ISOA4 },
            new { Name = "A5", Value = PageMediaSizeName.ISOA5 },
            new { Name = "B3(拡大縮小)", Value = PageMediaSizeName.JISB3 },
            new { Name = "B4(拡大縮小)", Value = PageMediaSizeName.JISB4 },
            new { Name = "B5", Value = PageMediaSizeName.JISB5 },
            new { Name = "EUR DL Env.", Value = PageMediaSizeName.ISODLEnvelope },
            new { Name = "KG", Value = PageMediaSizeName.JapanHagakiPostcard },
            new { Name = "L判", Value = PageMediaSizeName.JapanLPhoto },
            new { Name = "US 5x7", Value = PageMediaSizeName.NorthAmerica5x7 },
            new { Name = "US COMM. Env #10", Value = PageMediaSizeName.NorthAmericaNumber10Envelope },
            new { Name = "はがき", Value = PageMediaSizeName.JapanHagakiPostcard },
            new { Name = "リーガル", Value = PageMediaSizeName.NorthAmericaLegal },
            new { Name = "レター", Value = PageMediaSizeName.NorthAmericaLetter },
            new { Name = "往復はがき", Value = PageMediaSizeName.JapanDoubleHagakiPostcard },
            new { Name = "四切(拡大縮小)", Value = PageMediaSizeName.JapanQuadrupleHagakiPostcard },
            new { Name = "長形3号", Value = PageMediaSizeName.JapanChou3Envelope },
            new { Name = "長形4号", Value = PageMediaSizeName.JapanChou4Envelope },
            new { Name = "半切(拡大縮小)", Value = PageMediaSizeName.JapanDoubleHagakiPostcard },
            new { Name = "洋形4号", Value = PageMediaSizeName.JapanYou4Envelope },
            new { Name = "洋形6号", Value = PageMediaSizeName.JapanYou6Envelope },
            new { Name = "六切", Value = PageMediaSizeName.NorthAmerica8x10 },
        };

        // コンボボックスの選択肢
        public BindableReactiveProperty<IEnumerable<object>> InputBinOptions { get; }

        private string GetInputBinDisplayName(InputBin bin)
        {
            return bin switch
            {
                System.Printing.InputBin.AutoSelect => "自動選択",
                System.Printing.InputBin.AutoSheetFeeder => "自動給紙",
                System.Printing.InputBin.Manual => "手差し",
                System.Printing.InputBin.Cassette => "カセット",
                System.Printing.InputBin.Tractor => "トラクタ給紙",
                System.Printing.InputBin.Unknown => "不明なトレイ",
                _ => bin.ToString() // 定義外はそのままの名前を表示
            };
        }

        public IEditorService EditorService => _editorService;
        private readonly IEditorService _editorService;
        protected ISettingsService SettingsService => _settingsService;
        private readonly ISettingsService _settingsService;

        private DisposableBag _disposableCollection = new();

        public PageSettingsDialogViewModel(IEditorService editorService, ISettingsService settingsService)
        {
            _editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(SettingsService));

            PageSizeName = new(SettingsService.Settings.Page.PaperSizeName.Value);

            // InputBinに対する処理
            var capabilities = LocalPrintServer.GetDefaultPrintQueue().GetPrintCapabilities();
            var bins = capabilities.InputBinCapability;
            var settingsInputBin = SettingsService.Settings.Page.InputBin.Value;

            List<object> options = (bins == null || bins.Count == 0)
                ? new List<object> { new { Name = "自動選択", Value = System.Printing.InputBin.AutoSelect } }
                : bins.Select(bin => new { Name = GetInputBinDisplayName(bin), Value = bin }).ToList<object>();

            InputBinOptions = new(options);

            // 設定値がリストに含まれているかチェックしてセット
            bool exists = options.Any(opt => {
                var prop = opt.GetType().GetProperty("Value");
                return prop != null && (System.Printing.InputBin)prop.GetValue(opt)! == settingsInputBin;
            });

            if (exists)
            {
                // リストに含まれていれば設定値をそのままセット
                InputBin = new(settingsInputBin);
            }
            else
            {
                // 含まれていなければAutoSelectをセット
                InputBin = new(System.Printing.InputBin.AutoSelect);
            }
            // ------
            Orientation = new(SettingsService.Settings.Page.Orientation.Value);
            MarginLeft = new(SettingsService.Settings.Page.MarginLeft.Value);
            MarginRight = new(SettingsService.Settings.Page.MarginRight.Value);
            MarginTop = new(SettingsService.Settings.Page.MarginTop.Value);
            MarginBottom = new(SettingsService.Settings.Page.MarginBottom.Value);
            Header = new(SettingsService.Settings.Page.Header.Value);
            Footer = new(SettingsService.Settings.Page.Footer.Value);


            // --- 方向が変わった時の入れ替えロジック ---
            Orientation
                .Skip(1)
                .Subscribe(orientation =>
                {
                    // [左] と [上] を入れ替える
                    var tempLeft = MarginLeft.Value;
                    MarginLeft.Value = MarginTop.Value;
                    MarginTop.Value = tempLeft;

                    // [右] と [下] を入れ替える
                    var tempRight = MarginRight.Value;
                    MarginRight.Value = MarginBottom.Value;
                    MarginBottom.Value = tempRight;
                })
                .AddTo(ref _disposableCollection);
        }
        public void OnDialogOpened(IDialogParameters parameters) { }
        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void Dispose()
        {
            _disposableCollection.Dispose();
        }
    }
}
