using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectRename
{
	class RenameHelper
	{
		public string ProjectFolder { get; set; }
		public string OldName { get; set; }
		public string NewName { get; set; }		

		public int TimeTaken => (int)(m_endTime - m_startTime).TotalSeconds;
		public int AlteredFolders { get; private set; }
		public int AlteredFiles { get; private set; }

		DateTime m_startTime;
		DateTime m_endTime;

		// 新name中不允许使用的字符
		const string InvalidChars = "\\/:*?\"<>| ";
		static readonly string[] FolderBlackList = { ".git", ".vs", "obj", "bin", "node_modules", "dist", "packages", "libs" };
		static readonly string[] TextFileExtList = { ".sln", ".csproj", ".config", ".cs", ".asax", ".cshtml", ".xml", ".html", ".settings", ".resx" };

		public void Process()
		{
			ProjectFolder = ProjectFolder.Trim();
			if (ProjectFolder == "")
			{
				throw new Exception("Project folder is required.");
			}

			if (!Directory.Exists(ProjectFolder))
			{
				throw new DirectoryNotFoundException("Invalid project folder.");
			}

			OldName = OldName.Trim();
			NewName = NewName.Trim();

			if (OldName == "")
			{
				throw new Exception("Original name is required.");
			}

			if (NewName == "")
			{
				throw new Exception("New name is required.");
			}

			if (OldName == NewName)
			{
				throw new Exception("New name is same as original name.");
			}			

			foreach (char c in NewName)
			{
				if (InvalidChars.IndexOf(c) != -1)
				{
					throw new Exception("New name cannot contain: " + InvalidChars);
				}
			}						

			AlteredFolders = 0;
			AlteredFiles = 0;
			m_startTime = DateTime.Now;
			m_endTime = m_startTime.AddSeconds(-1);
			RenameProc(new DirectoryInfo(ProjectFolder));
			m_endTime = DateTime.Now;
		}

		void RenameProc(DirectoryInfo di)
		{
			if (FolderBlackList.Contains(di.Name.ToLower()))
			{
				return;
			}			

			FileInfo[] fileList = di.GetFiles();
			foreach (FileInfo fi in fileList)
			{
				bool fileChanged = ReplaceFileContents(fi);

				string name = Path.GetFileNameWithoutExtension(fi.FullName);
				if (string.Compare(name, OldName, true) == 0)
				{
					fi.MoveTo(fi.DirectoryName + '\\' + NewName + fi.Extension);
					fileChanged = true;
				}

				if (fileChanged)
				{
					AlteredFiles++;
				}
			}

			DirectoryInfo[] children = di.GetDirectories();
			foreach (DirectoryInfo child in children)
			{
				RenameProc(child);
			}

			if (string.Compare(di.Name, OldName, true) == 0)
			{
				di.MoveTo(di.Parent.FullName + '\\' + NewName);
				AlteredFolders++;
			}
		}

		bool ReplaceFileContents(FileInfo fi)
		{
			if (!TextFileExtList.Contains(fi.Extension.ToLower()))
			{
				return false;
			}

			string text = File.ReadAllText(fi.FullName);
			if (!text.Contains(OldName))
			{
				return false;
			}

			text = text.Replace(OldName, NewName);
			File.WriteAllText(fi.FullName, text, Encoding.UTF8);
			return true;
		}
	}
}
