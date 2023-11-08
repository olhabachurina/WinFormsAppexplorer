using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WinFormsAppexplorer
{
    public partial class Form1 : Form
    {
        private TreeView diskTreeView;
        private ListView fileListView;
        private TextBox addressTextBox;
        private ContextMenuStrip contextMenu;
        public Form1()
        {
            InitializeComponent();
            diskTreeView = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 200
            };
            diskTreeView.AfterSelect += DiskTreeView_AfterSelect;

            fileListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details
            };
            fileListView.Columns.Add("Имя файла", 200);
            fileListView.Columns.Add("Тип", 100);
            fileListView.Columns.Add("Размер", 100);
            fileListView.DoubleClick += FileListView_DoubleClick;

            addressTextBox = new TextBox
            {
                Dock = DockStyle.Top
            };
            addressTextBox.KeyDown += AddressTextBox_KeyDown;

            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Открыть");
            openMenuItem.Click += OpenMenuItem_Click;
            contextMenu.Items.Add(openMenuItem);

            // Создание формы
            Controls.Add(fileListView);
            Controls.Add(diskTreeView);
            Controls.Add(addressTextBox);

            Text = "Проводник";
            Width = 800;
            Height = 600;

            // Заполнение дерева дисков
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable)
                {
                    TreeNode driveNode = new TreeNode(drive.Name);
                    driveNode.Tag = drive.RootDirectory;
                    diskTreeView.Nodes.Add(driveNode);
                }
            }
        }

        private void DiskTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedPath = (e.Node.Tag as DirectoryInfo)?.FullName;
            if (!string.IsNullOrEmpty(selectedPath))
            {
                addressTextBox.Text = selectedPath;
                PopulateFileListView(selectedPath);
            }
        }

        private void FileListView_DoubleClick(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                string selectedPath = Path.Combine(addressTextBox.Text, fileListView.SelectedItems[0].Text);
                if (File.Exists(selectedPath))
                {
                    Process.Start(selectedPath);
                }
                else if (Directory.Exists(selectedPath))
                {
                    addressTextBox.Text = selectedPath;
                    PopulateFileListView(selectedPath);
                }
            }
        }

        private void AddressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string enteredPath = addressTextBox.Text;
                if (Directory.Exists(enteredPath))
                {
                    PopulateFileListView(enteredPath);
                }
            }
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count > 0)
            {
                string selectedPath = Path.Combine(addressTextBox.Text, fileListView.SelectedItems[0].Text);
                if (File.Exists(selectedPath))
                {
                    Process.Start(selectedPath);
                }
            }
        }

        private void PopulateFileListView(string directoryPath)
        {
            fileListView.Items.Clear();
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                ListViewItem item = new ListViewItem(subDirectory.Name);
                item.SubItems.Add("Папка");
                item.SubItems.Add("");
                fileListView.Items.Add(item);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                ListViewItem item = new ListViewItem(file.Name);
                item.SubItems.Add("Файл");
                item.SubItems.Add(file.Length.ToString() + " байт");
                fileListView.Items.Add(item);
            }
        }

    }
}