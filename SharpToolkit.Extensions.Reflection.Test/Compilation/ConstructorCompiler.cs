using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using SharpToolkit.Extensions.Reflection.Compilation;

namespace SharpToolkit.Extensions.Reflection.Test.Compilation
{
    [TestClass]
    public class ConstructorCompiler
    {
        [TestMethod]
        public void Compile_ConstructorCall()
        {
            var ci =
                typeof(TestTarget).GetTypeInfo().DeclaredConstructors
                .Where(
                    x => x.GetParameters()
                    .Select(y => y.ParameterType)
                    .SequenceEqual(new[] { typeof(string), typeof(string) })).Single();

            var l = (Func<string, string, TestTarget>)Compile.ConstructorCall(ci);

            var obj = l("some", "string");

            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.Str1, "some");
            Assert.AreEqual(obj.Str2, "string");
        }
    }
}
