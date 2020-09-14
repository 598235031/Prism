using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
 
namespace Wanghzh.Prism.ViewModel
{
    [Obsolete("Please use Prism.Mvvm.PropertySupport.")]
    public static class PropertySupport
    {
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
            }
            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
            }
            return memberExpression.Member.Name;
        }
    }
}
