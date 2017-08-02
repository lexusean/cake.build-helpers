using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.Clean;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Test
{
  public static class TestHelperExtensions
  {
    private const string TargetCategory = "Test";

    public static IHelperTask GetTestCleanTask(
      this TaskHelper helper,
      string testCategory = "Generic",
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      var taskName = $"{testCategory}-{targetName}";

      return helper.GetCleanTask(TargetCategory, taskName, isTarget);
    }

    public static IHelperTask AddToTestCleanTask(
      this TaskHelper helper,
      string targetName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if(string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if(string.IsNullOrWhiteSpace(testCategory))
        throw new ArgumentNullException(nameof(testCategory));

      if(!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget
        ? helper.GetTestCleanTask(testCategory)
        : helper.GetTestCleanTask(testCategory, parentTaskName);
      var newTask = helper.GetTestCleanTask(testCategory, newTaskName, isTarget);

      if (isTarget)
      {
        parentTask.GetBuildTask()
          .IsDependentOn(newTask.GetBuildTask());
      }

      return newTask;
    }

    public static IHelperTask GetTestTask(
      this TaskHelper helper,
      string testCategory = "Generic",
      string targetName = "All",
      bool isTarget = true)
    {
      if (helper == null)
        return null;

      testCategory = $"{TargetCategory}-{testCategory}";
      var taskName = $"{testCategory}-{targetName}";

      return helper.GetTask(taskName, isTarget, TargetCategory, testCategory);
    }

    public static IHelperTask AddToTestTask(
      this TaskHelper helper,
      string targetName,
      string testCategory,
      bool isTarget = true,
      string parentTaskName = "")
    {
      if (string.IsNullOrWhiteSpace(targetName))
        throw new ArgumentNullException(nameof(targetName));

      if (string.IsNullOrWhiteSpace(testCategory))
        throw new ArgumentNullException(nameof(testCategory));

      if (!isTarget && string.IsNullOrWhiteSpace(parentTaskName))
        throw new ArgumentNullException(nameof(parentTaskName));

      var newTaskName = isTarget ? targetName : $"{parentTaskName}-{targetName}";
      var parentTask = isTarget
        ? helper.GetTestTask(testCategory)
        : helper.GetTestTask(testCategory, parentTaskName);
      var newTask = helper.GetTestTask(testCategory, newTaskName, isTarget);

      if (isTarget)
      {
        parentTask.GetBuildTask()
          .IsDependentOn(newTask.GetBuildTask());
      }

      return newTask;
    }
  }
}
