using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        ((NameValueCollection)ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection"))["DpiAwareness"] = "PerMonitorV2";

        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form());
    }
}