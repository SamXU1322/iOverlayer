using HarmonyLib;
using System.Reflection;

namespace iOverLayer.Patch
{
    public static class PatchManager
    {
        private static Harmony _harmony;
        public static void ApplyPatches(string harmonyID)
        {
            if (_harmony != null)return;
            _harmony = new Harmony(harmonyID);
            Assembly assembly = typeof(PatchManager).Assembly;
            foreach (var type in assembly.GetTypes()) 
            {
                var methons = type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);
                foreach(var method in methons)
                {
                    var attributes = (iOverLayerPatchAttribute[])method.GetCustomAttributes(typeof(iOverLayerPatchAttribute), false);
                    foreach(var attribute in attributes)
                    {
                        var targetMethod = attribute.UseDeclaredOnly
                            ? AccessTools.DeclaredMethod(attribute.TargetType,attribute.TargetMethod)
                            : AccessTools.Method(attribute.TargetType,attribute.TargetMethod);
                        if (targetMethod == null)
                        {
                            LogSystem.Error($"Patch target not found: {attribute.TargetType.FullName}.{attribute.TargetMethod}");
                            continue;
                        }
                        var harmonyMethod = new HarmonyMethod(method);
                        switch (attribute.PatchType)
                        {
                            case PatchType.Prefix:
                                _harmony.Patch(targetMethod, prefix: harmonyMethod);
                                break;
                            case PatchType.Postfix:
                                _harmony.Patch(targetMethod, postfix: harmonyMethod);
                                break;
                            case PatchType.Transpiler:
                                _harmony.Patch(targetMethod, transpiler: harmonyMethod);
                                break;
                            case PatchType.Finalizer:
                                _harmony.Patch(targetMethod, finalizer: harmonyMethod);
                                break;
                            default:
                                LogSystem.Error($"Unsupported patch type: {attribute.PatchType}");
                                break;
                        }
                        LogSystem.Info($"Patched {attribute.TargetType.FullName}.{attribute.TargetMethod} with {method.DeclaringType.FullName}.{method.Name} ({attribute.PatchType}).");
                    }


                }
            }
        }
    }
}
