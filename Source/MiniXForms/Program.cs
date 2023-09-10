using System.Diagnostics;

namespace MiniXForms
{
    internal class Program
    {
        static void Test1()
        {
            var g = new DepGraph();

            // c = min(a*2, b*3)
            var a = new InstanceNode("a", 1);
            var b = new InstanceNode("b", 2);
            var c = new InstanceNode("c", new MinExpr(new List<Expr>()
                {
                    new MulExpr(new VarExpr(a), new ConstExpr(2)),
                    new MulExpr(new VarExpr(b), new ConstExpr(3))
                }));
            var instances = new List<InstanceNode>()
            {
                a, b, c
            };
            g.InitWithInstanceNodes(instances);

            a.Dirty = true;
            b.Dirty = true;
            var sg = g.CalculateDependencySubgraph();
            sg.Recalculate();
            Trace.Assert(instances.All(i => i.Dirty == false));
            Trace.Assert(c.Value == 2);
        }
        static void Test2()
        {
            var g = new DepGraph();

            // c = min(a*2, b*3)
            var a = new InstanceNode("a", 10);
            var b = new InstanceNode("b", 10);
            var c = new InstanceNode("c", new MulExpr(new VarExpr(a), new VarExpr(b)));
            var d = new InstanceNode("d", new AddExpr(new VarExpr(a), new VarExpr(b)));
            var w = new InstanceNode("w", new GeExpr(new VarExpr(c), new ConstExpr(100)));
            var y = new InstanceNode("y", new GeExpr(new VarExpr(d), new ConstExpr(20)));

            var instances = new List<InstanceNode>()
            {
                a, b, c, d, w, y
            };
            g.InitWithInstanceNodes(instances);

            {
                // this is the initial recalculation. just assume ALL nodes were modified.
                foreach (var instance in instances)
                    instance.Dirty = true;
                var sg = g.CalculateDependencySubgraph();
                sg.Recalculate();
                Trace.Assert(instances.All(i => i.Dirty == false));
                Trace.Assert(w.Value == 1);
                Trace.Assert(y.Value == 1);
            }

            a.Value = 11;
            a.Dirty = true;
            {
                var sg = g.CalculateDependencySubgraph();
                sg.Recalculate();
                Trace.Assert(instances.All(i => i.Dirty == false));
                Trace.Assert(w.Value == 0);
                Trace.Assert(y.Value == 0);
            }
        }

        static void Main(string[] args)
        {
            Test1();
            Test2();
        }
    }
}