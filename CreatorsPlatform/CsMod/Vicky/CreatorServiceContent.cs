namespace CreatorsPlatform.CsMod.Vicky
{
	public class CreatorServiceContent : CreatorService
	{

		public List<CreatorService> GetCreatorList()
		{
			return
		[
			new CreatorService { CreatorID = 1, CreatorName = "華仔",CreatorImage="../../img/Event/EventContent/InaImg.jpg",CreatorText="特帥" },
            //new CreatorService { CreatorID = 2, CreatorName = "堯仔",CreatorImage="~/img/Event/EventContent/InaImg.jpg" ,CreatorText="蟋蟀"},
            //new CreatorService { CreatorID = 3, CreatorName = "總統",CreatorImage="~/img/Event/EventContent/InaImg.jpg",CreatorText="帥"}
        ];
		}

		public List<CreatorService> GetWorkList()
		{
			return
		[
			new CreatorService { WorkID = 1, WorkName = "衣服" ,WorkImage ="../../img/Lolm/EventContent/PekoraImg.jpg",WorkDate=new DateTime (2023,1,1)},
			new CreatorService { WorkID = 2, WorkName = "鞋子", WorkImage ="../../img/Lolm/EventContent/PekoraImg.jpg",WorkDate= new DateTime(2021,1,1)},
			new CreatorService { WorkID = 3, WorkName = "褲子",WorkImage ="../../img/Lolm/EventContent/PekoraImg.jpg",WorkDate= new DateTime(2022,1,1)}
		];
		}

		public List<CreatorService> GetCommissionList()
		{

			return
		[
			new CreatorService { CommissionID = 1, CommissionName = "白色" ,CommissionImage ="../../img/Lolm/EventContent/SuiseiImg.jpg",CommissionPrice=10},
			new CreatorService { CommissionID  = 2, CommissionName = "黑色", CommissionImage ="../../img/Lolm/EventContent/SuiseiImg.jpg",CommissionPrice=100},
			new CreatorService { CommissionID  = 3, CommissionName = "藍色",CommissionImage ="../../img/Lolm/EventContent/SuiseiImg.jpg",CommissionPrice=1}
		];
		}
	}

}
