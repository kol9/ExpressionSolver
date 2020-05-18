using System;
using System.Collections.Generic;
using ExpressionParserLib.Exceptions;

namespace ExpressionParserLib.Parser
{
    public class Tokenizer
    {
        private readonly string _expression;
        private int _ind;
        private Token _curToken;
        private int _parenthesisBalance;

        private double _value;
        private readonly List<ParsingException> _errors;

        public Tokenizer(string expression)
        {
            _errors = new List<ParsingException>();
            _expression = expression;
            _ind = 0;
            _parenthesisBalance = 0;
            _curToken = Token.Begin;
        }

        public int GetInd()
        {
            return _ind;
        }

        public string GetExpression()
        {
            return _expression;
        }

        public List<ParsingException> GetErrors()
        {
            return _errors;
        }

        public void AddException(ParsingException e)
        {
            _errors.Add(e);
        }

        public Token GetCurToken()
        {
            return _curToken;
        }

        public Token GetNextToken()
        {
            NextToken();
            return _curToken;
        }

        public double GetValue()
        {
            return _value;
        }

        private void SkipWhiteSpaces()
        {
            while (_ind < _expression.Length && char.IsWhiteSpace(_expression[_ind]))
            {
                _ind++;
            }
        }

        private string GetNumber()
        {
            var l = _ind;
            while (_ind < _expression.Length &&
                   (_expression[_ind] == '.' || char.IsDigit(_expression[_ind])))
            {
                _ind++;
            }

            var r = _ind;
            _ind--;
            return _expression.Substring(l, r - l);
        }

        private void CheckForArgument()
        {
            if (_curToken != Token.OpenParenthesis && _curToken != Token.Begin && !_curToken.IsOperation())
            {
                return;
            }

            _errors.Add(new MissingArgumentException(_expression, _ind));
        }

        private bool CheckForOperation()
        {
            if (_curToken != Token.CloseParenthesis && _curToken != Token.Number)
            {
                return true;
            }

            _errors.Add(new MissingOperationException(_expression, _ind));
            return false;
        }

        private void NextToken()
        {
            if (_curToken == Token.End)
            {
                return;
            }

            SkipWhiteSpaces();
            if (_ind >= _expression.Length)
            {
                CheckForArgument();
                _curToken = Token.End;
                return;
            }

            var c = _expression[_ind];
            switch (c)
            {
                case '-':
                    if (_curToken == Token.Number || _curToken == Token.CloseParenthesis || _curToken == Token.Error)
                    {
                        _curToken = Token.Subtract;
                    }
                    else
                    {
                        if (_ind + 1 >= _expression.Length)
                        {
                            _errors.Add(new MissingArgumentException(_expression, _ind + 1));
                            _curToken = Token.Error;
                        }
                        else if (char.IsDigit(_expression[_ind + 1]))
                        {
                            _ind++;
                            var begin = _ind;
                            var firstDigit = _expression[_ind];
                            var tmp = GetNumber();
                            try
                            {
                                _value = double.Parse("-" + tmp);
                            }
                            catch (FormatException)
                            {
                                _errors.Add(new NumberParsingException(_expression, begin, _ind + 1));
                                _value = firstDigit;
                            }

                            _curToken = Token.Number;
                        }
                        else
                        {
                            _curToken = Token.Negate;
                        }
                    }

                    break;
                case '+':
                    CheckForArgument();
                    _curToken = Token.Add;
                    break;
                case '*':
                    CheckForArgument();
                    _curToken = Token.Multiply;
                    break;
                case '/':
                    CheckForArgument();
                    _curToken = Token.Divide;
                    break;
                case '(':
                    if (_curToken == Token.CloseParenthesis || _curToken == Token.Number)
                    {
                        _errors.Add(new UnexpectedOpenParenthesisException(_expression, _ind));
                    }
                    else
                    {
                        _parenthesisBalance++;
                        _curToken = Token.OpenParenthesis;
                    }

                    break;
                case ')':
                    if (_curToken == Token.OpenParenthesis || _curToken.IsOperation())
                    {
                        _errors.Add(new MissingArgumentException(_expression, _ind));
                    }

                    if (_parenthesisBalance - 1 < 0)
                    {
                        _errors.Add(new UnexpectedCloseParenthesisException(_expression, _ind));
                        _curToken = Token.Error;
                    }
                    else
                    {
                        _parenthesisBalance--;
                        _curToken = Token.CloseParenthesis;
                    }

                    break;
                default:
                    if (char.IsDigit(c))
                    {
                        if (CheckForOperation())
                        {
                            var begin = _ind;
                            var tmp = GetNumber();
                            try
                            {
                                _value = double.Parse(tmp);
                            }
                            catch (FormatException)
                            {
                                _errors.Add(new NumberParsingException(_expression, begin, _ind + 1));
                                _value = int.Parse(c.ToString());
                            }

                            _curToken = Token.Number;
                        }
                        else
                        {
                            _curToken = Token.Error;
                        }
                    }
                    else
                    {
                        _errors.Add(new UnexpectedTokenException(_expression, _ind));
                        _curToken = Token.Error;
                    }

                    break;
            }

            _ind++;
        }
    }
}