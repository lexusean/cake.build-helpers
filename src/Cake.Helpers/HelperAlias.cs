using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Helpers.Settings;

namespace Cake.Helpers
{
  [CakeAliasCategory("Helper")]
  public static class HelperAlias
  {
    [CakePropertyAlias]
    public static IHelperSettings HelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;

      return SingletonFactory.GetHelperSettings();
    }
  }
}
