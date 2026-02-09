namespace DeploymentManager.Web.Models;

public class ClassRelationships
{
    public Class1[] Classes { get; init; }
    public Relationship[] Relationships { get; init; }
}

public class Class1
{
    public string Name { get; init; }
    public int Methods { get; init; }
    public int Properties { get; init; }
}

public class Relationship
{
    public string From { get; init; }
    public string To { get; init; }
}
