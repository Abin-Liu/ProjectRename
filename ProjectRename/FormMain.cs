using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MFGLib;

namespace ProjectRename
{
	public partial class FormMain : Form
	{
		private string m_sourceFolder = null;

		public FormMain()
		{
			InitializeComponent();

			using (RegistryHelper reg = new RegistryHelper("Abin", Application.ProductName, false))
			{
				m_sourceFolder = reg.ReadString("Source Folder");
			}

			if (!string.IsNullOrWhiteSpace(m_sourceFolder))
			{
				txtSourceFolder.Text = m_sourceFolder;
				txtSaveTo.Focus();
			}

			this.CenterToScreen();
			
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			DialogResult dr = folderBrowserDialog1.ShowDialog();
			if (dr != DialogResult.OK || m_sourceFolder == folderBrowserDialog1.SelectedPath)
			{
				return;
			}

			m_sourceFolder = folderBrowserDialog1.SelectedPath;
			txtSourceFolder.Text = m_sourceFolder;

			using (RegistryHelper reg = new RegistryHelper("Abin", Application.ProductName, true))
			{
				reg.WriteString("Source Folder", m_sourceFolder);
			}			
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(m_sourceFolder))
			{
				MessageBox.Show(this, "Please select source folder.");
				return;
			}

			string saveTo = txtSaveTo.Text;
			if (string.IsNullOrWhiteSpace(saveTo))
			{
				MessageBox.Show(this, "Please select save to.");
				return;
			}

			btnStart.Enabled = false;

			string error = "";
			if (!PrjProcess.Process(m_sourceFolder, saveTo, out error))
			{
				MessageBox.Show(this, saveTo + " already exists.");
				btnStart.Enabled = true;
				return;
			}

			MessageBox.Show(this, "Project saved to " + saveTo + " successfully.");
			System.Diagnostics.Process.Start("explorer.exe", error);
			this.Close();
		}
	}
}
