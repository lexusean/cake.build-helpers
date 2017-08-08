using Cake.Core.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Documentation
{
  /// <summary>
  /// Cake Generic Build Task Aliases
  /// </summary>
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Doc")]
  public static class DocHelperAlias
  {
    /// <summary>
    /// Gets Documentation Clean Task for defined parameters
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "Clean-All"
    /// // Creates task "Clean-Doc-All"
    /// // Creates task "Clean-Doc-Sln"
    /// DocCleanTask("Sln", true)
    ///   .Does(() => { "Clean Doc" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> DocCleanTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToDocCleanTask(taskName, isTarget, parentTaskName)
        .GetTaskBuilder();
    }

    /// <summary>
    /// Gets DocExtract task for defined parameters. Runs associated DocClean task as dependency always
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "DocExtract-All"
    /// // Creates task "DocExtract-Sln"
    /// DocExtractTask("Sln", true)
    ///   .Does(() => { "Extract Doc From Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> DocExtractTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToDocExtractTask(taskName, isTarget, parentTaskName)
        .GetTaskBuilder();
    }

    /// <summary>
    /// Gets DocBuild task for defined parameters. Runs associated DocExtract task as dependency always
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "DocBuild-All"
    /// // Creates task "DocBuild-Sln"
    /// DocBuildTask("Sln", true)
    ///   .Does(() => { "Build Doc From Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> DocBuildTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToDocBuildTask(taskName, isTarget, parentTaskName)
        .GetTaskBuilder();
    }

    /// <summary>
    /// Gets DocPostBuild task for defined parameters. Runs associated DocExtract task as dependency always
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="taskName">Target Name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="parentTaskName">Parent Target name. Required if isTarget is False</param>
    /// <returns>Task</returns>
    /// <example>
    /// <code>
    /// // Creates task "DocBuild-All"
    /// // Creates task "DocBuild-Sln"
    /// DocPostBuildTask("Sln", true)
    ///   .Does(() => { "Copy Doc From Solution" });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> DocPostBuildTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToDocPostBuildTask(taskName, isTarget, parentTaskName)
        .GetTaskBuilder();
    }
  }
}
