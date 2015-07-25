using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    internal class ExpressionTreeModifier<T> : ExpressionVisitor
    {
        private readonly IQueryable<T> queryableItems;

        internal ExpressionTreeModifier(IQueryable<T> items)
        {
            this.queryableItems = items;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            //TODO: Replace the constant VsoQueryable arg with the queryable collection.
            if (c.Type.Name == "VsoQueryable`1")
                return Expression.Constant(this.queryableItems);

            return c;
        }
    }
}