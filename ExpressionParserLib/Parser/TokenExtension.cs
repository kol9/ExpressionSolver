namespace ExpressionParserLib.Parser
{
    public static class TokenExtension
    {
        public static bool IsBinaryOperation(this Token token)
        {
            return token == Token.Add || token == Token.Subtract ||
                   token == Token.Multiply || token == Token.Divide;
        }
    }
}