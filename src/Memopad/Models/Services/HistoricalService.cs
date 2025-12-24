using System.Windows;
using System.Windows.Input;

namespace Reoreo125.Memopad.Models.Services;

public interface IHistoricalService { }

public class HistoricalService : IHistoricalService
{


    public int MaxHistory { get; init; } = 100;
    public Stack<HistoricalItem> UndoStack { get; } = new ();
    public Stack<HistoricalItem> RedoStack { get; } = new ();
}

public record HistoricalItem (
    Guid Id,
    string TextDiff
    );
