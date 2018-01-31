using System;
using System.IO;

namespace Configurator.Uploader
{
    public enum StorageType { Local, Azure, AzureImages, AzureBadgeImages, AzureImages260X190, BitsOnTheRun, AzureImages449x252, AzureImage605x370, AzureImagesHomeSlide1920x600, WebVttToDB }

    public class FilesStatus
    {
        public const string HandlerPath = "/Upload/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }
        public string fullPath { get; set; }
        public string duration { get; set; }

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo)
        {
            SetValues(fileInfo.Name, (int)fileInfo.Length, fileInfo.FullName);
        }

        public FilesStatus(string fileName, int fileLength, string fullPath, string duration = "", string thumbnailurl = "") { SetValues(fileName, fileLength, fullPath, duration, thumbnailurl); }

        private void SetValues(string fileName, int fileLength, string fPath, string videoDuration = "", string thumbnailurl = "")
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            fullPath = fPath;
            progress = "1.0";
            url = HandlerPath + "UploadHandler.ashx?f=" + fileName;
            delete_url = HandlerPath + "UploadHandler.ashx?f=" + fileName;
            delete_type = "DELETE";
            duration = videoDuration;
            thumbnail_url = thumbnailurl;
            //var ext = Path.GetExtension(fullPath);

            //var fileSize = ConvertBytesToMegabytes(new FileInfo(fullPath).Length);
            //if (fileSize > 3 || !IsImage(ext)) thumbnail_url = "/Content/img/generalFile.png";
            //else thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath);
        }



        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}