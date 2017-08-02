using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Helpers.Tasks
{
  [CakeAliasCategory("Task")]
  [CakeAliasCategory("Helper")]
  public static class TaskAliases
  {
    [CakePropertyAlias]
    public static TaskHelper TaskHelper(this ICakeContext context)
    {
      if(context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetTaskHelper();
    }

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
        .GetBuildTask();
    }
  }
}
