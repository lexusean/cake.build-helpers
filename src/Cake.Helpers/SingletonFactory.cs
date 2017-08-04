using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Helpers.Command;
using Cake.Helpers.DotNetCore;
using Cake.Helpers.Settings;
using Cake.Helpers.Tasks;

[assembly: InternalsVisibleTo("Cake.Helpers.Tests.Unit")]

namespace Cake.Helpers
{
  internal static class SingletonFactory
  {
    #region Static Members

    [ExcludeFromCodeCoverage]
    static SingletonFactory()
    { 
    }

    internal static ICakeContext Context { get; set; }

    private static readonly ConcurrentDictionary<Type, object> _SingletonCache =
      new ConcurrentDictionary<Type, object>();

    internal static bool ExistsInCache(Type tt)
    {
      return _SingletonCache.ContainsKey(tt);
    }

    public static ICommandHelper GetCommandHelper()
    {
      return GetInstance(() => new CommandHelper(GetTaskHelper()));
    }

    public static DotNetCoreHelper GetDotNetCoreHelper()
    {
      return GetInstance(() => new DotNetCoreHelper());
    }

    public static IHelperSettings GetHelperSettings()
    {
      return GetInstance(() => new HelperSettings());
    }

    public static ITaskHelper GetTaskHelper()
    {
      return GetInstance(() => new TaskHelper());
    }

    internal static void ClearFactory()
    {
      Context = null;
      _SingletonCache.Clear();
    }

    [ExcludeFromCodeCoverage]
    private static TSingletonType GetInstance<TSingletonType>(Func<TSingletonType> createInstanceFunc)
      where TSingletonType : class, IHelperContext
    {
      if (Context == null)
        throw new ArgumentNullException(nameof(Context), "ICakeContext needs to be set before using any helper");

      if (createInstanceFunc == null)
        throw new ArgumentNullException(nameof(createInstanceFunc));

      var type = typeof(TSingletonType);
      var obj = _SingletonCache.GetOrAdd(type, x => createInstanceFunc());

      var t = obj as TSingletonType;
      if (t != null)
        t.Context = Context;

      return t;
    }

    #endregion
  }
}