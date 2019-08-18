using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            Title += " v" + GetAppVersion();

            DataContext = viewModel;

            filesListView.ItemsSource = viewModel.Model.SortedItems;
            ((INotifyCollectionChanged)filesListView.ItemsSource).CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    filesListView.ScrollIntoView(filesListView.Items[filesListView.Items.Count - 1]);
                }
            };
        }

        private String GetAppVersion()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Revision);
        }

        private void OpenCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.Model.NotSorting;
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.UpdateSourcePath(dialog.SelectedPath);
            }
        }
    }
}
