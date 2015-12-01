using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCustomerService.Core
{
    /// <summary>
    /// 错误码
    /// </summary>
    public class CodeMsg
    {
        public string code { get; set; }

        public string msg { get; set; }
    }

    /// <summary>
    /// 错误码字典
    /// </summary>
    public class CodeMsgData
    {
        public Dictionary<string, CodeMsg> CodeMsgDic { get; set; }
    }
}
