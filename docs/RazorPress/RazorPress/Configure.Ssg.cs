using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack.IO;
using ServiceStack.Text;

[assembly: HostingStartup(typeof(RazorPress.ConfigureSsg))]

namespace RazorPress;

public class ConfigureSsg : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) =>
        {
            context.Configuration.GetSection(nameof(AppConfig)).Bind(AppConfig.Instance);
            services.AddSingleton(AppConfig.Instance);
            services.AddSingleton<RazorPagesEngine>();
            services.AddSingleton<MarkdownIncludes>();
            services.AddSingleton<MarkdownPages>();
            services.AddSingleton<MarkdownWhatsNew>();
            services.AddSingleton<MarkdownVideos>();
            services.AddSingleton<MarkdownMeta>();
        })
        .ConfigureAppHost(
            appHost => appHost.Plugins.Add(new CleanUrlsFeature()),
            afterPluginsLoaded: appHost =>
            {
                MarkdigConfig.Set(new MarkdigConfig
                {
                    ConfigurePipeline = pipeline =>
                    {
                        // Extend Markdig Pipeline
                    },
                    ConfigureContainers = config =>
                    {
                        config.AddBuiltInContainers();
                        // Add Custom Block or Inline containers
                        config.AddBlockContainer("YouTube", new YouTubeContainer());
                        config.AddInlineContainer("YouTube", new YouTubeInlineContainer());
                    }
                });

                var includes = appHost.Resolve<MarkdownIncludes>();
                var pages = appHost.Resolve<MarkdownPages>();
                var whatsNew = appHost.Resolve<MarkdownWhatsNew>();
                var videos = appHost.Resolve<MarkdownVideos>();
                var meta = appHost.Resolve<MarkdownMeta>();

                meta.Features = [pages, whatsNew, videos];
                
                includes.LoadFrom("_includes");
                pages.LoadFrom("_pages");
                whatsNew.LoadFrom("_whatsnew");
                videos.LoadFrom("_videos");
                AppConfig.Instance.Init(appHost.ContentRootDirectory);
            },
            afterAppHostInit: appHost =>
            {
                // prerender with: `$ npm run prerender` 
                AppTasks.Register("prerender", args =>
                {
                    appHost.Resolve<MarkdownMeta>().RenderToAsync(
                        metaDir: appHost.ContentRootDirectory.RealPath.CombineWith("wwwroot/meta"),
                        baseUrl: HtmlHelpers.ToAbsoluteContentUrl("")).GetAwaiter().GetResult();

                    var distDir = appHost.ContentRootDirectory.RealPath.CombineWith("dist");
                    if (Directory.Exists(distDir))
                        FileSystemVirtualFiles.DeleteDirectory(distDir);
                    FileSystemVirtualFiles.CopyAll(
                        new DirectoryInfo(appHost.ContentRootDirectory.RealPath.CombineWith("wwwroot")),
                        new DirectoryInfo(distDir));
                    
                    // Render .html redirect files
                    RazorSsg.PrerenderRedirectsAsync(appHost.ContentRootDirectory.GetFile("redirects.json"), distDir)
                        .GetAwaiter().GetResult();

                    var razorFiles = appHost.VirtualFiles.GetAllMatchingFiles("*.cshtml");
                    RazorSsg.PrerenderAsync(appHost, razorFiles, distDir).GetAwaiter().GetResult();
                    
                    // Post-process HTML files to rewrite paths when BaseHref is set
                    var baseHref = AppConfig.Instance.BaseHref?.TrimEnd('/');
                    if (!string.IsNullOrEmpty(baseHref) && baseHref != "")
                    {
                        Console.WriteLine($"Post-processing HTML files to rewrite paths for BaseHref: {baseHref}/");
                        var htmlFiles = Directory.GetFiles(distDir, "*.html", SearchOption.AllDirectories);
                        foreach (var htmlFile in htmlFiles)
                        {
                            var content = File.ReadAllText(htmlFile);
                            
                            // Ensure the <base href> tag is set correctly
                            content = System.Text.RegularExpressions.Regex.Replace(content,
                                @"<base\s+href=""[^""]*""\s*/?>",
                                $"<base href=\"{baseHref}/\"/>",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            
                            // Replace absolute paths with baseHref-prefixed paths
                            // Handle asset paths (href and src attributes)
                            content = System.Text.RegularExpressions.Regex.Replace(content, 
                                @"(href|src)=""/(css|mjs|lib|img|js|pages)/", 
                                $"$1=\"{baseHref}/$2/");
                            
                            // Handle import map paths (JSON strings)
                            content = System.Text.RegularExpressions.Regex.Replace(content,
                                @"""/(mjs|lib)/",
                                $"\"{baseHref}/$1/");
                            
                            // Handle all other href absolute paths (navigation links)
                            // Match href="/" followed by any path that's not already prefixed
                            content = System.Text.RegularExpressions.Regex.Replace(content,
                                @"href=""(/[^""]*)""",
                                match => {
                                    var path = match.Groups[1].Value;
                                    // Skip if already has the baseHref prefix or is an external link
                                    if (path.StartsWith(baseHref) || path.StartsWith("http://") || path.StartsWith("https://"))
                                        return match.Value;
                                    return $"href=\"{baseHref}{path}\"";
                                });
                            
                            File.WriteAllText(htmlFile, content);
                        }
                        Console.WriteLine($"Post-processed {htmlFiles.Length} HTML files with absolute paths");
                    }
                });
            });
}

public class AppConfig
{
    public static AppConfig Instance { get; } = new();
    public string Title { get; set; }
    public string LocalBaseUrl { get; set; }
    public string PublicBaseUrl { get; set; }
    public string BaseHref { get; set; } = "/";
    public string? GitPagesBaseUrl { get; set; }
    public string? GitPagesRawBaseUrl { get; set; }

    public void Init(IVirtualDirectory contentDir)
    {
        ResolveGitBlobBaseUrls(contentDir);
    }

    public void ResolveGitBlobBaseUrls(IVirtualDirectory contentDir)
    {
        var srcDir = new DirectoryInfo(contentDir.RealPath);
        var gitConfig = new FileInfo(Path.Combine(srcDir.Parent!.FullName, ".git", "config"));
        if (gitConfig.Exists)
        {
            var txt = gitConfig.ReadAllText();
            var pos = txt.IndexOf("url = ", StringComparison.Ordinal);
            if (pos >= 0)
            {
                var url = txt[(pos + "url = ".Length)..].LeftPart(".git").LeftPart('\n').Trim();
                GitPagesBaseUrl = url.CombineWith($"blob/main/{srcDir.Name}");
                GitPagesRawBaseUrl = url.Replace("github.com","raw.githubusercontent.com").CombineWith($"refs/heads/main/{srcDir.Name}");
            }
        }
    }
}

// Add additional frontmatter info to include
public class MarkdownFileInfo : MarkdownFileBase
{
}

public static class HtmlHelpers
{
    public static string ToAbsoluteContentUrl(string? relativePath) => HostContext.DebugMode 
        ? AppConfig.Instance.LocalBaseUrl.CombineWith(relativePath)
        : AppConfig.Instance.PublicBaseUrl.CombineWith(relativePath);
    public static string ToAbsoluteApiUrl(string? relativePath) => HostContext.DebugMode 
        ? AppConfig.Instance.LocalBaseUrl.CombineWith(relativePath)
        : AppConfig.Instance.PublicBaseUrl.CombineWith(relativePath);


    public static string ContentUrl(this IHtmlHelper html, string? relativePath) => ToAbsoluteContentUrl(relativePath); 
    public static string ApiUrl(this IHtmlHelper html, string? relativePath) => ToAbsoluteApiUrl(relativePath);
}

// Example of implementing a custom Block Container
public class YouTubeContainer : HtmlObjectRenderer<CustomContainer>
{
    protected override void Write(HtmlRenderer renderer, CustomContainer obj)
    {
        if (obj.Arguments == null)
        {
            renderer.WriteLine($"Missing YouTube Id, Usage :::{obj.Info} <id>");
            return;
        }
        
        renderer.EnsureLine();

        var youtubeId = obj.Arguments!;
        var attrs = obj.TryGetAttributes()!;
        attrs.Classes ??= new();
        attrs.Classes.Add("not-prose text-center");
        
        renderer.Write("<div").WriteAttributes(obj).Write('>');
        renderer.WriteLine("<div class=\"text-3xl font-extrabold tracking-tight\">");
        renderer.WriteChildren(obj);
        renderer.WriteLine("</div>");
        renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}"" 
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
            </div>
        </div>");
    }
}

public class YouTubeInlineContainer : HtmlObjectRenderer<CustomContainerInline>
{
    protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
    {
        var youtubeId = obj.FirstChild is Markdig.Syntax.Inlines.LiteralInline literalInline
            ? literalInline.Content.AsSpan().RightPart(' ').ToString()
            : null;
        if (string.IsNullOrEmpty(youtubeId))
        {
            renderer.WriteLine($"Missing YouTube Id, Usage ::YouTube <id>::");
            return;
        }
        renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}"" 
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
        </div>");
    }
}
