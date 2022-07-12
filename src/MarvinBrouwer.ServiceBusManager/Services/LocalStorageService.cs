using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvinBrouwer.ServiceBusManager.Services;

internal sealed class LocalStorageService
{
	private const string TimeStampFormat = "yyyy-MM-dd HHmmss";
	private const string DownloadFolderName = "Downloads";

	private readonly string _applicationPath;

	public LocalStorageService()
	{
		_applicationPath = Path.Join(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			Path.GetFileNameWithoutExtension(GetType().Module.Name)
		);
	}

	public void PrepareDownloadFolder()
	{
		var fullDownloadPath = Path.Join(_applicationPath, DownloadFolderName);
		if (!Directory.Exists(fullDownloadPath)) Directory.CreateDirectory(fullDownloadPath);
	}

	public void OpenDownloadFolder()
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = DownloadFolderName,
			Verb = "open",
			UseShellExecute = true,
			WorkingDirectory = _applicationPath
		});
	}
}
