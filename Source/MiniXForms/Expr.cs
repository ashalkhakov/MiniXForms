namespace MiniXForms
{
    public abstract class Expr
    {
        public abstract int Eval();

        public static void FreeVars(Expr e, List<InstanceNode> names)
        {
            switch (e)
            {
                case VarExpr v:
                    names.Add(v.Name);
                    break;
                case ConstExpr:
                    break;
                case MinExpr m:
                    foreach (var s in m.SubExprs)
                    {
                        FreeVars(s, names);
                    }
                    break;
                case MulExpr m:
                    FreeVars(m.A, names);
                    FreeVars(m.B, names);
                    break;
                case DivExpr m:
                    FreeVars(m.A, names);
                    FreeVars(m.B, names);
                    break;
                case AddExpr m:
                    FreeVars(m.A, names);
                    FreeVars(m.B, names);
                    break;
                case GeExpr m:
                    FreeVars(m.A, names);
                    FreeVars(m.B, names);
                    break;
                default:
                    break;
            }
        }
    }
    public class ConstExpr : Expr
    {
        public ConstExpr(int c)
        {
            K = c;
        }
        public int K { get; }
        public override int Eval()
        {
            return K;
        }
    }
    public class VarExpr : Expr
    {
        public VarExpr(InstanceNode n)
        {
            Name = n;
        }
        public InstanceNode Name { get; }
        public override int Eval()
        {
            return Name.Value;
        }
    }
    public class MinExpr : Expr
    {
        public MinExpr(List<Expr> subexprs)
        {
            SubExprs = subexprs;
        }
        public List<Expr> SubExprs { get; }
        public override int Eval()
        {
            var m = SubExprs.Select(se => se.Eval()).Min();
            return m;
        }
    }
    public class AddExpr : Expr
    {
        public AddExpr(Expr a, Expr b)
        {
            A = a;
            B = b;
        }
        public Expr A { get; }
        public Expr B { get; }
        public override int Eval()
        {
            return A.Eval() + B.Eval();
        }
    }
    public class MulExpr : Expr
    {
        public MulExpr(Expr a, Expr b)
        {
            A = a;
            B = b;
        }
        public Expr A { get; }
        public Expr B { get; }
        public override int Eval()
        {
            return A.Eval() * B.Eval();
        }
    }
    public class DivExpr : Expr
    {
        public DivExpr(Expr a, Expr b)
        {
            A = a;
            B = b;
        }
        public Expr A { get; }
        public Expr B { get; }
        public override int Eval()
        {
            return A.Eval() / B.Eval();
        }
    }
    public class GeExpr : Expr
    {
        public GeExpr(Expr a, Expr b)
        {
            A = a;
            B = b;
        }
        public Expr A { get; }
        public Expr B { get; }
        public override int Eval()
        {
            return A.Eval() <= B.Eval() ? 1 : 0;
        }
    }
}
