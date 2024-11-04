using System.Reflection;

namespace Prisma.Core
{
    public static class EntityExtensions
    {
        public static string GetColumns(this object entity)
        {
            var properties = entity.GetObjectProperties();

            return string.Join(", ", properties.Select(p => p.Name));
        }

        public static string GetValues(this object entity)
        {
            var properties = entity.GetObjectProperties();

            return string.Join(", ", properties.Select(p => "@" + p.Name));
        }

        public static string ConstructWhereClause(this object entity)
        {
            var properties = entity.GetType().GetProperties()
               .Where(p => p.CanRead && p.CanWrite)
               .ToArray();

            var left = properties.FirstOrDefault(p => p.Name.Contains("Id"))!.Name;
            var right = string.Join(", ", properties.Where(p => p.Name.Contains("Id")).Select(p => "@" + p.Name));

            return $"{left} = {right}";
        }

        public static string ConstructSetClause(this object entity)
        {
            var properties = entity.GetType().GetProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.Name.Contains("Id"))
                .ToArray();

            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            return setClause;
        }

        public static PropertyInfo[] GetObjectProperties(this object entity)
        {
            var properties = entity.GetType().GetProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.Name.Contains("Id"))
                .ToArray();

            return properties;
        }
    }
}
