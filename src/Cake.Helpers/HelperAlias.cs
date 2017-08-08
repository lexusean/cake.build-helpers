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
  /// <summary>
  /// Cake Helper Aliases
  /// </summary>
  [CakeAliasCategory("Helper")]
  public static class HelperAlias
  {
    /// <summary>
    /// Cake Property to get HelperSettings
    /// </summary>
    /// <param name="context">CakeContext</param>
    /// <returns>HelperSettings</returns>
    /// <example>
    /// <code>
    /// var settings = HelperSettings;
    /// </code>
    /// </example>
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
