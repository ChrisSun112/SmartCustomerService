using SmartCustomerService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

            //获取所有支持的平台
            string[] platformTypes = type.GetEnumNames();

            var codeMsgDic = SpringContainer.GetObject<CodeMsgData>("CodeMsgData").CodeMsgDic;
            

            if (!platformTypes.Contains(param.platformtype))
            {
                //传入的平台不支持或名称错误
                var codeMsg = codeMsgDic["wrongPlatformType"];
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
