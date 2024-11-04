namespace Prisma.Core
{
    public class NavigationProperty
    {
        public object? ForeignKey { get; set; }

        public object? ForeignKeyReference { get; set; }

        public string? NavigationProprtyName { get; set; }

        public NavigationProperty(object foreignKey, object foreignKeyReference, string navigationPropertyName)
        {
            ForeignKey = foreignKey;
            ForeignKeyReference = foreignKeyReference;
            NavigationProprtyName = navigationPropertyName;
        }
    }
}
