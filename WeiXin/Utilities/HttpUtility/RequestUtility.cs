using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WeiXin.Utilities.WebProxy;
using Microsoft.AspNetCore.Http;
using System.Net.Security;
using System.Net.Http.Headers;
using WeiXin.Helpers.Extensions;
using System.Threading.Tasks;
using WeiXin.Exceptions;
using WeiXin.Helpers;

namespace WeiXin.Utilities.HttpUtility
{
    public static class RequestUtility
    {
        #region 基础

        private static IWebProxy _webproxy = null;
        /// <summary>
        /// 设置Web代理
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void SetHttpProxy(string host, string port, string username, string password)
        {
            ICredentials cred;
            cred = new NetworkCredential(username, password);
            if (!string.IsNullOrEmpty(host))
            {
                _webproxy = new CoreWebProxy(new Uri(host + ":" + port ?? "80"), cred);
            }
        }

        /// <summary>
        /// 清除Web代理状态
        /// </summary>
        public static void RemoveHttpProxy()
        {
            _webproxy = null;
        }

        /// <summary>
        /// 从 Request.Body 中读取流，并复制到一个独立的 MemoryStream 对象中
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Stream GetRequestMemoryStream(this HttpRequest request)
        {
            string body = new StreamReader(request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputStream = new MemoryStream(requestData);
            return inputStream;
        }



        /// <summary>
        /// 验证服务器证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 验证服务器证书
        /// </summary>
        /// <param name="request"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static StreamContent CreateFileContent(Stream stream, string formName, string fileName, string contentType = "application/octet-stream")
        {
            fileName = UrlEncode(fileName);
            var fileContent = new StreamContent(stream);
            //上传格式参考：
            //https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1444738729
            //https://work.weixin.qq.com/api/doc#10112
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"{0}\"".FormatWith(formName),
                FileName = "\"" + fileName + "\"",
                Size = stream.Length
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }

        /// <summary>
        /// 设置HTTP头
        /// </summary>
        /// <param name="client"></param>
        /// <param name="refererUrl"></param>
        /// <param name="useAjax">是否使用Ajax</param>
        /// <param name="timeOut"></param>
        private static void HttpClientHeader(HttpClient client, string refererUrl, bool useAjax, int timeOut)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            client.DefaultRequestHeaders.Add("Timeout", timeOut.ToString());
            client.DefaultRequestHeaders.Add("KeepAlive", "true");

            if (!string.IsNullOrEmpty(refererUrl))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(refererUrl);
            }

            if (useAjax)
            {
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            }
        }

        #region 同步方法

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        public static void FillFormDataStream(this Dictionary<string, string> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            stream.Write(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }

        #endregion


        #region 异步方法

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        public static async Task FillFormDataStreamAsync(this Dictionary<string, string> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            await stream.WriteAsync(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }

        #endregion

        #region 只需要使用同步的方法

        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&amp;连接，首位没有符号，如：a=1&amp;b=2&amp;c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string html)
        {
            return WebUtility.HtmlEncode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string html)
        {
            return WebUtility.HtmlDecode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            return WebUtility.UrlEncode(url);//转义后字母为大写
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url)
        {
            return WebUtility.UrlDecode(url);
        }

        /// <summary>
        /// <para>将 URL 中的参数名称/值编码为合法的格式。</para>
        /// <para>可以解决类似这样的问题：假设参数名为 tvshow, 参数值为 Tom&Jerry，如果不编码，可能得到的网址： http://a.com/?tvshow=Tom&Jerry&year=1965 编码后则为：http://a.com/?tvshow=Tom%26Jerry&year=1965 </para>
        /// <para>实践中经常导致问题的字符有：'&', '?', '=' 等</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AsUrlData(this string data)
        {
            if (data == null)
            {
                return null;
            }
            return Uri.EscapeDataString(data);
        }
        #endregion

        #endregion

        #region Get

        /// <summary>
        /// .NET Core 版本的HttpWebRequest参数设置
        /// </summary>
        /// <returns></returns>
        private static HttpClient HttpGet_Common_NetCore(string url, CookieContainer cookieContainer = null,
            Encoding encoding = null, X509Certificate2 cer = null,
            string refererUrl = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookieContainer ?? new CookieContainer(),
                UseProxy = _webproxy != null,
                Proxy = _webproxy,
            };

            if (cer != null)
            {
                handler.ClientCertificates.Add(cer);
            }

            HttpClient httpClient = new HttpClient(handler);
            HttpClientHeader(httpClient, refererUrl, useAjax, timeOut);

            return httpClient;
        }

        #region 同步方法

        /// <summary>
        /// 使用Get方法获取字符串结果（没有加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url, Encoding encoding = null)
        {
            var handler = new HttpClientHandler
            {
                UseProxy = _webproxy != null,
                Proxy = _webproxy,
            };
            HttpClient httpClient = new HttpClient(handler);
            return httpClient.GetStringAsync(url).Result;
        }

        /// <summary>
        /// 使用Get方法获取字符串结果（加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="encoding"></param>
        /// <param name="cer">证书，如果不需要则保留null</param>
        /// <param name="refererUrl">referer参数</param>
        /// <param name="useAjax">是否使用Ajax</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static string HttpGet(string url, CookieContainer cookieContainer = null, Encoding encoding = null, X509Certificate2 cer = null,
            string refererUrl = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            var httpClient = HttpGet_Common_NetCore(url, cookieContainer, encoding, cer, refererUrl, useAjax, timeOut);
            return httpClient.GetStringAsync(url).Result;
        }

        /// <summary>
        /// 获取HttpWebResponse或HttpResponseMessage对象，本方法通常用于测试）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="encoding"></param>
        /// <param name="cer"></param>
        /// <param name="refererUrl"></param>
        /// <param name="useAjax">是否使用Ajax请求</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage HttpResponseGet(string url, CookieContainer cookieContainer = null, Encoding encoding = null, X509Certificate2 cer = null,
   string refererUrl = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            var httpClient = HttpGet_Common_NetCore(url, cookieContainer, encoding, cer, refererUrl, useAjax, timeOut);
            var task = httpClient.GetAsync(url);
            HttpResponseMessage response = task.Result;
            return response;
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// 使用Get方法获取字符串结果（没有加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url, Encoding encoding = null)
        {
            var handler = new HttpClientHandler
            {
                UseProxy = _webproxy != null,
                Proxy = _webproxy,
            };
            HttpClient httpClient = new HttpClient(handler);
            return await httpClient.GetStringAsync(url);
        }

        /// <summary>
        /// 使用Get方法获取字符串结果（加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="encoding"></param>
        /// <param name="cer">证书，如果不需要则保留null</param>
        /// <param name="timeOut"></param>
        /// <param name="refererUrl">referer参数</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url, CookieContainer cookieContainer = null, Encoding encoding = null, X509Certificate2 cer = null,
            string refererUrl = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            var httpClient = HttpGet_Common_NetCore(url, cookieContainer, encoding, cer, refererUrl, useAjax, timeOut);
            return await httpClient.GetStringAsync(url);
        }

        #endregion

        #endregion


        #region Post


        #region 静态公共方法

        /// <summary>
        /// 给.NET Core使用的HttpPost请求公共设置方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="hc"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="fileDictionary"></param>
        /// <param name="refererUrl"></param>
        /// <param name="encoding"></param>
        /// <param name="cer"></param>
        /// <param name="useAjax"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult"></param>
        /// <returns></returns>
        public static HttpClient HttpPost_Common_NetCore(string url, out HttpContent hc, CookieContainer cookieContainer = null,
            Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null,
            Encoding encoding = null, X509Certificate2 cer = null, bool useAjax = false, int timeOut = Config.TIME_OUT,
            bool checkValidationResult = false)
        {
            var handler = new HttpClientHandler()
            {
                UseProxy = _webproxy != null,
                Proxy = _webproxy,
                UseCookies = true,
                CookieContainer = cookieContainer,
            };

            if (checkValidationResult)
            {
                handler.ServerCertificateCustomValidationCallback = new Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>(CheckValidationResult);
            }

            if (cer != null)
            {
                handler.ClientCertificates.Add(cer);
            }

            HttpClient client = new HttpClient(handler);
            HttpClientHeader(client, refererUrl, useAjax, timeOut);


            #region 处理Form表单文件上传

            var formUploadFile = fileDictionary != null && fileDictionary.Count > 0;//是否用Form上传文件
            if (formUploadFile)
            {

                //通过表单上传文件
                string boundary = "----" + DateTime.Now.Ticks.ToString("x");

                var multipartFormDataContent = new MultipartFormDataContent(boundary);
                hc = multipartFormDataContent;

                foreach (var file in fileDictionary)
                {
                    try
                    {
                        var fileName = file.Value;
                        //准备文件流
                        using (var fileStream = FileHelper.GetFileStream(fileName))
                        {
                            if (fileStream != null)
                            {
                                //存在文件
                                var memoryStream = new MemoryStream();
                                fileStream.CopyTo(memoryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                //multipartFormDataContent.Add(new StreamContent(memoryStream), file.Key, Path.GetFileName(fileName)); //报流已关闭的异常

                                multipartFormDataContent.Add(CreateFileContent(memoryStream, file.Key, Path.GetFileName(fileName)), file.Key, Path.GetFileName(fileName));
                                fileStream.Dispose();
                            }
                            else
                            {
                                //不存在文件或只是注释
                                multipartFormDataContent.Add(new StringContent(string.Empty), file.Key, file.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                hc.Headers.ContentType = MediaTypeHeaderValue.Parse(string.Format("multipart/form-data; boundary={0}", boundary));
            }
            else
            {
                hc = new StreamContent(postStream);

                hc.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

                //使用Url格式Form表单Post提交的时候才使用application/x-www-form-urlencoded
                //去掉注释以测试Request.Body为空的情况
                //hc.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }

            //HttpContentHeader(hc, timeOut);
            #endregion

            if (!string.IsNullOrEmpty(refererUrl))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(refererUrl);
            }

            return client;
        }

        #endregion

        #region 同步方法

        /// <summary>
        /// 使用Post方法获取字符串结果，常规提交
        /// </summary>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, X509Certificate2 cer = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            MemoryStream ms = new MemoryStream();
            formData.FillFormDataStream(ms);//填充formData
            return HttpPost(url, cookieContainer, ms, null, null, encoding, cer, useAjax, timeOut);
        }

        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="fileDictionary">需要上传的文件，Key：对应要上传的Name，Value：本地文件名</param>
        /// <param name="encoding"></param>
        /// <param name="cer">证书，如果不需要则保留null</param>
        /// <param name="useAjax"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null, X509Certificate2 cer = null, bool useAjax = false, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
            if (cookieContainer == null)
            {
                cookieContainer = new CookieContainer();
            }

            var myResponse = HttpResponsePost(url, cookieContainer, postStream, fileDictionary, refererUrl, encoding, cer, useAjax, timeOut, checkValidationResult);
            var response = myResponse.Result;

            if (response.Content.Headers.ContentType.CharSet != null &&
                response.Content.Headers.ContentType.CharSet.ToLower().Contains("utf8"))
            {
                response.Content.Headers.ContentType.CharSet = "utf-8";
            }

            var retString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return retString;
        }


        /// <summary>
        /// 使用Post方法获取HttpWebResponse或HttpResponseMessage对象，本方法独立使用时通常用于测试）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="fileDictionary">需要上传的文件，Key：对应要上传的Name，Value：本地文件名</param>
        /// <param name="encoding"></param>
        /// <param name="cer">证书，如果不需要则保留null</param>
        /// <param name="useAjax"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <param name="refererUrl"></param>
        /// <returns></returns>
        public static WeiXinHttpResponse HttpResponsePost(string url, CookieContainer cookieContainer = null, Stream postStream = null,
            Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null,
            X509Certificate2 cer = null, bool useAjax = false, int timeOut = Config.TIME_OUT,
            bool checkValidationResult = false)
        {
            if (cookieContainer == null)
            {
                cookieContainer = new CookieContainer();
            }

            var postStreamIsDefaultNull = postStream == null;
            if (postStreamIsDefaultNull)
            {
                postStream = new MemoryStream();
            }

            HttpContent hc;
            var client = HttpPost_Common_NetCore(url, out hc, cookieContainer, postStream, fileDictionary, refererUrl, encoding, cer, useAjax, timeOut, checkValidationResult);

            var response = client.PostAsync(url, hc).GetAwaiter().GetResult();

            try
            {
                if (postStreamIsDefaultNull && postStream.Length > 0)
                {
                    postStream.Close();
                }

                hc.Dispose();//关闭HttpContent（StreamContent）
            }
            catch (WeixinException ex)
            {
            }

            return new WeiXinHttpResponse(response);
        }

        #endregion


        #region 异步方法

        /// <summary>
        /// 使用Post方法获取字符串结果，常规提交
        /// </summary>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, X509Certificate2 cer = null, bool useAjax = false, int timeOut = Config.TIME_OUT)
        {
            MemoryStream ms = new MemoryStream();
            await formData.FillFormDataStreamAsync(ms);//填充formData
            return await HttpPostAsync(url, cookieContainer, ms, null, null, encoding, cer, useAjax, timeOut);
        }


        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="fileDictionary">需要上传的文件，Key：对应要上传的Name，Value：本地文件名</param>
        /// <param name="cer"></param>
        /// <param name="useAjax"></param>
        /// <param name="timeOut"></param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <param name="refererUrl"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, CookieContainer cookieContainer = null, Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null, X509Certificate2 cer = null,
            bool useAjax = false, int timeOut = Config.TIME_OUT, bool checkValidationResult = false)
        {
            if (cookieContainer == null)
            {
                cookieContainer = new CookieContainer();
            }

            var postStreamIsDefaultNull = postStream == null;
            if (postStreamIsDefaultNull)
            {
                postStream = new MemoryStream();
            }

            HttpContent hc;
            var client = HttpPost_Common_NetCore(url, out hc, cookieContainer, postStream, fileDictionary, refererUrl, encoding, cer, useAjax, timeOut, checkValidationResult);

            var r = await client.PostAsync(url, hc);

            if (r.Content.Headers.ContentType.CharSet != null &&
                r.Content.Headers.ContentType.CharSet.ToLower().Contains("utf8"))
            {
                r.Content.Headers.ContentType.CharSet = "utf-8";
            }

            var retString = await r.Content.ReadAsStringAsync();

            try
            {
                if (postStreamIsDefaultNull && postStream.Length > 0)
                {
                    postStream.Close();
                }

                hc.Dispose();//关闭HttpContent（StreamContent）
            }
            catch (WeixinException ex)
            {
            }

            return retString;
        }


        #endregion


        #endregion

    }
}
