using System.Windows.Input;

namespace Reedy.Commands
{
    public static class Commands
    {
        public static ICommand ExtractFileCommand { get; } = new ExtractFileCommand();
    }
}
