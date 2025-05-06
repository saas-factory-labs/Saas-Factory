namespace DeploymentManager.Codeflow;

public class DGraphRequestSchema
{
    public Set[] Set { get; set; }
}

public class Set
{
    public string Uid { get; set; }
    public string Dgraphtype { get; set; }
    public string ClassName { get; set; }
    public Method[] Methods { get; set; }
    public Property1[] Properties { get; set; }
    public Dependson[] DependsOn { get; set; }
}

public class Method
{
    public string Uid { get; set; }
    public string Dgraphtype { get; set; }
    public string MethodName { get; set; }
}

public class Property1
{
    public string Uid { get; set; }
    public string Dgraphtype { get; set; }
    public string PropertyName { get; set; }
}

public class Dependson
{
    public string Uid { get; set; }
}
