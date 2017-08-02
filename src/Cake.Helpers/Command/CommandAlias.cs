using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Helpers.Command
{
  [CakeAliasCategory("Command")]
  [CakeAliasCategory("Helper")]
  public static class CommandAlias
  {
    [CakePropertyAlias]
    public static ICommandHelper CommandHelper(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      var commandHelper = SingletonFactory.GetCommandHelper();

      return commandHelper;
    }

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
