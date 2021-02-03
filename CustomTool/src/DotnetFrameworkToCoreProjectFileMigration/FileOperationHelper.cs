using System;
using System.IO;

/// 
namespace com.infosys.migration
{

	/// <summary>
	/// @author Vinod_S08
	/// 
	/// </summary>
	public class FileOperationHelper
	{

		public virtual bool searchInFile(File file, string key)
		{
			bool isKeyFound = false;
			try
			{

				StreamReader reader = new StreamReader(file);
				string k = null;
				while (!string.ReferenceEquals((k = reader.ReadLine()), null))
				{
					if (k.Contains(key))
					{
						isKeyFound = true;
						break;
					}
				}
				reader.Close();

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Console.WriteLine("In the search file " + file.Name + "  and key is " + key + "  and isKeyFound is" + isKeyFound);
			return isKeyFound;
		}

		public virtual void writeToFile(File file, string statement)
		{
			try
			{
				string renameFileName = file.AbsolutePath;
				string[] split = splitFileNameWithExtension(file);
				File newFile = new File(split[0] + "_migrator." + split[1]);
				PrintWriter writer = new PrintWriter(new StreamWriter(newFile,true));
				StreamReader reader = new StreamReader(file);
				string k = null;
				writer.println(statement);
				while (!string.ReferenceEquals((k = reader.ReadLine()), null))
				{
					writer.println(k);
				}
				reader.Close();
				writer.flush();
				writer.close();
				bool b = file.delete();
				Console.WriteLine("File is deleted" + b);
				File renameFile = new File(renameFileName);
				newFile.renameTo(renameFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		public virtual void replaceInFile(File file, string toBeReplaced, string replacement, bool isRegEx)
		{
			Console.WriteLine(" In the replaceInFile...." + file.Name + "," + toBeReplaced + "," + replacement);
			try
			{
				string renameFileName = file.AbsolutePath;
				string[] split = splitFileNameWithExtension(file);
				File newFile = new File(split[0] + "_migrator." + split[1]);
				StreamReader reader = new StreamReader(file);

				PrintWriter writer = new PrintWriter(new StreamWriter(newFile,true));
				string k = null;
				while (!string.ReferenceEquals((k = reader.ReadLine()), null))
				{
					//System.out.println("k="+k);    			
					if (isRegEx)
					{
						if (k.matches(toBeReplaced))
						{
							k = k.Replace(toBeReplaced, replacement);
							Console.WriteLine("k is replaced ");
						}
					}
					else
					{
						if (k.Contains(toBeReplaced))
						{
							k = k.Replace(toBeReplaced, replacement);
							Console.WriteLine("k is replaced ");
						}
					}
					writer.println(k);
				}
				reader.Close();
				writer.flush();
				writer.close();
				file.delete();
				File renameFile = new File(renameFileName);
				newFile.renameTo(renameFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private string[] splitFileNameWithExtension(File file)
		{

			string[] fileNameSplitArray = new string[2];
			string fileName = file.AbsolutePath;
			int finalDotIdx = fileName.LastIndexOf(".", StringComparison.Ordinal);
			fileNameSplitArray[0] = fileName.Substring(0, finalDotIdx);
			fileNameSplitArray[1] = fileName.Substring(finalDotIdx + 1);
			return fileNameSplitArray;
		}
	}
}
