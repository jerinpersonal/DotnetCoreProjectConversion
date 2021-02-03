using System;

namespace com.infosys.migration
{


	public class Migrator
	{

		internal FileOperationHelper helper = new FileOperationHelper();
		public virtual void migrateFiles(File directory)
		{
			Console.WriteLine(" Starting the processing....");

			File[] files = directory.listFiles();
			foreach (File file in files)
			{

				if (!file.Directory)
				{
				   handleMvc(file);
				   handleBind(file);
				   handleSelectList(file);
				   handleHttpStatusCode(file);
				   handleRequestUrlScheme(file);
				   handleRenderPartialRequest(file);
				   deleteAssemblyInfo(file);
				   deleteBundleConfig(file);
				   deleteFilterConfig(file);
				   deleteRouteConfig(file);

				}
				else
				{
					migrateFiles(file);
				}
			}
		}


		private void handleMvc(File file)
		{

			helper.replaceInFile(file, "System.Web.Mvc","Microsoft.AspNetCore.Mvc",false);
		}

		private void handleBind(File file)
		{

			helper.replaceInFile(file, "Bind(Include =","Bind(",false);

		}
		private void handleSelectList(File file)
		{

			 if (helper.searchInFile(file,"SelectList"))
			 {
				 helper.writeToFile(file,"using Microsoft.AspNetCore.Mvc.Rendering;");
			 }

		}

		private void handleHttpStatusCode(File file)
		{
			helper.replaceInFile(file,"HttpStatusCodeResult(HttpStatusCode.BadRequest)","BadRequest()",false);
			helper.replaceInFile(file,"HttpNotFound","NotFound",false);
		}

		private void handleRequestUrlScheme(File file)
		{
			helper.replaceInFile(file,"Request.Url.Scheme","Request.Scheme",false);

		}

		private void handleRenderPartialRequest(File file)
		{
			helper.replaceInFile(file,"@Html.Partial","@Html.RenderPartialAsync",false);
		}

		private void deleteAssemblyInfo(File file)
		{
			if (file.Name.equalsIgnoreCase("AssemblyInfo.cs"))
			{
				file.delete();
			}

		}
		private void deleteBundleConfig(File file)
		{
			if (file.Name.equalsIgnoreCase("BundleConfig.cs"))
			{
				file.delete();
			}

		}
		private void deleteFilterConfig(File file)
		{
			if (file.Name.equalsIgnoreCase("FilterConfig.cs"))
			{
				file.delete();
			}

		}
		private void deleteRouteConfig(File file)
		{
			if (file.Name.equalsIgnoreCase("RouteConfig.cs"))
			{
				file.delete();
			}

		}

		public virtual void copyStartupFilesForCore(string baseDirectoryPath)
		{
			try
			{
				File programFile = new File("./ref/Program.cs");
				File startUpFile = new File("./ref/Startup.cs");
				Files.copy(programFile.toPath(), (new File(baseDirectoryPath + programFile.Name)).toPath(), StandardCopyOption.REPLACE_EXISTING);
				Files.copy(startUpFile.toPath(), (new File(baseDirectoryPath + startUpFile.Name)).toPath(), StandardCopyOption.REPLACE_EXISTING);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

		}

		public static void Main(string[] args)
		{
			Migrator proj = new Migrator();
			proj.migrateFiles(new File("D:\\workspace\\rackathon\\code"));
			proj.copyStartupFilesForCore("D:\\workspace\\rackathon\\code\\ASPDotNetMVCLegacy-master\\src\\eShopLegacyMVC\\");
		}

	}
}
