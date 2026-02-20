using HarmonyLib;
using System;
using System.Reflection;

namespace iOverLayer.Patch
{
    public static class PatchManager
    {
        private static Harmony _harmony;
        public static void AddAllPatches(string harmonyID)
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

        public static bool AddPatch(Type targetType, string targetMethodName, MethodInfo patchMethod, PatchType patchType, bool useDeclaredOnly = false)
        {
            if (targetType == null)
            {
                LogSystem.Error("AddPatch: targetType is null.");
                return false;
            }

            if (string.IsNullOrEmpty(targetMethodName))
            {
                LogSystem.Error("AddPatch: targetMethodName is null or empty.");
                return false;
            }

            if (patchMethod == null)
            {
                LogSystem.Error("AddPatch: patchMethod is null.");
                return false;
            }

            if (_harmony == null)
            {
                _harmony = new Harmony("iOverLayer.Dynamic");
            }

            var targetMethod = useDeclaredOnly
                ? AccessTools.DeclaredMethod(targetType, targetMethodName)
                : AccessTools.Method(targetType, targetMethodName);

            if (targetMethod == null)
            {
                LogSystem.Error($"AddPatch: target not found: {targetType.FullName}.{targetMethodName}");
                return false;
            }

            try
            {
                var harmonyMethod = new HarmonyMethod(patchMethod);
                switch (patchType)
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
                        LogSystem.Error($"AddPatch: unsupported patch type: {patchType}");
                        return false;
                }

                LogSystem.Info($"AddPatch: patched {targetType.FullName}.{targetMethodName} with {patchMethod.DeclaringType.FullName}.{patchMethod.Name} ({patchType}).");
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error($"AddPatch exception: {ex}");
                return false;
            }
        }
    }
}
