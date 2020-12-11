using System.Windows;

namespace WavePlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string SoftwareName = "WavePlayer";

        internal Project CurProject
        {
            get => _project;
            set
            {
                _project = value;
                var mainWindow = MainWindow as MainWindow;
                _project.PropertyChanged += mainWindow.OnCurProjectPropertyChanged;
                mainWindow.OnCurProjectChanged();
            }
        }

        private Project _project;
    }
}
