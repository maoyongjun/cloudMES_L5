using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.BaseClass
{
    /// <summary>
    /// 
    /// </summary>
    public class APIInfo
    {
        public string FunctionName = "";
        public string Description = "";
        public List<APIInputInfo> Parameters = new List<APIInputInfo>();
        public List<MESPermission> Permissions = new List<MESPermission>();
    }
}
