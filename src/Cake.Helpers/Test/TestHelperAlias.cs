using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Test
{
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Test")]
  public static class TestHelperAlias
  {
    #region Static Members

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
        .GetTaskBuilder();
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
        .GetTaskBuilder();
    }

    #endregion
  }
}