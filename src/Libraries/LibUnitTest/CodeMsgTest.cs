using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spring.Core;
using Spring.Context;
using Spring.Context.Support;
using SmartCustomerService.Core;

namespace LibUnitTest
{
    [TestClass]
    public class CodeMsgTest
    {
        [TestMethod]
        public void CodeMsgIocTest()
        {

            var CodeMsgDic = SpringContainer.GetObject<CodeMsgData>("CodeMsgData").CodeMsgDic;
            string errCode = CodeMsgDic["missingParam"].Code;

            Assert.AreEqual<string>("40001", errCode);
        }
    }
}
