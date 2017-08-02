using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Helpers
{
  [CakeAliasCategory("Helper")]
  public static class HelperAlias
  {
    [CakePropertyAlias]
    public static bool HelperBuildAllDependencies(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return HelperSettingsCache.BuildAllDependencies;
    }

    [CakeMethodAlias]
    public static void SetHelperBuildAllDependencies(this ICakeContext context, bool buildAllDependencies = false)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      HelperSettingsCache.BuildAllDependencies = buildAllDependencies;
    }
  }
}
