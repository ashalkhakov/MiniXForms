namespace MiniXForms
{
    public class InstanceNode
    {
        public InstanceNode(string name, Expr expression)
        {
            Name = name;
            Expression = expression;
        }
        public InstanceNode(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Value { get; set; }
        public bool Dirty { get; set; } = true;
        public Expr? Expression { get; set; }
        public int Index { get; set; }
    }
}
