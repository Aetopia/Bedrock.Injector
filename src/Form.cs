using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minecraft.Extensions;

sealed class Form : System.Windows.Forms.Form
{
    internal Form()
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(".ico")) Icon = new(stream);
        Text = "Bedrock Injector";
        Font = SystemFonts.MessageBoxFont;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        ClientSize = new(400, 300);
        MaximizeBox = MinimizeBox = false
        ;
        OpenFileDialog dialog = new()
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = true,
            DereferenceLinks = true,
            Filter = "Dynamic-Link Libraries (*.dll)|*.dll"
        };

        TableLayoutPanel _ = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = default
        };
        Controls.Add(_);

        MenuStrip toolStrip = new() { Dock = DockStyle.Fill, AutoSize = true, GripStyle = ToolStripGripStyle.Hidden };
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.Add(toolStrip);

        ToolStripButton toolStripButton1 = new() { Text = "📂", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton2 = new() { Text = "🗑️", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton3 = new() { Text = "✔️", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton4 = new() { Text = "🔺", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton5 = new() { Text = "🔻", AutoSize = true, AutoToolTip = false, Margin = default };
        toolStrip.Items.AddRange([toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5]);

        CheckedListBox listBox = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, AutoSize = true, Margin = default };
        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.Controls.Add(listBox);

        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = default
        };

        Button button1 = new() { Text = "▶", Dock = DockStyle.Fill, Margin = default };
        Button button2 = new() { Text = "⬛", Dock = DockStyle.Fill, Margin = default };
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.Controls.Add(button1, 0, 0);
        tableLayoutPanel.Controls.Add(button2, 1, 0);

        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.Add(tableLayoutPanel);

        toolStripButton1.Click += (_, _) =>
        {
            if (dialog.ShowDialog() is DialogResult.OK)
                foreach (var item in dialog.FileNames)
                    if (!listBox.Items.Contains(item))
                        listBox.Items.Add(item);
        };

        toolStripButton2.Click += (_, _) =>
        {
            for (var index = listBox.CheckedItems.Count - 1; index >= 0; index--)
                listBox.Items.Remove(listBox.CheckedItems[index]);
            toolStripButton3.Text = "✔️";
        };

        toolStripButton3.Click += (_, _) =>
        {
            var value = listBox.CheckedItems.Count != listBox.Items.Count;
            for (int index = default; index < listBox.Items.Count; index++)
                listBox.SetItemChecked(index, value);
        };

        void Reorder(bool _)
        {
            var index = listBox.SelectedIndex;
            if (index is -1) return;

            var item = listBox.SelectedItem;
            var value = listBox.GetItemChecked(index);

            listBox.Items.RemoveAt(_ ? index++ : index--);
            listBox.Items.Insert(index, item);
            listBox.SetItemChecked(index, value);
            listBox.SelectedIndex = index;
        }

        toolStripButton4.Click += (_, _) =>
        {
            if (listBox.SelectedIndex > 0)
                Reorder(false);
        };

        toolStripButton5.Click += (_, _) =>
        {
            if (listBox.SelectedIndex < listBox.Items.Count - 1)
                Reorder(true);
        };

        async Task LaunchAsync()
        {
            _.Enabled = false;
            await Loader.LaunchAsync(listBox.Items.Cast<string>());
            _.Enabled = true;
        }

        button1.Click += async (_, _) => await LaunchAsync();

        button2.Click += async (_, _) =>
        {
            await LaunchAsync();
            Close();
        };

        listBox.ItemCheck += (_, e) => BeginInvoke(() => { toolStripButton3.Text = listBox.CheckedItems.Count == listBox.Items.Count ? "❌" : "✔️"; });

        Closed += async (_, _) =>
        {
            using (StreamWriter writer = new(new FileStream("Bedrock.Injector.txt", FileMode.Create)))
                foreach (string item in listBox.Items) await writer.WriteLineAsync(item);
            Environment.Exit(default);
        };

        Shown += async (_, _) =>
        {
            _.Enabled = false;
            try
            {
                using StreamReader reader = File.OpenText("Bedrock.Injector.txt");
                string item = default;
                while ((item = await reader.ReadLineAsync()) is not null)
                    if (!listBox.Items.Contains(item = item.Trim()) && !string.IsNullOrEmpty(item))
                        listBox.Items.Add(item);
            }
            catch (IOException) { }
            _.Enabled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var exception = (Exception)e.ExceptionObject;
            while (exception.InnerException is not null) exception = exception.InnerException;
            MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        };
    }
}