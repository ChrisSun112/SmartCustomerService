using SmartCustomerService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace SmartCustomerService.ResponseWebAPI
{
    public class NoHandleExceptionFilter:ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            // 取得发生例外时的错误讯息
             if(actionExecutedContext.Exception.Message!=null){
                 
                 // 重新打包回传的讯息
                 //actionExecutedContext.Response.StatusCode = System.Net.HttpStatusCode.OK;
                 actionExecutedContext.Response.Content = new StringContent("{\"code\":\"500\",\"msg\":\"系统错误\"}");
                
             }   
           
        } 

    }
}