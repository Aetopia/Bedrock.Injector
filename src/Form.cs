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
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        Controls.Add(_);

        ListView listView = new()
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            HeaderStyle = ColumnHeaderStyle.None,
            CheckBoxes = true
        };
        listView.Columns.Add(new ColumnHeader() { Width = -2 });

        TableLayoutPanel tableLayoutPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        Button button1 = new() { Text = "Add", Dock = DockStyle.Fill };

        Button button2 = new() { Text = "Remove", Dock = DockStyle.Fill };

        Button button3 = new() { Text = "Launch", Dock = DockStyle.Fill };

        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.ColumnStyles.Add(new() { SizeType = SizeType.Percent, Width = 50 });
        tableLayoutPanel.Controls.Add(button1, 0, 0);
        tableLayoutPanel.Controls.Add(button2, 1, 0);

        _.RowStyles.Add(new() { SizeType = SizeType.Percent, Height = 100 });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.RowStyles.Add(new() { SizeType = SizeType.AutoSize });
        _.Controls.AddRange([listView, tableLayoutPanel, button3]);

        button1.Click += (_, _) =>
        {
            if (Dialog.ShowDialog() is DialogResult.OK)
                foreach (var item in Dialog.FileNames)
                    if (!listView.Items.ContainsKey(item))
                        listView.Items.Add(new ListViewItem() { Text = item, Name = item });
        };

        button2.Click += (_, _) =>
        {
            foreach (ListViewItem item in listView.CheckedItems)
                listView.Items.RemoveByKey(item.Name);
        };

        button3.Click += async (_, _) =>
        {
            button1.Enabled = button2.Enabled = button3.Enabled = false;
            await Task.Run(() => Injector.Launch(listView.Items.Cast<ListViewItem>().Select(_ => _.Name)));
            button1.Enabled = button2.Enabled = button3.Enabled = true;
        };

        Closed += async (_, _) =>
        {
            using StreamWriter writer = new(new FileStream("Paths.txt", FileMode.Create));
            foreach (ListViewItem item in listView.Items) await writer.WriteLineAsync(item.Name);

        };

        Load += async (_, _) =>
        {
            try
            {
                using StreamReader reader = File.OpenText("Paths.txt");
                string item = default;
                while ((item = await reader.ReadLineAsync()) is not null)
                    if (!listView.Items.ContainsKey(item = item.Trim()) && !string.IsNullOrEmpty(item))
                        listView.Items.Add(new ListViewItem() { Text = item, Name = item });
            }
            catch (IOException) { }
        };
    }
}