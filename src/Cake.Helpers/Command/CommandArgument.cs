using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Helpers.Command
{
  public interface ICommandArgument
  {
    string Name { get; set; }
    string Shortname { get; set; }
    string Description { get; set; }
    List<ICommandArgument> Arguments { get; }
    Action<ICommandArgument> ArgumentAction { get; set; }
  }

  public class CommandArgument : ICommandArgument
  {
    public string Name { get; set; }
    public string Shortname { get; set; }
    public string Description { get; set; }
    public List<ICommandArgument> Arguments { get; } = new List<ICommandArgument>();

    private Action<ICommandArgument> _ArgumentAction = null;
    public Action<ICommandArgument> ArgumentAction
    {
      get
      {
        if (this._ArgumentAction == null)
        {
          this._ArgumentAction = arg => { };
        }

        return this._ArgumentAction;
      }
      set { this._ArgumentAction = value; }
    }
  }
}
