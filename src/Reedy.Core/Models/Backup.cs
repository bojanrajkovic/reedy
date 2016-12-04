using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reedy.Core.Models
{
    public class Backup
    {
        public string DeviceName { get; internal set; }
        public string ProductName { get; internal set; }
        public string ProductType { get; internal set; }
        public Version OSVersion { get; internal set; }
        public string SerialNumber { get; internal set; }
        public string BackupPath { get; internal set; }
        public DateTimeOffset BackupTime { get; internal set; }

        public override string ToString() => $"{DeviceName} ({ProductName}, iOS {OSVersion}, taken " +
            $"on {BackupTime.LocalDateTime.ToShortDateString()} at {BackupTime.LocalDateTime.ToShortTimeString()})";
    }
}
