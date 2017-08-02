using Cake.Core.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Build;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Test
{
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Test")]
  public static class TestHelperAlias
  {
    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> TestCleanTask(
      this ICakeContext context,
      string taskName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToTestCleanTask(taskName, testCategory, isTarget, parentTaskName)
        .GetBuildTask();
    }

    [CakeMethodAlias]
    public static CakeTaskBuilder<ActionTask> TestTask(
      this ICakeContext context,
      string taskName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.TaskHelper()
        .AddToTestTask(taskName, testCategory, isTarget, parentTaskName)
        .GetBuildTask();
    }
  }
}
