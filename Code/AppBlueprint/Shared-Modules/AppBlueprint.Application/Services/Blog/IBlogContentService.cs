using AppBlueprint.Application.Models.Blog;

namespace AppBlueprint.Application.Services.Blog;

public interface IBlogContentService
{
    IReadOnlyList<BlogArticle> GetAllArticles();

    BlogArticle? GetFeaturedArticle();

    BlogArticle? GetArticle(string slug);

    IReadOnlyList<BlogArticle> GetRelatedArticles(string slug, int take = 3);
}
