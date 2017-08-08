using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cake.Helpers.Command
{
  /// <summary>
  ///   Command Argument
  /// </summary>
  public interface ICommandArgument
  {
    /// <summary>
    ///   Long Name of Command Argument
    /// </summary>
    string Name { get; set; }

    /// <summary>
    ///   Short name of Command Argument
    /// </summary>
    string Shortname { get; set; }

    /// <summary>
    ///   Description of Command Argument
    /// </summary>
    string Description { get; set; }

    /// <summary>
    ///   Sub Arguments
    /// </summary>
    List<ICommandArgument> Arguments { get; }

    /// <summary>
    ///   Action to be executed when command argument defined
    /// </summary>
    Action<ICommandArgument> ArgumentAction { get; set; }
  }

  /// <inheritdoc />
  [ExcludeFromCodeCoverage]
  public class CommandArgument : ICommandArgument
  {
    #region Private Fields

    private Action<ICommandArgument> _ArgumentAction;

    #endregion

    #region ICommandArgument Members

    /// <inheritdoc />
    public Action<ICommandArgument> ArgumentAction
    {
      get
      {
        if (this._ArgumentAction == null)
          this._ArgumentAction = arg => { };

        return this._ArgumentAction;
      }
      set { this._ArgumentAction = value; }
    }

    /// <inheritdoc />
    public List<ICommandArgument> Arguments { get; } = new List<ICommandArgument>();

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Shortname { get; set; }

    #endregion
  }
}