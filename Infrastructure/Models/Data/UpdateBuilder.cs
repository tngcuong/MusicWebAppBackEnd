using MongoDB.Driver;
using System.Linq.Expressions;

namespace MusicWebAppBackend.Infrastructure.Models.Data
{
    public class UpdateBuilder<T>
    {
        private readonly List<UpdateDefinition<T>> _list = new List<UpdateDefinition<T>>();

        private readonly List<ExpressionFieldDefinition<T, object>> _expressionFieldDefinitions = new List<ExpressionFieldDefinition<T, object>>();

        public IEnumerable<UpdateDefinition<T>> Fields => _list;

        public IEnumerable<ExpressionFieldDefinition<T, object>> ExpressionFields => _expressionFieldDefinitions;

        protected UpdateBuilder()
        {
        }

        public static UpdateBuilder<T> Create()
        {
            return new UpdateBuilder<T>();
        }

        public UpdateBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value)
        {
            _list.Add(Builders<T>.Update.Set(selector, value));
            _expressionFieldDefinitions.Add(new ExpressionFieldDefinition<T, object>(selector, value));
            return this;
        }
    }
}
