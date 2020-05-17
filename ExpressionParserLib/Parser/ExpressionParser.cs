using System;
using System.Collections.Generic;
using ExpressionParserLib.Exceptions;
using ExpressionParserLib.Model;

namespace ExpressionParserLib.Parser
{
    public class ExpressionParser
    {
        private Tokenizer _tokenizer;

        public Tuple<IExpression, List<ParsingException>> Parse(string s)
        {
            _tokenizer = new Tokenizer(s);
            var result = ParseAddSubtract();
            return new Tuple<IExpression, List<ParsingException>>(result, _tokenizer.GetErrors());
        }

        private IExpression ParseUnary()
        {
            while (true)
            {
                IExpression res = new ErrorExpression();
                switch (_tokenizer.GetNextToken())
                {
                    case Token.Number:
                        res = new Number(_tokenizer.GetValue());
                        _tokenizer.GetNextToken();
                        break;
                    case Token.Negate:
                        res = new Negate(ParseUnary());
                        break;
                    case Token.OpenParenthesis:
                        res = ParseAddSubtract();
                        if (_tokenizer.GetCurToken() != Token.CloseParenthesis)
                        {
                            _tokenizer.AddException(new MissingCloseParenthesisException(
                                _tokenizer.GetExpression(),
                                _tokenizer.GetInd()));
                        }

                        _tokenizer.GetNextToken();
                        break;
                    case Token.Error:
                        continue;
                }

                return res;
            }
        }

        private IExpression ParseMultiplyDivide()
        {
            var res = ParseUnary();
            while (true)
            {
                switch (_tokenizer.GetCurToken())
                {
                    case Token.Multiply:
                        res = new Multiply(res, ParseUnary());
                        break;
                    case Token.Divide:
                        res = new Divide(res, ParseUnary());
                        break;
                    case Token.Error:
                        _tokenizer.GetNextToken();
                        break;
                    default:
                        return res;
                }
            }
        }

        private IExpression ParseAddSubtract()
        {
            var res = ParseMultiplyDivide();
            while (true)
            {
                switch (_tokenizer.GetCurToken())
                {
                    case Token.Add:
                        res = new Add(res, ParseMultiplyDivide());
                        break;
                    case Token.Subtract:
                        res = new Subtract(res, ParseMultiplyDivide());
                        break;
                    case Token.Error:
                        _tokenizer.GetNextToken();
                        break;
                    default:
                        return res;
                }
            }
        }
    }
}