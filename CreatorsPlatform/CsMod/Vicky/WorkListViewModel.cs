namespace CreatorsPlatform.CsMod.Vicky
{
    public class WorkListViewModel : WorkViewModel
    {
        public List<WorkViewModel>? WorkList { get; set; }
        public int PageNumber { get; set; }//當前頁數
        public int TotalPages { get; set; }//總共頁數
        public int PageSize { get; set; } //每頁顯示幾筆
        public string? SearchKey { get; set; }
        public Dictionary<string, SubtitleCount> Count { get; set; }

    }

    public class SubtitleCount
    {
        public int Count { get; set; }
    }
}
