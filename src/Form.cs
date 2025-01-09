using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Reflection;

sealed partial class Form : System.Windows.Forms.Form
{
    readonly OpenFileDialog Dialog = new()
    {
        CheckFileExists = true,
        CheckPathExists = true,
        Multiselect = true,
        DereferenceLinks = true,
        Filter = "Dynamic-Link Libraries (*.dll)|*.dll"
    };

    internal Form()
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(".ico")) Icon = new(stream);
        Text = "Bedrock Injector";
        Font = SystemFonts.MessageBoxFont;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ClientSize = new(400, 300);
        MaximizeBox = MinimizeBox = false;

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var exception = (Exception)e.ExceptionObject;
            while (exception.InnerException is not null) exception = exception.InnerException;
            _ = Unmanaged.ShellMessageBox(hWnd: Handle, lpcText: exception.Message);
            Close();
        };

        TableLayoutPanel _ = new() { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        TableLayoutPanel tableLayoutPanel = new() { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        CheckedListBox listBox = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Margin = default };
        Button button1 = new() { Text = "📂", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button2 = new() { Text = "🗑️", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button3 = new() { Text = "✔️", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button4 = new() { Text = "🔺", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button5 = new() { Text = "🔻", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button6 = new() { Text = "▶", Dock = DockStyle.Fill, Margin = default };
        Button button7 = new() { Text = "⬛", Dock = DockStyle.Fill, Margin = default };

        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.AutoSize });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.AutoSize });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.AutoSize });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.AutoSize });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.AutoSize });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 100 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 100 });

        tableLayoutPanel.Controls.Add(button1, 0, default);
        tableLayoutPanel.Controls.Add(button2, 1, default);
        tableLayoutPanel.Controls.Add(button3, 2, default);
        tableLayoutPanel.Controls.Add(button4, 3, default);
        tableLayoutPanel.Controls.Add(button5, 4, default);
        tableLayoutPanel.Controls.Add(button6, 5, default);
        tableLayoutPanel.Controls.Add(button7, 6, default);

        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.AddRange([listBox, tableLayoutPanel]);
        Controls.Add(_);

        button1.Click += (_, _) =>
        {
            if (Dialog.ShowDialog() is DialogResult.OK)
                foreach (var item in Dialog.FileNames)
                    if (!listBox.Items.Contains(item))
                        listBox.Items.Add(item);
        };

        button2.Click += (_, _) =>
        {
            for (var index = listBox.CheckedItems.Count - 1; index >= 0; index--)
                listBox.Items.Remove(listBox.CheckedItems[index]);
        };

        button3.Click += (_, _) =>
        {
            var value = listBox.CheckedItems.Count != listBox.Items.Count;
            for (int index = default; index < listBox.Items.Count; index++)
                listBox.SetItemChecked(index, value);
        };

        void Reorder(bool _)
        {
            var item = listBox.SelectedItem;
            var index = listBox.SelectedIndex;
            var value = listBox.GetItemChecked(index);

            listBox.Items.RemoveAt(_ ? index++ : index--);
            listBox.Items.Insert(index, item);
            listBox.SetItemChecked(index, value);
            listBox.SelectedIndex = index;
        }

        button4.Click += (_, _) => { if (listBox.SelectedIndex > 0) Reorder(false); };

        button5.Click += (_, _) => { if (listBox.SelectedIndex < listBox.Items.Count - 1) Reorder(true); };

        async Task LaunchAsync() { _.Enabled = false; await Task.Run(() => Injector.Launch(listBox.Items.Cast<string>())); _.Enabled = true; }

        button6.Click += async (_, _) => await LaunchAsync();

        button7.Click += async (_, _) => { await LaunchAsync(); Close(); };

        listBox.ItemCheck += (_, e) => BeginInvoke(() => button3.Text = listBox.CheckedItems.Count == listBox.Items.Count ? "❌" : "✔️");

        Closed += async (_, _) =>
        {
            using (StreamWriter writer = new(new FileStream("Bedrock.Injector.txt", FileMode.Create)))
                foreach (string item in listBox.Items) await writer.WriteLineAsync(item);
            Environment.Exit(default);
        };

        Load += async (_, _) =>
        {
            try
            {
                using StreamReader reader = File.OpenText("Bedrock.Injector.txt");
                string item = default;
                while ((item = await reader.ReadLineAsync()) is not null)
                    if (!listBox.Items.Contains(item = item.Trim()) && !string.IsNullOrEmpty(item))
                        listBox.Items.Add(item);
            }
            catch (IOException) { }
        };
    }
}