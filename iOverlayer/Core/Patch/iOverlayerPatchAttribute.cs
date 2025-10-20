using System;
using System.Linq;
using System.Reflection;

namespace iOverlayer.Core.Patch
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class iOverlayerPatchAttribute:Attribute
    {
        public static readonly int CurrentVersion = (int)typeof(GCNS).GetField("releaseNumber").GetValue(null);
        public string Id { get; }
        public string TargetType { get; }
        public string TargetMethod { get; }
        public string[] TargetMethodArgs { get; }
        public BindingFlags ORFlags { get; set; } = BindingFlags.DeclaredOnly;
        public BindingFlags ANDNOTFlags { get; set; } = BindingFlags.Default;

        public MethodBase Resolve()
        {
            var bf = ((BindingFlags)15420|ORFlags)&~ANDNOTFlags;
            try
            {
                var tt = Type.GetType(TargetType);
                var tma = TargetMethodArgs?.Select(x => Type.GetType(x))?.ToArray();
                if (TargetMethod == ".ctor")
                {
                    return tma != null
                        ? tt?.GetConstructor(bf, null, tma, null)
                        : tt?.GetConstructors(bf).FirstOrDefault();
                }
                else if (TargetMethod == "cctor") return tt.TypeInitializer;
                else
                    return tma != null
                        ? tt?.GetMethod(TargetMethod, bf, null, tma, null)
                        : tt?.GetMethod(TargetMethod, bf);
            }
            catch (AmbiguousMatchException)
            {
                return null;
            }
        }
    }
}