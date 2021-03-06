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
                if (typeof(string).IsAssignableFrom(item.PropertyType) && item.GetValue(request, null) == null)
                    continue;

                if ((typeof(int).IsAssignableFrom(item.PropertyType) || typeof(int?).IsAssignableFrom(item.PropertyType)) && (item.GetValue(request, null) == null || (int)item.GetValue(request, null) == 0))
                    continue;

                if ((typeof(short).IsAssignableFrom(item.PropertyType) || typeof(short?).IsAssignableFrom(item.PropertyType)) && (item.GetValue(request, null) == null || (short)item.GetValue(request, null) == 0))
                    continue;

                if ((typeof(decimal).IsAssignableFrom(item.PropertyType) || typeof(decimal?).IsAssignableFrom(item.PropertyType)) && (item.GetValue(request, null) == null || (decimal)item.GetValue(request, null) == 0))
                    continue;

                if ((typeof(double).IsAssignableFrom(item.PropertyType) || typeof(double?).IsAssignableFrom(item.PropertyType)) && (item.GetValue(request, null) == null || (double)item.GetValue(request, null) == 0))
                    continue;

                if ((typeof(DateTime).IsAssignableFrom(item.PropertyType) || typeof(DateTime?).IsAssignableFrom(item.PropertyType)) && item.GetValue(request, null) == null)
                    continue;

                //when model property was string, called object does getting lowercase
                if (typeof(string).IsAssignableFrom(item.PropertyType))
                {
                    methodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                    var arguments = Expression.Constant(item.GetValue(request, null).ToString().ToLower());
                    var property = Expression.Property(parameter, item.Name);
                    var stringObjectWithLowerCase = Expression.Call(property, "ToLower", null);
                    expression = Expression.Call(stringObjectWithLowerCase, methodInfo, arguments);
                }
                //if you need to filter between two date, you have to set properties names to be 'TrhBasTarih','TrhBitTarih' 
                else if ((typeof(DateTime).IsAssignableFrom(item.PropertyType) || typeof(DateTime?).IsAssignableFrom(item.PropertyType)) && item.Name.Split('_').Length > 1)
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
                else if ((typeof(DateTime).IsAssignableFrom(item.PropertyType) || typeof(DateTime?).IsAssignableFrom(item.PropertyType)))
                {
                    Expression property = Expression.PropertyOrField(parameter, item.Name);
                    Expression arguments = Expression.Constant(item.GetValue(request, null));
                    methodInfo = typeof(DateTime).GetMethod("Equals", new Type[] { typeof(DateTime) });
                    expression = Expression.Call(Expression.MakeMemberAccess(Expression.Convert(property, typeof(DateTime)), typeof(DateTime).GetMember("Date").Single()), methodInfo, arguments);
                }
                else
                {
                    var arguments = Expression.Constant(item.GetValue(request, null));
                    var property = Expression.Property(parameter, item.Name);
                    expression = Expression.Equal(property, Expression.Convert(arguments, property.Type));
                }

                if (expression == null)
                    continue;

                finalExpression = Expression.AndAlso(finalExpression, expression);
                expression = null;
            }

            return Expression.Lambda<Func<Tres, bool>>(finalExpression, parameter);
        }
```
