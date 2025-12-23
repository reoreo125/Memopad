using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Reoreo125.Memopad.Models.Services;
using Reoreo125.Memopad.Views.Windows;
using UtfUnknown;

namespace Reoreo125.Memopad.Models.Commands;

public interface IOpenTextFileWindowCommand : ICommand { }
public class OpenTextFileWindowCommand : CommandBase, IOpenTextFileWindowCommand
{
    [Dependency]
    public IMemopadCoreService? MemopadCoreService { get; set; }
    public override bool CanExecute(object? parameter) => true;

    public override void Execute(object? parameter)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "ファイルを開く",
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            try
            {
                // ファイルのエンコーディングを検出
                var detection = CharsetDetector.DetectFromFile(filePath);
                if (detection is null || detection.Detected is null) throw new InvalidOperationException($"ファイルのエンコーディングを検出できませんでした: {filePath}");
                DetectionDetail encodingResult = detection.Detected;

                string fileContent = File.ReadAllText(filePath, encodingResult.Encoding);

                if(MemopadCoreService is null) throw new Exception("MemopadCoreServiceが初期化されていません。");


                MemopadCoreService.ChangeText(fileContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ファイルを開くことができませんでした。\n\n{ex.Message}", 
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
