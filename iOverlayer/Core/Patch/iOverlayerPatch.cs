using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace iOverlayer.Core.Patch
{
    public class iOverlayerPatch
    {
        public const string InternalTrigger = "INTERNAL";
        public static readonly Dictionary<string, iOverlayerPatch> Patches = new Dictionary<string, iOverlayerPatch>();
        public Type PatchType;
        public Harmony Harmony;
        public MethodInfo Prefix;
        public MethodInfo Postfix;
        public MethodInfo Transpiler;
        public MethodInfo Finalizer;
        public MethodBase Target;
        public MethodInfo AppliedPatch;
        public iOverlayerPatchAttribute Attr;
        public bool Patched { get; private set; }
        public bool Locked { get; set; }
        
        public int IgnorePatchCount { get; set; }
        public iOverlayerPatch(Type patchType, Harmony harmony, iOverlayerPatchAttribute attr)
        {
            this.PatchType = patchType;
            this.Attr = attr;
            this.Harmony = harmony;
            Prefix = PatchType.GetMethod("Prefix", (BindingFlags)15420);
            Postfix = PatchType.GetMethod("Postfix", (BindingFlags)15420);
            Transpiler = PatchType.GetMethod("Transpiler", (BindingFlags)15420);
            Finalizer = PatchType.GetMethod("Finalizer", (BindingFlags)15420);
            Target = attr.Resolve();
            Patches.Add(attr.Id, this);
        }

        public void Patch(bool force = false)
        {
            if(Patched || Target == null || AppliedPatch != null) return;
            if (!force && Locked)
            {
                Main.Logger.Log($"ID:{Attr.Id} Is Locked! Cannot Be Unpatched!");
                return;
            }
            if (force) Locked = false;
            var pre_hm = Prefix != null ? new HarmonyMethod(Prefix) : null;
            var post_hm = Postfix != null ? new HarmonyMethod(Postfix) : null;
            var trans_hm = Transpiler != null ? new HarmonyMethod(Transpiler) : null;
            var final_hm = Finalizer != null ? new HarmonyMethod(Finalizer) : null;
            AppliedPatch = Harmony.Patch(Target, pre_hm, post_hm, trans_hm, final_hm);
            Main.Logger.Log($"ID:{Attr.Id} Patched!");
            Patched = true;
        }

        public void Unpatch(bool force = false)
        {
            if(!Patched || IgnorePatchCount-- > 0 || Target == null || AppliedPatch == null) return;
            if (!force && Locked)
            {
                Main.Logger.Log($"ID:{Attr.Id} Is Locked! Cannot Be Unpatched!");
                return;
            }

            if (force) Locked = false;
            Harmony.Unpatch(Target,AppliedPatch);
            Main.Logger.Log("ID:{Attr.Id} Unpatched for Update!");
            AppliedPatch = null;
            Patched = false;
        }

    }
}