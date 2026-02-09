using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace DeploymentManager.Web;

public class LegendNodeModel : NodeModel
{
    public LegendNodeModel(
        string legendText,
        string color,
        Point? position = null
    ) : base(position)
    {
        LegendText = legendText;
        Color = color;
    }

    public string LegendText { get; init; }
    public string Color { get; init; }
}
