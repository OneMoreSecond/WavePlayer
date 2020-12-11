using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WavePlayer
{
    /// <summary>
    /// Interaction logic for TextDialog.xaml
    /// </summary>
    public partial class TextDialog : Window, INotifyPropertyChanged
    {
        public TextDialog()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string textDescription;
        public string TextDescription
        {
            get => textDescription;
            set
            {
                textDescription = value;
                NotifyPropertyChanged();
            }
        }
        public string EnteredText => EnterTextBox.Text;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnteredText))
            {
                MessageBox.Show(this, "No text is entered!", "Error");
                return;
            }
            DialogResult = true;
        }

    }
}
