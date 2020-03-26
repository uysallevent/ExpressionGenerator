using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionGenerator
{
    public class ExpressionGenerator<Treq, Tres> : IExpressionGenerator<Treq, Tres>
        where Treq : class, new()
        where Tres : class, new()
    {
        MethodInfo methodInfo;
        public Expression<Func<Treq, bool>> Generate(Treq request)
        {
            Expression finalExpression = Expression.Constant(true);
            var parameter = Expression.Parameter(typeof(Treq), "x");
            Expression expression = null;
            foreach (var item in request.GetType().GetProperties().ToList())
            {
                if (typeof(string).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                //Optional. Our 
                if (typeof(int).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (int)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(decimal).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (decimal)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(double).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (double)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(DateTime).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                if (typeof(string).IsAssignableFrom(item.PropertyType))
                    methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                if (typeof(int).IsAssignableFrom(item.PropertyType))
                    methodInfo = typeof(int).GetMethod("Equals", new Type[] { typeof(int) });
                if (typeof(decimal).IsAssignableFrom(item.PropertyType))
                    methodInfo = typeof(decimal).GetMethod("Equals", new Type[] { typeof(decimal) });
                if (typeof(double).IsAssignableFrom(item.PropertyType))
                    methodInfo = typeof(double).GetMethod("Equals", new Type[] { typeof(double) });
                if (typeof(DateTime).IsAssignableFrom(item.PropertyType))
                    methodInfo = typeof(DateTime).GetMethod("Equals", new Type[] { typeof(DateTime) });

                var constanttxtBayiKod = Expression.Constant(item.GetValue(request, null));
                var propertytxtBayiKod = Expression.Property(parameter, item.Name);
                expression = Expression.Call(propertytxtBayiKod, methodInfo, constanttxtBayiKod);

                if (expression == null)
                    continue;
                finalExpression = Expression.AndAlso(finalExpression, expression);
                expression = null;
            }

            if (finalExpression == null || parameter == null)
                return null;
            return Expression.Lambda<Func<Treq, bool>>(finalExpression, parameter);
        }
    }

}
