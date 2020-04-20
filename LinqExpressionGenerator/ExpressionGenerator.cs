﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExpressionGenerator
{
    public static class ExpressionGenerator<Treq, Tres>
        where Treq : class, new()
        where Tres : class, new()
    {
        static MethodInfo methodInfo;
        public static Expression<Func<Tres, bool>> Generate(Treq request)
        {
            Expression finalExpression = Expression.Constant(true);
            var parameter = Expression.Parameter(typeof(Tres), "x");
            Expression expression = null;
            foreach (var item in request.GetType().GetProperties().ToList())
            {
                if (typeof(string).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                if (typeof(int).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (int)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(decimal).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (decimal)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(double).IsAssignableFrom(item.PropertyType) && (item.GetValue(request, null) == null || (double)item.GetValue(request, null) == 0))
                    continue;

                if (typeof(DateTime).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                if (typeof(DateTime?).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                //when model property was string, called object does getting lowercase
                if (typeof(string).IsAssignableFrom(item.PropertyType))
                {
                    methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                    var argumentsForStringObject = Expression.Constant(item.GetValue(request, null).ToString().ToLower());
                    var propertyForStringObject = Expression.Property(parameter, item.Name);
                    var stringObjectWithLowerCase = Expression.Call(propertyForStringObject, "ToLower", null);
                    expression = Expression.Call(stringObjectWithLowerCase, methodInfo, argumentsForStringObject);
                }
                //if you need to filter between two date, you have to set properties names to be 'TrhBasTarih','TrhBitTarih' 
                else if ((typeof(DateTime).IsAssignableFrom(item.PropertyType) || typeof(DateTime?).IsAssignableFrom(item.PropertyType)) && item.Name.Split('_').Length > 0)
                {
                    var splittedDate = item.Name.Split('_');
                    var propDate = Expression.Property(parameter, splittedDate[0]);
                    var constDate = Expression.Constant(Convert.ToDateTime(item.GetValue(request, null)).Date, (typeof(DateTime?)));

                    expression =
                        (splittedDate[1] == "BasTarih") ?
                        Expression.GreaterThanOrEqual(Expression.MakeMemberAccess(Expression.Convert(propDate, typeof(DateTime)), typeof(DateTime).GetMember("Date").Single()), Expression.Convert(constDate, typeof(DateTime))) :
                        (splittedDate[1] == "BitTarih") ?
                        Expression.LessThanOrEqual(Expression.MakeMemberAccess(Expression.Convert(propDate, typeof(DateTime)), typeof(DateTime).GetMember("Date").Single()), Expression.Convert(constDate, typeof(DateTime))) :
                        Expression.AndAlso(finalExpression, parameter);
                }
                else
                {
                    if (typeof(int).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(int).GetMethod("Equals", new Type[] { typeof(int) });
                    if (typeof(decimal).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(decimal).GetMethod("Equals", new Type[] { typeof(decimal) });
                    if (typeof(double).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(double).GetMethod("Equals", new Type[] { typeof(double) });
                    if(typeof(DateTime).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(DateTime).GetMethod("Equals", new Type[] { typeof(DateTime) });


                    var arguments = Expression.Constant(item.GetValue(request, null));
                    var property = Expression.Property(parameter, item.Name);
                    expression = Expression.Call(property, methodInfo, arguments);
                }

                if (expression == null)
                    continue;

                finalExpression = Expression.AndAlso(finalExpression, expression);
                expression = null;
            }

            return Expression.Lambda<Func<Tres, bool>>(finalExpression, parameter);
        }

    }

}
