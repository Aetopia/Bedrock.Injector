using System;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.ProcessPath));
        
        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetColorMode(SystemColorMode.Dark);
        Application.Run(new Form());
    }
}