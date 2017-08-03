using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Helpers.Nuget
{
  public interface INugetSource
  {
    string FeedName { get; set; }
    string FeedSource { get; set; }
    bool IsSecure { get; set; }
    string Username { get; set; }
    string Password { get; set; }
  }
}
