using System;
using System.IO;
using System.Windows.Forms;
using Bedrock.Injector;
using Minecraft.Bedrock;

static class Program
{
    [STAThread]
    static void Main()
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form());
    }
}