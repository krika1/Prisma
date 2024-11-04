namespace Prisma.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public Type RelatedEntityType { get; }
        public string RelatedKey { get; }

        public ForeignKeyAttribute(Type relatedEntityType, string relatedKey)
        {
            RelatedEntityType = relatedEntityType;
            RelatedKey = relatedKey;
        }
    }
}
