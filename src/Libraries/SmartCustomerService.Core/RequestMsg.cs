using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCustomerService.Core
{
    /// <summary>
    /// 回复接口请求消息类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestMsg<T> 
    {
        // 平台为接入方提供的appkey
        public string appkey { get; set; }

        // 平台为接入方提供的appsecret
        public string appsecret { get; set; }

        //平台类型，wechat,qq,weibo,alipay...
        public string platformtype { get; set; }

        //业务参数
        public T data { get; set; }
    }
}
