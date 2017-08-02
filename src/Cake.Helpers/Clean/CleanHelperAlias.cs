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
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("Clean")]
  public static class CleanHelperAlias
  {
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
        .GetBuildTask();
    }
  }
}
