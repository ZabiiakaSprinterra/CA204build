using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public class KCNamespaceReview
    {
        private readonly string[] _errorCodes = new string[]
        {
            "CS0234", // The type or namespace name '{0}' does not exist in the namespace '{1}' (are you missing an assembly reference?)
            "CS0246", // The type or namespace name '{0}' could not be found (are you missing a using directive or an assembly reference?)
            "CS0400", // The type or namespace name '{0}' could not be found in the global namespace (are you missing an assembly reference?)
            "CS1068", // The type name '{0}' could not be found in the global namespace. This type has been forwarded to assembly '{1}' Consider adding a reference to that assembly.
            "CS1069", // The type name '{0}' could not be found in the namespace '{1}'. This type has been forwarded to assembly '{2}' Consider adding a reference to that assembly.
        };
        private readonly CompilerParameters _settings = new CompilerParameters();
        private readonly Dictionary<string, bool> _cache = new Dictionary<string, bool>();
        private readonly object _locker = new object();

        public KCNamespaceReview()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();
            _settings.ReferencedAssemblies.AddRange(assemblies);
            _settings.GenerateExecutable = false;
            _settings.GenerateInMemory = true;
        }

        public bool Test(string _namespace)
        {
            Monitor.Enter(_locker);

            try
            {
                if (!_cache.ContainsKey(_namespace))
                {
                    bool result = false;
                    CompilerResults results = null;

                    using (CodeDomProvider provider = CodeDomProvider.CreateProvider("c#"))
                    {
                        try
                        {
                            _settings.OutputAssembly = $"KNCBTemp_{DateTime.Now.Ticks}.dll";
                            results = provider.CompileAssemblyFromSource(_settings, $"using {_namespace};");
                            result = !results.Errors.OfType<CompilerError>().Any(e => _errorCodes.Any(code => string.Equals(e.ErrorNumber, code, StringComparison.Ordinal)));
                        }
                        finally
                        {
                            try
                            {
                                results?.TempFiles.Delete();
                            }
                            catch
                            {
                            }

                            try
                            {
                                var path = results?.PathToAssembly;
                                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                                {
                                    File.Delete(path);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    _cache.Add(_namespace, result);
                }
            }
            finally
            {
                Monitor.Exit(_locker);
            }

            return _cache[_namespace];
        }
    }
}
