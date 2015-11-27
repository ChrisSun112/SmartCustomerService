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
    public class QQTextProcess:IProcess
    {
        private IDictionary<string, string> ParamDic { get; set; }

        public void Process()
        {
            TextMessage textMsg = null;

            try
            {
                string dataString = RequestHelper.GetParamData("data");
                textMsg = JsonConvert.DeserializeObject<TextMessage>(dataString);
                ParamDic = SpringContainer.GetObject<ParamList>("SystemList").QQList;
            }
            catch (Exception ex)
            {
                throw new Exception("发送QQ平台文本消息异常," + ex.Message);
            }

            string result = SendQQMessage(textMsg);
            if (!string.IsNullOrEmpty(result))
            {
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                throw new Exception("调用QQ客服接口单发消息返回的消息为空");
            }

        }

        #region QQ相关方法
        public string SendQQMessage(TextMessage msg)
        {
            string nonce = RandomHelper.GetRandom();
            string token = string.Empty; 

            QqTextSendInfo sendInfo = new QqTextSendInfo();
            sendInfo.msgtype = "text";

            try
            {
                sendInfo.spid = ParamDic[msg.AppCode + "_spid"];
                token = ParamDic[msg.AppCode + "_token"];
            }
            catch (Exception ex)
            {
                throw new Exception("获取"+msg.AppCode+"spid或token失败");
            }
            sendInfo.touser = msg.Openid;
            sendInfo.spsc = "00";
            msg.Text.Content = HttpUtility.UrlDecode(msg.Text.Content); //url编码
            sendInfo.text = msg.Text;

            string signature = SHA1.SHA1_Encrypt(sendInfo.spid + token + nonce); //签名

            string url = ParamDic["SendMsgUrl"] + "?spid=" + sendInfo.spid + "&signature=" + signature + "&nonce=" + nonce + "";
            string JsonData = JsonConvert.SerializeObject(sendInfo);


            string ruStr = WebRequserSender.SendPOST(JsonData, url, "text/json;charset=utf-8");
            //JObject jsonObj = JObject.Parse(ruStr);
            //if (jsonObj["errcode"].ToString() != "0")
            //{
            //    ExceptionHelper.HandleException(new Exception("调用QQ接口，发送消息接口请求异常：" + ruStr),"Job");
            //}

            return ruStr;

        }


       
        #endregion
    }
}
