using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace MakeImage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("Hello");
        }

        [HttpPost]
        public string DrawImage()
        {
            object result;
            try
            {
                HttpPostedFile file = System.Web.HttpContext.Current.Request.Files["file"];
                string name = System.Web.HttpContext.Current.Request.Form["name"];
                string zhufu = System.Web.HttpContext.Current.Request.Form["zhufu"];
                Stream imageStream = file.InputStream;
                Image sel = Image.FromStream(imageStream);

                Image image = Image.FromFile(Request.MapPath("~/image/muban.png"));
                Bitmap bitmap = CombinImage(sel, image, name, zhufu, 0, 0);

                string fileName = GetFileName();
                bitmap.Save(Request.MapPath(fileName));

                string host = Request.Url.Host;
                int port = Request.Url.Port;
                string imageUrl = $"http://{host}:{port}/{fileName.Substring(2)}";
                result = new { msg = imageUrl, isOk = true };
            }
            catch (Exception ex)
            {
                result = new { msg = ex.Message.ToString(), isOk = false };
            }

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="imgBack"></param>
        /// <param name="muban"></param>
        /// <param name="xDeviation"></param>
        /// <param name="yDeviation"></param>
        /// <returns></returns>
        private Bitmap CombinImage(Image backImage, Image muban, string name, string zhufu, int xDeviation = 0, int yDeviation = 0)
        {
            name = string.IsNullOrWhiteSpace(name) ? "你好" : "我是" + name;
            zhufu = string.IsNullOrWhiteSpace(zhufu) ? "祝新春快乐" : "祝" + zhufu;
            string str = $"{name}\n{zhufu}";

            Bitmap bmp = new Bitmap(muban.Width, muban.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.Clear(Color.White);

            //压缩
            ImageHelper imageHelper = new ImageHelper();
            Image imgBack = Image.FromStream(imageHelper.CreateThumbnailGetStream(backImage, 750, 680, ThumbnailCutModel.Cut, ""));

            int start = 0;
            int width = muban.Width;
            int height = imgBack.Height;

            g.DrawImage(imgBack, start, 0, width, height);
            g.DrawImage(muban, 0, 0, muban.Width, muban.Height);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center; //居中
            format.LineAlignment = StringAlignment.Center;
            Rectangle rectangle = new Rectangle(85, 720, 663 - 85, 924 - 720);

            Font font = new Font("宋体", 35, FontStyle.Bold);
            SolidBrush sbrush = new SolidBrush(Color.FromArgb(245, 196, 152));
            g.DrawString(str, font, sbrush, rectangle, format);
            //GC.Collect();
            return bmp;
        }

        private string GetFileName()
        {
            string virtualPath = "~/image/Maked/" + DateTime.Now.ToString("yyyy-MM-dd-HH");
            string path = Request.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = Guid.NewGuid().ToString() + ".jpg";
            return virtualPath + "/" + name;
        }
    }
}
