using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class FileUploadController : Controller
    {

        public String UploadFile(HttpPostedFileBase file, String FolderName)
        {
            String FilePath = null;
            String FileName = null;
            if (file != null && file.ContentLength > 0)
            {
                FileName = Guid.NewGuid() + ".jpg";

                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + "travellinekashmir.com/Resources/uploadedimages/" + FolderName + "/" + FileName));
                reqFTP.Credentials = new NetworkCredential("TMSS_fTP", "Tlk#fTp^123#");
                reqFTP.KeepAlive = true;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                byte[] data = getMediaBytes(file);
                Stream strm = reqFTP.GetRequestStream();
                BinaryWriter bwObj = new BinaryWriter(strm);
                bwObj.Write(data, 0, data.Length);
                bwObj.Flush();
                bwObj.Close();
                strm.Close();
                FilePath = "https://travellinekashmir.com/Resources/uploadedimages/" + FolderName + "/" + FileName;

            }
            return FilePath;
        }

        #region UploadFile
        /// <summary>
        /// Upload a file using HTTPPostedFileBase
        /// </summary>
        /// <param name="file">HttpPostedFileBase object of the file to be uploaded</param>
        /// <param name="FolderName">The name of the folder where file to stored</param>
        /// <returns></returns>
        public String[] UploadFileWithThumb(HttpPostedFileBase file, String FolderName)
        {
            String FilePath = null;
            String ThumbnailPath = null;
            String[] Paths = new string[2];
            String FileName = null;
            if (file != null && file.ContentLength > 0)
            {
                FileName = Guid.NewGuid() + ".jpg";

                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + "travellinekashmir.com/Resources/uploadedimages/" + FolderName + "/" + FileName));
                reqFTP.Credentials = new NetworkCredential("TMSS_fTP", "Tlk#fTp^123#");
                reqFTP.KeepAlive = true;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                byte[] data = getMediaBytes(file);
                Stream strm = reqFTP.GetRequestStream();
                BinaryWriter bwObj = new BinaryWriter(strm);
                bwObj.Write(data, 0, data.Length);
                bwObj.Flush();
                bwObj.Close();
                strm.Close();
                FilePath = "https://travellinekashmir.com/Resources/uploadedimages/" + FolderName + "/" + FileName;
                ThumbnailPath = UploadThm(file, FolderName);

            }
            Paths[0] = FilePath;
            Paths[1] = ThumbnailPath;
            return Paths;
        }
        #endregion

        #region ConvertMediaBytes
        public byte[] getMediaBytes(HttpPostedFileBase FileName)
        {
            byte[] rawdata = new byte[FileName.ContentLength];
            FileName.InputStream.Read(rawdata, 0, FileName.ContentLength);

            return rawdata;
        }
        #endregion

        #region DeleteFile
        /// <summary>
        /// Delete Image using ftp protocol
        /// </summary>
        /// <param name="ImagePath">The path of the image to be deleted</param>
        public void DeleteFile(String ImagePath)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + "travellinekashmir.com/Resources/" + ImagePath.Substring(ImagePath.IndexOf("uploadedimages")));
            request.Credentials = new NetworkCredential("TMSS_fTP", "Tlk#fTp^123#");
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        }
        #endregion

        #region DeleteMultiple
        /// <summary>
        /// Delete Image using ftp protocol
        /// </summary>
        /// <param name="ImagePath">The path of the image to be deleted</param>
        public void DeleteMultipleFiles(String[] ImagePaths)
        {
            for (int i = 0; i < ImagePaths.Length; i++)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + "travellinekashmir.com/Resources/" + ImagePaths[i].Substring(ImagePaths[i].IndexOf("uploadedimages")));
                request.Credentials = new NetworkCredential("TMSS_fTP", "Tlk#fTp^123#");
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
        }
        #endregion

        #region UploadThm
        public string UploadThm(HttpPostedFileBase File, String Folder)
        {
            Image ResizedImage = ResizeImage(File, 350, 200);
            byte[] ReducedBytes = ReduceImage(ResizedImage);
            String ThumbnailPath = UploadThumbnail(ReducedBytes, Folder);
            return ThumbnailPath;
        }
        #endregion

        #region ResizeImage
        public static Bitmap ResizeImage(HttpPostedFileBase File, int width, int height)
        {
            Image image = Image.FromStream(File.InputStream);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        #endregion

        #region ReduceQuality
        public byte[] ReduceImage(Image Img)
        {
            MemoryStream ms = new MemoryStream();
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 50L);
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            Img.Save(ms, jpegCodec, encoderParams);
            return ms.ToArray();

        }
        #endregion

        #region GetEncoderInfo
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
        #endregion

        #region UploadThumbnail
        public String UploadThumbnail(byte[] data, String Folder)
        {
            String FileName = "Thm_" + Guid.NewGuid() + ".jpg";

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + "travellinekashmir.com/Resources/uploadedimages/" + Folder + "/Thumbnails/" + FileName));
            reqFTP.Credentials = new NetworkCredential("TMSS_fTP", "Tlk#fTp^123#");
            reqFTP.KeepAlive = true;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            Stream strm = reqFTP.GetRequestStream();
            BinaryWriter bwObj = new BinaryWriter(strm);
            bwObj.Write(data, 0, data.Length);
            bwObj.Flush();
            bwObj.Close();
            strm.Close();
            String FilePath = "https://travellinekashmir.com/Resources/uploadedimages/" + Folder + "/Thumbnails/" + FileName;
            return FilePath;
        }
        #endregion
    }
}
