namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class SearchEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public required string Url { get; set; }
    public required string SearchType { get; set; }
    public string SearchCriteria { get; set; }
    public string SearchResults { get; set; }
    public string SearchStatus { get; set; }
    public string SearchError { get; set; }
    public string SearchErrorMessage { get; set; }
}
