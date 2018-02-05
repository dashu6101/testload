using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliverLoad.Models;
using DeliverLoad.Services;
using Configurator.Uploader;
using System.Threading;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using System.Diagnostics;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using DeliverLoad.Mvc.Mailers;
using System.Net.Mail;
using Mvc.Mailer;
using System.Threading.Tasks;
using System.Net;

namespace DeliverLoad.Controllers
{


    [Authorize]
    public class VehicleownerController : BaseController
    {
        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }



        private readonly string AzureMediaFolder = ConfigurationManager.AppSettings["MediaFolder"];
        private readonly JavaScriptSerializer js;

        public IJob job { get; set; }
        public CloudMediaContext mediaContext { get; set; }
        public IAsset asset { get; set; }
        public int ExerciseId { get; set; }

        public VehicleownerController()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = (int)(FileSizeUnits.Gb + (300 * FileSizeUnits.Mb));
        }

        private string StorageRoot
        {
            get
            {

                string path = System.Web.HttpContext.Current.Server.MapPath("~/Temp/"); //Path should! always end with '/'
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        //
        // GET: /Category/
        private DeliverLoadService service = new DeliverLoadService();


        public ActionResult Index()
        {
            var model = service.getVehicleLoadCategoryList(sUser.UserId);
            return View(model);
        }

        public ActionResult Create()
        {

            if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            {
                ViewBag.URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                ViewBag.URL = "https://www.paypal.com/cgi-bin/webscr";
            }

            if (ConfigurationManager.AppSettings["SendToReturnURL"].ToString() == "true")
            {
                ViewBag.rm = "2";
            }
            else
            {
                ViewBag.rm = "1";
            }
            CategoryModel model = new CategoryModel();
            ViewBag.UserIdAuthenticate = service.UserIsAuthenticated(sUser.UserId);
            model.LoadSpaceList = service.getLoadSpaceList();
            ViewBag.Title = "Create Channel";
            return View(model);
        }

        [HttpGet]
        public ActionResult CategoryDetails(string id)
        {

            //throw new Exception("channel not found");

            UserModel UModel = null;
            if (id != null)
            {
                ViewBag.CategoryId = id;
            }
            else
            {
                ViewBag.CategoryId = Session["CategoryId"];
            }
            int Categoryid = Convert.ToInt32(ViewBag.CategoryId);
            var model = service.GetTreeVeiwList(Categoryid);
            ViewBag.ChannelName = service.getCategoryDetails(Categoryid, sUser.UserId).Name;

            UModel = service.GetUserDetails(User.Identity.Name);
            ViewBag.ChannelNo = service.getCategoryDetails(Categoryid, sUser.UserId).ChannelNo;
            ViewBag.UserName = UModel.ScreenName + "." + UModel.ChannelNo;
            return View(model);
        }


        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditCategory(CategoryModel model)
        {
            if (model.CategoryId == 0)
            {
                //create category
                string returnvalue = service.CreateCategory(model, sUser.UserId, sUser.ChannelNo);

            }
            else
            {
                //update category
                string returnvalue = service.EditCategoryOwner(model);

                Session["CategoryId"] = model.CategoryId;

            }


            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult getCategoryDetail(int CategoryId)
        {

            if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            {
                ViewBag.URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                ViewBag.URL = "https://www.paypal.com/cgi-bin/webscr";
            }

            if (ConfigurationManager.AppSettings["SendToReturnURL"].ToString() == "true")
            {
                ViewBag.rm = "2";
            }
            else
            {
                ViewBag.rm = "1";
            }

            var model = service.getOwnerCategoryDetails(CategoryId, sUser.UserId);
            model.LoadSpaceList = service.getLoadSpaceList();
            ViewBag.UserIdAuthenticate = service.UserIsAuthenticated(sUser.UserId);
            ViewBag.Title = "Edit Channel";
            return PartialView("_CreateOrEditCategory", model);
        }

        [HttpGet]
        public ActionResult _CreateOrEditCategory(int id)
        {


            var model = service.getCategoryDetails(Convert.ToInt32(id), sUser.UserId);
            ViewBag.Title = "Edit Channel";
            return PartialView(model);
        }


        //[ValidateInput(false)]
        //public ActionResult CreateContent(CategoryModel model)
        //{

        //    return RedirectToAction("CategoryDetails");
        //}


        public ActionResult GetParticipantList(int CategoryId)
        {
            var model = service.getParticipantsList(CategoryId);
            return PartialView("_ParticipantList", model);
        }


        public ActionResult PresenterDetails(string ChannelNo)
        {

            ViewBag.CNo = ChannelNo;

            var No = ChannelNo.Split('.');

            var model = service.GetUserDetailsByChanneNo(No[0].ToString());
            // var model = service.getCategoryListByChannel(Convert.ToInt32(id));
            return View(model);
        }


        public ActionResult ChannelList(string ChannelNo)
        {

            var model = service.getCategoryListByChannelNo(ChannelNo);
            return View(model);
        }


        public ActionResult GetChannelByChannelNo(string ChannelNo)
        {

            var model = service.getChannelDetailsByChannelNo(ChannelNo);


            //display selected channel list.
            if (model.Any())
            {
                ViewBag.IsChannelSelected = "1";
                return View("ChannelList", model);

            }

            //display all channels if no channel selected.
            var allcategoryList = service.getAllCategoryList();
            ViewBag.IsChannelSelected = "0";
            return View("ChannelList", allcategoryList);
        }

        public ActionResult GetChannelByName(string Name)
        {

            var model = service.getChannelDetailsByName(Name);

            //display selected channel list.
            if (model.Any())
            {
                ViewBag.IsChannelSelected = "1";
                return View("ChannelList", model);

            }

            //display all channels if no channel selected.
            var allcategoryList = service.getAllCategoryList();
            ViewBag.IsChannelSelected = "0";
            return View("ChannelList", allcategoryList);

        }


        public ActionResult Subscriber()
        {
            var model = service.getSubscriberList();
            return View("SubscriberList", model);

        }
        public ActionResult UserList()
        {
            var model = service.GetUsersList();
            return View("UserList", model);
        }

        [HttpPost]
        public JsonResult DeleteParticepent(int CategoryId, int UserId)
        {
            try
            {
                string ReturnValue = "";
                ReturnValue = service.DeleteCategoryParticepentById(CategoryId, UserId);
                if (ReturnValue == "1")
                {
                    RedirectToAction("GetParticipantList", new { CategoryId });
                    return Json("1", JsonRequestBehavior.AllowGet);

                }
                else if (ReturnValue == "-1")
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ReturnValue, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult BlockParticepent(int CategoryId, int UserId, bool status)
        {
            string ReturnValue = "";
            ReturnValue = service.BlockCategoryParticepentById(CategoryId, UserId, status);

            try
            {

                if (ReturnValue == "1")
                {
                    RedirectToAction("GetParticipantList", new { CategoryId });
                    return Json("1", JsonRequestBehavior.AllowGet);

                }
                else if (ReturnValue == "-1")
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ReturnValue, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            //return RedirectToAction("GetParticipantList", new { CategoryId });
        }
        [HttpPost]
        public ActionResult DeleteCommentPresenter(int UserId, int CommentId)
        {
            string ReturnValue = "";
            ReturnValue = service.DeleteCommnet(UserId, CommentId);
            try
            {
                return Json(ReturnValue, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Status = "500";
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IsAvailableCategory(int ChannelId, bool Status)
        {
            string ReturnValue = "";
            ReturnValue = service.CategoryAvailability(ChannelId, Status);
            try
            {
                return Json(ReturnValue, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Status = "500";
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult DeleteUser(int UserId)
        {
            string ReturnValue = "";
            ReturnValue = service.DeleteUser(UserId);
            try
            {
                return Json(ReturnValue, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Status = "500";
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult BlockUser(int UserId, bool Status)
        {
            string ReturnValue = "";
            ReturnValue = service.BlockUser(UserId, Status);
            try
            {
                if (ReturnValue == "1")
                {
                    RedirectToAction("UserList");
                    return Json("1", JsonRequestBehavior.AllowGet);

                }
                else if (ReturnValue == "-1")
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ReturnValue, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult RightAndContentPolicy()
        {
            return View();
        }

        #region

        // Upload file to the server
        public void UploadFile()
        {
            try
            {
                var context = System.Web.HttpContext.Current;

                var statuses = new List<FilesStatus>();
                var headers = context.Request.Headers;

                StorageType storageType = (StorageType)Enum.Parse(typeof(StorageType), context.Request["storagetype"]);

                int ContentID = Convert.ToInt32(context.Request["ContentId"]);


                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(context, statuses, storageType, ContentID);
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], context, statuses, storageType);
                }

                WriteJsonIframeSafe(context, statuses);


            }
            catch (Exception ex)
            {

            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses, StorageType storagetype)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);
            string fullPath = string.Empty;

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }



                fs.Flush();
                fs.Close();
            }

            if (storagetype == StorageType.Azure /*&& maybe need to check file completeness after upload to azure*/ )
            {
                if (ConfigurationManager.AppSettings["AccountName"] != null || ConfigurationManager.AppSettings["AccountKey"] != null)
                {
                    using (var fs = new FileStream(fullName, FileMode.Open, FileAccess.Read))
                    {
                        fullPath = UploadToAzure(context, fs, fileName, AzureMediaFolder, 0, "");
                    }
                }
            }

            statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }

        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses, StorageType storagetype, int NodeId)
        {
            try
            {
                //var common = new Configurator.Utility.Common();
                for (int i = 0; i < context.Request.Files.Count; i++)
                {
                    var file = context.Request.Files[i];

                    string fullPath = string.Empty;
                    string duration = string.Empty;
                    string thumbNailUrl = string.Empty;
                    switch (storagetype)
                    {

                        case StorageType.Azure:
                            fullPath = UploadToAzure(context, context.Request.Files[i].InputStream, context.Request.Files[i].FileName, AzureMediaFolder, NodeId, "");
                            break;

                    }




                    statuses.Add(new FilesStatus(Path.GetFileName(file.FileName), file.ContentLength, fullPath, duration, thumbNailUrl));
                }
            }
            catch (Exception ex)
            {


            }

        }


        private string UploadToAzure(HttpContext context, Stream stream, string fullName, string containerName, int nodeid, string categoryname)
        {



            //if (ConfigurationManager.AppSettings["AccountName"] == null || ConfigurationManager.AppSettings["AccountKey"] == null)
            //{
            //    return "AccountName or AccountKey are missing";
            //}

            ////BlobStorage blobStorage = DatabaseModel.GetStorageAccountDetail(Convert.ToInt64(context.Session[Const.ConferenceId]));
            //BlobStorage blobStorage = new BlobStorage();
            //blobStorage.AccountName = ConfigurationManager.AppSettings["AccountName"];
            //blobStorage.AccountKey = ConfigurationManager.AppSettings["AccountKey"];
            //blobStorage.ContainerName = containerName;
            //var configuratorType = ConfigurationManager.AppSettings["ConfiguratorType"];
            //var guidFileName = Guid.NewGuid() + Path.GetExtension(fullName);
            //var filename = String.Format("{0}/{1}", configuratorType, guidFileName);

            //string Result = new DeliverLoad.Models.Comman().UploadDocumentToAzure(stream, filename, blobStorage);

            //return Result;

            try
            {
                //ExerciseId = id;

                string mediaAccountName = ConfigurationManager.AppSettings["MediaAccountName"];
                string mediaAccountKey = ConfigurationManager.AppSettings["MediaAccountKey"];
                string storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
                string storageAccountKey = ConfigurationManager.AppSettings["StorageAccountKey"];

                mediaContext = new CloudMediaContext(mediaAccountName, mediaAccountKey);
                var storageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();
                var mediaBlobContainer = cloudBlobClient.GetContainerReference("temporary-media");

                mediaBlobContainer.CreateIfNotExists();

                // Create a new asset.
                asset = mediaContext.Assets.Create("NewAsset_" + Guid.NewGuid(), AssetCreationOptions.None);
                IAccessPolicy writePolicy = mediaContext.AccessPolicies.Create("writePolicy",
                    TimeSpan.FromMinutes(120), AccessPermissions.Write);
                ILocator destinationLocator = mediaContext.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

                // Get the asset container URI and copy blobs from mediaContainer to assetContainer.
                Uri uploadUri = new Uri(destinationLocator.Path);
                string assetContainerName = uploadUri.Segments[1];
                CloudBlobContainer assetContainer =
                    cloudBlobClient.GetContainerReference(assetContainerName);
                string OriginalFileName = Guid.NewGuid().ToString().ToLower() + Path.GetExtension(fullName);

                var sourceCloudBlob = mediaBlobContainer.GetBlockBlobReference(OriginalFileName);

                //sourceCloudBlob.UploadFromByteArray(video, 0, video.Length);
                BlobRequestOptions bro = new BlobRequestOptions() { ServerTimeout = TimeSpan.FromSeconds(5) };

                byte[] Video = ReadToEnd(stream);

                sourceCloudBlob.UploadFromByteArray(Video, 0, Video.Length);
                sourceCloudBlob.SetProperties();

                sourceCloudBlob.FetchAttributes();

                if (sourceCloudBlob.Properties.Length > 0)
                {
                    IAssetFile assetFile = asset.AssetFiles.Create(OriginalFileName);
                    var destinationBlob = assetContainer.GetBlockBlobReference(OriginalFileName);

                    destinationBlob.DeleteIfExists();
                    destinationBlob.StartCopyFromBlob(sourceCloudBlob);
                    destinationBlob.FetchAttributes();

                }
                destinationLocator.Delete();
                writePolicy.Delete();

                // Refresh the asset.
                asset = mediaContext.Assets.Where(a => a.Id == asset.Id).FirstOrDefault();
                string encodeAssetId = string.Empty;

                foreach (var item in asset.AssetFiles)
                {
                    item.IsPrimary = true;
                    item.ContentFileSize = stream.Length;
                    item.Update();
                    encodeAssetId = item.Id;
                    break;
                }

                string thumbnailToken = "_min.jpg";
                // Preset reference documentation: http://msdn.microsoft.com/en-us/library/windowsazure/jj129582.aspx
                var encodingPreset = "H264 Adaptive Bitrate MP4 Set 720p";
                var assetToEncode = mediaContext.Assets.Where(a => a.Id == asset.Id).FirstOrDefault();

                if (assetToEncode == null)
                {
                    throw new ArgumentException("Could not find assetId: " + encodeAssetId);
                }

                job = mediaContext.Jobs.Create("Encoding " + assetToEncode.Name + " to " + encodingPreset);

                IMediaProcessor latestWameMediaProcessor = (from p in mediaContext.MediaProcessors where p.Name == "Windows Azure Media Encoder" select p).ToList().OrderBy(wame => new Version(wame.Version)).LastOrDefault();
                ITask encodeTask = job.Tasks.AddNew("Encoding", latestWameMediaProcessor, encodingPreset, TaskOptions.None);
                encodeTask.InputAssets.Add(assetToEncode);
                encodeTask.OutputAssets.AddNew(assetToEncode.Name + " as " + encodingPreset, AssetCreationOptions.None);




                job.StateChanged += new EventHandler<JobStateChangedEventArgs>((sender, jsc) => Console.WriteLine(string.Format("{0}\n  State: {1}\n  Time: {2}\n\n", ((IJob)sender).Name, jsc.CurrentState, DateTime.UtcNow.ToString(@"yyyy_M_d_hhmmss"))));
                job.Submit();

                ThreadStart ts = new ThreadStart(() => EncodeVideoInThread(nodeid, ""));
                Thread t1 = new Thread(ts);
                t1.IsBackground = true;
                t1.Start();
                string PrimaryURL = sourceCloudBlob.StorageUri.PrimaryUri.ToString();

                int status = service.updateVideoURL(nodeid, PrimaryURL, User.Identity.Name);

                return PrimaryURL;

            }
            catch (Exception ex)
            {
                return "-1";
            }

        }

        /// <summary>
        /// this function will convert stream to bytes array!
        /// </summary>
        /// <param name="stream">its a video stream</param>
        /// <returns>Returns byte array.</returns>
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        public void EncodeVideoInThread(int nodeId, string categoryName)
        {
            EncodeVideo(nodeId, categoryName);
        }

        public string EncodeVideo(int nodeId, string categoryName)
        {
            var result = 0;
            job.GetExecutionProgressTask(CancellationToken.None).Wait();
            string thumbnailToken = "_min.jpg";
            var assetToEncode = mediaContext.Assets.Where(a => a.Id == asset.Id).FirstOrDefault();
            var preparedAsset = job.OutputMediaAssets.FirstOrDefault();

            var streamingAssetId = preparedAsset.Id; // "YOUR ASSET ID";
            var daysForWhichStreamingUrlIsActive = 365;
            var streamingAsset = mediaContext.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();
            var accessPolicy = mediaContext.AccessPolicies.Create(streamingAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                                                     AccessPermissions.Read);
            string streamingUrl = string.Empty;
            var assetFiles = streamingAsset.AssetFiles.ToList();
            var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith("m3u8-aapl.ism")).FirstOrDefault();

            streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".mp4")).FirstOrDefault();
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = mediaContext.Locators.CreateLocator(LocatorType.Sas, streamingAsset, accessPolicy);
                var mp4Uri = new UriBuilder(locator.Path);
                mp4Uri.Path += "/" + streamingAssetFile.Name;
                streamingUrl = mp4Uri.ToString();
            }

            #region Thumbnail

            Uri thumbnailUri = null;

            //
            // Get the asset
            //
            IAsset tbAsset = mediaContext.Assets.Where(it => it.Name == (assetToEncode.Name + thumbnailToken)).FirstOrDefault();



            if (tbAsset != null)
            {
                //
                // Find the .jpg in the asset
                //
                var jpgFile = (from f in tbAsset.AssetFiles
                               where f.Name.EndsWith(".jpg")
                               select f).FirstOrDefault();
                if (jpgFile == null)
                    throw new ApplicationException("Could not find a .jpg file in the asset.id: " + tbAsset.Id);

                //
                // Look for an existing locator
                //
                ILocator locator = null;
                var locators = from rows in tbAsset.Locators where rows.Type == LocatorType.Sas orderby rows.ExpirationDateTime select rows;
                if (locators != null && locators.Count() > 0)
                {
                    //Get the one that expires last:
                    locator = locators.LastOrDefault();
                }



                //
                // Create a locator if required.
                //
                if (locator == null)
                {
                    var accessPolicyTimeout = TimeSpan.FromDays(365);
                    IAccessPolicy readPolicy = mediaContext.AccessPolicies.Create("Read Policy " + assetToEncode.Name + thumbnailToken, accessPolicyTimeout, AccessPermissions.Read);
                    var startTime = DateTime.UtcNow.AddMinutes(-5);
                    locator = mediaContext.Locators.CreateSasLocator(tbAsset, readPolicy, startTime);
                }

                //
                // Build Uri
                //
                if (locator != null)
                {
                    UriBuilder ub = new UriBuilder(locator.Path);
                    ub.Path += "/" + jpgFile.Name;
                    thumbnailUri = ub.Uri;
                }


            }

            #endregion

            //mail not working 
            int status = service.updateVideoURL(nodeId, streamingUrl, User.Identity.Name);

            //var mail = UserMailer.AzureVideoUploadNotification(User.Identity.Name, status, "");
            //mail.Subject = "ChitChatChannel account confirmation";
            //mail.To.Add(new MailAddress(User.Identity.Name));

            //var client = new SmtpClientWrapper();
            //mail.SendAsync("async send", client);
                        
            string subject = "Video encoding successful";
            string body = "";

            var user = service.GetUserDetails(User.Identity.Name);

            body += "<p>Dear  " + user.FirstName + ",</p>";
            body += "<p>Your video has been encoded and ready for viewing.</p>";
            body += "<p>Login to take a <a href=\"http://www.chitchatchannel.com/\">look</a></p>";
            body += "<p>Sincerely,</p><p>ChitChatChannel Admin.</p>";
            body += "<p>-------------------------------------------------<br/>This e-mail and any files transmitted with it may contain privileged or confidential information. It is solely for use by the individual for whom it is intended, even if addressed incorrectly. If you received this e-mail in error, please notify the sender; do not disclose, copy, distribute, or take any action in reliance on the contents of this information; and delete it from your system. Any other use of this e-mail is prohibited. Thank you for your compliance.<br/>";
            body +=  "--------------------------------------------------</p>";

            var msg = new MailMessage("support@chitchatchannel.com", user.UserName);
            msg.Subject = subject;
            msg.Body = body;

            msg.IsBodyHtml = true;

            var client = new SmtpClient();                        
            client.Send(msg);

            return streamingUrl;
        }      

        #endregion
    }
}
