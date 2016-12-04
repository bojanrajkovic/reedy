using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;

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
            PickBackupAsync();
        }

        private async void PickBackupAsync()
        {
            var dlg = new CommonOpenFileDialog() {
                Title = "Choose an iDevice Backup",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                var folder = dlg.FileName;
                var manifest = Path.Combine(folder, "Manifest.db");

                if (!System.IO.File.Exists(manifest)) {
                    await this.ShowMessageAsync(
                        "Not a backup folder",
                        "The folder you chose does not appear to be a backup folder (missing manifest).",
                        MessageDialogStyle.Affirmative
                    );
                } else {
                    await PopulateBackupAsync(folder, manifest);
                }
            }
        }

        private async Task PopulateBackupAsync(string folder, string manifest)
        {
            var db = new SQLiteAsyncConnection(manifest);
            var files = await db.QueryAsync<AppleFile>("SELECT * FROM Files");

            var root = new Core.Models.Directory("<Root>", null);
            var misc = new Core.Models.Directory("Miscellaneous Files", null);

            files.Where(x => x.RelativePath == "")
                 .Select(x => new Core.Models.File(x.Domain, Path.Combine(folder, x.FileID.Substring(0, 2), x.FileID)))
                 .ToList()
                 .ForEach(misc.Children.Add);

            root.Children.Add(misc);

            var filesWithPath = files.Where(x => !string.IsNullOrWhiteSpace(x.RelativePath));

            foreach (var file in filesWithPath) {
                var diskPath = Path.Combine(folder, file.FileID.Substring(0, 2), file.FileID);

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
