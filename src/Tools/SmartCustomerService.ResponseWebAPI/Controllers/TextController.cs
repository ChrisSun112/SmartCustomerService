using SmartCustomerService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace SmartCustomerService.ResponseWebAPI.Controllers
{
    public class TextController : ApiController
    {

      
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        //public void Post([FromBody]string value)
        //{
            
        //}

        
        public CodeMsg Post([FromBody] RequestMsg<TextMessage> param)
        {

            Type type = typeof(PlatformType);

            CodeMsg codeMsg = null;
            //获取所有支持的平台
            string[] platformTypes = type.GetEnumNames();

            try
            {
                var codeMsgDic = SpringContainer.GetObject<CodeMsgData>("CodeMsgData").CodeMsgDic;

                if (!platformTypes.Contains(param.platformtype))
                {
                    //传入的平台不支持或名称错误
                    codeMsg = codeMsgDic["wrongPlatformType"];
                   
                }
                
            }
            catch (Exception)
            {
                codeMsg = new CodeMsg()
                {
                    code = "500",
                    msg = "系统错误"
                };
                return codeMsg;
            }


            return null;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
