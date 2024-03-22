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
                $("#preview_progressbarTW_img").attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);

        }
    }

    $(".ExampleImgSection input").change(
        function () {
            ReadExURL(this);
        }
    );

    function ReadExURL(TheEle) {
        if (TheEle.files && TheEle.files[0]) {

            var reader = new FileReader();
            console.log($(TheEle).next("div").children("img"));
            reader.onload = function (e) {
                $(TheEle).next("div").children("img").attr('src', e.target.result);
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
    $.ajax({
        url: "/Lolm/CreateEvent",
        method: "post",
        data: { DataFromClient: DeltaJson }
    }).done(function (data) {
        alert(data);
    });
}

// 拿到所有的input內容並新增到資料庫
function PostAllToSQL() {
    //var eventName = $("#eventName").val();
    //var startDate = $("#startDate").val();
    //var endDate = $("#endDate").val();
    //var eventStyle = $("#eventStyle").val();
    //var data = {
    //    EventName: eventName,
    //    StartDate: startDate,
    //    EndDate: endDate,
    //    EventStyle: eventStyle
    //};

    //var dataFromClient = {
    //    EventName: '阿巴巴',
    //    StartDate: "2024-03-22T10:04:35.123",
    //    EndDate: "2024-03-23T10:04:35.123",
    //    Description: "低死哭順",
    //    CategoryID: 1
    //};

    var dataFromClient = {
        EventName: $("#eventName").val(),
        StartDate: $("#startDate").val(),
        EndDate: $("#endDate").val(),
        Description: "低死哭順",
        CategoryID: 1
    };

    $.ajax({
        url: "/Lolm/Create",
        method: "post",
        contentType: 'application/json',
        data: JSON.stringify(dataFromClient),
        success: function (response) {
            console.log(response);
            alert(response);
        },
        error: function (xhr, status, error) {
            // 處理錯誤 
            alert("fail");
        }
     });
}

function PostAllToSQL02() {
    var startDate = new Date("2024-03-22T10:04:35.123").toISOString();
    var endDate = new Date("2024-03-22T10:05:35.123").toISOString();

    $.ajax({
        url: '/Lolm/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            EventName: '阿巴巴',
            StartDate: startDate,
            EndDate: endDate,
            Description: "低死哭順",
            CategoryID: 1
        }),
        success: function (response) {
            // 處理成功回應
            alert('成功');
        },
        error: function (xhr, status, error) {
            // 處理錯誤回應
            alert('失敗');
            console.log(xhr);
            console.log(status);
            console.log(error);
        }
    });
}