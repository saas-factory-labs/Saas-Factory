namespace AppBlueprint.UiKit.Models.Blog;

public sealed class AppBlueprintBlogOptions
{
    public string SiteName { get; set; } = "AppBlueprint";
    public string BlogName { get; set; } = "AppBlueprint Blog";
    public string BlogSeoTitle { get; set; } = "Blog | AppBlueprint";
    public string BlogDescription { get; set; } = "Read guides, updates, and practical product insights.";
    public string Locale { get; set; } = "en_US";
    public string Language { get; set; } = "en";
    public string CultureName { get; set; } = "en-US";
    public string BlogPath { get; set; } = "/blog";
    public string LogoPath { get; set; } = "/favicon.png";

    public string HomeBreadcrumbName { get; set; } = "Home";
    public string BlogBreadcrumbName { get; set; } = "Blog";
    public string BlogEyebrow { get; set; } = "Knowledge";
    public string BlogHeading { get; set; } = "Guides and insight";
    public string BlogIntro { get; set; } = "Explore practical articles from the editorial team.";
    public string EmptyBlogTitle { get; set; } = "No articles have been published yet";
    public string EmptyBlogDescription { get; set; } = "Add articles through BlogContentOptions or provide a host-specific IBlogContentService.";
    public string PrimaryCtaText { get; set; } = "Get started";
    public string PrimaryCtaHref { get; set; } = "/";
    public string SecondaryCtaText { get; set; } = "Learn more";
    public string SecondaryCtaHref { get; set; } = "/";
    public string ArticleCountLabel { get; set; } = "articles";
    public string CategoryCountLabel { get; set; } = "topics";
    public string AudienceLabel { get; set; } = "Written for product users";

    public string FeaturedEyebrow { get; set; } = "Editorial focus";
    public string FeaturedHeading { get; set; } = "Popular right now";
    public string FeaturedLinkText { get; set; } = "Explore";
    public string FeaturedLinkHref { get; set; } = "/";
    public string LatestEyebrow { get; set; } = "Latest articles";
    public string LatestHeading { get; set; } = "Insight you can use";

    public string FinalCtaEyebrow { get; set; } = "From insight to action";
    public string FinalCtaHeading { get; set; } = "Ready for the next step?";
    public string FinalCtaDescription { get; set; } = "Use what you learned and continue in the product.";
    public string FinalCtaPrimaryText { get; set; } = "Get started";
    public string FinalCtaPrimaryHref { get; set; } = "/";
    public string FinalCtaSecondaryText { get; set; } = "Browse";
    public string FinalCtaSecondaryHref { get; set; } = "/";

    public string ReadArticleText { get; set; } = "Read article";
    public string ReadMoreText { get; set; } = "Read more";
    public string ReadMinutesSuffix { get; set; } = "min.";
    public string ReadTimeSuffix { get; set; } = "min. read";

    public string ArticleNotFoundEyebrow { get; set; } = "Article not found";
    public string ArticleNotFoundTitle { get; set; } = "This article is no longer available";
    public string ArticleNotFoundDescription { get; set; } = "Try one of the other guides instead.";
    public string KeyPointsEyebrow { get; set; } = "Quick overview";
    public string KeyPointsHeading { get; set; } = "The most important points";
    public string NextStepEyebrow { get; set; } = "Next step";
    public string NextStepHeading { get; set; } = "Make this article useful";
    public string NextStepDescription { get; set; } = "Continue with the relevant workflow in the product.";
    public string NextStepCtaText { get; set; } = "Continue";
    public string NextStepCtaHref { get; set; } = "/";
    public string AuthorName { get; set; } = "AppBlueprint Editorial";
    public string UpdatedPrefix { get; set; } = "Updated";
    public string RelatedArticlesEyebrow { get; set; } = "Related articles";
    public string RelatedArticlesHeading { get; set; } = "Read next";
    public string TagsHeading { get; set; } = "Tags";
    public string ArticleCtaHeading { get; set; } = "Ready to continue?";
    public string ArticleCtaDescription { get; set; } = "Turn this article into action inside the product.";
    public string ArticleCtaPrimaryText { get; set; } = "Continue";
    public string ArticleCtaPrimaryHref { get; set; } = "/";
    public string BackToBlogText { get; set; } = "Back to blog";

    public Func<string, bool> ShouldIndexHost { get; set; } = _ => true;
}
