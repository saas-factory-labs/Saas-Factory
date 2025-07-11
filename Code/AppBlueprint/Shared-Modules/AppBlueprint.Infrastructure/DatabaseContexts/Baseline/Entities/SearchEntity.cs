using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class SearchEntity: BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Uri Url { get; set; }
    public required string SearchType { get; set; }
    public required string SearchCriteria { get; set; }
    public required string SearchResults { get; set; }
    public required string SearchStatus { get; set; }
    public required string SearchError { get; set; }
    public required string SearchErrorMessage { get; set; }
}
