using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SharpToolkit.Extensions.Reflection.Compilation
{
    public static class ConstructorCompiler
    {
        /// <summary>
        /// Compiles a constructor call wrapped in lambda. This method works almost as fast as static call.
        /// </summary>
        /// <remarks>
        /// C# doesn't allow delegates in generic type, so the client code would need to cast the delegate
        /// to the correct Func<> delegate.
        /// </remarks>
        /// <param name="constr">The constructor info for which the labbda would be compiled.</param>
        /// <returns>A lambda delegate.</returns>
        public static Delegate CompileConstructorCall(this ConstructorInfo constr)
        {
            var parameters =
                constr.GetParameters()
                .Select(x => new { type = x.ParameterType, name = x.Name });

            var parametersExpr =
                parameters
                .Select(x => Expression.Parameter(x.type, x.name))
                .ToArray();

            var newExpr    = Expression.New(constr, parametersExpr);
            var lambdaExpr = Expression.Lambda(newExpr, parametersExpr);

            return lambdaExpr.Compile();
        }        
    }
}
