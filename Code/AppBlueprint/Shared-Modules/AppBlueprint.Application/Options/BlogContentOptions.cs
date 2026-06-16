using AppBlueprint.Application.Models.Blog;

namespace AppBlueprint.Application.Options;

public sealed class BlogContentOptions
{
    public IReadOnlyList<BlogArticle> Articles { get; set; } = [];
}
