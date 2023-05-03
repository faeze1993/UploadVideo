using AsynVideoStreaming.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AsynVideoStreaming.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/video")]
    [ApiController]
    public class VideoController : ApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VideoController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        [Microsoft.AspNetCore.Mvc.HttpGet("{ext}/{fileName}")]
        public HttpResponseMessage Get(string ext, string fileName)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string videoPath = string.Format("~/Videos/{0}.{1}", fileName, ext);
            videoPath = "D:\\AsynVideoStreaming\\AsynVideoStreaming\\Videos\\sample.mp4";//Path.Combine(webRootPath, videoPath);

            if (File.Exists(videoPath))
            {
                FileInfo fi = new FileInfo(videoPath);
                var video = new VideoStream(videoPath);

                var response = Request.CreateResponse();

                response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)video.WriteToStream,
                new MediaTypeHeaderValue("video/" + ext));

                response.Content.Headers.Add("Content-Disposition", "attachment;filename=" + fi.Name.Replace(" ", ""));
                response.Content.Headers.Add("Content-Length", video.FileLength.ToString());// در صورتی که این Header‌‌ها را تعریف نکنید سایز فایل دریافتی و مدت زمان آن نامعلوم خواهد بود که تجربه کاربری خوبی بدست نمی‌دهد.

                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
