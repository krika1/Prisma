namespace Prisma.Core
{
    public static class DatabaseTranslator
    {
        public static string Translate(QueryType queryType)
        {
            if (queryType == QueryType.SelectAll)
            {
                return "SELECT * FROM {0}";
            }

            if (queryType == QueryType.SelectOne)
            {
                return "SELECT * FROM {0} WHERE {1}";
            }

            if (queryType == QueryType.Insert)
            {
                return "INSERT INTO {0} ({1}) VALUES ({2})";
            }

            if (queryType == QueryType.Delete)
            {
                return "DELETE FROM {0} WHERE {1}";
            }

            if (queryType == QueryType.Update)
            {
                return "UPDATE {0} SET {1} WHERE {2}";
            }

            return null!;
        }
    }
}
