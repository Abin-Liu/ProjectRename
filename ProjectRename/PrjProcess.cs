using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MFGLib;

namespace ProjectRename
{
	class PrjProcess
	{
		public static bool Process(string folder, string newName, out string error)
		{
			error = "";
			string oldName;
			try
			{
				oldName = GetProjectName(folder);
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}

			if (oldName == null)
			{
				error = "Folder " + folder + " not exists.";
				return false;
			}

			string newFolder = GetDirectory(folder) + '\\' + newName;
			if (Directory.Exists(newFolder))
			{
				error = "Folder " + newFolder + " already exists.";
				return false;
			}

			try
			{
				Directory.Move(folder, newFolder);
			}
			catch (Exception e)
			{
				error = e.Message;
				return false;
			}

			error = newFolder;

			folder = newFolder;

			File.Move(folder + '\\' + oldName + ".sln", folder + '\\' + newName + ".sln");
			ReplaceFileContent(folder + '\\' + newName + ".sln", oldName, newName);

			Directory.Move(folder + '\\' + oldName, folder + '\\' + newName);
			folder = folder + '\\' + newName;

			File.Move(folder + '\\' + oldName + ".csproj", folder + '\\' + newName + ".csproj");
			File.Move(folder + '\\' + oldName + ".csproj.user", folder + '\\' + newName + ".csproj.user");

			ReplaceFileContent(folder + '\\' + newName + ".csproj", oldName, newName);
			ReplaceFileContent(folder + "\\Views\\Web.config", oldName, newName);
			ReplaceFileContent(folder + "\\Global.asax", oldName, newName);
			ReplaceFileContent(folder + "\\Global.asax.cs", oldName, newName);
			ReplaceFolderFilesContent(folder + "\\Properties", oldName, newName);
			ReplaceFolderFilesContent(folder + "\\App_Start", oldName, newName);
			ReplaceFolderFilesContent(folder + "\\Controllers", oldName, newName);
			ReplaceFolderFilesContent(folder + "\\Filters", oldName, newName);
			ReplaceFolderFilesContent(folder + "\\Models", oldName, newName);

			DeleteOldFiles(folder + "\\bin", oldName);
			DeleteOldFiles(folder + "\\bin\\Debug", oldName);
			DeleteOldFiles(folder + "\\bin\\Release", oldName);

			ReplaceFileBlock(folder + "\\Properties\\AssemblyInfo.cs", "[assembly: Guid(\"", "\"", Guid.NewGuid().ToString());
			ReplaceFileBlock(folder + '\\' + newName + ".csproj", "<ProjectGuid>{", "}</ProjectGuid>", Guid.NewGuid().ToString());

			DeleteUnnecessaryItems(folder);

			return true;
		}

		public static void ReplaceFileBlock(string pathName, string prefix, string surfix, string replaceWith)
		{
			string content = File.ReadAllText(pathName);
			content = StringHelper.ReplaceBlock(content, prefix, surfix, replaceWith);
			File.WriteAllText(pathName, content, Encoding.UTF8);
		}

		private static string GetDirectory(string path)
		{
			int pos = path.LastIndexOf('\\');
			if (pos == -1)
			{
				return "";
			}

			return path.Substring(0, pos);
		}

		private static void ReplaceGUID(string filePath)
		{
			string contents = File.ReadAllText(filePath);
			int pos = contents.IndexOf("[assembly: Guid(\"");
			if (pos == -1)
			{
				return;
			}

			string oldGuid = contents.Substring(pos + "[assembly: Guid(\"".Length, 36);
			contents = contents.Replace(oldGuid, Guid.NewGuid().ToString());
			File.WriteAllText(filePath, contents, Encoding.UTF8);
		}

		private static string GetProjectName(string folder)
		{
			DirectoryInfo dir = new DirectoryInfo(folder);
			FileSystemInfo[] fis = dir.GetFileSystemInfos();
			foreach (FileSystemInfo fi in fis)
			{
				if (fi is FileInfo)
				{
					if (fi.Extension.ToLower() == ".sln")
					{
						string name = fi.Name;
						return name.Substring(0, name.Length - 4);
					}
				}
			}

			return null;
		}

		private static void ReplaceFolderFilesContent(string folderPath, string source, string dest)
		{
			try
			{
				DirectoryInfo dir = new DirectoryInfo(folderPath);
				FileSystemInfo[] fis = dir.GetFileSystemInfos();
				foreach (FileSystemInfo fi in fis)
				{
					if (fi is FileInfo)
					{
						ReplaceFileContent(fi.FullName, source, dest);
					}
				}
			}
			catch
			{
			}			
		}

		private static void ReplaceFileContent(string filePath, string source, string dest)
		{
			try
			{
				string contents = File.ReadAllText(filePath);
				contents = contents.Replace(source, dest);
				File.WriteAllText(filePath, contents, Encoding.UTF8);
			}
			catch
			{
			}			
		}

		private static void DeleteOldFiles(string folderPath, string name)
		{
			try
			{
				DirectoryInfo dir = new DirectoryInfo(folderPath);
				FileSystemInfo[] fis = dir.GetFileSystemInfos();
				foreach (FileSystemInfo fi in fis)
				{
					if (fi is FileInfo)
					{
						string ext = fi.Extension;
						string title = fi.Name.Substring(0, fi.Name.Length - ext.Length);
						if (title == name)
						{
							fi.Delete();
						}
					}
				}
			}
			catch
			{
			}			
		}

		private static void DeleteUnnecessaryItems(string folderPath)
		{
			int pos = folderPath.LastIndexOf('\\');
			if (pos == -1)
			{
				return;
			}

			folderPath = folderPath.Substring(0, pos);

			try
			{
				Directory.Delete(folderPath + "\\.vs", true);
			}
			catch
			{
			}

			try
			{
				Directory.Delete(folderPath + "\\.svn", true);
			}
			catch
			{
			}
			
			try
			{
				Directory.Delete(folderPath + "\\.git", true);				
			}
			catch
			{
			}

			try
			{
				File.Delete(folderPath + "\\.gitattributes");
			}
			catch
			{
			}

			try
			{
				File.Delete(folderPath + "\\README.md");
			}
			catch
			{
			}
		}
	}
}
