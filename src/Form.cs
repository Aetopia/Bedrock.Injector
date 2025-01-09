using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

sealed class Form : System.Windows.Forms.Form
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
        Text = "Bedrock Injector";
        Font = SystemFonts.MessageBoxFont;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ClientSize = LogicalToDeviceUnits(new Size(400, 300));
        MaximizeBox = MinimizeBox = false;

        Application.ThreadException += (_, e) =>
        {
            var exception = e.Exception;
            while (exception.InnerException is not null) exception = exception.InnerException;
            Unmanaged.ShellMessageBox(hWnd: Handle, lpcText: exception.Message);
            Close();
        };

        TableLayoutPanel _ = new() { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        CheckedListBox listBox = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Margin = default };
        TableLayoutPanel tableLayoutPanel1 = new() { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        TableLayoutPanel tableLayoutPanel2 = new() { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = default };
        Button button1 = new() { Text = "📂", Dock = DockStyle.Fill, Margin = default };
        Button button2 = new() { Text = "🗑️", Dock = DockStyle.Fill, Margin = default };
        Button button3 = new() { Text = "🔺", Dock = DockStyle.Fill, Margin = default };
        Button button4 = new() { Text = "🔻", Dock = DockStyle.Fill, Margin = default };
        Button button5 = new() { Text = "▶", Dock = DockStyle.Fill, Margin = default };
        Button button6 = new() { Text = "⬛", Dock = DockStyle.Fill, Margin = default };

        tableLayoutPanel1.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel1.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel1.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel1.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });

        tableLayoutPanel1.Controls.Add(button1, 0, 0);
        tableLayoutPanel1.Controls.Add(button2, 1, 0);
        tableLayoutPanel1.Controls.Add(button3, 2, 0);
        tableLayoutPanel1.Controls.Add(button4, 3, 0);

        tableLayoutPanel2.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel2.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel2.Controls.Add(button5, 0, 0);
        tableLayoutPanel2.Controls.Add(button6, 1, 0);

        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.AddRange([listBox, tableLayoutPanel1, tableLayoutPanel2]);

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
            if (listBox.SelectedIndex > 0)
            {
                var item = listBox.SelectedItem;
                var index = listBox.SelectedIndex;
                listBox.Items.RemoveAt(index--);
                listBox.Items.Insert(index, item);
                listBox.SelectedIndex = index;
            }
        };

        button4.Click += (_, _) =>
        {
            if (listBox.SelectedIndex < listBox.Items.Count - 1)
            {
                var item = listBox.SelectedItem;
                var index = listBox.SelectedIndex;
                listBox.Items.RemoveAt(index++);
                listBox.Items.Insert(index, item);
                listBox.SelectedIndex = index;
            }
        };

        button5.Click += async (_, _) =>
        {
            _.Enabled = false;
            await Task.Run(() => Injector.Launch(listBox.Items.Cast<string>()));
            _.Enabled = true;
        };

        button6.Click += async (_, _) =>
        {
            _.Enabled = false;
            await Task.Run(() => Injector.Launch(listBox.Items.Cast<string>()));
            _.Enabled = true;
            Close();
        };

        Closed += async (_, _) =>
        {
            using StreamWriter writer = new(new FileStream("Bedrock.Injector.txt", FileMode.Create));
            foreach (string item in listBox.Items) await writer.WriteLineAsync(item);
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