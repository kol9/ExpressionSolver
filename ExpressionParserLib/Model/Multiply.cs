namespace ExpressionParserLib.Model
{
    public class Multiply : IExpression
    {
        private readonly IExpression _lhs;
        private readonly IExpression _rhs;

        public Multiply(IExpression lhs, IExpression rhs)
        {
            _lhs = lhs;
            _rhs = rhs;
        }

        public double Evaluate()
        {
            return _lhs.Evaluate() * _rhs.Evaluate();
        }
        
        public override string ToString()
        {
            return "(" + _lhs.ToString() + "*" + _rhs.ToString() + ")";
        }
    }
}