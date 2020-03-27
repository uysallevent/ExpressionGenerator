# ExpressionGenerator
This code block generate expression for according model with using reflection.
All you have to do is adding the model you want to expression to the parameters.

This class accept 2 generic models. If your input and output models are the same, you can give the same model in both. But When your input and output model are different,  properties names must be same.

I provided control for just 4 data types, but you can increase it if you want.


# Important
1) If your input and output models are different, properties names must be same but not all properties are required for response model (You can prevent unwanted properties from appearing on the output)

```
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
            return Expression.Lambda<Func<Tres, bool>>(finalExpression, parameter);
        }
    }
```
