﻿var BannerDataURL;
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
    if ($("#SetColorOrNot").prop("checked") == true) {
        var EventdataFromClient = {
            EventId: TheEventId,
            EventName: $("#eventName").val(),
            StartDate: $("#startDate").val(),
            EndDate: $("#endDate").val(),
            Description: getQuillContent(),
            EventStyle: GetEventStyle(),
            Banner: BannerDataURL,
            //CategoryID: 1,
            DescriptionString: getQuillText()
        };
    } else {
        var EventdataFromClient = {
            EventId: TheEventId,
            EventName: $("#eventName").val(),
            StartDate: $("#startDate").val(),
            EndDate: $("#endDate").val(),
            Description: getQuillContent(),
            Banner: BannerDataURL,
            //CategoryID: 1,
            DescriptionString: getQuillText()
        };
    }
    $.ajax({
        url: "/Lolm/UpdateEvent",
        method: "put",
        data: EventdataFromClient,
        success: function (response) {
            UpdateEventExImg();
            alert("活動修改成功");
            history.back();
        },
        error: function (xhr, status, error) {
            // 處理錯誤 
            alert("活動修改失敗");
        }
    });
}

function UpdateEventExImg() {
    var TheDataString = JSON.stringify(ExImgDataURLs);
    $.ajax({
        url: "/Lolm/UpdateEventImg",
        method: "post",
        //contentType: 'application/json',
        data: { EventImgArray: TheDataString, Id: TheEventId },
        success: function (response) {
            //alert('OK');
        },
        error: function () {
            alert("活動範例圖片上傳失敗");
        }
    });
}
