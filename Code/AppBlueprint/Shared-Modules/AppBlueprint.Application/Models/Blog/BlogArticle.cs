namespace AppBlueprint.Application.Models.Blog;

public sealed record BlogArticle(
    string Slug,
    string Title,
    string Category,
    string Excerpt,
    string HeroLabel,
    string ImagePath,
    string ImageAlt,
    DateOnly PublishedOn,
    DateOnly UpdatedOn,
    int ReadMinutes,
    bool IsFeatured,
    IReadOnlyList<string> Tags,
    string Introduction,
    IReadOnlyList<string> KeyPoints,
    IReadOnlyList<BlogArticleSection> Sections);

public sealed record BlogArticleSection(
    string Heading,
    IReadOnlyList<string> Paragraphs,
    IReadOnlyList<string>? BulletPoints = null,
    string? Highlight = null);
