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

	private static readonly Regex HeadingsIdRegex = new (@"<h(?<level>[1-4])>(?<innerText>[^<]*)<\/h[1-4]>", RegexOptions.ECMAScript);

	private HelpPage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Load and render a new <see cref="HelpPage"/> based on the `./Readme.md` of this `.csproj`
	/// </summary>
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

		// Fix the links so the page behaves like on GitHub
		htmlReadme = HeadingsIdRegex.Replace(htmlReadme, x =>
		{
			var level = x.Groups["level"].Value;
			var text = x.Groups["innerText"].Value;
			var id = text.Trim().Replace(" ", "-").ToLowerInvariant();
			return $"<h{level} id=\"{id}\">{text}</h{level}>";
		});

		page.ReadmeRenderer.NavigateToString(htmlReadme);
		page.ReadmeRenderer.Navigated += (_, _) =>
		{
			page.ReadmeRenderer.Visibility = Visibility.Visible;
			taskCompletionSource.TrySetResult();
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

			try
			{
				var stream = Application.GetResourceStream(new Uri($"pack://application:,,,{source}"))?.Stream;
				if (stream != null)
				{
					var image = Image.FromStream(stream);
					using var memoryStream = new MemoryStream();
					image.Save(memoryStream, image.RawFormat);
					return $"data:image/{imageType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
				}
			}
			catch (IOException)
			{
				// Just return the pack url if not found.
				// It'll render a not found image but that's what we want anyway here.
				return $"pack://application:,,,{source}";
			}
		}

		return string.Empty;
	}

	private static void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
	{
		if (e.NavigationMode != NavigationMode.New || !e.Uri.Scheme.Contains("http")) return;
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
