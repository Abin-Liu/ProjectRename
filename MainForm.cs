using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MFGLib;
using ToolkitForms;

namespace ProjectRename
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			RegistryHelper reg = new RegistryHelper();
			reg.Open("Abin", ProductName, false);
			string path = reg.ReadString("Saved Path");
			reg.Close();

			if (!Directory.Exists(path))
			{
				path = Path.GetDirectoryName(path);
				if (!Directory.Exists(path))
				{
					path = "";
				}
			}

			txtFolder.Text = path;
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			OpenFolderDialog dlg = new OpenFolderDialog();
			if (Directory.Exists(txtFolder.Text))
			{
				dlg.SelectedPath = txtFolder.Text;
			}
			else
			{
				dlg.SelectedPath = Path.GetDirectoryName(txtFolder.Text);
			}
			
			if (dlg.ShowDialog(this) != DialogResult.OK)
			{
				return;
			}			

			txtFolder.Text = dlg.SelectedPath;

			RegistryHelper reg = new RegistryHelper();
			reg.Open("Abin", ProductName, true);
			reg.WriteString("Saved Path", dlg.SelectedPath);
			reg.Close();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			RenameHelper helper = new RenameHelper();
			helper.ProjectFolder = txtFolder.Text;
			helper.OldName = txtOldName.Text;
			helper.NewName = txtNewName.Text;

			TaskForm form = new TaskForm();
			form.ParameterTaskProc = Process;
			form.Parameter = helper;
			if (form.ShowDialog(this) == DialogResult.OK)
			{
				string message = string.Format("Project renamed successfully.\nTime taken: {0}s\nFolders altered: {1}\nFiles altered: {2}",
					helper.TimeTaken, helper.AlteredFolders, helper.AlteredFiles);
				MessageForm.Info(this, message);
			}
			else
			{
				MessageForm.Error(this, form.Error);
			}
		}		

		private void Process(object param)
		{
			RenameHelper helper = param as RenameHelper;
			helper.Process();
		}

		private void txtFolder_TextChanged(object sender, EventArgs e)
		{
			txtOldName.Text = Path.GetFileName(txtFolder.Text);			
		}
	}
}
