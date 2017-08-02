using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Tasks;

namespace Cake.Helpers
{
  internal static class SingletonFactory
  {
    internal static ICakeContext Context { get; set; }

    private static ConcurrentDictionary<Type, object> _SingletonCache = new ConcurrentDictionary<Type, object>();

    public static TSingletonType GetInstance<TSingletonType>(Func<TSingletonType> createInstanceFunc)
      where TSingletonType : class, IHelperContext
    {
      if(Context == null)
        throw new ArgumentNullException(nameof(Context), "ICakeContext needs to be set before using any helper");

      if(createInstanceFunc == null)
        throw new ArgumentNullException(nameof(createInstanceFunc));

      var type = typeof(TSingletonType);
      var obj = _SingletonCache.GetOrAdd(type, t => createInstanceFunc);

      var t = obj as TSingletonType;
      if (t != null)
      {
        t.Context = Context;
      }

      return t;
    }

    public static TaskHelper GetTaskHelper()
    {
      return GetInstance<TaskHelper>(() => new TaskHelper());
    }
  }
}
