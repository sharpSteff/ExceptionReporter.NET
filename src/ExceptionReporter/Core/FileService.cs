﻿using System;
using System.IO;

namespace ExceptionReporting.Core
{
	internal interface IFileService 
	{
		bool Exists(string file);
		string TempFile(string file);
		FileSaveResult Write(string file, string report);
	}

	internal class FileSaveResult
	{
		public bool Saved { get; set; }
		public Exception Exception { get; set; }
	}

	internal class FileService : IFileService
	{
		public bool Exists(string file)
		{
			return File.Exists(file);
		}

		/// <summary>
		/// Returns a file with given name, in system TEMP path
		/// file is never deleted but reused (deleted before being reused)
		/// </summary>
		/// <returns>The filename, ready for use</returns>
		public string TempFile(string file)
		{
			var tempFile = Path.Combine(Path.GetTempPath(), file);
			DeleteIfExists(tempFile);
			return tempFile;
		}

		public FileSaveResult Write(string file, string report)
		{
			var result = new FileSaveResult();
			
			try
			{
				using (var stream = File.OpenWrite(file))
				{
					var writer = new StreamWriter(stream);
					writer.Write(report);
					writer.Flush();
				}
				result.Saved = true;
			}
			catch (Exception exception)
			{
				result.Saved = false;
				result.Exception = exception;
			}

			return result;
		}

		private static void DeleteIfExists(string file)
		{
			if (File.Exists(file)) 
			{ 
				File.Delete(file);
			}
		}
	}
}
