using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Helpers.Nuget
{
    internal class NugetSource : INugetSource
    {
      public string FeedName { get; set; }
      public string FeedSource { get; set; }
      public bool IsSecure { get; set; }
      public string Username { get; set; }
      public string Password { get; set; }
    }
}
