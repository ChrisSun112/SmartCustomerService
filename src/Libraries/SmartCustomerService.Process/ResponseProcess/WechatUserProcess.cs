using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using SmartCustomerService.Process;
using SmartCustomerService.Core;

namespace YTO.Alipay.ServiceWindow.DataBus.Process
{
    public class WechatUserProcess:IProcess
    {
        private IDictionary<string, string> ParamDic { get; set; }

        public void Process()
        {
            string openid = RequestHelper.GetPostParamData("openid");
            string appcode = RequestHelper.GetPostParamData("appcode");

            ParamDic = SpringContainer.GetObject<ParamList>("SystemList").WechatList;

            string token = WechatTokenHelper.GetWechatToken(appcode, ParamDic);

            

            string info = GetWechatUserInfo(openid,appcode,token);
  
            HttpContext.Current.Response.Write(info);
           
        }

        private string GetWechatUserInfo(string openid,string appcode,string token)
        {
            string url = string.Format(ParamDic["get_userinfoUrl"], token, openid);

            string result = string.Empty;

            try
            {
                result = WebRequserSender.SendGET(url);
                if (result.Contains("\"errcode\":42001") || result.Contains("\"errcode\":42014"))
                {
                    WechatTokenHelper.SetWechatToken(appcode, ParamDic);
                    Logger.Write(result, "ReturnError");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取微信用户信息异常:"+ex.Message);
            }
     

            return result;              
            
           
        }
    }
}
