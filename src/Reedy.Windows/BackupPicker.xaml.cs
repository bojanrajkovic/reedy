using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using Reedy.Core.Models;

namespace Reedy
{
    /// <summary>
    /// Interaction logic for BackupPicker.xaml
    /// </summary>
    public partial class BackupPicker : MetroWindow
    {
        MainWindow mainWindow;

        public BackupPicker(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;
        }

        public ObservableCollection<Backup> Backups { get; } =
            new ObservableCollection<Backup>();

        private void PickBackup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindow.PopulateBackupAsync((Backup)backupList.SelectedItem);
            Close();
        }
    }
}
