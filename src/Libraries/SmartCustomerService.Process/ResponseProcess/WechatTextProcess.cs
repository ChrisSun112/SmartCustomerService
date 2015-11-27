using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Web;
using SmartCustomerService.Core;


namespace SmartCustomerService.Process
{
    public class WechatTextProcess:IProcess
    {

        public IDictionary<string, string> ParamDic { get; set; }

        public void Process()
        {
            TextMessage textMsg = null;

            try
            {
                string dataString = RequestHelper.GetPostParamData("data");
                textMsg = JsonConvert.DeserializeObject<TextMessage>(dataString);
                ParamDic = SpringContainer.GetObject<ParamList>("SystemList").WechatList;
            }
            catch (Exception ex)
            {
                throw new Exception("发送微信平台文本消息异常,"+ex.Message);
            }
           
            string result = SendWechat(textMsg);

            if (!string.IsNullOrEmpty(result))
            {
                HttpContext.Current.Response.Write(result);
            }
            else
            {
                throw new Exception("调用微信客服接口单发消息返回的消息为空");
            }
        }

        /// <summary>
        /// 推送微信消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string SendWechat(TextMessage msg)
        {

           
            string access_token = WechatTokenHelper.GetWechatToken(msg.AppCode,ParamDic); //获取微信token

            WechatTextRequestInfo info = new WechatTextRequestInfo() { touser = msg.Openid, text =  msg.Text  };

            string data = JsonConvert.SerializeObject(info);

            string url = string.Format(ParamDic["CSSendMsgUrl"], access_token);

            string result = string.Empty;            

            try
            {
                result = WebRequserSender.SendPOST(data, url, "application/xml;");

                if (result.Contains("\"errcode\":42001") || result.Contains("\"errcode\":42014"))
                {
                    WechatTokenHelper.SetWechatToken(msg.AppCode, ParamDic);

                    //微信token过期，获取新生成的token二次发送
                    access_token = WechatTokenHelper.GetWechatToken(msg.AppCode, ParamDic); //获取微信token
                    url = string.Format(ParamDic["CSSendMsgUrl"], access_token);

                    result = WebRequserSender.SendPOST(data, url, "application/xml;");

                    if (result.Contains("\"errcode\":42001") || result.Contains("\"errcode\":42014"))
                    {
                        Logger.Write(result, "ReturnError");
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("微信客服接口发送信息异常：" + ex.Message);
            }

           
            return result;



        }


        #region 在线客服openAPI存储微信token相关方法，已取消

        /*
        internal string GetToken(string appcode)
        {
            //微信token存储在在线客服二期openapi中，key为platform_app_key的方式，
            string key = appcode.ToLower()+"_token";

            string access_token = GetCustomResoure(key);
            //string access_token = string.Empty;
            
            //如果openapi中无微信token，调用微信接口从新生成，并存入openapi中
            if (string.IsNullOrEmpty(access_token))
            {
                AccessToken accessToken = GetAccessTokenFormWechat(appcode);
                SetCustomResource(key, accessToken.access_token,accessToken.expires_in-10);
                access_token = accessToken.access_token;
            }     

            return access_token;
        }
       
        

        
        /// <summary>
        /// 设置自定义资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire_in"></param>
        private void SetCustomResource(string key, string value, int expire_in)
        {
            key = HttpUtility.UrlEncode(key, System.Text.Encoding.GetEncoding("UTF-8"));
            value = HttpUtility.UrlEncode(value, System.Text.Encoding.GetEncoding("UTF-8"));

            string sendData = "{\"action\": \"customresource.set\",\"data\":{\"key\": \"" + key + "\",\"value\":\"" + value
                + "\",\"expire_in\":" + expire_in + "}}";

            string url = string.Format(ParamDic["openapiurl"], ParamDic["openapi_token"]);

            string result = string.Empty;
            try
            {
                result = WebRequserSender.SendPOST(sendData, url, "application/json;");
            }
            catch (Exception ex)
            {
                throw new Exception("设置自定义资源异常," + ex.Message);
            }

            JObject jobject = null;

            try
            {
                jobject = JObject.Parse(result);
               
            }
            catch (Exception ex)
            {
                throw new Exception("处理设置自定义资源返回结果异常:" + ex.Message);
            }

            if (jobject["status_code"].ToString() != "0")
            {
                throw new Exception("处理设置自定义资源返回结果异常:" + jobject["status_message"].ToString());
            }

        }


        /// <summary>
        /// 获取自定义资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCustomResoure(string key)
        {
            string sendData = "{\"action\": \"customresource.get\",\"data\":{\"key\": \"" + HttpUtility.UrlEncode(key, System.Text.Encoding.GetEncoding("UTF-8")) + "\"}}";

            string url = string.Format(ParamDic["openapiurl"], ParamDic["openapi_token"]);

            string result = string.Empty;
            try
            {
                result = WebRequserSender.SendPOST(sendData, url, "application/json;");
            }
            catch (Exception ex)
            {
                throw new Exception("获取自定义资源异常," + ex.Message);
            }

            try
            {
                var jobject = JObject.Parse(result);
                
                if (jobject["status_code"].ToString() != "0")
                {
                    //获取失败，返回空串
                    return string.Empty; 
                }
                else
                {
                    return (string)jobject["data"]["value"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("处理获取自定义资源返回结果异常:" + ex.Message);
            }


        }

        internal AccessToken GetAccessTokenFormWechat(string appcode)
        {
            string app_key = string.Empty;
            string secret_Key = string.Empty;

            try
            {
                app_key = ParamDic[appcode + "_app_key"];
                secret_Key = ParamDic[appcode + "_secret_key"];
            }
            catch (Exception ex)
            {
               throw new Exception("获取"+appcode+" appkey或secretkey失败,"+ex.Message);
            }
            string getTokenUrl = string.Format(ParamDic["get_tokenUrl"], app_key, secret_Key);

            string result = string.Empty;
            try
            {
                result = WebRequserSender.SendGET(getTokenUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("获取微信access token异常：" + ex.Message);
            }


            //解析微信响应消息，获取access token
            AccessToken access_token = null;
            try
            {
                if (result.Contains("access_token"))
                {
                    access_token = JsonConvert.DeserializeObject<AccessToken>(result);
                }
                else 
                {
                    throw new Exception("获取微信access token出错,错误信息:" + result);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("解析access token异常：" + ex.Message);
            }
            return access_token;
        }
         * */
        #endregion
    }
}
