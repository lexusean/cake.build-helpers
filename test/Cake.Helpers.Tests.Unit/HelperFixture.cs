using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Helpers.Settings;
using Cake.Testing.Fixtures;

namespace Cake.Helpers.Tests.Unit
{
  public class HelperFixture : ToolFixture<HelperSettings>
  {
    public static HelperFixture CreateFixture()
    {
      return new HelperFixture();
    }

    public HelperFixture()
      : base("none")
    { }

    protected override void RunTool()
    {
      throw new NotImplementedException();
    }
  }
}
