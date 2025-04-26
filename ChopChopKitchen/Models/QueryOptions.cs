using System.Linq.Expressions;

namespace ChopChopKitchen.Models
{
    public class QueryOptions<T> where T : class
    {
        public Expression<Func<T, Object>> OrderBy { get; set; } = null!;
        public Expression<Func<T, bool>> Where { get; set; } = null!;

        private string[] includes = Array.Empty<string>();

        public string Incldues
        {
            set => includes = value.Replace(" ","").Split(',');
        }

        public string[] GetIncludes() => includes;

        public bool HasWhere => Where != null;

        public bool HasOrderBy => OrderBy != null; 
    }
}