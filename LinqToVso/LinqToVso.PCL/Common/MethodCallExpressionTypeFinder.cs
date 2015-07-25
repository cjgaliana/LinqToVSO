using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso.Common
{
    internal class MethodCallExpressionTypeFinder : ExpressionVisitor
    {
        private Type genericType;

        /// <summary>
        ///     Gets the underlying type of the whole method call expression
        /// </summary>
        /// <param name="exp">MethodCallExpression</param>
        /// <returns>Type</returns>
        public Type GetGenericType(Expression exp)
        {
            var expresion = this.Visit(exp);
            if (expresion!=null)
            {
                if (expresion.Type.GenericTypeArguments == null)
                {
                    return expresion.Type;
                }

                return expresion.Type.GenericTypeArguments[0];
            }
            return this.genericType;
        }


        /// <summary>
        ///     Sets the expression type when found
        /// </summary>
        /// <param name="expression">a MethodCallExpression node from the expression tree</param>
        /// <returns>expression that was passed in</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Arguments.Count > 0)
            {
                this.genericType = expression.Method.GetGenericArguments()[0];
            }

            // look at extension source to see if there is an inner type
            var visitedExpression = this.Visit(expression.Arguments[0]);

            return expression;
        }
    }
}