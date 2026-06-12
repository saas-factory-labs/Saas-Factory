using AppBlueprint.Application.Models.Blog;
using AppBlueprint.Application.Options;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Application.Services.Blog;

public sealed class OptionsBlogContentService(IOptions<BlogContentOptions> options) : IBlogContentService
{
    public IReadOnlyList<BlogArticle> GetAllArticles()
        => Articles;

    public BlogArticle? GetFeaturedArticle()
    {
        if (Articles.Count == 0)
        {
            return null;
        }

        return Articles.FirstOrDefault(article => article.IsFeatured) ?? Articles[0];
    }

    public BlogArticle? GetArticle(string slug)
        => Articles.FirstOrDefault(article => article.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<BlogArticle> GetRelatedArticles(string slug, int take = 3)
    {
        BlogArticle? currentArticle = GetArticle(slug);

        if (currentArticle is null)
        {
            return Articles.Take(take).ToArray();
        }

        return Articles
            .Where(article => !article.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(article => article.Category == currentArticle.Category)
            .ThenByDescending(article => article.Tags.Intersect(currentArticle.Tags, StringComparer.OrdinalIgnoreCase).Count())
            .ThenByDescending(article => article.PublishedOn)
            .Take(take)
            .ToArray();
    }

    private IReadOnlyList<BlogArticle> Articles
        => options.Value.Articles;
}
