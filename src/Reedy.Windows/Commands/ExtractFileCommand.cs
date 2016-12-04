using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace Reedy.Commands
{
    class ExtractFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var file = (Core.Models.File)parameter;
            var fi = new FileInfo(file.Path);

            return fi.Exists && fi.Length > 0;
        }

        public void Execute(object parameter)
        {
            var file = (Core.Models.File)parameter;

            var dlg = new CommonSaveFileDialog() {
                Title = "Where do you want to save this file?",
                AddToMostRecentlyUsedList = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                DefaultFileName = file.Name,
                EnsureValidNames = true,
                DefaultDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Desktop),
                ShowPlacesList = true
            };

            var mainWindow = ((MetroWindow)Application.Current.MainWindow);

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                var target = dlg.FileName;
                try {
                    File.Copy(target, file.Path, true);
                    mainWindow.ShowMessageAsync("File saved!", $"Saved {file.Name} successfully to {target}.");
                } catch (Exception e) {
                    mainWindow.ShowMessageAsync("File failed to save!", $"{file.Name} failed to save to {target}: {e.Message}");
                }
            }
        }
    }
}
