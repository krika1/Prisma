using System.Reflection;

namespace Prisma.Core
{
    public static class DatabaseSetsInitlizer
    {
        public static void Initlize(DatabaseContext context, ICollection<(Type setType, string name)>? _sets)
        {
            Type contextType = context.GetType();

            PropertyInfo[] contextProperties = contextType.GetProperties();

            foreach (var property in contextProperties)
            {
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DatabaseSet<>))
                {
                    Type genericType = property.PropertyType;

                    _sets!.Add((genericType.GetGenericArguments().First(), property.Name));

                    var databaseSetInstance = Activator.CreateInstance(genericType, context);

                    property.SetValue(context, databaseSetInstance);
                }
            }
        }
    }
}
