using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bedrockix.Minecraft;

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
        MaximizeBox = MinimizeBox = false;

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
        ToolStripButton toolStripButton4 = new() { Text = "❌", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton5 = new() { Text = "🔺", AutoSize = true, AutoToolTip = false, Margin = default };
        ToolStripButton toolStripButton6 = new() { Text = "🔻", AutoSize = true, AutoToolTip = false, Margin = default };
        toolStrip.Items.AddRange([toolStripButton1, toolStripButton2, toolStripButton3, toolStripButton4, toolStripButton5, toolStripButton6]);

        ListView listView = new()
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            AutoSize = true,
            Margin = default,
            View = View.Details,
            CheckBoxes = true,
            HeaderStyle = ColumnHeaderStyle.None
        };
        listView.Columns.Add(new ColumnHeader() { Width = -2 });
        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.Controls.Add(listView);

        Button button = new() { Text = "▶", Dock = DockStyle.Fill, Margin = default };
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.Add(button);

        toolStripButton1.Click += (_, _) =>
        {
            if (dialog.ShowDialog() is DialogResult.OK)
                foreach (var item in dialog.FileNames)
                    if (!listView.Items.ContainsKey(item))
                        listView.Items.Add(new ListViewItem { Text = Path.GetFileNameWithoutExtension(item), Name = item });
            listView.Columns[default(int)].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        };

        toolStripButton2.Click += (_, _) =>
        {
            foreach (ListViewItem item in listView.CheckedItems)
                listView.Items.Remove(item);
        };

        void Select(bool _) { foreach (ListViewItem item in listView.Items) item.Checked = _; }

        toolStripButton3.Click += (_, _) => Select(true);

        toolStripButton4.Click += (_, _) => Select(false);

        void Reorder(bool _)
        {
            var index = listView.SelectedIndices[default];
            var item = listView.SelectedItems[default(int)];
            listView.Items.RemoveAt(_ ? index++ : index--);
            listView.Items.Insert(index, item);
        }

        toolStripButton5.Click += (_, _) => { if (listView.SelectedIndices.Count is not default(int) && listView.SelectedIndices[default] > default(int)) Reorder(false); };

        toolStripButton6.Click += (_, _) => { if (listView.SelectedIndices.Count is not default(int) && listView.SelectedIndices[default] < listView.Items.Count - 1) Reorder(true); };


        button.Click += async (_, _) =>
        {
            if (Game.Installed)
            {
                MessageBox.Show(this, "Minecraft: Bedrock Edition isn't installed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            _.Enabled = false;
            await Task.Run(() => Loader.Launch(listView.Items.Cast<ListViewItem>().Select(_ => _.Name)));
            _.Enabled = true;
        };

        Closed += (_, _) =>
        {
            using (StreamWriter writer = new(new FileStream("Bedrock.Injector.txt", FileMode.Create)))
                foreach (string item in listView.Items.Cast<ListViewItem>().Select(_ => _.Name)) writer.WriteLine(item);
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
                    if (!string.IsNullOrEmpty(item) && !listView.Items.ContainsKey(item))
                        listView.Items.Add(new ListViewItem { Text = Path.GetFileNameWithoutExtension(item), Name = item });
                listView.Columns[default(int)].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
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