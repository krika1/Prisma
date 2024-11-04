using System.Linq.Expressions;

namespace Prisma.Core
{
    public static class ExpressionExtractor
    {
        public static string ConvertExpressionToSql<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            var body = predicate.Body as BinaryExpression;
            if (body == null)
                throw new NotSupportedException("Only binary expressions are supported.");

            var left = body.Left.ToString().Split('.').Last(); // Left side of the expression
            var right = body.Right.ToString(); // Right side of the expression
            var operatorSymbol = GetOperatorSymbol(body.NodeType); // Get the SQL equivalent of the operator

            // Example: "Name = 'John'"
            return $"{left} {operatorSymbol} {right}";
        }

        public static string SubstituteWhereForFK(string clause, string primaryKey, string foreignKey)
            => clause.Replace(primaryKey, foreignKey);

        private static string GetOperatorSymbol(ExpressionType nodeType)
        {
            return nodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Operator {nodeType} is not supported.")
            };
        }
    }
}
