using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Helpers.Tasks
{
  /// <summary>
  /// Cake aliases for Task Related actions
  /// </summary>
  [CakeAliasCategory("Task")]
  [CakeAliasCategory("Helper")]
  public static class TaskAliases
  {
    /// <summary>
    /// Gets TaskHelper as Cake Property
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <returns>TaskHelper</returns>
    /// <example>
    /// <code>
    /// var taskHelper = TaskHelper;
    /// </code>
    /// </example>
    [CakePropertyAlias]
    public static ITaskHelper TaskHelper(this ICakeContext context)
    {
      if(context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetTaskHelper();
    }

    /// <summary>
    /// Adds or gets Task based on defined parameters
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="taskCategory">Task Category (PostBuild, PreBuild, Build, etc). Defaults to "Generic"</param>
    /// <param name="taskType">Task Type (Clean, Build, Etc). Defaults to "Unknown"</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// HelperTask("WriteStuff")
    ///   .Does(() => Debug("Stuff"));
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> HelperTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string taskCategory = "",
      string taskType = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .GetTask(taskName, isTarget, taskCategory, taskType)
        .GetTaskBuilder();
    }
  }
}
