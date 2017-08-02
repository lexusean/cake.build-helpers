using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers
{
  public interface IHelper
  { }

  public interface IHelperContext : IHelper
  {
    ICakeContext Context { get; set; }
  }
}
