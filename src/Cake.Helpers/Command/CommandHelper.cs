using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Helpers.Settings;
using Cake.Helpers.Tasks;

namespace Cake.Helpers.Command
{
  /// <summary>
  /// Command Helper Contract
  /// </summary>
  public interface ICommandHelper : IHelperContext
  {
    /// <summary>
    /// List of defined arguments
    /// </summary>
    IEnumerable<ICommandArgument> Arguments { get; }
    /// <summary>
    /// Help Argument - Lists help descriptions
    /// </summary>
    ICommandArgument HelpArgument { get; }
    /// <summary>
    /// Run Argument - Runs target defined
    /// </summary>
    ICommandArgument RunArgument { get; }
    /// <summary>
    /// Available Targets argument - Lists all public targets defined 
    /// </summary>
    ICommandArgument AvailableTargetsArgument { get; }
    /// <summary>
    /// Default target to run (Empty)
    /// </summary>
    string DefaultTarget { get; set; }
    /// <summary>
    /// Description of script to list in help
    /// </summary>
    string ScriptDescription { get; set; }
    /// <summary>
    /// Add argument to command helper
    /// </summary>
    /// <param name="name">Long name</param>
    /// <param name="shortName">Short Name</param>
    /// <param name="desc">description</param>
    /// <returns>Command Argument</returns>
    /// <example>
    /// <code>
    /// var newArg = CommandHelper.AddArgument("test", "t", "Testing stuff");
    /// newArg.ArgumentAction = () => { "Do Something" };
    /// </code>
    /// </example>
    ICommandArgument AddArgument(string name, string shortName, string desc);
    /// <summary>
    /// Runs any command defined in Cake commandline.
    /// </summary>
    void Run();
  }

  internal class CommandHelper : ICommandHelper
  {
    #region Internal Properties

    internal ITaskHelper TaskHelper { get; }

    #endregion

    #region Private Fields

    private readonly List<ICommandArgument> _Arguments = new List<ICommandArgument>();
    private readonly IHelperSettings _helperSettings = SingletonFactory.GetHelperSettings();

    #endregion

    #region Ctor

    internal CommandHelper(
      ITaskHelper taskHelper)
    {
      if (taskHelper == null)
        throw new ArgumentNullException(nameof(taskHelper));

      this.TaskHelper = taskHelper;
      this.AddDefaultArguments();
    }

    #endregion

    #region Private Methods

    [ExcludeFromCodeCoverage]
    private void AddAvailableTargetsArgument()
    {
      var desc = "Action: -available-targets | -at\n";
      desc += "Description: Lists all targets defined for use in script\n";
      desc += "Typical Usage:\n";
      desc += "  Windows:\n";
      desc += "    build.ps1 -at(-available-targets)\n";
      desc += "  Linux:\n";
      desc += "    build.sh -at(-available-targets)\n";

      this.AvailableTargetsArgument = this.AddArgument("available-targets", "at", desc);
      this.AvailableTargetsArgument.ArgumentAction = arg =>
      {
        var categories = this.TaskHelper.GetCategories().OrderBy(t => t).ToArray();
        this.Context.Information("");
        this.Context.Information("{0} Target Categories Available:", categories.Length);

        foreach (var category in categories)
        {
          var tasksForCategory = this.TaskHelper.Tasks
            .Where(t => t.IsTarget)
            .Where(t => t.Category == category)
            .OrderBy(t => t.Task.Name)
            .ToArray();

          this.Context.Information("");
          this.Context.Information("  Category: {0}", category);
          this.Context.Information("  {0} Targets Available:", tasksForCategory.Length);

          foreach (var task in tasksForCategory)
          {
            this.AddAvailTargetsArgumentForTask(tasksForCategory, task, 4, 0);
          }
        }

        this.Context.Information("");
      };
    }

    [ExcludeFromCodeCoverage]
    private void AddAvailTargetsArgumentForTask(
      IEnumerable<IHelperTask> allTargets,
      IHelperTask task,
      int indent = 0,
      int currentDepth = 0)
    {
      if (currentDepth >= 10)
        return;

      var taskString = string.Format("{0}- {1}", new string(' ', indent), task.Task.Name);
      this.Context.Information(taskString);

      var dependencyTargets = task.Task.Dependencies
        .Where(x => allTargets.Any(y => y.Task.Name == x && y.TaskType == task.TaskType))
        .Select(x => this.TaskHelper.Tasks.FirstOrDefault(y => y.Task.Name == x))
        .Where(x => x != null)
        .OrderBy(t => t.Task.Name)
        .ToArray();

      foreach (var dTarget in dependencyTargets)
      {
        this.AddAvailTargetsArgumentForTask(allTargets, dTarget, indent + 2, currentDepth + 1);
      }
    }

    private void AddDefaultArguments()
    {
      this.AddHelpArgument();
      this.AddAvailableTargetsArgument();
      this.AddRunArgument();
    }

    private void AddHelpArgument()
    {
      var desc = "Action: --support | -h\n";
      desc += "Description: Shows help for arguments\n";
      desc += "Typical Usage:\n";
      desc += "  Windows:\n";
      desc += "    build.ps1 -h(--support)\n";
      desc += "    build.ps1 <action> -h(--support)\n";
      desc += "  Linux:\n";
      desc += "    build.sh -h(--support)\n";
      desc += "    build.sh <action> -h(--support)\n";

      this.HelpArgument = this.AddArgument("support", "h", desc);
    }

    [ExcludeFromCodeCoverage]
    private void AddRunArgument()
    {
      var desc = "Action: -run | -r\n";
      desc += "Description: Runs a build target\n";
      desc += "Typical Usage:\n";
      desc += "  Windows:\n";
      desc += "    build.ps1 -r(-run)=<target>\n";
      desc += "  	   Run to get available targets: build.ps1 -at(--available-targets)\n";
      desc += "  Linux:\n";
      desc += "    build.sh -r(-run)=<target>\n";
      desc += "  	   Run to get available targets: build.sh -at(--available-targets)\n";

      this.RunArgument = this.AddArgument("run", "r", desc);
      this.RunArgument.ArgumentAction = arg =>
      {
        var target = arg.GetArgumentValue(this.Context);
        target = string.IsNullOrWhiteSpace(target) ? this.DefaultTarget : target;

        if (string.IsNullOrWhiteSpace(target))
        {
          var message = "No target or Default Target defined for --run | -r";
          this.Context.Error(message);
          throw new ArgumentNullException($"target", message);
        }

        this.TaskHelper.RunTarget(target);
      };
    }

    [ExcludeFromCodeCoverage]
    private void RunHelp(ICommandArgument arg = null)
    {
      this.Context.Information("\n");

      if (arg != null)
      {
        this.Context.Information(arg.Description);
      }
      else
      {
        var desc = this.ScriptDescription + "\n";
        desc += "Typical Usage:\n";
        desc += "  Windows:\n";
        desc += "    build.ps1 <--longAction | -shortAction>=<action option>\n";
        desc += "  Linux:\n";
        desc += "    build.sh <--longAction | -shortAction>=<action option>\n";

        desc += "\n";
        desc += "Available Actions:\n";
        foreach (var a in this.Arguments)
        {
          desc += $"  --{a.Name}|-{a.Shortname}\n";
        }

        desc += "\n";
        desc += this.HelpArgument.Description;
        this.Context.Information(desc);
      }

      this.Context.Information("\n");
    }

    #endregion

    #region ICommandHelper Members

    public IEnumerable<ICommandArgument> Arguments
    {
      get { return this._Arguments; }
    }

    public ICommandArgument AvailableTargetsArgument { get; private set; }

    public string DefaultTarget { get; set; } = string.Empty;

    public ICommandArgument HelpArgument { get; private set; }
    public ICommandArgument RunArgument { get; private set; }

    public string ScriptDescription { get; set; } = "Build Script";

    public ICommandArgument AddArgument(string name, string shortName, string desc)
    {
      if (string.IsNullOrWhiteSpace(name) ||
          string.IsNullOrWhiteSpace(shortName) ||
          string.IsNullOrWhiteSpace(desc))
      {
        this.Context.Debug($"Missing name, shortName, or description for command");
        return null;
      }

      var arg = this.Arguments
        .FirstOrDefault(t => t.Name == name || t.Shortname == shortName);

      if (arg == null)
      {
        arg = new CommandArgument();
        this._Arguments.Add(arg);
      }

      arg.Name = name;
      arg.Shortname = shortName;
      arg.Description = desc;

      return arg;
    }

    [ExcludeFromCodeCoverage]
    public void Run()
    {
      this._helperSettings.SetupSetting();

      var isHelp = this.HelpArgument.HasArgument(this.Context);
      this.Context.Debug("Help Set: {0}", isHelp);

      var hadArguments = false;
      foreach (var arg in this.Arguments.Where(t => t != this.HelpArgument))
      {
        if (!arg.HasArgument(this.Context))
          continue;

        hadArguments = true;
        if (isHelp)
        {
          this.RunHelp(arg);
          return;
        }
        if (arg.ArgumentAction == null)
          this.Context.Information("No Action Defined for Argument: {0}", arg.Name);
        else
          arg.ArgumentAction(arg);

        break;
      }

      if (isHelp || !hadArguments)
        this.RunHelp();
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { get; set; }

    #endregion
  }

  /// <summary>
  /// Extensions for CommandHelper and Dependencies
  /// </summary>
  public static class CommandHelperExtensions
  {
    #region Static Members

    /// <summary>
    /// Gets argument value from Cake commandline.
    /// </summary>
    /// <param name="arg">Command Argument</param>
    /// <param name="context">Cake Context</param>
    /// <returns>Value as string if defined. Empty string if not defined.</returns>
    /// <example>
    /// <code>
    /// var newArg = CommandHelper.AddArgument("test", "t", "Testing stuff");
    /// var argVal = newArg.GetArgumentValue(Context)
    /// </code>
    /// </example>
    public static string GetArgumentValue(this ICommandArgument arg, ICakeContext context)
    {
      if (arg == null)
        throw new ArgumentNullException(nameof(arg));

      if (context == null)
        throw new ArgumentNullException(nameof(context));

      var longArgValue = context.Argument(arg.Name, string.Empty);
      if (!string.IsNullOrWhiteSpace(longArgValue))
        return longArgValue;

      var shortArgValue = context.Argument(arg.Shortname, string.Empty);

      return shortArgValue;
    }

    /// <summary>
    /// Gets if argument defined in Cake commandline
    /// </summary>
    /// <param name="arg">Command Argument</param>
    /// <param name="context">Cake Context</param>
    /// <returns>True if it exists, false otherwise</returns>
    /// <example>
    /// <code>
    /// var newArg = CommandHelper.AddArgument("test", "t", "Testing stuff");
    /// var hasArg = newArg.HasArgument(Context)
    /// </code>
    /// </example>
    public static bool HasArgument(this ICommandArgument arg, ICakeContext context)
    {
      if (arg == null)
        throw new ArgumentNullException(nameof(arg));

      if (context == null)
        throw new ArgumentNullException(nameof(context));

      var hasArg = context.HasArgument(arg.Name);
      hasArg |= context.HasArgument(arg.Shortname);

      return hasArg;
    }

    #endregion
  }
}