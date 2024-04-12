using CreatorsPlatform.Models;
using CreatorsPlatform.CsMod.Vicky;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Azure;
using System.Linq;

namespace CreatorsPlatform.CsMod.Vicky
{

    public class VickyWorkContent : WorkViewModel
    {
        private readonly ImaginkContext _context;

        public VickyWorkContent(ImaginkContext context)
        {
            _context = context;
        }

        //搜尋
        public List<WorkViewModel> GetSearchContents(string searchKey, string sortOrder)
        {
            //int skip = (page - 1) * pageSize;
            var query = from content in _context.Contents
                        join user in _context.Users
                        on content.CreatorId equals user.CreatorId//以下新增勾選
                                                                  //join category in _context.Categories
                                                                  //on content.CategoryId equals category.CategoryId
                        join subtitle in _context.Subtitles
                        on content.SubtitleId equals subtitle.SubtitleId
                        where content.Title.Contains(searchKey)
                        select new WorkViewModel
                        {
							CreatorID = content.CreatorId,
							ContentsID = content.ContentId, //作品ID
                            UsersID = user.UserId, //使用者ID
                            Title = content.Title, //標籤
                            UploadDate = content.UploadDate, //作品更新時間
                            ImageUrl = content.ImageUrl, //作品圖
                            UserName = user.UserName, //使用者名稱
                                                      //新增部分
                                                      //CreatorID = content.CreatorId, //創作者ID
                                                      //CategoryID = content.CategoryId, //大分類ID
                            SubtitleID = content.SubtitleId, //中分類ID
                                                             //CategoryName = category.CategoryName, // 大分類名稱
                                                             //SubtitleName = subtitle.SubtitleName, //中分類名稱
                            Description = content.Description, //創作者描述
                            IsContent = true,
                        };
            query = sortOrder switch
            {
                "ascending" => query.OrderBy(content => content.UploadDate),
                "descending" => query.OrderByDescending(content => content.UploadDate),
                _ => query.OrderBy(content => content.UploadDate),
            };
            //var pagedWorkList = query.Skip(skip).Take(pageSize);
            //var resultList = query.ToList();
            var paginatedQuery = query;

            var resultList = paginatedQuery.ToList();
            return resultList;
        }
        public List<WorkViewModel> GetSearchCommission(string searchKey, string sortOrder)
        {

            var query = from commission in _context.Commissions
                        join user in _context.Users
                        on commission.CreatorId equals user.CreatorId//以下新增勾選
                        join image in _context.CommissionImages
                       on commission.CommissionId equals image.CommissionId
                        join subtitle in _context.Subtitles
                        on commission.SubtitleId equals subtitle.SubtitleId
                        where commission.Title!.Contains(searchKey)
                        select new WorkViewModel
                        {
							CreatorID = commission.CreatorId,
							CommissionID = commission.CommissionId, //作品ID
                            UsersID = user.UserId, //使用者ID
                            CommissionTitle = commission.Title, //標籤
                            CommissionImage = image.ImageUrl, //作品圖
                            UserName = user.UserName, //使用者名稱
                            SubtitleID = commission.SubtitleId, //中分類ID
                            SubtitleName = subtitle.SubtitleName, //中分類名稱
                            PriceMax = commission.PriceMax,
                            PriceMin = commission.PriceMin,
                            IsCommission = true
                        };
            query = sortOrder switch
            {
                "ascending" => query.OrderBy(commission => commission.PriceMax),
                "descending" => query.OrderByDescending(commission => commission.PriceMax),
                _ => query.OrderBy(commission => commission.PriceMax),
            };
            var paginatedQuery = query;
            var resultList = paginatedQuery.ToList();
            return resultList;
        }
        public List<WorkViewModel> GetCreatorContents(string searchKey)
        {

            var query = from content in _context.Contents
                        join user in _context.Users
                        on content.CreatorId equals user.CreatorId
                        join creator in _context.Creators
                        on content.CreatorId equals creator.CreatorId
                        where user.UserName.Contains(searchKey)
                        orderby content.UploadDate ascending
                        select new WorkViewModel
                        {
                            CreatorID = creator.CreatorId,
                            ContentsID = content.ContentId,
                            UsersID = user.UserId,
                            Title = content.Title,
                            UploadDate = content.UploadDate,
                            UsersAvatar = user.Avatar,
                            ImageUrl = content.ImageUrl,
                            UserName = user.UserName,
                            SubtitleID = content.SubtitleId,
                            Description = creator.Description,
                            Notice = creator.Notice,
                            IsCreator = true,
                        };
            var paginatedQuery = query;
            var resultList = paginatedQuery.ToList();
            return resultList;
        }


        //中標籤
        public List<WorkViewModel> GetSubtitleAscending(int subtitleId, string searchKey, string sortOrder)
        {

            var query = from content in _context.Contents
                        join user in _context.Users
                        on content.CreatorId equals user.CreatorId
                        join subtitle in _context.Subtitles
                        on content.SubtitleId equals subtitle.SubtitleId
                        where content.SubtitleId == subtitleId
                        //orderby content.UploadDate ascending
                        select new WorkViewModel
                        {
							CreatorID = content.CreatorId,
							ContentsID = content.ContentId,
                            UsersID = user.UserId,
                            Title = content.Title,
                            UploadDate = content.UploadDate,
                            ImageUrl = content.ImageUrl,
                            UserName = user.UserName,
                            SubtitleID = content.SubtitleId,
                            SubtitleName = subtitle.SubtitleName,
                            Description = content.Description,
                        };

            query = sortOrder switch
            {
                "ascending" => query.OrderBy(content => content.UploadDate),
                "descending" => query.OrderByDescending(content => content.UploadDate),
                _ => query.OrderBy(content => content.UploadDate),
            };
            var paginatedQuery = query;

            var resultList = paginatedQuery.ToList();
            return resultList;
        }
        public List<WorkViewModel> GetSubtitleAsCommission(int subtitleId, string searchKey, string sortOrder)
        {

            var query = from commission in _context.Commissions
                        join user in _context.Users
                        on commission.CreatorId equals user.CreatorId
                        join image in _context.CommissionImages
                       on commission.CommissionId equals image.CommissionId
                        join subtitle in _context.Subtitles
                        on commission.SubtitleId equals subtitle.SubtitleId
                        where commission.SubtitleId == subtitleId && commission.Title!.Contains(searchKey)
                        select new WorkViewModel
                        {
							CreatorID = commission.CreatorId,
							CommissionID = commission.CommissionId,
                            UsersID = user.UserId,
                            Title = commission.Title,
                            CommissionImage = image.ImageUrl,
                            UserName = user.UserName,
                            SubtitleID = commission.SubtitleId,
                            SubtitleName = subtitle.SubtitleName,
                            PriceMax = commission.PriceMax,
                            PriceMin = commission.PriceMin,
                            IsCommission = true
                        };
            query = sortOrder switch
            {
                "ascending" => query.OrderBy(content => content.PriceMax),
                "descending" => query.OrderByDescending(content => content.PriceMax),
                _ => query.OrderBy(content => content.PriceMax),
            };

            var paginatedQuery = query;

            var resultList = paginatedQuery.ToList();
            return resultList;
        }
        //小標籤
        public List<WorkViewModel> GetTagContents(int tagId)
        {
            var query = from content in _context.Contents
                        join user in _context.Users
                        on content.CreatorId equals user.CreatorId

                        join contentTag in _context.ContentTags
                        on content.ContentId equals contentTag.ContentId
                        where contentTag.TagId == tagId
                        orderby content.UploadDate ascending
                        select new WorkViewModel
                        {
							CreatorID = content.CreatorId,
							ContentsID = content.ContentId,
                            UsersID = user.UserId,
                            Title = content.Title,
                            UploadDate = content.UploadDate,
                            ImageUrl = content.ImageUrl,
                            UserName = user.UserName,
                            TagID = content.TagId,
                        };

            //var resultList = query.ToList();
            var paginatedQuery = query;

            var resultList = paginatedQuery.ToList();
            return resultList;
        }


        //刪除的排序方法
        //排序
        //public List<WorkViewModel> GetContentsDescending(int page = 1, int pageSize = 9)
        //{
        //	int skip = (page - 1) * pageSize;
        //	var query = from content in _context.Contents
        //				join user in _context.Users
        //				on content.CreatorId equals user.CreatorId//以下新增勾選
        //														  //join category in _context.Categories
        //														  //on content.CategoryId equals category.CategoryId
        //				join subtitle in _context.Subtitles
        //				on content.SubtitleId equals subtitle.SubtitleId
        //				orderby content.UploadDate descending
        //				select new WorkViewModel
        //				{
        //					ContentsID = content.ContentId, //作品ID
        //					UsersID = user.UserId, //使用者ID
        //					Title = content.Title, //標籤
        //					UploadDate = content.UploadDate, //作品更新時間
        //					ImageUrl = content.ImageUrl, //作品圖
        //					UserName = user.UserName, //使用者名稱
        //											  //新增部分
        //											  //CreatorID = content.CreatorId, //創作者ID
        //											  //CategoryID = content.CategoryId, //大分類ID
        //					SubtitleID = content.SubtitleId, //中分類ID
        //													 //CategoryName = category.CategoryName, // 大分類名稱
        //													 //SubtitleName = subtitle.SubtitleName, //中分類名稱
        //					Description = content.Description, //創作者描述
        //				};
        //	//var resultList = query.ToList();
        //	var paginatedQuery = query.Skip(skip).Take(pageSize); // Apply pagination

        //	var resultList = paginatedQuery.ToList();
        //	return resultList;
        //}
        //public List<WorkViewModel> GetContentsAscending(int page = 1, int pageSize = 9)
        //{
        //	int skip = (page - 1) * pageSize;
        //	var query = from content in _context.Contents
        //				join user in _context.Users
        //				on content.CreatorId equals user.CreatorId//以下新增勾選
        //														  //join category in _context.Categories
        //														  //on content.CategoryId equals category.CategoryId
        //				join subtitle in _context.Subtitles
        //				on content.SubtitleId equals subtitle.SubtitleId
        //				orderby content.UploadDate ascending
        //				select new WorkViewModel
        //				{
        //					ContentsID = content.ContentId, //作品ID
        //					UsersID = user.UserId, //使用者ID
        //					Title = content.Title, //標籤
        //					UploadDate = content.UploadDate, //作品更新時間
        //					ImageUrl = content.ImageUrl, //作品圖
        //					UserName = user.UserName, //使用者名稱
        //											  //新增部分
        //											  //CreatorID = content.CreatorId, //創作者ID
        //											  //CategoryID = content.CategoryId, //大分類ID
        //					SubtitleID = content.SubtitleId, //中分類ID
        //													 //CategoryName = category.CategoryName, // 大分類名稱
        //													 //SubtitleName = subtitle.SubtitleName, //中分類名稱
        //					Description = content.Description, //創作者描述
        //				};
        //	//var resultList = query.ToList();
        //	var paginatedQuery = query.Skip(skip).Take(pageSize); // Apply pagination

        //	var resultList = paginatedQuery.ToList();
        //	return resultList;
        //}
        //public List<WorkViewModel> GetCommissionAscending(int page = 1, int pageSize = 9)
        //{
        //	int skip = (page - 1) * pageSize;
        //	var query = from commission in _context.Commissions
        //				join user in _context.Users
        //				on commission.CreatorId equals user.CreatorId//以下新增勾選
        //				join image in _context.CommissionImages
        //			   on commission.CommissionId equals image.CommissionId
        //				join subtitle in _context.Subtitles
        //				on commission.SubtitleId equals subtitle.SubtitleId
        //				orderby commission.PriceMax ascending
        //				select new WorkViewModel
        //				{
        //					CommissionID = commission.CommissionId, //作品ID
        //					UsersID = user.UserId, //使用者ID
        //					CommissionTitle = commission.Title, //標籤
        //					CommissionImage = image.ImageUrl, //作品圖
        //					UserName = user.UserName, //使用者名稱
        //					SubtitleID = commission.SubtitleId, //中分類ID
        //					SubtitleName = subtitle.SubtitleName, //中分類名稱
        //					PriceMax = commission.PriceMax,
        //					PriceMin = commission.PriceMin,

        //				};
        //	//var resultList = query.ToList();
        //	var paginatedQuery = query.Skip(skip).Take(pageSize); // Apply pagination

        //	var resultList = paginatedQuery.ToList();
        //	return resultList;
        //}
        //public List<WorkViewModel> GetCommissionDescending(int page = 1, int pageSize = 9)
        //{
        //	int skip = (page - 1) * pageSize;
        //	var query = from commission in _context.Commissions
        //				join user in _context.Users
        //				on commission.CreatorId equals user.CreatorId//以下新增勾選
        //				join image in _context.CommissionImages
        //			   on commission.CommissionId equals image.CommissionId
        //				join subtitle in _context.Subtitles
        //				on commission.SubtitleId equals subtitle.SubtitleId
        //				orderby commission.PriceMax descending
        //				select new WorkViewModel
        //				{
        //					CommissionID = commission.CommissionId, //作品ID
        //					UsersID = user.UserId, //使用者ID
        //					CommissionTitle = commission.Title, //標籤
        //					CommissionImage = image.ImageUrl, //作品圖
        //					UserName = user.UserName, //使用者名稱
        //					SubtitleID = commission.SubtitleId, //中分類ID
        //					SubtitleName = subtitle.SubtitleName, //中分類名稱
        //					PriceMax = commission.PriceMax,
        //					PriceMin = commission.PriceMin,

        //				};
        //	//var resultList = query.ToList();
        //	var paginatedQuery = query.Skip(skip).Take(pageSize); // Apply pagination

        //	var resultList = paginatedQuery.ToList();
        //	return resultList;
        //}

    }

}

