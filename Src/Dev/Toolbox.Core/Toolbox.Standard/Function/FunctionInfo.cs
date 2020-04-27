using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class FunctionInfo
    {
        public FunctionInfo(MethodInfo method, Attribute attribute)
        {
            method.VerifyNotNull(nameof(method));
            attribute.VerifyNotNull(nameof(attribute));

            MethodInfo = method;
            Attribute = attribute;
        }

        public string Name =>
            Attribute.GetType().GetProperty(nameof(Name))
                ?.GetValue(Attribute)
                ?.CastAs<string>()
            ?? $"{MethodInfo.DeclaringType.Name}.{MethodInfo.Name}";

        public MethodInfo MethodInfo { get; }

        public Attribute Attribute { get; }

        public Type GetDeclaringType() => MethodInfo.DeclaringType;

        public TAttr GetAttribute<TAttr>() where TAttr : Attribute => Attribute.CastAs<TAttr>();

        public IReadOnlyList<ParameterInfo> GetParameters() => MethodInfo.GetParameters();
    }
}
