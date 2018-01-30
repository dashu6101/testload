using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace CCC.Models
{
    public class Comman
    {

        public string UploadDocumentToAzure(Stream stream, string documentName, BlobStorage storage)
        {
            try
            {


                //StorageCredentialsAccountAndKey accountAndKey = new StorageCredentialsAccountAndKey(storage.AccountName, storage.AccountKey);
                //var azureStorage = new CloudStorageAccount(accountAndKey, false);
                //var blobClient = azureStorage.CreateCloudBlobClient();
                //var container = blobClient.GetContainerReference(storage.ContainerName);
                //container.CreateIfNotExist();
                //var blob = container.GetBlobReference(documentName);
                //if (!string.IsNullOrEmpty(storage.ContentType))
                //{
                //    blob.Properties.ContentType = storage.ContentType;
                //}

                //blob.UploadFromStream(stream, new BlobRequestOptions() { Timeout = new TimeSpan(0, 0, 600) });
                //blob.SetProperties();

                //return blob.Uri.ToString();


                StorageCredentials acckey = new StorageCredentials(storage.AccountName, storage.AccountKey);
                var azurestorage = new CloudStorageAccount(acckey, false);
                var blobClient = azurestorage.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(storage.ContainerName);
                container.CreateIfNotExists();

                var blob = container.GetBlockBlobReference(documentName);
                if (!string.IsNullOrEmpty(storage.ContentType))
                {
                    blob.Properties.ContentType = storage.ContentType;
                }

                BlobRequestOptions bro = new BlobRequestOptions() { ServerTimeout = TimeSpan.FromSeconds(5) };

                blob.UploadFromStream(stream,null, bro);
                blob.SetProperties();


                //blob.UploadFromStream(stream, new BlobRequestOptions() { Timeout = new TimeSpan(0, 0, 600) });
                //blob.SetProperties();

                return blob.Uri.ToString();


            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }
    }

    public class BlobStorage
    {
        public string ContainerUrl { get; set; }
        public string ContainerName { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string ContentType { get; set; }
    }
    public class LTVResult
    {
        public string Url { get; set; }
        public string Duration { get; set; }
        public string ThumbNailUrl { get; set; }
        public NameValueCollection COl { get; set; }
        public string Tempfilepath { get; set; }
    }
    public static class FileSizeUnits
    {
        public const ulong Kb = 1024;
        public const ulong Mb = 1024 * Kb;
        public const ulong HomeVideoMb = 100 * Kb * Kb;
        public const ulong Gb = 1024 * Mb;
        public const ulong Tb = 1024 * Gb;
    }
    public class UploadHandler
    {
        public string URL { get; set; }
        public NameValueCollection COL { get; set; }
        public string FILEPATH { get; set; }
        public int SESSIONID { get; set; }
        public int CONFERENCEID { get; set; }
        public string USEREMAIL { get; set; }
        public string CLASSNAME { get; set; }
        public long PAGEID { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISHOMEVIDEO { get; set; }
        public int CONFMEDIAID { get; set; }
    }
    public class PresenterContentLinkViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string CategoryPresenterName { get; set; }
        public string ImageName { get; set; }
        public string VideoName { get; set; }
        public string VideoFrom { get; set; }
        public string ChannelName { get; set; }
        public string ChannelNo { get; set; }
        public string CategoryName { get; set; }
        public bool IsChannelAvailable { get; set; }

        public virtual LoginModel LoginModel { get; set; }

        //public virtual Category Category { get; set; }
    }
    public class CategoryCommentDetailUsers
    {
        public int CommentID { get; set; }
        public string Comment { get; set; }
        public int? ContentID { get; set; }
        public string ContentName { get; set; }
        public DateTime? CommentDate { get; set; }
        public string UserType { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ChannelNo { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
    }
}