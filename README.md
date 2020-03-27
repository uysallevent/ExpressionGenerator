# ExpressionGenerator
This generic class, can be used to generate expression for according to model. 
This metod generated with using reflection. All you have to do is adding the model you want to expression to the parameters.

This class accept 2 generic models. If your input and output models are the same, you can give the same model in both. But When your input and output model are different,  properties names must be same. (You prevent unwanted properties from appearing on the output )


# Important
1) If input and output model are different, properties names must be same but not all properties are required (only the properties you want to show in the answer)

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
