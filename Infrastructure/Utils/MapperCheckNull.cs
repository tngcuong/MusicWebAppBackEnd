using AutoMapper;
using System.Reflection;

namespace MusicWebAppBackend.Infrastructure.Utils
{
    public static class MapperCheckNull
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {

            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                var sourceProperty = sourceType.GetProperty(property.Name, flags);
                if (sourceProperty == null)
                {
                    // Ignore mapping for properties that do not exist in the source type
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
                else
                {
                    // Conditionally map properties: only map if the source property is not null
                    expression.ForMember(property.Name, opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                }
            }
            return expression;
        }
    }
}
