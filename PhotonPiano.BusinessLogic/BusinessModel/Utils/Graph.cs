namespace PhotonPiano.BusinessLogic.BusinessModel.Utils;

public class Graph<T> where T : notnull
{
    public Dictionary<T, List<T>> Edges { get; set; } = [];
}