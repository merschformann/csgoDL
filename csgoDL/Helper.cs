using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace csgoDL
{
    /// <summary>
    /// Contains some helping functions
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Returns the name of a field providing more type-safety - use <code>string name = Check(() => field);</code> (on the basis of <see cref="http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html"/>)
        /// </summary>
        /// <typeparam name="T">The type of the field</typeparam>
        /// <param name="expr">The expression including the field</param>
        /// <returns>Code-name of the field</returns>
        public static string Check<T>(Expression<Func<T>> expr)
        {
            if (expr.Body is MemberExpression)
            {
                var body = ((MemberExpression)expr.Body);
                return body.Member.Name;
            }
            else
            {
                if (expr.Body is ConstantExpression) { var body = ((ConstantExpression)expr.Body); return body.Value.ToString(); }
                else { throw new ArgumentException("Cannot handle this expression: " + expr.ToString()); }
            }
        }
    }
}
