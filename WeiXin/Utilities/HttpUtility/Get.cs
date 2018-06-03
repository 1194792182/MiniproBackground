using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeiXin.Entities.JsonResult;
using WeiXin.Exceptions;

namespace WeiXin.Utilities.HttpUtility
{
    /// <summary>
    /// Get 请求处理
    /// </summary>
    public static class Get
    {
        /// <summary>
        /// 获取随机文件名
        /// </summary>
        /// <returns></returns>
        private static string GetRandomFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);
        }

        #region 同步方法

        /// <summary>
        /// GET方式请求URL，并返回T类型
        /// </summary>
        /// <typeparam name="T">接收JSON的数据类型</typeparam>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="maxJsonLength">允许最大JSON长度</param>
        /// <returns></returns>
        public static T GetJson<T>(string url, Encoding encoding = null, int? maxJsonLength = null)
        {
            string returnText = RequestUtility.HttpGet(url, encoding);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误

                WxJsonResult errorResult =
                                    Newtonsoft.Json.JsonConvert.DeserializeObject<WxJsonResult>(returnText);

                if (errorResult.errcode != ReturnCode.请求成功)
                {
                    //发生错误
                    throw new ErrorJsonResultException(
                        string.Format("微信请求发生错误！错误代码：{0}，说明：{1}",
                                        (int)errorResult.errcode, errorResult.errmsg), null, errorResult, url);
                }
            }
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(returnText);
            return result;
        }

        /// <summary>
        /// 从Url下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        public static void Download(string url, Stream stream)
        {
            var httpClient = new HttpClient();
            var t = httpClient.GetByteArrayAsync(url);
            t.Wait();
            var data = t.Result;
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 从Url下载，并保存到指定目录
        /// </summary>
        /// <param name="url">需要下载文件的Url</param>
        /// <param name="filePathName">保存文件的路径，如果下载文件包含文件名，按照文件名储存，否则将分配Ticks随机文件名</param>
        /// <returns></returns>
        public static string Download(string url, string filePathName, int timeOut = 999)
        {
            var dir = Path.GetDirectoryName(filePathName) ?? "/";
            Directory.CreateDirectory(dir);

            System.Net.Http.HttpClient httpClient = new HttpClient();
            using (var responseMessage = httpClient.GetAsync(url).Result)
            {
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseFileName = null;
                    //ContentDisposition可能会为Null
                    if (responseMessage.Content.Headers.ContentDisposition != null &&
                        responseMessage.Content.Headers.ContentDisposition.FileName != null &&
                        responseMessage.Content.Headers.ContentDisposition.FileName != "\"\"")
                    {
                        responseFileName = Path.Combine(dir, responseMessage.Content.Headers.ContentDisposition.FileName.Trim('"'));
                    }

                    var fullName = responseFileName ?? Path.Combine(dir, GetRandomFileName());
                    using (var fs = File.Open(fullName, FileMode.Create))
                    {
                        using (var responseStream = responseMessage.Content.ReadAsStreamAsync().Result)
                        {
                            responseStream.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    return fullName;

                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region 异步方法

        /// <summary>
        /// 【异步方法】异步GetJson
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="maxJsonLength">允许最大JSON长度</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ErrorJsonResultException"></exception>
        public static async Task<T> GetJsonAsync<T>(string url, Encoding encoding = null, int? maxJsonLength = null)
        {
            string returnText = await RequestUtility.HttpGetAsync(url, encoding);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误

                WxJsonResult errorResult =
                                   Newtonsoft.Json.JsonConvert.DeserializeObject<WxJsonResult>(returnText);

                if (errorResult.errcode != ReturnCode.请求成功)
                {
                    //发生错误
                    throw new ErrorJsonResultException(
                        string.Format("微信请求发生错误！错误代码：{0}，说明：{1}",
                                        (int)errorResult.errcode, errorResult.errmsg), null, errorResult, url);
                }
            }

            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(returnText);
            return result;
        }

        /// <summary>
        /// 【异步方法】异步从Url下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task DownloadAsync(string url, Stream stream)
        {
            HttpClient httpClient = new HttpClient();
            var data = await httpClient.GetByteArrayAsync(url);
            await stream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// 【异步方法】从Url下载，并保存到指定目录
        /// </summary>
        /// <param name="url">需要下载文件的Url</param>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public static async Task<string> DownloadAsync(string url, string filePathName)
        {
            var dir = Path.GetDirectoryName(filePathName) ?? "/";
            Directory.CreateDirectory(dir);

            var httpClient = new HttpClient();
            using (var responseMessage = await httpClient.GetAsync(url))
            {
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string responseFileName = null;
                    //ContentDisposition可能会为Null
                    if (responseMessage.Content.Headers.ContentDisposition != null &&
                        responseMessage.Content.Headers.ContentDisposition.FileName != null &&
                        responseMessage.Content.Headers.ContentDisposition.FileName != "\"\"")
                    {
                        responseFileName = Path.Combine(dir, responseMessage.Content.Headers.ContentDisposition.FileName.Trim('"'));
                    }

                    var fullName = responseFileName ?? Path.Combine(dir, GetRandomFileName());
                    using (var fs = File.Open(fullName, FileMode.Create))
                    {
                        using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
                        {
                            await responseStream.CopyToAsync(fs);
                            await fs.FlushAsync();
                        }
                    }
                    return fullName;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

    }
}
