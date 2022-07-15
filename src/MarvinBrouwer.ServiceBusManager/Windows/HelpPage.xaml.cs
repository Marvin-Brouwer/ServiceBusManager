using Markdig;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Image = System.Drawing.Image;

namespace MarvinBrouwer.ServiceBusManager.Windows;

/// <summary>
/// Interaction logic for HelpPage.xaml
/// </summary>
public partial class HelpPage : Page
{
	private static readonly Regex FindFilePathRegex = new ("\\.\\/(?<filePath>(?!https?\\\\:).*?)[\\)\\\"]", RegexOptions.ECMAScript);

	private HelpPage()
	{
		InitializeComponent();
	}

	public static async Task<Page> Load()
	{
		var page = new HelpPage();
		var taskCompletionSource = new TaskCompletionSource();
		var resource = Application.GetResourceStream(new Uri("pack://application:,,,./Readme.md"));
		using var reader = new StreamReader(resource!.Stream);
		var markDownReadme = await reader.ReadToEndAsync();

		var filesMatch = FindFilePathRegex.Matches(markDownReadme);
		var markdownLinks = filesMatch
			.Select(match => match.Groups["filePath"].Value)
			.Select(path => string.Concat("./", path))
			.Distinct();

		foreach (var path in markdownLinks)
		{
			markDownReadme = markDownReadme
				.Replace(path, ConvertImageToEmbeddedString(path));
		}

		var htmlReadme = Markdown.ToHtml(markDownReadme);

		page.ReadmeRenderer.NavigateToString(htmlReadme);
		page.ReadmeRenderer.Navigated += (_, _) =>
		{
			page.ReadmeRenderer.Visibility = Visibility.Visible;
			taskCompletionSource.SetResult();
		};

		page.ReadmeRenderer.Navigating += Browser_Navigating;

		// This may seem odd, but it is to await the initial browser render.
		await taskCompletionSource.Task;
		return page;
	}

	private static string ConvertImageToEmbeddedString(string source)
	{
		// find the image type
		string? imageType = null;
		if (source.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) imageType = "png";
		if (source.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) imageType = "jpg";
		if (source.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)) imageType = "ico";
		if (imageType is null) return string.Empty;

		if (source.Length > 4)
		{
			if (!source.StartsWith("/"))
			{
				source = "/" + source;
			}

			var stream = Application.GetResourceStream(new Uri($"pack://application:,,,{source}"))?.Stream;
			if (stream != null)
			{
				var image = Image.FromStream(stream);
				using var memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				return $"data:image/{imageType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
			}
		}

		return string.Empty;
	}

	private static void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
	{
		if (e.NavigationMode != NavigationMode.New && !e.Uri.Scheme.Contains("http")) return;
		e.Cancel = true;

		//this opens the URL in the user's default browser
		var proc = new ProcessStartInfo
		{
			FileName = e.Uri.ToString(),
			Verb = "open",
			UseShellExecute = true,
			WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
		};
		Process.Start(proc);
	}

}
