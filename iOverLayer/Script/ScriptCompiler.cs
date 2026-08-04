using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using MelonLoader;
using MelonLoader.Utils;

namespace iOverlayer.Script
{
    public static class ScriptCompiler
    {
        private static readonly object _lock = new object();
        private static Assembly _assembly;
        private static Dictionary<string, long> _stamps;

        public static string ScriptDirectory =>
            Path.Combine(Path.Combine(MelonEnvironment.UserDataDirectory, "iOverlayer"), "Scripts");

        public static IOverlayScript CreateInstance(string className)
        {
            if (string.IsNullOrWhiteSpace(className)) return null;

            var assembly = GetOrCompile();
            if (assembly == null) return null;

            var type = assembly.GetType(className);
            if (type == null) return null;

            return Activator.CreateInstance(type) as IOverlayScript;
        }

        public static Assembly GetOrCompile()
        {
            lock (_lock)
            {
                var files = Directory.Exists(ScriptDirectory)
                    ? Directory.GetFiles(ScriptDirectory, "*.cs")
                    : new string[0];

                var stamps = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                foreach (var file in files)
                    stamps[file] = File.GetLastWriteTimeUtc(file).Ticks;

                if (_assembly != null && _stamps != null && SameStamps(stamps))
                    return _assembly;

                _assembly = Compile(files);
                _stamps = stamps;
                return _assembly;
            }
        }

        private static bool SameStamps(Dictionary<string, long> stamps)
        {
            if (stamps.Count != _stamps.Count) return false;
            foreach (var kv in stamps)
            {
                if (!_stamps.TryGetValue(kv.Key, out var old) || old != kv.Value)
                    return false;
            }
            return true;
        }

        private static Assembly Compile(string[] files)
        {
            if (files == null || files.Length == 0)
                return null;

            var options = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!string.IsNullOrEmpty(asm.Location))
                        options.ReferencedAssemblies.Add(asm.Location);
                }
                catch { }
            }

            try
            {
                using (var provider = new CSharpCodeProvider())
                {
                    var result = provider.CompileAssemblyFromFile(options, files);
                    if (result.Errors.HasErrors)
                    {
                        foreach (CompilerError err in result.Errors)
                            MelonLogger.Error($"[iOverlayer] 脚本编译错误 {err.FileName}:{err.Line}: {err.ErrorText}");
                        return null;
                    }
                    return result.CompiledAssembly;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[iOverlayer] 脚本编译异常: {ex.Message}");
                return null;
            }
        }
    }
}
