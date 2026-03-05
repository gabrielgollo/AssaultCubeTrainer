using System.Windows.Media;

namespace AssaultCubeTrainer.App.Models
{
    public class LogEntry
    {
        public string Message { get; set; } = string.Empty;
        public Brush LevelBrush { get; set; } = Brushes.White;
    }
}
