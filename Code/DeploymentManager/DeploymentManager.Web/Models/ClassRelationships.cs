namespace DeploymentManager.Web.Models;

public class ClassRelationships
{
    public Class1[] Classes { get; set; }
    public Relationship[] Relationships { get; set; }
}

public class Class1
{
    public string Name { get; set; }
    public int Methods { get; set; }
    public int Properties { get; set; }
}

public class Relationship
{
    public string From { get; set; }
    public string To { get; set; }
}
