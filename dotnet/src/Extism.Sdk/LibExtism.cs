using System.Runtime.InteropServices;

namespace Extism.Sdk.Native;

/// <summary>
/// Functions exposed by the native Extism library.
/// </summary>
internal static class LibExtism
{
    internal enum ExtismValType
    {
        /// <summary>
        /// Signed 32 bit integer. Equivalent of <see cref="int"/> or <see cref="uint"/>
        /// </summary>
        I32,

        /// <summary>
        /// Signed 64 bit integer. Equivalent of <see cref="long"/> or <see cref="ulong"/>
        /// </summary>
        I64,

        /// <summary>
        /// Floating point 32 bit integer. Equivalent of <see cref="float"/>
        /// </summary>
        F32,

        /// <summary>
        /// Floating point 64 bit integer. Equivalent of <see cref="double"/>
        /// </summary>
        F64,

        /// <summary>
        /// A 128 bit number.
        /// </summary>
        V128,

        /// <summary>
        /// A reference to opaque data in the Wasm instance.
        /// </summary>
        FuncRef,

        /// <summary>
        /// A reference to opaque data in the Wasm instance.
        /// </summary>
        ExternRef
    }

    /// <summary>
    /// A `Context` is used to store and manage plugins.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtismContext { }

    /// <summary>
    /// Wraps host functions
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtismFunction { }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtismPlugin { }

    /// <summary>
    /// Plugin contains everything needed to execute a WASM function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtismCurrentPlugin { }

    /// <summary>
    /// A union type for host function argument/return values.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct ExtismValUnion
    {
        [FieldOffset(0)]
        internal int i32;

        [FieldOffset(0)]
        internal long i64;

        [FieldOffset(0)]
        internal float f32;

        [FieldOffset(0)]
        internal double f64;
    }

    /// <summary>
    /// `ExtismVal` holds the type and value of a function argument/return
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtismVal
    {
        internal ExtismValType t;
        internal ExtismValUnion v;
    }

    /// <summary>
    /// Host function signature
    /// </summary>
    /// <param name="plugin"></param>
    /// <param name="inputs"></param>
    /// <param name="n_inputs"></param>
    /// <param name="outputs"></param>
    /// <param name="n_outputs"></param>
    /// <param name="data"></param>
    internal delegate void ExtismFunctionType(ref ExtismCurrentPlugin plugin, Span<ExtismVal> inputs, uint n_inputs, Span<ExtismVal> outputs, uint n_outputs, IntPtr data);

    /// <summary>
    /// Create a new context.
    /// </summary>
    /// <returns>A pointer to the newly created context.</returns>
    [DllImport("extism")]
    unsafe internal static extern ExtismContext* extism_context_new();

    /// <summary>
    /// Remove a context from the registry and free associated memory.
    /// </summary>
    /// <param name="context"></param>
    [DllImport("extism")]
    unsafe internal static extern void extism_context_free(ExtismContext* context);

    /// <summary>
    /// Load a WASM plugin.
    /// </summary>
    /// <param name="context">Pointer to the context the plugin will be associated with.</param>
    /// <param name="wasm">A WASM module (wat or wasm) or a JSON encoded manifest.</param>
    /// <param name="wasmSize">The length of the `wasm` parameter.</param>
    /// <param name="functions">Array of host function pointers.</param>
    /// <param name="nFunctions">Number of host functions.</param>
    /// <param name="withWasi">Enables/disables WASI.</param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern ExtismPlugin* extism_plugin_new(ExtismContext* context, byte* wasm, int wasmSize, IntPtr* functions, int nFunctions, bool withWasi);

    /// <summary>
    /// Update a plugin, keeping the existing ID.
    /// Similar to <see cref="extism_plugin_new"/> but takes an `plugin` argument to specify which plugin to update.
    /// Memory for this plugin will be reset upon update.
    /// </summary>
    /// <param name="context">Pointer to the context the plugin is associated with.</param>
    /// <param name="plugin">Pointer to the plugin you want to update.</param>
    /// <param name="wasm">A WASM module (wat or wasm) or a JSON encoded manifest.</param>
    /// <param name="wasmLength">The length of the `wasm` parameter.</param>
    /// <param name="functions">Array of host function pointers.</param>
    /// <param name="nFunctions">Number of host functions.</param>
    /// <param name="withWasi">Enables/disables WASI.</param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern bool extism_plugin_update(ExtismContext* context, ExtismPlugin* plugin, byte* wasm, int wasmLength, IntPtr* functions, int nFunctions, bool withWasi);

    /// <summary>
    /// Remove a plugin from the registry and free associated memory.
    /// </summary>
    /// <param name="context">Pointer to the context the plugin is associated with.</param>
    /// <param name="plugin">Pointer to the plugin you want to free.</param>
    [DllImport("extism")]
    unsafe internal static extern void extism_plugin_free(ExtismContext* context, ExtismPlugin* plugin);

    /// <summary>
    /// Remove all plugins from the registry.
    /// </summary>
    /// <param name="context"></param>
    [DllImport("extism")]
    unsafe internal static extern void extism_context_reset(ExtismContext* context);

    /// <summary>
    /// Update plugin config values, this will merge with the existing values.
    /// </summary>
    /// <param name="context">Pointer to the context the plugin is associated with.</param>
    /// <param name="plugin">Pointer to the plugin you want to update the configurations for.</param>
    /// <param name="json">The configuration JSON encoded in UTF8.</param>
    /// <param name="jsonLength">The length of the `json` parameter.</param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern bool extism_plugin_config(ExtismContext* context, ExtismPlugin* plugin, byte* json, int jsonLength);

    /// <summary>
    /// Returns true if funcName exists.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="plugin"></param>
    /// <param name="funcName"></param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern bool extism_plugin_function_exists(ExtismContext* context, ExtismPlugin* plugin, string funcName);

    /// <summary>
    /// Call a function.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="plugin"></param>
    /// <param name="funcName">The function to call.</param>
    /// <param name="data">Input data.</param>
    /// <param name="dataLen">The length of the `data` parameter.</param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern int extism_plugin_call(ExtismContext* context, ExtismPlugin* plugin, string funcName, byte* data, int dataLen);

    /// <summary>
    /// Get the error associated with a Context or Plugin, if plugin is -1 then the context error will be returned.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="plugin">A plugin pointer, or -1 for the context error.</param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern IntPtr extism_error(ExtismContext* context, ExtismPlugin* plugin);

    /// <summary>
    /// Get the length of a plugin's output data.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern long extism_plugin_output_length(ExtismContext* context, ExtismPlugin* plugin);

    /// <summary>
    /// Get the plugin's output data.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    [DllImport("extism")]
    unsafe internal static extern IntPtr extism_plugin_output_data(ExtismContext* context, ExtismPlugin* plugin);

    /// <summary>
    /// Set log file and level.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    [DllImport("extism")]
    internal static extern bool extism_log_file(string filename, string logLevel);

    /// <summary>
    /// Get the Extism version string.
    /// </summary>
    /// <returns></returns>
    [DllImport("extism", EntryPoint = "extism_version")]
    internal static extern IntPtr extism_version();

    /// <summary>
    /// Extism Log Levels
    /// </summary>
    internal static class LogLevels
    {
        /// <summary>
        /// Designates very serious errors.
        /// </summary>
        internal const string Error = "Error";

        /// <summary>
        /// Designates hazardous situations.
        /// </summary>
        internal const string Warn = "Warn";

        /// <summary>
        /// Designates useful information.
        /// </summary>
        internal const string Info = "Info";

        /// <summary>
        /// Designates lower priority information.
        /// </summary>
        internal const string Debug = "Debug";

        /// <summary>
        /// Designates very low priority, often extremely verbose, information.
        /// </summary>
        internal const string Trace = "Trace";
    }
}