using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YTO.Alipay.ServiceWindow.DataBus.Core;
using YTO.Alipay.ServiceWindow.DataBus.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YTO.Framework.Core;
using System.Web;

namespace YTO.Alipay.ServiceWindow.DataBus.Process
{
    public class AlipayTextProcess:IProcess
    {
        private IDictionary<string, string> ParamDic { get; set; }

        public void Process()
        {
            TextMessage textMsg = null;

            try
            {
                string dataString = RequestHelper.GetParamData("data");
                textMsg = JsonConvert.DeserializeObject<TextMessage>(dataString);
                ParamDic = SpringContainer.GetObject<ParamList>("SystemList").AlipayList;
            }
            catch (Exception ex)
            {
                throw new Exception("发送支付宝服务窗平台文本消息异常," + ex.Message);
            }

            string result = SendAlipay(textMsg);
            if(!string.IsNullOrEmpty(result)) 
            {
                HttpContext.Current.Response.Write(result);
            }
            else{
                throw new Exception("调用支付宝客服接口异步单发消息返回的消息为空");
            }

            
        }


        #region 支付宝服务窗
        /// <summary>
        /// 发送支付宝服务窗消息
        /// </summary>
        /// <param name="msg"></param>
        private string SendAlipay(TextMessage msg)
        {
            string url = ParamDic["url"];
            string appid = string.Empty;
            try
            {
                appid = ParamDic[msg.AppCode + "_app_id"]; //获取appid,通过appcode_app_id找ParamList.config
            }
            catch (Exception ex)
            {
                throw new Exception("获取" + msg.AppCode + " appid失败" + ex.Message);
                
            }

            

            string content = msg.Text.Content;
            List<string> lists = new List<string>();

            //支付宝服务窗异步文本消息不能超过4k，所以限制消息文本不得超过360个字符，
            while (content.Length > 360)
            {
                lists.Add(content.Substring(0,360));
                content = content.Remove(0, 360);
            }

            lists.Add(content);

            string result = string.Empty;
                        
            foreach (var s in lists)
            {
                string str = s;

                str = HttpUtility.HtmlEncode(str); //对发送文本中包含html转义字符做html转义
                str = HttpUtility.JavaScriptStringEncode(str); //对发送字符做javascript转义

                //生成发送数据
                string sendData = StringHelper.GetSendStringContent(msg.Openid, str, appid, "alipay.mobile.public.message.custom.send", "utf-8");//生成要发送的数据


                try
                {
                    result = SendHelper.SendPOSTAndGetStr(url, sendData, "utf-8");//发送
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                //返回其中一个发送异常
                if (!result.Contains("\"code\":200"))
                {
                    Logger.Write("SendData:"+sendData+"\n Response Data:"+result, "ReturnError");
                    return result;
                }
            }

            return result;

        }

        #endregion
    }
}
