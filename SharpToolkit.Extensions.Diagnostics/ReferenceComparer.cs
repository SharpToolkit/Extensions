using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Diagnostics;

namespace SharpToolkit.Extensions.Diagnostics
{
    public sealed class ReferenceComparer
    {
        private static IDictionary<Type, ReferenceComparer> cache = 
            new Dictionary<Type, ReferenceComparer>();

        public static ReferenceComparer GetComparer(Type objectType)
        {
            if (cache.TryGetValue(objectType, out var cmp))
                return cmp;

            cache.Add(objectType, new ReferenceComparer(objectType));

            return cache[objectType];
        }

        public static ReferenceComparer GetComparerFor(object obj)
        {
            if (obj == null)
                // Doesn't matter, reference is not the same.
                return GetComparer(typeof(object));
            return GetComparer(obj.GetType());
        }

        internal readonly IEnumerable<FieldInfo> Members;
        internal readonly IEnumerable<Delegate> fieldComparers;

        private ReferenceComparer(Type objType)
        {
            var flags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public;

            this.Members =
                objType
                .GetFields(flags)
                .ToArray();

            this.fieldComparers =
                this.Members
                    .Select(x => generateFieldComparer(x).Compile())
                    .ToArray();
        }

        private LambdaExpression generateFieldComparer(FieldInfo info)
        {
            if (info.FieldType == typeof(string) ||
                info.FieldType.IsValueType)
                return Expression.Lambda(
                    Expression.Constant(false), 
                    Expression.Parameter(typeof(object)), 
                    Expression.Parameter(typeof(object)));
            

            if (info.FieldType.IsArray)
                return generateArrayComparer(info);

            return generateTypeComparer(info);
        }

        private LambdaExpression generateTypeComparer(FieldInfo info)
        {
            var leftParam =
                Expression.Parameter(info.DeclaringType, "leftParam");

            var rightParam =
                Expression.Parameter(info.DeclaringType, "rightParam");

            var leftVar =
                Expression.Variable(info.FieldType, "leftVar");

            var rightVar =
                Expression.Variable(info.FieldType, "rightVar");
            

            var returnValue = Expression.Variable(typeof(bool), "returnValue");

            var body =
                Expression.Block(
                    typeof(bool),
                    new[] { leftVar, rightVar, returnValue },
                    Expression.Assign(leftVar, Expression.Field(leftParam, info)),
                    Expression.Assign(rightVar, Expression.Field(rightParam, info)),
                    
                    // if
                    Expression.IfThenElse(
                        // ReferenceEquals
                        Expression.Call(
                            ((Func<object, object, bool>)referenceEquals).Method,
                            //Expression.Convert(leftVar, typeof(object)),
                            //Expression.Convert(rightVar, typeof(object))),
                            leftVar,
                            rightVar),
                        // Set return to true
                        Expression.Assign(returnValue, Expression.Constant(true)),
                        // Else set return to anyResultEquals(GetComparer(left)(left, right))
                        Expression.Assign(returnValue, Expression.Call(
                            // Call anyResultEquals
                            ((Func<IEnumerable<(string, bool)>, bool>)anyResultEquals).Method,
                            Expression.Call(
                                // Get comparer
                                Expression.Call(
                                    ((Func<object, ReferenceComparer>)GetComparerFor).Method, 
                                    leftVar),
                                // Use method of comparer
                                ((Func<object, object, IEnumerable<(string, bool)>>)Compare).Method,
                                leftVar,
                                rightVar
                            )))
                        ),
                    // return
                    returnValue
                    );


            

            var lambda =
                Expression.Lambda(body, leftParam, rightParam);

            return lambda;
        }

        private LambdaExpression generateArrayComparer(FieldInfo info)
        {
            var leftParam =
                Expression.Parameter(info.DeclaringType, "leftParam");

            var rightParam =
                Expression.Parameter(info.DeclaringType, "rightParam");

            var leftVar =
                Expression.Variable(info.FieldType, "leftVar");

            var rightVar =
                Expression.Variable(info.FieldType, "rightVar");


            var returnValue = Expression.Variable(typeof(bool), "returnValue");

            var body =
                Expression.Block(
                    typeof(bool),
                    new[] { leftVar, rightVar, returnValue },
                    Expression.Assign(leftVar, Expression.Field(leftParam, info)),
                    Expression.Assign(rightVar, Expression.Field(rightParam, info)),

                    // if
                    Expression.IfThenElse(
                        // ReferenceEquals
                        Expression.Call(
                            ((Func<object, object, bool>)referenceEquals).Method,
                            //Expression.Convert(leftVar, typeof(object)),
                            //Expression.Convert(rightVar, typeof(object))),
                            leftVar,
                            rightVar),
                        // Set return to true
                        Expression.Assign(returnValue, Expression.Constant(true)),
                        // Else set return to anyResultEquals(GetComparer(left)(left, right))
                        Expression.Assign(returnValue, Expression.Call(
                            // Call anyResultEquals
                            ((Func<IEnumerable<(string, bool)>, bool>)anyResultEquals).Method,
                            Expression.Call(
                                // Get comparer
                                Expression.Call(
                                    ((Func<object, ReferenceComparer>)GetComparerFor).Method,
                                    leftVar),
                                // Use method of comparer
                                ((Func<object, object, IEnumerable<(string, bool)>>)Compare).Method,
                                leftVar,
                                rightVar
                            )))
                        ),
                    // return
                    returnValue
                    );




            var lambda =
                Expression.Lambda(body, leftParam, rightParam);

            return lambda;
        }

        public IEnumerable<(string name, bool result)> Compare(object left, object right)
        {
            Debug.WriteLine("In Compare");

            if (left == null || right == null)
                return new[] { ("one of objects is null", false) };

            if (left.GetType() != right.GetType())
                return new[] { ("Objects are not of the same type", false) };

            return 
                this.Members.Zip(
                this.fieldComparers,
                    (m, c) => (m.Name, (bool)c.DynamicInvoke(left, right)))
                    .ToArray();
        }

        private static bool anyResultEquals(IEnumerable<(string name, bool result)> results)
        {
            return results.Any(x => x.result == true);
        }

        private static bool referenceEquals(object left, object right)
        {
            if (left == null || right == null)
                return false;
            
            return object.ReferenceEquals(left, right);
        }
    }
}
