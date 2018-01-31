using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;
using DeliverLoad.Services;
using DeliverLoad.Mvc.Mailers;
using System.Net.Mail;
using Mvc.Mailer;


namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {

      


        public TreeViewNodeVM GetTreeVeiwList(int CategoryId)
        {
            var rootNode = (from C in dbContext.CategoryContents
                            where C.ParentId == null
                            select new TreeViewNodeVM()
                            {
                                NodeId = C.Id,
                                Name = C.Name
                            }).SingleOrDefault();
            BuildChildNode(rootNode, CategoryId);

            return rootNode;
        }


        private void BuildChildNode(TreeViewNodeVM rootNode, int CategoryId)
        {
            if (rootNode != null)
            {
                List<TreeViewNodeVM> chidNode = (from C in dbContext.CategoryContents
                                                 where C.ParentId == rootNode.NodeId && C.CategoryId == CategoryId
                                                 select new TreeViewNodeVM()
                                                 {
                                                     NodeId = C.Id,
                                                     Name = C.Name
                                                 }).ToList<TreeViewNodeVM>();
                if (chidNode.Count > 0)
                {
                    foreach (var childRootNode in chidNode)
                    {
                        BuildChildNode(childRootNode, CategoryId);
                        rootNode.ChildNode.Add(childRootNode);
                    }
                }

            }
        }

        public string CreateChildNode(int CategoryId, int ParentId, string Name, int UserId, string Description, string imageName, string VideoName, string VideoFrom)
        {

            try
            {
                CategoryContent objCategoryContent = new CategoryContent();
                objCategoryContent.Name = Name;
                objCategoryContent.ParentId = ParentId;
                objCategoryContent.CategoryId = CategoryId;
                objCategoryContent.UserId = UserId;
                objCategoryContent.Description = Description;
                objCategoryContent.ImageName = imageName;
                objCategoryContent.VideoName = VideoName == null ? "" : VideoName.Trim() == "" ? "" : VideoName.Trim();
                objCategoryContent.VideoFrom = VideoFrom == null ? "" : VideoFrom.Trim() == "" ? "" : VideoFrom.Trim();
                dbContext.CategoryContents.Add(objCategoryContent);

                dbContext.SaveChanges();

                return objCategoryContent.Id.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;

            }


        }

        public string EditTreeNode(int NodeId, string Title, string Description, string imageName, string VideoName, string VideoFrom)
        {
            try
            {
                //check parent nodeid
                if (NodeId == 162)
                {
                    return "3";
                }

                CategoryContent objCategoryContent = dbContext.CategoryContents.Where(x => x.Id == NodeId).SingleOrDefault();
                objCategoryContent.Name = Title;

                if (Description != null && Description != "")
                {
                    objCategoryContent.Description = Description;
                }

                if (imageName.Trim() != "" && imageName.Trim() != null)
                {
                    objCategoryContent.ImageName = imageName;
                }

                if (VideoName.Trim() != "" && VideoName != null)
                {
                    objCategoryContent.VideoName = VideoName;
                    objCategoryContent.VideoFrom = VideoFrom;
                }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "2";
        }

        public string DeleteTreeNode(int NodeId, int UserId)
        {
            try
            {
                //check parent
                var parentnode = dbContext.CategoryContents.Where(x => x.ParentId == NodeId && x.UserId == UserId).FirstOrDefault();

                if (parentnode != null || NodeId == 162)
                {
                    return "-1";
                }

                var objCategoryContent = dbContext.CategoryContents.Where(x => x.Id == NodeId).SingleOrDefault();


                dbContext.CategoryContents.Remove(objCategoryContent);

                dbContext.SaveChanges();


            }
            catch (Exception ex)
            {
                return ex.Message;

            }

            return "1";

        }

        public string ContentDetails(int UserId, int NodeId)
        {
            var Details = dbContext.CategoryContents.Where(x => x.Id == NodeId).SingleOrDefault();
            if (Details == null || Details.Description == null)
                return "-1";
            return Details.Description;
        }

        public TreeViewNodeVM GetCategoryDescription(int UserId, int NodeId)
        {
            TreeViewNodeVM Details = (from C in dbContext.CategoryContents
                                      where C.Id == NodeId
                                      select new TreeViewNodeVM
                                       {
                                           NodeId = C.Id,
                                           ContentDescription = C.Description,
                                           ImageName = C.ImageName,
                                           VideoName = C.VideoName,
                                           VideoFrom = C.VideoFrom,
                                           Name = C.Name
                                       }).First();

            Details.ContentCommentModel = getCommentList(UserId, NodeId);
            return Details;
        }

        public string SaveComments(int NodeId, string Comment, int UserId, string UserType)
        {
            try
            {
                CategoryContentComment objComment = new CategoryContentComment();

                objComment.ContentId = NodeId;
                objComment.Comments = Comment;
                objComment.UserId = UserId;
                objComment.CommentDate = DateTime.Now;
                objComment.UserType = UserType;

                dbContext.CategoryContentComments.Add(objComment);

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }

        public IEnumerable<ContentCommentModel> getCommentList(int UserId, int NodeId)
        {
            //(U.UserId == UserId || U.UserType == "A") &&
            var CommentList = (from C in dbContext.CategoryContentComments
                               join U in dbContext.Users on C.UserId equals U.UserId
                               where C.ContentId == NodeId
                               select new ContentCommentModel
                               {
                                   Id = C.Id,
                                   ContentId = C.ContentId,
                                   Comments = C.Comments,
                                   CommentDate = C.CommentDate,
                                   ScreenName = U.ScreenName,
                                   Image = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                   UserId = C.UserId
                               }).OrderByDescending(x => x.CommentDate);

            return CommentList;

        }
        public List<CategoryCommentDetailUsers> GetCategoryCommentDetailUsers(int NodeId, int UserId)
        {
            //var objCatUsers = (from DeliverLoad in dbContext.CategoryContentComments
            //                   join CC in dbContext.CategoryContents on DeliverLoad.ContentId equals CC.Id
            //                   join C in dbContext.Categories on CC.CategoryId equals C.CategoryId
            //                   join U in dbContext.Users on DeliverLoad.UserId equals U.UserId
            //                   where DeliverLoad.ContentId == NodeId && U.UserId != UserId
            //                   select new CategoryCommentDetailUsers
            //                   {
            //                       CategoryID = CC.CategoryId,
            //                       CategoryName = C.Name,
            //                       ChannelNo = C.ChannelNo,
            //                       Comment = DeliverLoad.Comments,
            //                       CommentDate = DeliverLoad.CommentDate,
            //                       CommentID = DeliverLoad.Id,
            //                       ContentID = DeliverLoad.ContentId,
            //                       ContentName = CC.Name,
            //                       UserID = U.UserId,
            //                       UserName = U.UserName,
            //                       UserEmail = U.EmailID,
            //                       UserType = U.UserType

            //                   }).ToList();
            var objCatUsers = (from CC in dbContext.CategoryContents
                               join C in dbContext.Categories on CC.CategoryId equals C.CategoryId
                               join presenter in dbContext.PresenterCategories on C.CategoryId equals presenter.CategoryId
                               join U in dbContext.Users on presenter.UserId equals U.UserId
                               where CC.Id == NodeId
                               select new CategoryCommentDetailUsers
                               {
                                   CategoryID = CC.CategoryId,
                                   CategoryName = C.Name,
                                   ChannelNo = C.ChannelNo,
                                   ContentName = CC.Name,
                                   UserID = U.UserId,
                                   UserName = U.UserName,
                                   UserEmail = U.EmailID,
                                   UserType = U.UserType

                               }).Union(from CC in dbContext.CategoryContents
                                        join C in dbContext.Categories on CC.CategoryId equals C.CategoryId
                                        join particioants in dbContext.ParticipantCategories on C.CategoryId equals particioants.CategoryId
                                        join U in dbContext.Users on particioants.UserId equals U.UserId
                                        where CC.Id == NodeId && U.UserId != UserId
                                        select new CategoryCommentDetailUsers
                                        {
                                            CategoryID = CC.CategoryId,
                                            CategoryName = C.Name,
                                            ChannelNo = C.ChannelNo,
                                            ContentName = CC.Name,
                                            UserID = U.UserId,
                                            UserName = U.UserName,
                                            UserEmail = U.EmailID,
                                            UserType = U.UserType
                                        }).ToList();
            return objCatUsers;
        }

        public List<CategoryCommentDetailUsers> GetCategoryCommentDetailPresenter(int NodeId, int UserId)
        {

            var objCatUsers = (from CC in dbContext.CategoryContents
                               join C in dbContext.Categories on CC.CategoryId equals C.CategoryId
                               join particioants in dbContext.ParticipantCategories on C.CategoryId equals particioants.CategoryId
                               join U in dbContext.Users on particioants.UserId equals U.UserId
                               where CC.Id == NodeId && U.UserId != UserId
                               select new CategoryCommentDetailUsers
                               {
                                   CategoryID = CC.CategoryId,
                                   CategoryName = C.Name,
                                   ChannelNo = C.ChannelNo,
                                   ContentName = CC.Name,
                                   UserID = U.UserId,
                                   UserName = U.UserName,
                                   UserEmail = U.EmailID,
                                   UserType = U.UserType
                               }).ToList();
            return objCatUsers;
        }


        public IEnumerable<ContentCommentModel> getCommentList(int NodeId)
        {
            return (from C in dbContext.CategoryContentComments
                    join U in dbContext.Users on C.UserId equals U.UserId
                    where C.ContentId == NodeId
                    select new ContentCommentModel
                    {
                        Id = C.Id,
                        ContentId = C.ContentId,
                        Comments = C.Comments,
                        CommentDate = C.CommentDate,
                        ScreenName = U.ScreenName,
                        Image = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                        UserId = C.UserId
                    }).OrderByDescending(x => x.CommentDate).ToList();
        }

        public int DeleteChannel(int channelId)
        {
            var channel = dbContext.Categories.FirstOrDefault(x => x.CategoryId == channelId);

            if (channel != null)
            {
                dbContext.Categories.Remove(channel);
                dbContext.SaveChanges();
                return 1;
            }

            return -1;

        }

        public int updateVideoURL(int NodeId, string URL, string username)
        {
        

            int status;
            string channelName = "";
            try
            {
                var Content = dbContext.CategoryContents.FirstOrDefault(x => x.Id == NodeId);
                channelName = (from cc in dbContext.CategoryContents
                               join c in dbContext.Categories on cc.CategoryId equals c.CategoryId
                               select (c.ChannelNo + " : " + c.Name)).FirstOrDefault();


                if (Content != null)
                {
                    Content.VideoName = URL;
                    dbContext.SaveChanges();
                    status = 1;

                }
                else
                {
                    status = 0;
                }
            }
            catch (Exception ex)
            {
                status = -1;

            }

           

            return status;
        }

    }
}