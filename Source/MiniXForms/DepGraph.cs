namespace MiniXForms
{
    public class DepGraphNode
    {
        public DepGraphNode(InstanceNode node, int index)
        {
            InstanceNode = node;
            Index = index;
            DepList = new List<DepGraphNode>();
        }
        public InstanceNode InstanceNode { get; set; }
        public List<DepGraphNode> DepList { get; set; }
        public int InDegree { get; set; }
        public bool Visited { get; set; }
        public int Index { get; set; }
    }

    public class DepGraph
    {
        public DepGraph()
        {
            nodes = new List<DepGraphNode>();
        }

        public void InitWithInstanceNodes(List<InstanceNode> instances)
        {
            foreach (var inst in instances)
            {
                var n = new DepGraphNode(inst, nodes.Count);
                inst.Dirty = true;
                inst.Index = n.Index;
                nodes.Add(n);
            }
            var fv = new List<InstanceNode>();
            for (var i = 0; i < instances.Count; i++)
            {
                var n = nodes[i];
                fv.Clear();
                Expr.FreeVars(n.InstanceNode.Expression, fv);
                foreach (var v in fv)
                {
                    var m = nodes[v.Index];
                    m.InDegree++;
                    m.DepList.Add(n);
                }
            }
        }

        public DepGraph CalculateDependencySubgraph()
        {
            var updates = nodes
                .Where(n => n.InstanceNode.Dirty)
                .Select(n => n.InstanceNode)
                .ToList();

            var S = new DepGraph();
            var stack = new Stack<(DepGraphNode?, DepGraphNode)>();

            // Use depth-first search to explore master digraph subtrees rooted at
            // each changed vertex. A 'visited' flag is used to stop exploration
            // at the boundaries of previously explored subtrees (because subtrees
            // can overlap in directed graphs).
            foreach (var r1 in updates)
            {
                var r = nodes[r1.Index];
                if (!r.Visited)
                {
                    stack.Push((null, r));
                    while (stack.Count > 0)
                    {
                        var (v, w) = stack.Pop();
                        DepGraphNode wS;
                        if (!w.Visited)
                        {
                            w.Visited = true;
                            // Create a vertex wS in S to represent w
                            // Set the index of wS equal to the array location of w
                            wS = new DepGraphNode(w.InstanceNode, w.Index);
                            // Set the index of w equal to the array location of wS
                            w.Index = S.nodes.Count;
                            S.nodes.Add(wS);
                            // Set the InstanceNode of wS equal to the InstanceNode of w
                            wS.InstanceNode = w.InstanceNode;
                            foreach (var dep in w.DepList)
                            {
                                stack.Push((w, dep));
                            }
                        }
                        else
                        {
                            // Obtain wS from index of w
                            wS = S.nodes[w.Index];
                        }
                        if (v != null)
                        {
                            // Obtain vS from index of v
                            var vS = S.nodes[v.Index];
                            // Add dependency node for wS to vS
                            vS.DepList.Add(wS);
                            // Increment inDegree of wS
                            wS.InDegree++;
                        }
                    }
                }
            }

            // Now clear the visited flags set in the loop above
            foreach (var n in S.nodes)
            {
                // Obtain v from index of vS
                var v = nodes[n.Index];
                // Assign false to the visited flag of v
                v.Visited = false;
            }

            return S;
        }

        public void Recalculate()
        {
            while (true)
            {
                var vi = nodes.FindIndex(n => n.InDegree == 0);
                if (vi == -1)
                {
                    if (nodes.Count > 0)
                        throw new Exception("Calculation loop detected");
                    else
                        break;
                }
                var v = nodes[vi];
                if (v.InstanceNode.Expression != null)
                {
                    v.InstanceNode.Value = v.InstanceNode.Expression.Eval();
                }
                v.InstanceNode.Dirty = false;
                foreach (var m in v.DepList)
                {
                    m.InDegree--;
                }
                nodes.RemoveAt(vi);
            }
        }

        List<DepGraphNode> nodes;
    }
}
