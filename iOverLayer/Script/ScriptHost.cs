using System.Collections.Generic;
using UnityEngine;

namespace iOverlayer.Script
{
    public class ScriptHost
    {
        private readonly List<ScriptInstance> _instances = new List<ScriptInstance>();

        public void Attach(GameObject target, IOverlayScript script, OverlayScriptContext context)
        {
            _instances.Add(new ScriptInstance
            {
                Target = target,
                Script = script,
                Context = context
            });
            script.OnInit(context);
        }

        public void Update(float deltaTime)
        {
            for (int i = _instances.Count - 1; i >= 0; i--)
            {
                var inst = _instances[i];
                if (inst.Target == null)
                {
                    inst.Script.OnDestroy();
                    _instances.RemoveAt(i);
                    continue;
                }
                inst.Script.OnUpdate(inst.Context, deltaTime);
            }
        }

        public void Clear()
        {
            foreach (var inst in _instances)
                inst.Script.OnDestroy();
            _instances.Clear();
        }

        private class ScriptInstance
        {
            public GameObject Target;
            public IOverlayScript Script;
            public OverlayScriptContext Context;
        }
    }
}
