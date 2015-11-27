using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SmartCustomerService.Core
{
    /// <summary>
    /// 请求helper
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 获取POST请求参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetPostParamData(string paramName)
        {
            string dataString = HttpContext.Current.Request.Form[paramName];
            if (string.IsNullOrEmpty(dataString))
            {
                throw new Exception("缺少" + paramName + "参数");
            }
            return dataString;
        }
    }
}
