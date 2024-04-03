using CreatorsPlatform.Models;

namespace CreatorsPlatform.CsMod.Vicky
{
	public class WorkViewModel
	{
		public int CreatorID { get; set; } //創作者
		public int ContentsID { get; set; } //作品
		public int CategoryID { get; set; } //大分類
		public int? SubtitleID { get; set; } //中分類
		public int UsersID { get; set; } //使用者
		public string? Title { get; set; } //標題
		public string? UserName { get; set; } //使用者名稱
		public string? CategoryName { get; set; } //大分類名稱
		public string? SubtitleName { get; set; } //中分類名稱
		public string? Description { get; set; } //創作者描述
		public byte[]? ImageUrl { get; set; } //作品圖
		public byte[]? UsersAvatar { get; set; } //頭像
		public DateTime? UploadDate { get; set; } //作品更新時間
		public bool IsContent { get; set; } = false;
		public bool IsCreator { get; set; } = false;
		public bool IsCommission { get; set; } = false;//判斷是不是Commission


		public int CommissionID { get; set; }
		public string? CommissionTitle { get; set; }
		public string? Notice { get; set; }
		public int? PriceMax { get; set; }
		public int PriceMin { get; set; }
		public string? CommissionImage { get; set; }

	}
}
