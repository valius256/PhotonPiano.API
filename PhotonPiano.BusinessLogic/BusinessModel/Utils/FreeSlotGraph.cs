

namespace PhotonPiano.BusinessLogic.BusinessModel.Utils
{
    public class FreeSlotGraph
    {
        private Dictionary<string, HashSet<string>> adjacencyList = [];

        public void AddEdge(string studentA, string studentB)
        {
            if (!adjacencyList.ContainsKey(studentA))
                adjacencyList[studentA] = [];

            if (!adjacencyList.ContainsKey(studentB))
                adjacencyList[studentB] = [];

            adjacencyList[studentA].Add(studentB);
            adjacencyList[studentB].Add(studentA);
        }

        public List<List<string>> FindCliques(int minSize, int maxSize)
        {
            var cliques = new List<List<string>>();

            // Bron-Kerbosch algorithm (for finding maximal cliques)
            BronKerbosch([], [.. adjacencyList.Keys], [], cliques);

            // Filter valid cliques based on size
            return [.. cliques.Where(c => c.Count >= minSize && c.Count <= maxSize)];
        }

        private void BronKerbosch(List<string> R, List<string> P, List<string> X, List<List<string>> cliques)
        {
            if (P.Count == 0 && X.Count == 0)
            {
                cliques.Add([.. R]);
                return;
            }

            foreach (var v in P.ToList())
            {
                var neighbors = adjacencyList[v].ToList();
                BronKerbosch([.. R, v],
                    [.. P.Intersect(neighbors)],
                    [.. X.Intersect(neighbors)],
                    cliques);

                P.Remove(v);
                X.Add(v);
            }
        }
    }
}
