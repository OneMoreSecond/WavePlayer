using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WavePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = App.SoftwareName;
        }

        private ListView scriptListView = null;

        public void OnCurProjectChanged()
        {
            WelcomeGrid.Visibility = Visibility.Collapsed;
            WorkingDockPanel.Visibility = Visibility.Visible;

            UpdateTitle();
            UpdateScript();
        }

        public void OnCurProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" || e.PropertyName == "HasUnsavedChanges")
            {
                UpdateTitle();
            }
            else if (e.PropertyName == "ScriptLines")
            {
                UpdateScript();
            }
        }

        private void UpdateScript()
        {
            Debug.Assert(CurProject.ScriptLines != null);
        }

        private void UpdateTitle()
        {
            Title = $"{App.SoftwareName} - Project: {CurProject.Name}" + (CurProject.HasUnsavedChanges ? "*" : "");
        }

        private static Project CurProject
        {
            get => CurApp.CurProject;
            set
            {
                CurApp.CurProject = value;
            }
        }

        private static App CurApp => Application.Current as App;

        private string ShowDialogForString(string title, string textDescription)
        {
            var dialog = new TextDialog()
            {
                Owner = this,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                TextDescription = textDescription,
                Title = title,
            };
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult ?? false)
            {
                return dialog.EnteredText;
            }
            else
            {
                return null;
            }
        }

        public void NewProject(object sender, RoutedEventArgs e)
        {
            bool IsContinue = PromptForSave(sender, e);
            if (!IsContinue)
            {
                return;
            }

            string projectName = ShowDialogForString(title: "New Project", textDescription: "Please enter the new project name:");
            if (projectName != null)
            {
                CurProject = new Project(projectName)
                {
                    HasUnsavedChanges = true
                };
            }
        }

        private string ShowDialogForPath(FileDialog dialog)
        {
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult ?? false)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private string ShowDialogForProjectConfigPath<T>(string title, string filename = "")
            where T : FileDialog, new()
        {
            string extension = Project.ConfigExtension;
            var dialog = new T()
            {
                DefaultExt = extension,
                Filter = $"{App.SoftwareName} Project configuration ({extension})|*{extension}",
                FileName = filename,
                Title = title,
            };
            return ShowDialogForPath(dialog);
        }

        public void LoadProject(object sender, RoutedEventArgs e)
        {
            bool IsContinue = PromptForSave(sender, e);
            if (!IsContinue)
            {
                return;
            }

            string projectConfigPath = ShowDialogForProjectConfigPath<OpenFileDialog>("Load project");
            if (projectConfigPath != null)
            {
                try
                {
                    CurProject = Project.Load(projectConfigPath);
                }
                catch
                {
                    // TODO: add exception handling
                    throw;
                }
            }
        }

        public void SaveProject(object sender, RoutedEventArgs e)
        {
            string projectConfigPath = ShowDialogForProjectConfigPath<SaveFileDialog>("Save project", filename: CurProject.Name);
            if (projectConfigPath != null)
            {
                CurProject.Save(projectConfigPath);
            }
        }

        public bool PromptForSave(object sender, RoutedEventArgs e)
        {
            if (CurProject == null || !CurProject.HasUnsavedChanges)
            {
                return true;
            }

            MessageBoxResult confirmResult = MessageBox.Show(
                owner: this,
                messageBoxText: "Current project is edited but not saved. Do you want to save it?",
                caption: "Confirmation",
                MessageBoxButton.YesNoCancel);

            if (confirmResult == MessageBoxResult.Yes)
            {
                SaveProject(sender, e);
            }

            switch (confirmResult)
            {
                case MessageBoxResult.Cancel:
                    return false;

                case MessageBoxResult.Yes:
                case MessageBoxResult.No:
                    return true;
            }
            Debug.Fail("Unexpected MessageBoxResult");
            return false;
        }

        public void ImportScript(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                DefaultExt = ".txt",
                Title = "Import Script",
            };
            string scriptPath = ShowDialogForPath(dialog);
            if (scriptPath != null)
            {
                CurProject.ScriptPath = scriptPath;
            }
        }
    }
}
