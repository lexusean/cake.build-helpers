using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Helpers.Settings;
using Cake.Testing.Fixtures;

namespace Cake.Helpers.Tests.Unit
{
  [ExcludeFromCodeCoverage]
  internal class HelperFixture : ToolFixture<HelperSettings>
  {
    #region Static Members

    public static HelperFixture CreateFixture()
    {
      return new HelperFixture();
    }

    #endregion

    #region Ctor

    public HelperFixture()
      : base("none")
    { }

    #endregion

    #region Protected Methods

    protected override void RunTool()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}