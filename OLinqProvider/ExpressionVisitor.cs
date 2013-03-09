using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace OLinqProvider
{
    public class OExpressionVisitor : ExpressionVisitor
    {
        private bool _isCount;
        private string _filter = string.Empty;
        private string _orderBy;
        private string _skip;
        private string _take;
        private string _first;
        private string _specialFunction;
        private Stack<string> _orExpressionStack = new Stack<string>();
        private Stack<string> _andExpressionStack = new Stack<string>();
        public string Parse(Expression expression)
        {
            Visit(expression);

            var result = string.Empty;

            if (_isCount)
            {
                result += "/$count";
            }
 
            if (!string.IsNullOrWhiteSpace(_filter))
            {
                result = result.AddParameter("$filter=" + _filter);
            }
            result = ProcessAdditionalTokens(result);
            if (!_isCount)
            {

                result = result.AddParameter("$format=json");
            }
            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                /*These conditions are handled elsewhere*/
                case "ToUpper":
                case "Length":
                case "ToLower":
                case "YearCompare":
                case "MinuteCompare":
                case "DayCompare":
                    break;
                case "First":
                case "FirstOrDefault":
                    SetFirst(m);
                    break;
                case "OrderBy":
                     SetOrderingClause(m);
                    break;
                case "OrderByDescending":
                    SetDescOrderingClause(m);
                    break;
                case "Count":
                    _isCount = true;
                    break;
                case "GroupBy":
                case "StartsWith":
                case "Contains":
                case "Where":
                case "EndsWith":
                    SetWhereQuery(m);
                    break;
                case "Skip":
                   SetSkip(m);
                    break;
                case "Take":
                    SetTake(m);
                    break;
                case "Max":
                case "Min":
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }

            return base.VisitMethodCall(m);
        }

        private void SetWhereQuery(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
            {
                return;
            }

            Expression arg = m.Arguments[1];
            var unary = arg as UnaryExpression;
            var op = unary.Operand as LambdaExpression;

            AppendFilter(CreateOperand(op.Body));
        }
        private void SetFirst(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
            {
                return;
            }

            Expression arg = m.Arguments[1];
            var unary = arg as UnaryExpression;
            var op = unary.Operand as LambdaExpression;

            AppendFilter(CreateOperand(op.Body));
            _first = "$top=1";
        }
   
        private void SetOrderingClause(MethodCallExpression m)
        {
            // need to make sure DataServiceVersion > 2.0
            if (m.Arguments.Count == 1)
            {
                return;
            }
            Expression arg = m.Arguments[1];
            var unary = arg as UnaryExpression;
            var op = unary.Operand as LambdaExpression;
            var exp = op.Body as MemberExpression;

           _orderBy = string.Format("$orderby={0}", ParseProperty(exp.Member.Name));
        }
        private void SetDescOrderingClause(MethodCallExpression m)
        {
            //need to make sure DataServiceVersion > 2.0
            if (m.Arguments.Count == 1)
            {
                return;
            }
            Expression arg = m.Arguments[1];
            var unary = arg as UnaryExpression;
            var op = unary.Operand as LambdaExpression;
            var exp = op.Body as MemberExpression;

            _orderBy = string.Format("$orderby={0} desc", ParseProperty(exp.Member.Name));
        }

        private void SetSkip(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
            {
                return;
            }
            var arg = m.Arguments[1] as ConstantExpression;


            _skip = string.Format("$skip={0}", arg.Value);
        }
        private void SetTake(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
            {
                return;
            }
            var arg = m.Arguments[1] as ConstantExpression;


            _take = string.Format("$top={0}", arg.Value);
        }
        private void AppendFilter(string param)
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                _filter = param;
                return;
            }

            throw new Exception("Unable to Create Operand");
        }
  
        private string CreateOperand(Expression expression)
        {
            var binary = expression as BinaryExpression;
            
            if (binary != null)
            {
                return IsSpecialFunction(binary.Left.ToString()) ? TranslateSpecialFunction(binary)
                    : CreateOdataOperand(binary);
            }

            var methodCall = expression as MethodCallExpression;

            if (methodCall != null)
            {
                return CreateOperand(methodCall);
            }

            throw new Exception("Unable to Create Operand");
        }
        private string CreateOdataOperand(BinaryExpression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return CreateUnary(body.NodeType, body.Left as MemberExpression, body.Right);
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return CreateAnd(body.Left, body.Right);
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return CreateOr(body.Left, body.Right);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public string CreateAnd(
                                Expression left, Expression right)
        {
            var sb = new StringBuilder();
            Action<Expression> recurse = null;
            recurse = (lexp) =>
                {

                    if (lexp.NodeType == ExpressionType.And || lexp.NodeType == ExpressionType.AndAlso)
                    {
                        var exp = lexp as BinaryExpression;

                        _andExpressionStack.Push(CreateOperand(exp.Right));
                        recurse(exp.Left);
                    }
                    else
                    {
                        _andExpressionStack.Push(CreateOperand(lexp));
         
                    }
  
                };
            if (left.NodeType == ExpressionType.And || left.NodeType == ExpressionType.AndAlso)
            {
                recurse(left);
            }
            else
            {
                _andExpressionStack.Push(CreateOperand(left));
            }
            var rightExpression = CreateOperand(right);

            foreach (var ex in _andExpressionStack)
            {
                sb.AppendFormat("{0} and ", ex);
            }
            var res = string.Concat(sb.ToString(), rightExpression);
            return res;
        }
        public string CreateOr(Expression left, Expression right)
        {
            var sb = new StringBuilder();
            Action<Expression> recurse = null;
            recurse = (lexp) =>
            {

                if (lexp.NodeType == ExpressionType.Or || lexp.NodeType == ExpressionType.OrElse)
                {
                    var exp = lexp as BinaryExpression;

                    _orExpressionStack.Push(CreateOperand(exp.Right));
                    recurse(exp.Left);
                }
                else
                {
                    _orExpressionStack.Push(CreateOperand(lexp));

                }

            };
            if (left.NodeType == ExpressionType.Or || left.NodeType == ExpressionType.OrElse)
            {
                recurse(left);
            }
            else
            {
                _orExpressionStack.Push(CreateOperand(left));
            }
            var rightExpression = CreateOperand(right);

            foreach (var ex in _orExpressionStack)
            {
                sb.AppendFormat("{0} or ", ex);
            }
            var res = string.Concat(sb.ToString(), rightExpression);
            return res;
        }

        public string CreateUnary(ExpressionType nodeType,
         MemberExpression left, Expression right)
        {
            var prop = ParseProperty(left.Member.Name);
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return string.Format("{0} eq {1}", prop, GetValue(right));
                case ExpressionType.NotEqual:
                    return string.Format("{0} ne {1}", prop, GetValue(right));
                case ExpressionType.GreaterThan:
                    return string.Format("{0} gt {1}", prop, GetValue(right));
                case ExpressionType.GreaterThanOrEqual:
                    return string.Format("{0} ge {1}", prop, GetValue(right));
                case ExpressionType.LessThan:
                    return string.Format("{0} lt {1}", prop, GetValue(right));
                case ExpressionType.LessThanOrEqual:
                    return string.Format("{0} le {1}", prop, GetValue(right));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object GetValue(Expression memberExpression)
        {
            return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
        }
        private string CreateUnary(string name, MemberExpression left, Expression right)
        {
            return string.Format("{0}({1},'{2}')", name, ParseProperty(left.Member.Name), GetValue(right));
        }
        private string CreateUnary(string name, ConstantExpression left, MemberExpression right)
        {
            return string.Format("{0}('{1}',{2})", name,  left.Value, ParseProperty(right.Member.Name));
        }
        private string CreateUnary(string name, MemberExpression left)
        {
            return string.Format("{0}({1})", name, ParseProperty(left.Member.Name));
        }
        private string CreateOperand(MethodCallExpression methodCall)
        {
            switch (methodCall.Method.Name)
            {
                case "StartsWith":
                    return CreateUnary("startswith", methodCall.Object as MemberExpression, methodCall.Arguments[0]);
                case "EndsWith":
                    return CreateUnary("endswith", methodCall.Object as MemberExpression, methodCall.Arguments[0]);
                case "ToUpper":
                    return CreateUnary("toupper", methodCall.Object as MemberExpression, methodCall.Arguments[0]);
                case "ToLower":
                    return CreateUnary("tolower", methodCall.Object as MemberExpression, methodCall.Arguments[0]);
                case "Trim":
                    return CreateUnary("trim", methodCall.Object as MemberExpression, methodCall.Arguments[0]);
                case "Contains":
                    return CreateUnary("substringof", methodCall.Arguments[0] as ConstantExpression, methodCall.Object as MemberExpression);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        private string TranslateLengthFunction(BinaryExpression exp)
        {
            var nodeType = exp.NodeType;
            var lft = exp.Left as MemberExpression;

            var left = lft.Expression as MemberExpression;
            var right = exp.Right;

            var prop = ParseProperty(left.Member.Name);
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return string.Format("length({0}) eq {1}", prop, GetValue(right));
                case ExpressionType.NotEqual:
                    return string.Format("length({0}) ne {1}", prop, GetValue(right));
                case ExpressionType.GreaterThan:
                    return string.Format("length({0}) gt {1}", prop, GetValue(right));
                case ExpressionType.GreaterThanOrEqual:
                    return string.Format("length({0}) ge {1}", prop, GetValue(right));
                case ExpressionType.LessThan:
                    return string.Format("length({0}) lt {1}", prop, GetValue(right));
                case ExpressionType.LessThanOrEqual:
                    return string.Format("length({0}) le {1}", prop, GetValue(right));
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        private string SetDateFilter(BinaryExpression exp)
        {
            var nodeType = exp.NodeType;
            var lft = exp.Left as MethodCallExpression;
            var arg = lft.Arguments[0] as MemberExpression;
            string member = ParseProperty(arg.Member.Name);
   
            var right = exp.Right as ConstantExpression;
            string method = _specialFunction.Replace("Compare", "").ToLower();
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return string.Format("{0}({1}) eq {2}",method, member, right.Value);
                case ExpressionType.NotEqual:
                    return string.Format("{0}({1}) ne {2}",method, member, right.Value);
                case ExpressionType.GreaterThan:
                    return string.Format("{0}({1}) gt {2}",method, member, right.Value);
                case ExpressionType.GreaterThanOrEqual:
                    return string.Format("{0}({1}) ge {2}",method, member, right.Value);
                case ExpressionType.LessThan:
                    return string.Format("{0}({1}) lt {2}",method, member, right.Value);
                case ExpressionType.LessThanOrEqual:
                    return string.Format("{0}({1}) le {2}",method, member, right.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private string TranslateSpecialFunction(BinaryExpression exp)
        {
            switch (_specialFunction.ToUpper())
            {
                case "LENGTH":
                    return TranslateLengthFunction(exp);
                case "TOLOWER":
                    return TranslateCaseFunction(exp, "tolower");
                case "TOUPPER":
                    return TranslateCaseFunction(exp,"toupper");
                case "YEARCOMPARE":
                    return SetDateFilter(exp);
                case "MONTHCOMPARE":
                    return SetDateFilter(exp);
                case "DAYCOMPARE":
                    return SetDateFilter(exp);
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
        private string TranslateCaseFunction(BinaryExpression exp, string function)
        {
            var lft = exp.Left as MethodCallExpression;
            var left = lft.Object as MemberExpression;
            var right = exp.Right as ConstantExpression;

            return string.Format("{2}({0}) eq {1}", left.Member.Name, right.Value,function);
        }
        /*This is questionable*/
        private bool IsSpecialFunction(string leftExpression)
        {
            var r = new Regex(@"(\w+\.\w+\.Length)|(\w+\.\w+\.ToUpper)|(\w+\.\w+\.ToLower)|(\w+\.\w+\.YearCompare)");

            Match m = r.Match(leftExpression);
            if (m.Success)
            {
                var match = m.Value;
                _specialFunction = match.Substring(match.LastIndexOf(".")+1);
                return true;
            }
            return false;
        }
        private string ProcessAdditionalTokens(string req)
        {
            if (!string.IsNullOrWhiteSpace(_orderBy))
            {
                req = req.AddParameter(_orderBy);
            }
            if (!string.IsNullOrWhiteSpace(_first))
            {
                req = req.AddParameter(_first);
                return req;
            }
            if (!string.IsNullOrEmpty(_skip))
            {
                req = req.AddParameter(_skip);
            }
            if (!string.IsNullOrEmpty(_take))
            {
                req = req.AddParameter(_take);
            }
            return req;
        }
        private string ParseProperty(string val)
        {
            return val.Replace(".", "/");
        }

       
    }
}
