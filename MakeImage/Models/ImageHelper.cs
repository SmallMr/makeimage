﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ImageHelper
{
    #region 缩略图

    ///<summary>  
    /// 生成缩略图   
    /// </summary>   
    /// <param name="stream">原图stream</param>   
    /// <param name="thumbnailPath">缩略图路径</param>   
    /// <param name="width">缩略图宽度</param>   
    /// <param name="height">缩略图高度</param>   
    /// <param name="thumbnailCutModel">生成缩略图的方式</param>　　   
    /// <param name="imageType">要缩略图保存的格式(gif,jpg,bmp,png) 为空或未知类型都视为jpg</param>　　   
    public void CreateThumbnail(Stream stream, string thumbnailPath, int width, int height, ThumbnailCutModel thumbnailCutModel, string imageType)
    {
        Image originalImage = Image.FromStream(stream);
        CreateThumbnail(originalImage, thumbnailPath, width, height, thumbnailCutModel, imageType);
    }

    ///<summary>  
    /// 生成缩略图   
    /// </summary>   
    /// <param name="originalImagePath">源图路径</param>   
    /// <param name="thumbnailPath">缩略图路径</param>   
    /// <param name="width">缩略图宽度</param>   
    /// <param name="height">缩略图高度</param>   
    /// <param name="thumbnailCutModel">生成缩略图的方式</param>　　   
    /// <param name="imageType">要缩略图保存的格式(gif,jpg,bmp,png) 为空或未知类型都视为jpg</param>　　   
    public void CreateThumbnail(string originalImagePath, string thumbnailPath, int width, int height, ThumbnailCutModel thumbnailCutModel, string imageType)
    {
        Image originalImage = Image.FromFile(originalImagePath);
        CreateThumbnail(originalImage, thumbnailPath, width, height, thumbnailCutModel, imageType);
    }

    ///<summary>  
    /// 生成缩略图   
    /// </summary>   
    /// <param name="originalImagePath">源图路径</param>   
    /// <param name="thumbnailPath">缩略图路径</param>   
    /// <param name="width">缩略图宽度</param>   
    /// <param name="height">缩略图高度</param>   
    /// <param name="thumbnailCutModel">生成缩略图的方式</param>　　   
    /// <param name="imageType">要缩略图保存的格式(gif,jpg,bmp,png) 为空或未知类型都视为jpg</param>　　   
    private void CreateThumbnail(Image originalImage, string thumbnailPath, int width, int height, ThumbnailCutModel thumbnailCutModel, string imageType)
    {
        int towidth = width;
        int toheight = height;
        int x = 0;
        int y = 0;
        int ow = originalImage.Width;
        int oh = originalImage.Height;
        switch (thumbnailCutModel)
        {
            case ThumbnailCutModel.WidthAndHeight://指定高宽缩放（可能变形）　　　　　　　　   
                break;
            case ThumbnailCutModel.Width://指定宽，高按比例　　　　　　　　　　   
                toheight = originalImage.Height * width / originalImage.Width;
                break;
            case ThumbnailCutModel.Height://指定高，宽按比例   
                towidth = originalImage.Width * height / originalImage.Height;
                break;
            case ThumbnailCutModel.Cut://指定高宽裁减（不变形）　　　　　　　　   
                if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                {
                    oh = originalImage.Height;
                    ow = originalImage.Height * towidth / toheight;
                    y = 0;
                    x = (originalImage.Width - ow) / 2;
                }
                else
                {
                    ow = originalImage.Width;
                    oh = originalImage.Width * height / towidth;
                    x = 0;
                    y = (originalImage.Height - oh) / 2;
                }
                break;
            default:
                break;
        }
        //新建一个bmp图片   
        System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
        //新建一个画板   
        Graphics g = System.Drawing.Graphics.FromImage(bitmap);
        //设置高质量插值法   
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //设置高质量,低速度呈现平滑程度   
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //清空画布并以透明背景色填充   
        g.Clear(Color.Transparent);
        //在指定位置并且按指定大小绘制原图片的指定部分   
        g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
           new Rectangle(x, y, ow, oh),
           GraphicsUnit.Pixel);
        try
        {
            //以jpg格式保存缩略图   
            switch (imageType.ToLower())
            {
                case "gif":
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "jpg":
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "bmp":
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case "png":
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                default:
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            originalImage.Dispose();
            bitmap.Dispose();
            g.Dispose();
        }
    }


    public MemoryStream CreateThumbnailGetStream(Image originalImage, int width, int height, ThumbnailCutModel thumbnailCutModel, string imageType)
    {
        MemoryStream ms = new MemoryStream();
        int towidth = width;
        int toheight = height;
        int x = 0;
        int y = 0;
        int ow = originalImage.Width;
        int oh = originalImage.Height;
        switch (thumbnailCutModel)
        {
            case ThumbnailCutModel.WidthAndHeight://指定高宽缩放（可能变形）　　　　　　　　   
                break;
            case ThumbnailCutModel.Width://指定宽，高按比例　　　　　　　　　　   
                toheight = originalImage.Height * width / originalImage.Width;
                break;
            case ThumbnailCutModel.Height://指定高，宽按比例   
                towidth = originalImage.Width * height / originalImage.Height;
                break;
            case ThumbnailCutModel.Cut://指定高宽裁减（不变形）　　　　　　　　   
                if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                {
                    oh = originalImage.Height;
                    ow = originalImage.Height * towidth / toheight;
                    y = 0;
                    x = (originalImage.Width - ow) / 2;
                }
                else
                {
                    ow = originalImage.Width;
                    oh = originalImage.Width * height / towidth;
                    x = 0;
                    y = (originalImage.Height - oh) / 2;
                }
                break;
            default:
                break;
        }
        //新建一个bmp图片   
        System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
        //新建一个画板   
        Graphics g = System.Drawing.Graphics.FromImage(bitmap);
        //设置高质量插值法   
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //设置高质量,低速度呈现平滑程度   
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //清空画布并以透明背景色填充   
        g.Clear(Color.Transparent);
        //在指定位置并且按指定大小绘制原图片的指定部分   
        g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
           new Rectangle(x, y, ow, oh),
           GraphicsUnit.Pixel);
        try
        {
            //以jpg格式保存缩略图   
            switch (imageType.ToLower())
            {
                case "gif":
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "jpg":
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "bmp":
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case "png":
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                default:
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            originalImage.Dispose();
            bitmap.Dispose();
            g.Dispose();
        }
        return ms;
    }
    #endregion
}

public enum ThumbnailCutModel
{
    /// <summary>
    /// 指定宽，高按比例
    /// </summary>
    Width,
    /// <summary>
    /// /指定高，宽按比例
    /// </summary>
    Height,
    /// <summary>
    /// 指定高宽缩放（可能变形）
    /// </summary>
    WidthAndHeight,
    /// <summary>
    /// 指定高宽裁减（不变形）
    /// </summary>
    Cut
}

