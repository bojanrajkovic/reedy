using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;
using Reedy.Core;
using Reedy.Core.Models;

using SQLite;

namespace Reedy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<Core.Models.Directory> BackupTree { get; }
            = new ObservableCollection<Core.Models.Directory>();

        protected override void OnContentRendered(EventArgs e)
        {
            var backups = BackupHelper.EnumerateBackups(isMac: false).ToArray();
            PickBackup(backups);
        }

        private void PickBackup(Backup[] backups)
        {
            var backupPicker = new BackupPicker(this);

            foreach (var backup in backups)
                backupPicker.Backups.Add(backup);

            backupPicker.ShowDialog();
        }

        internal async Task PopulateBackupAsync(Backup backup)
        {
            var manifest = Path.Combine(backup.BackupPath, "Manifest.db");
            var db = new SQLiteAsyncConnection(manifest);
            var files = await db.QueryAsync<AppleFile>("SELECT * FROM Files");

            var root = new Core.Models.Directory("<Root>", null);
            var misc = new Core.Models.Directory("Miscellaneous Files", null);

            files.Where(x => x.RelativePath == "")
                 .Select(x => new Core.Models.File(x.Domain, Path.Combine(backup.BackupPath, x.FileID.Substring(0, 2), x.FileID)))
                 .ToList()
                 .ForEach(misc.Children.Add);

            root.Children.Add(misc);

            var filesWithPath = files.Where(x => !string.IsNullOrWhiteSpace(x.RelativePath));

            foreach (var file in filesWithPath) {
                var diskPath = Path.Combine(backup.BackupPath, file.FileID.Substring(0, 2), file.FileID);

                if (!System.IO.File.Exists(diskPath))
                    continue;

                var pathStack = file.RelativePath.Split('/');

                var localRoot = root;
                foreach (var chunk in pathStack.Take(pathStack.Length - 1)) {
                    var dir = localRoot.Children.OfType<Core.Models.Directory>().SingleOrDefault(x => x.Name == chunk);
                    if (dir == null) {
                        dir = new Core.Models.Directory(chunk, null);
                        localRoot.Children.Add(dir);
                    }
                    localRoot = dir;
                }

                var realFile = new Core.Models.File(pathStack.Last(), diskPath);
                localRoot.Children.Add(realFile);
            }

            BackupTree.Add(root);
        }
    }
}
