
namespace CreatorsPlatform.CsMod.Vicky
{
    public class CreatorService
    {
        public int CreatorID { get; set; }
        public string? CreatorName { get; set; }
        public string? CreatorImage { get; set; }
        public string? CreatorText { get; set; }

        public int WorkID { get; set; }
        public string? WorkName { get; set; }
        public string? WorkImage { get; set; }
        public DateTime? WorkDate { get; set; }

        public int CommissionID { get; set; }
        public int CommissionPrice { get; set; }
        public string? CommissionName { get; set; }
        public string? CommissionImage { get; set; }

    }
}
