
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using ExtractorCollection = System.Collections.Generic.IEnumerable<(
    System.Type type,
    System.Reflection.FieldInfo field,
    System.Func<object, System.Collections.Generic.HashSet<object>, System.Collections.Generic.IEnumerable<object>> extractor)>;

namespace SharpToolkit.Extensions.Diagnostics
{
    /// <summary>
    /// Recursevly extracts all fields that contain reference type of can contain reference types from an object.
    /// </summary>
    public sealed class FieldExtractor
    {
        private static 
            new Dictionary<
                Type, 
                ExtractorCollection> cache =
            new Dictionary<
                Type, 
                ExtractorCollection>();

        public IEnumerable<object> ExtractedObjects { get; private set; }
        
        public FieldExtractor(Type type)
        {
            
        }

        private ExtractorCollection getExtractors(Type type)
        {
            if (cache.TryGetValue(type, out var r))
                return r;

            cache[type] = generateTypeExtractors(type);

            return cache[type];
        }

        private ExtractorCollection generateTypeExtractors(Type type)
        {
            var flags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public;

            var members =
                type
                .GetFields(flags)
                .Where(x => x.FieldType != typeof(string) && x.FieldType.GetTypeInfo().IsPrimitive == false && x.FieldType.IsEnum == false)
                .ToArray();

            var extractors =
                members
                    
                    .Select(x => generateFieldExtractor(x))
                    .ToArray();

            return
                members.Zip(
                extractors,
                    (m, e) => (type, m, e))
                    .ToArray();

            
        }

        private Func<object, HashSet<object>, IEnumerable<object>> generateFieldExtractor(FieldInfo info)
        {
            var param = Expression.Parameter(typeof(object), "obj");
            var extracted = Expression.Parameter(typeof(HashSet<object>), "extracted");

            return
                (Func<object, HashSet<object>, IEnumerable<object>>)
                Expression.Lambda(
                    Expression.Call(
                        ((Func<object, HashSet<object>, IEnumerable<object>>)convertObject).Method,
                        Expression.Convert(
                            Expression.Field(
                                Expression.Convert(param, info.DeclaringType), 
                                info),                            
                            typeof(object)
                        ),
                        extracted
                    ),
                    param,
                    extracted
                )
                .Compile();
        }

        // TODO: optimize
        private static IEnumerable<object> convertObject(object obj, HashSet<object> extracted)
        {
            if (obj == null)
                return Enumerable.Empty<object>();

            if (obj.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEqualityComparer))
                || obj.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEqualityComparer<>)))
                return Enumerable.Empty<object>();

            if (obj.GetType().IsArray == false)
            {
                return new FieldExtractor(obj.GetType())
                    .Extract(obj)
                    .Union(new[] { obj });
            }

            if (obj.GetType().GetElementType().GetTypeInfo().IsPrimitive || obj.GetType().IsEnum)
                return new[] { obj };

            var arr = (IEnumerable)obj;

            return arr
                .Cast<object>()
                .SelectMany(x => convertObject(x, extracted))
                .Union(new[] { obj })
                .ToArray();
        }

        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                if (x == null || y == null)
                    return false;

                return object.ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }


        public IEnumerable<object> Extract(object obj)
        {
            return Extract(obj, new HashSet<object>());
        }

        private IEnumerable<object> Extract(object obj, HashSet<object> extracted)
        {
            var extractors = getExtractors(obj.GetType());

            var list = new HashSet<object>(new ReferenceEqualityComparer());

            foreach (var i in extractors)
            {
                if (obj == null ||
                    list.Contains(obj))
                    continue;

                var es = i.extractor(obj, list)
                    .Where(x => x != null)
                    .Distinct(new ReferenceEqualityComparer());

                foreach (var e in es)
                    list.Add(e);
            }

            this.ExtractedObjects = list;

            return this.ExtractedObjects;
        }
    }
}
