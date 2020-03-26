using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LinqExpressionGenerator
{
    public interface IExpressionGenerator<Treq, Tres>
        where Treq : class, new()
        where Tres : class, new()
    {
        Expression<Func<Treq, bool>> Generate(Treq request);
    }
}
