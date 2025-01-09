using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bedrock.Injector;

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
        ClientSize = new(400, 300);
        MaximizeBox = MinimizeBox = false;

        TableLayoutPanel _ = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = default
        };
        Controls.Add(_);

        CheckedListBox listBox = new()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = default
        };

        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = default
        };

        Button button1 = new() { Text = "📂", Dock = DockStyle.Fill, Margin = default };
        Button button2 = new() { Text = "🗑️", Dock = DockStyle.Fill, Margin = default };
        Button button3 = new() { Text = "🔺", Dock = DockStyle.Fill, Margin = default };
        Button button4 = new() { Text = "🔻", Dock = DockStyle.Fill, Margin = default };
        Button button5 = new() { Text = "▶️", Dock = DockStyle.Fill, Margin = default };

        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 25 });

        tableLayoutPanel.Controls.Add(button1, 0, 0);
        tableLayoutPanel.Controls.Add(button2, 1, 0);
        tableLayoutPanel.Controls.Add(button3, 2, 0);
        tableLayoutPanel.Controls.Add(button4, 3, 0);

        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.AddRange([listBox, tableLayoutPanel, button5]);

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

        Closed += async (_, _) =>
        {
            using StreamWriter writer = new(new FileStream("Paths.txt", FileMode.Create));
            foreach (string item in listBox.Items) await writer.WriteLineAsync(item);
        };

        Load += async (_, _) =>
        {
            try
            {
                using StreamReader reader = File.OpenText("Paths.txt");
                string item = default;
                while ((item = await reader.ReadLineAsync()) is not null)
                    if (!listBox.Items.Contains(item = item.Trim()) && !string.IsNullOrEmpty(item))
                        listBox.Items.Add(item);
            }
            catch (IOException) { }
        };
    }
}