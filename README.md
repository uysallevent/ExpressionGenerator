# ExpressionGenerator
This code block generate expression for according model with using reflection.
All you have to do is adding the model you want to expression to the parameters.

This class accept 2 generic models. If your input and output models are the same, you can give the same model in both. But When your input and output model are different,  properties names must be same.

I provided control for just 4 data types, but you can increase it if you want.

# Important
If your request and response models are different, properties names must be same but not all properties are required for response model (You can prevent unwanted properties from appearing on the output)

# Result
I used it to generate linq queries for filtering operations coming from users table.
Hopefully it benefit your business.

```
        static MethodInfo methodInfo;
        public static Expression<Func<Tres, bool>> Generate(Treq request)
        {
            Expression finalExpression = Expression.Constant(true);
            var parameter = Expression.Parameter(typeof(Tres), "x");
            Expression expression = null;
            foreach (var item in request.GetType().GetProperties().ToList())
            {
                //when Model property was string object, called object does getting lowercase
                if (typeof(string).IsAssignableFrom(item.PropertyType))
                {
                    methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                    var argumentsForStringObject = Expression.Constant(item.GetValue(request, null).ToString().ToLower());
                    var propertyForStringObject = Expression.Property(parameter, item.Name);
                    var stringObjectWithLowerCase = Expression.Call(propertyForStringObject, "ToLower", null);
                    expression = Expression.Call(stringObjectWithLowerCase, methodInfo, argumentsForStringObject);
                }
                else
                {
                    if (typeof(int).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(int).GetMethod("Equals", new Type[] { typeof(int) });
                    if (typeof(decimal).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(decimal).GetMethod("Equals", new Type[] { typeof(decimal) });
                    if (typeof(double).IsAssignableFrom(item.PropertyType))
                        methodInfo = typeof(double).GetMethod("Equals", new Type[] { typeof(double) });
                    if (typeof(DateTime).IsAssignableFrom(item.PropertyType))
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

            if (finalExpression == null || parameter == null)
                return null;
            return Expression.Lambda<Func<Tres, bool>>(finalExpression, parameter);
        }
```
