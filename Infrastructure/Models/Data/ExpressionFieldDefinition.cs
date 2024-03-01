using System.Linq.Expressions;

namespace MusicWebAppBackend.Infrastructure.Models.Data
{
    public class ExpressionFieldDefinition<TDocument, TField>
    {
        public LambdaExpression Expression { get; }

        public TField Value { get; }

        public ExpressionFieldDefinition(LambdaExpression expression, TField value)
        {
            Expression = expression;
            Value = value;
        }
    }
}
