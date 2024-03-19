using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorsPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddSubtitleIDToCommission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__19093A2B66E41C26", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    CreatorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BangerURL = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Creators__6C7548111D29706E", x => x.CreatorID);
                });

            migrationBuilder.CreateTable(
                name: "Subtitles",
                columns: table => new
                {
                    SubtitleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubtitleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subtitle__A442F915E502DF79", x => x.SubtitleID);
                    table.ForeignKey(
                        name: "FK__Subtitles__Categ__628FA481",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tags__657CFA4CBEA41F21", x => x.TagID);
                    table.ForeignKey(
                        name: "FK__Tags__CategoryID__619B8048",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventURL = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    EventStyle = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Events__7944C87015B57FAF", x => x.EventID);
                    table.ForeignKey(
                        name: "FK__Events__Category__5EBF139D",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK__Events__CreatorI__3493CFA7",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlanPrice = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PlanLevel = table.Column<int>(type: "int", nullable: true),
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Plans__755C22D7617F78FB", x => x.PlanID);
                    table.ForeignKey(
                        name: "FK__Plans__CreatorID__5DCAEF64",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    EMail = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Avatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BirthdayDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Point = table.Column<int>(type: "int", nullable: true),
                    EmailCertification = table.Column<bool>(type: "bit", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC6C448E9F", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__Users__CategoryI__52593CB8",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK__Users__CreatorID__5165187F",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                });

            migrationBuilder.CreateTable(
                name: "EventImages",
                columns: table => new
                {
                    EventImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURL = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    ImageSample = table.Column<bool>(type: "bit", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventIma__9B3A6940E9049B02", x => x.EventImageID);
                    table.ForeignKey(
                        name: "FK__EventImag__Creat__3587F3E0",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                    table.ForeignKey(
                        name: "FK__EventImag__Event__6383C8BA",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID");
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    CommissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PriceMin = table.Column<int>(type: "int", nullable: false),
                    PriceMax = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    PutUpDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OverDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SubtitleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Commissi__6C2C8CEC19A627C3", x => x.CommissionID);
                    table.ForeignKey(
                        name: "FK__Commissio__Creat__5812160E",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                    table.ForeignKey(
                        name: "FK__Commissio__UserI__59063A47",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "FriendLists",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendRelations = table.Column<bool>(type: "bit", nullable: false),
                    RequesterUserID = table.Column<int>(type: "int", nullable: false),
                    AcceptorUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FriendLi__3214EC2717DC78BB", x => x.ID);
                    table.ForeignKey(
                        name: "FK__FriendLis__Accep__60A75C0F",
                        column: x => x.AcceptorUserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__FriendLis__Reque__5FB337D6",
                        column: x => x.RequesterUserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PaymentMade = table.Column<bool>(type: "bit", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__9A2B24BDD2252C05", x => x.SubscriptionID);
                    table.ForeignKey(
                        name: "FK__Subscript__Categ__5AEE82B9",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK__Subscript__Creat__5BE2A6F2",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                    table.ForeignKey(
                        name: "FK__Subscript__PlanI__367C1819",
                        column: x => x.PlanID,
                        principalTable: "Plans",
                        principalColumn: "PlanID");
                    table.ForeignKey(
                        name: "FK__Subscript__UserI__5CD6CB2B",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "CommissionImages",
                columns: table => new
                {
                    CommissionImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURL = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CommissionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Commissi__9C714FFEB1239DCE", x => x.CommissionImageID);
                    table.ForeignKey(
                        name: "FK__Commissio__Commi__19DFD96B",
                        column: x => x.CommissionID,
                        principalTable: "Commissions",
                        principalColumn: "CommissionID");
                });

            migrationBuilder.CreateTable(
                name: "CommissionOrders",
                columns: table => new
                {
                    CommissionOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeadlineDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WorkStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CommissionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Commissi__F3359E7398ACB25F", x => x.CommissionOrderID);
                    table.ForeignKey(
                        name: "FK__Commissio__Commi__59FA5E80",
                        column: x => x.CommissionID,
                        principalTable: "Commissions",
                        principalColumn: "CommissionID");
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    ContentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    PullOffDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageURL = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    NumsInStock = table.Column<int>(type: "int", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    SubtitleID = table.Column<int>(type: "int", nullable: true),
                    TagID = table.Column<int>(type: "int", nullable: true),
                    CreatorID = table.Column<int>(type: "int", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: true),
                    EventSample = table.Column<bool>(type: "bit", nullable: true),
                    CommissionID = table.Column<int>(type: "int", nullable: true),
                    CommissionSample = table.Column<bool>(type: "bit", nullable: true),
                    ContentPrice = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contents__2907A87E585570FC", x => x.ContentID);
                    table.ForeignKey(
                        name: "FK__Contents__Catego__534D60F1",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK__Contents__Commis__656C112C",
                        column: x => x.CommissionID,
                        principalTable: "Commissions",
                        principalColumn: "CommissionID");
                    table.ForeignKey(
                        name: "FK__Contents__Creato__5629CD9C",
                        column: x => x.CreatorID,
                        principalTable: "Creators",
                        principalColumn: "CreatorID");
                    table.ForeignKey(
                        name: "FK__Contents__EventI__6477ECF3",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK__Contents__PlanID__571DF1D5",
                        column: x => x.PlanID,
                        principalTable: "Plans",
                        principalColumn: "PlanID");
                    table.ForeignKey(
                        name: "FK__Contents__Subtit__5441852A",
                        column: x => x.SubtitleID,
                        principalTable: "Subtitles",
                        principalColumn: "SubtitleID");
                    table.ForeignKey(
                        name: "FK__Contents__TagID__5535A963",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "TagID");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ContentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__C3B4DFAAD1800B14", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK__Comments__Conten__07C12930",
                        column: x => x.ContentID,
                        principalTable: "Contents",
                        principalColumn: "ContentID");
                    table.ForeignKey(
                        name: "FK__Comments__UserID__06CD04F7",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "CommentResponses",
                columns: table => new
                {
                    CommentResponseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ContentID = table.Column<int>(type: "int", nullable: false),
                    CommentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CommentR__41BC2FBF68EB0A06", x => x.CommentResponseID);
                    table.ForeignKey(
                        name: "FK__CommentRe__Comme__0C85DE4D",
                        column: x => x.CommentID,
                        principalTable: "Comments",
                        principalColumn: "CommentID");
                    table.ForeignKey(
                        name: "FK__CommentRe__Conte__0B91BA14",
                        column: x => x.ContentID,
                        principalTable: "Contents",
                        principalColumn: "ContentID");
                    table.ForeignKey(
                        name: "FK__CommentRe__UserI__0A9D95DB",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentResponses_CommentID",
                table: "CommentResponses",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentResponses_ContentID",
                table: "CommentResponses",
                column: "ContentID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentResponses_UserID",
                table: "CommentResponses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ContentID",
                table: "Comments",
                column: "ContentID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserID",
                table: "Comments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionImages_CommissionID",
                table: "CommissionImages",
                column: "CommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionOrders_CommissionID",
                table: "CommissionOrders",
                column: "CommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CreatorID",
                table: "Commissions",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_UserID",
                table: "Commissions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CategoryID",
                table: "Contents",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CommissionID",
                table: "Contents",
                column: "CommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatorID",
                table: "Contents",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_EventID",
                table: "Contents",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_PlanID",
                table: "Contents",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_SubtitleID",
                table: "Contents",
                column: "SubtitleID");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_TagID",
                table: "Contents",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_CreatorID",
                table: "EventImages",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_EventID",
                table: "EventImages",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryID",
                table: "Events",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatorID",
                table: "Events",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_FriendLists_AcceptorUserID",
                table: "FriendLists",
                column: "AcceptorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_FriendLists_RequesterUserID",
                table: "FriendLists",
                column: "RequesterUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_CreatorID",
                table: "Plans",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CategoryID",
                table: "Subscriptions",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CreatorID",
                table: "Subscriptions",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanID",
                table: "Subscriptions",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserID",
                table: "Subscriptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_CategoryID",
                table: "Subtitles",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CategoryID",
                table: "Tags",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CategoryID",
                table: "Users",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatorID",
                table: "Users",
                column: "CreatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentResponses");

            migrationBuilder.DropTable(
                name: "CommissionImages");

            migrationBuilder.DropTable(
                name: "CommissionOrders");

            migrationBuilder.DropTable(
                name: "EventImages");

            migrationBuilder.DropTable(
                name: "FriendLists");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Subtitles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Creators");
        }
    }
}
