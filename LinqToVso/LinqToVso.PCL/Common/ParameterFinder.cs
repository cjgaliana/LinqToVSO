/***********************************************************
 * Credits:
 *
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 *
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 *
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 *
 * http://blogs.msdn.com/mattwar/default.aspx
 * *********************************************************/

using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToVso
{
    /// <summary>
    ///     extracts parameters from an expression
    ///     - called for extracting parameters and values on where clauses
    /// </summary>
    /// <typeparam name="T">type to get parameters for</typeparam>
    internal class ParameterFinder<T> : ExpressionVisitor
    {
        /// <summary>
        ///     expression being searched
        /// </summary>
        private readonly Expression expression;

        /// <summary>
        ///     parameters to search for
        /// </summary>
        private Dictionary<string, string> parameters;

        /// <summary>
        ///     keep track of expression and parameter list
        /// </summary>
        /// <param name="exp">expression to search</param>
        /// <param name="parameters">parameters to search for</param>
        public ParameterFinder(Expression exp, List<string> parameters)
        {
            this.expression = exp;
            this.ParameterNames = parameters;
        }

        /// <summary>
        ///     name/value pairs of parameters and their values
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new Dictionary<string, string>();
                    this.Visit(this.expression);
                }
                return this.parameters;
            }
        }

        /// <summary>
        ///     names of input parameters
        /// </summary>
        public List<string> ParameterNames { get; set; }

        /// <summary>
        ///     extracts values from equality expressions that match parameter names
        /// </summary>
        /// <param name="be">binary expression to evaluate</param>
        /// <returns>binary expression - supports recursive tree traversal in visitor</returns>
        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal
                || be.NodeType == ExpressionType.GreaterThan
                || be.NodeType == ExpressionType.GreaterThanOrEqual
                || be.NodeType == ExpressionType.LessThan
                || be.NodeType == ExpressionType.LessThanOrEqual
                || be.NodeType == ExpressionType.NotEqual)
            {
                foreach (string param in this.ParameterNames)
                {
                    if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof (T), param))
                    {
                        this.parameters.Add(param,
                            ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof (T), param));
                        return be;
                    }
                }

                return base.VisitBinary(be);
            }
            return base.VisitBinary(be);
        }

        protected override Expression VisitMethodCall(MethodCallExpression me)
        {
            foreach (string param in this.ParameterNames)
            {
                if (me.Method.Name == "CompareString" && (me.Arguments[0] as MemberExpression).Member.Name == param)
                {
                    this.parameters.Add(param, (me.Arguments[1] as ConstantExpression).Value.ToString());
                    return me;
                }
            }

            return base.VisitMethodCall(me);
        }
    }
}