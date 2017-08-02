using Cake.Core.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Build
{
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Build")]
  public static class BuildHelperAlias
  {
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> BuildCleanTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToBuildCleanTask(taskName, isTarget, parentTaskName)
        .GetBuildTask();
    }

    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> PreBuildTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToPreBuildTask(taskName, isTarget, parentTaskName)
        .GetBuildTask();
    }

    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> BuildTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToBuildTask(taskName, isTarget, parentTaskName)
        .GetBuildTask();
    }

    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> PostBuildTask(
      this ICakeContext context,
      string taskName,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToPostBuildTask(taskName, isTarget, parentTaskName)
        .GetBuildTask();
    }
  }
}
