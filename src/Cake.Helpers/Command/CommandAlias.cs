using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Helpers.Command
{
  /// <summary>
  /// Cake Alias for interacting with CommandHelper
  /// </summary>
  [CakeAliasCategory("Command")]
  [CakeAliasCategory("Helper")]
  public static class CommandAlias
  {
    /// <summary>
    /// Cake Property Alias to get CommandHelper instance
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <returns>CommandHelper</returns>
    /// <example>
    /// <code>
    /// var cmdHelper = CommandHelper;
    /// </code>
    /// </example>
    [CakePropertyAlias]
    public static ICommandHelper CommandHelper(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      var commandHelper = SingletonFactory.GetCommandHelper();

      return commandHelper;
    }

    /// <summary>
    /// Cake Alias to run actions defined in command helper. You would typically call this instead of RunTarget()
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <example>
    /// <code>
    /// // Instead of RunTarget(targetName);
    /// RunCommand();
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static void RunCommand(this ICakeContext context)
    {
      if(context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      var commandHelper = SingletonFactory.GetCommandHelper();
      commandHelper.Run();
    }
  }
}
