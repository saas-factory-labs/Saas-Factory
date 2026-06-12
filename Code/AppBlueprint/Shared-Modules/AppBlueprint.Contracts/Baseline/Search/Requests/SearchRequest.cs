using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.Search.Requests;

public sealed class SearchRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string QueryText { get; set; } = string.Empty;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(0.0, 1.0)]
    public float MinRelevanceScore { get; set; }

    [StringLength(100)]
    public string? SortBy { get; set; }

    [RegularExpression("^(?i)(Ascending|Descending)$",
        ErrorMessage = "SortDirection must be 'Ascending' or 'Descending'.")]
    public string SortDirection { get; set; } = "Descending";
}
