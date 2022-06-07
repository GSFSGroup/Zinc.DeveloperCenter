using System;
using Opa.Wasm;
using Wasmtime;

namespace RedLine.Application
{
    /// <summary>
    /// This factory specifically loads and manages WASMs for OPA, which are better off left in memory.
    /// It should be a singleton.
    /// </summary>
    public abstract class OpaPolicyFactory : IDisposable
    {
        private readonly OpaRuntime runtime;
        private readonly Module wasmModule;

        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        /// <param name="assembly">An assembly where a .wasm is embedded.</param>
        /// <param name="wasmPath">The path to a compiled .wasm for OPA.</param>
        protected OpaPolicyFactory(System.Reflection.Assembly assembly, string wasmPath)
        {
            runtime = new OpaRuntime();
            string fullyQualifiedPath = $"{assembly.GetName().Name}.{wasmPath}";

            // this step interprets and loads the module into the Wasmtime runtime.
            // ths is the costly operation we're protecting with a startup singleton.
            using var stream = assembly.GetManifestResourceStream(fullyQualifiedPath);
            var compiled = new byte[stream.Length];
            _ = stream.Read(compiled, 0, compiled.Length);
            wasmModule = runtime.Load(wasmPath, compiled);
        }

        /// <summary>
        /// Instantiate the wasm.
        /// </summary>
        /// <returns>A new instance of the wasm that can be evaluated.</returns>
        public OpaPolicy CreatePolicy() => new(runtime, wasmModule);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the thing.
        /// </summary>
        /// <param name="disposing">Are we disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                runtime?.Dispose();
                wasmModule?.Dispose();
            }
        }
    }
}
