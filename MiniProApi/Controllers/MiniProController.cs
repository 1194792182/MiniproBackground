using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProApi.Model.MiniPro;
using Weixin.WxOpen.AdvancedAPIs.Sns;
using Weixin.WxOpen.Containers;
using WeiXin;
using WeiXin.Exceptions;

namespace MiniProApi.Controllers
{
    [Produces("application/json")]
    [Route("MiniPro")]
    public class MiniProController : Controller
    {
        [HttpPost]
        [Route("GetThirdSessionId")]
        public ActionResult GetThirdSessionId([FromBody]GetThirdSessionIdRequest request)
        {
            try
            {
                var jsonResult = SnsApi.JsCode2Json("wxa0d2127a7dc890b8", "", request.Code);

                if (jsonResult.errcode == ReturnCode.请求成功)
                {
                    var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);

                    //注意：生产环境下SessionKey属于敏感信息，不能进行传输！
                    return Ok(new { success = true, msg = "OK", sessionId = sessionBag.Key });
                }
                else
                {
                    return Ok(new { success = false, msg = jsonResult.errmsg });
                }
            }
            catch (Exception ex)
            {
                if (ex is ErrorJsonResultException)
                {
                    return Ok(new { success = false, msg = ex.Message });
                }
                return Ok(new { success = false, msg = ex.Message });
            }
        }
    }
}