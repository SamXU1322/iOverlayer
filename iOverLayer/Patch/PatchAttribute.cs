using System;
namespace iOverLayer.Patch
{
    public enum PatchType
    {
        Prefix,
        Postfix,
        Transpiler,
        Finalizer
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class iOverLayerPatchAttribute: Attribute
    {
        private Type _targetType;
        private string _targetMethod;
        private PatchType _patchType;
        private bool _useDeclaredOnly;
        public Type TargetType =>_targetType;
        public string TargetMethod => _targetMethod;
        public PatchType PatchType => _patchType;
        public bool UseDeclaredOnly => _useDeclaredOnly;
        public iOverLayerPatchAttribute(Type targetType, string targetMethod, PatchType patchType,bool useDeclaredOnly = false)
        {
            _targetType = targetType;
            _targetMethod = targetMethod;
            _patchType = patchType;
            _useDeclaredOnly = useDeclaredOnly;
        }
        
    }
}

    