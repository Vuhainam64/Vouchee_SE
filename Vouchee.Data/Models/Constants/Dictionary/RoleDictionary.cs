using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Constants.Dictionary
{
    public class RoleDictionary
    {
        public static readonly Dictionary<string, string> role = new Dictionary<string, string>
        {
            {"ADMIN", "ff54acc6-c4e9-4b73-a158-fd640b4b6940"},
            {"STAFF", "015ae3c5-eee9-4f5c-befb-57d41a43d9df"},
            {"USER", "2d80393a-3a3d-495d-8dd7-f9261f85cc8f"},
        };
    }
}
