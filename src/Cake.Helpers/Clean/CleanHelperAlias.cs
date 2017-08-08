using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Clean
{
  /// <summary>
  /// Cake Generic Clean Task Aliases
  /// </summary>
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Clean")]
  public static class CleanHelperAlias
  {
    /// <summary>
    /// Gets Clean Task
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="cleanCategory">Clean Category. If empty or null, then category is Generic</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "Clean-All"
    /// // Creates task "Clean-Test-All"
    /// // Creates task "Clean-Test-Sln"
    /// CleanTask("Sln", true, "Test")
    ///   .Does(() => { "Clean Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> CleanTask(
      this ICakeContext context,
      string taskName,
      string cleanCategory = "",
      bool isTarget = true,
      string parentTaskName = "")
    {
      if(context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToCleanTask(taskName, cleanCategory, isTarget, parentTaskName)
        .GetTaskBuilder();
    }
  }
}
