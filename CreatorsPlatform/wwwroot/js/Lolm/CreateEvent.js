var BannerDataURL;
var ExImgDataURLs = [];
var ExImgDataURL;
$(function () {
    // 上傳圖片並且預覽功能
    // 封面圖片
    $("#progressbarTWInput").change(function () {
        readURL(this);
        // this是改變的那個元素(input)
    });
    function readURL(input) {

        if (input.files && input.files[0]) {

            var reader = new FileReader();

            reader.onload = function (e) {
                BannerDataURL = e.target.result;
                $("#preview_progressbarTW_img").attr('src', BannerDataURL);
            }

            reader.readAsDataURL(input.files[0]);

        }
    }
    // 範例圖片
    $(".ExampleImgSection input").change(
        function () {
            ReadExURL(this);
        }
    );
    function ReadExURL(TheEle) {
        if (TheEle.files && TheEle.files[0]) {

            var reader = new FileReader();
            //console.log($(TheEle).next("div").children("img"));
            reader.onload = function (e) {
                ExImgDataURL = e.target.result;
                $(TheEle).next("div").children("img").attr('src', ExImgDataURL);
                GetAllExImgDataURL(ExImgDataURL);
            }

            reader.readAsDataURL(TheEle.files[0]);

        }
    }
});
// 獲得Quill內容
function getQuillContent() {
    // 拿到編輯器內容 Delta
    const QuillContent = quill.getContents();
    // Delta 轉 Json
    var DeltaJson = JSON.stringify(QuillContent);
    console.log(DeltaJson);
    return DeltaJson;
}

// 獲得Quill內容的純文字
function getQuillText() {
    const QuillText = quill.getText();
    return QuillText;
}

function GetEventStyle() {
    var eventStyleArray = [$("#EventTitleColorInput").val(), $("#EventIntroColorInput").val()];
    return JSON.stringify(eventStyleArray);
}

function GetAllExImgDataURL(TheURL) {
    ExImgDataURLs.push(TheURL);
}

// 拿到所有的input內容並新增到資料庫
function PostAllToSQL() {
    //var dataFromClient = {
    //    EventName: '阿巴巴',
    //    StartDate: "2024-03-22T10:04:35.123",
    //    EndDate: "2024-03-23T10:04:35.123",
    //    Description: "低死哭順",
    //    CategoryID: 1
    //};

    var EventdataFromClient = {
        EventName: $("#eventName").val(),
        StartDate: $("#startDate").val(),
        EndDate: $("#endDate").val(),
        Description: getQuillContent(),
        EventStyle: GetEventStyle(),
        Banner: BannerDataURL,
        CategoryID: 1,
        DescriptionString: getQuillText()
    };

    //ExImgURLArray: JSON.stringify(ExImgDataURLs)

    $.ajax({
        url: "/Lolm/Create",
        method: "post",
        data: EventdataFromClient,
        success: function (response) {
            UploadEventExImg();
            alert("活動發布成功");
        },
        error: function (xhr, status, error) {
            // 處理錯誤 
            alert("活動發布失敗");
        }
    });
}
function UploadEventExImg() {
    for (var i = 0; i < ExImgDataURLs.length; i++) {
        $.ajax({
            url: "/Lolm/CreateEventExImg",
            method: "post",
            /*processData: false, // 不对 FormData 进行处理*/
            data: {
                ImageURL: ExImgDataURLs[i],
                ImageSample: 1
            },
            success: function (response) {
            },
            error: function () {
                alert("活動範例圖片上傳失敗");
            }
        })
    }

}
