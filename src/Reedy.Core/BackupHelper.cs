using System;
using System.Collections.Generic;
using System.IO;

using Xamarin.MacDev;

using Reedy.Core.Models;

using IODirectory = System.IO.Directory;
using IOFile = System.IO.File;

namespace Reedy.Core
{
    public static class BackupHelper
    {
        public static IEnumerable<Backup> EnumerateBackups(bool isMac)
        {
            string backupRootPath;

            if (isMac) {
                backupRootPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "Library",
                    "ApplicationSupport",
                    "MobileSync",
                    "Backup"
                );
            } else {
                backupRootPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Apple Computer",
                    "MobileSync",
                    "Backup"
                );
            }

            foreach (var backupDirectory in IODirectory.EnumerateDirectories (backupRootPath)) {
                // Check if this is a backup we can deal with. It has to have an Info.plist and a Manifest.db.
                var plistFile = Path.Combine(backupDirectory, "Info.plist");
                var manifest = Path.Combine(backupDirectory, "Manifest.db");

                if (!IOFile.Exists(plistFile) || !IOFile.Exists(manifest))
                    continue;

                var plist = PDictionary.FromFile(plistFile);
                var backup = new Backup {
                    BackupPath = backupDirectory,
                    DeviceName = plist.GetString("Device Name"),
                    OSVersion = Version.Parse(plist.GetString("Product Version")),
                    ProductName = plist.GetString("Product Name"),
                    ProductType = plist.GetString("Product Type"),
                    SerialNumber = plist.GetString("Serial Number"),
                    BackupTime = plist.Get<PDate>("Last Backup Date").Value,
                };

                yield return backup;
            }
        }
    }
}
