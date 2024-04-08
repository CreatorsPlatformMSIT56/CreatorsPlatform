var imageDataURLs = [];

// 對應的大標到對應的中標
$(document).ready(function () {
    $('#categorySelect').change(function () {
        var selectedValue = $(this).find('option:selected').text();
        var selectedValueToServer = { CategoryName: selectedValue };

        console.log(selectedValue);

        // 用 JS 處理副標相關部分
        switch (selectedValue) {
            case "繪圖":
                CategoryNum = 1;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>
                    <option>原創</option>
                    <option>同人</option>
                    <option>AI繪圖</option>
                    <option>Live2D</option>
                    <option>設計</option>`);
                return;
            case "影片":
                CategoryNum = 2;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>
                    <option>2D動畫</option>
                    <option>3D動畫</option>
                    <option>YT</option>
                    <option>VT</option>`);
                return;
            case "聲音":
                CategoryNum = 3;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>
                    <option>音樂</option>
                    <option>聲音</option>
                    <option>ASMR</option>`);
                return;
            case "手做":
                CategoryNum = 4;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>`);
                return;
            case "遊戲&工具程式":
                CategoryNum = 5;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>`);
                return;
            case "教學":
                CategoryNum = 6;
                $("#subtitleSelect").empty();
                $("#subtitleSelect").append(`
                    <option>----</option>
                    <option>繪圖</option>
                    <option>影片製作</option>
                    <option>聲音製作</option>
                    <option>音樂製作</option>
                    <option>手做</option>`);
                return;
        }

    });
});

// 新增作品
function NewPostToSQL() {

    var SubNameToId;
    var SubName = $("#subtitleSelect").val();
    switch (SubName) {
        case "原創":
            SubNameToId = 1;
            break;
        case "同人":
            SubNameToId = 2;
            break;
        case "AI繪圖":
            SubNameToId = 3;
            break;
        case "2D動畫":
            SubNameToId = 4;
            break;
        case "3D動畫":
            SubNameToId = 5;
            break;
        case "YT":
            SubNameToId = 6;
            break;
        case "VT":
            SubNameToId = 7;
            break;
        case "音樂":
            SubNameToId = 8;
            break;
        case "聲音":
            SubNameToId = 9;
            break;
        case "ASMR":
            SubNameToId = 10;
            break;
        case "繪圖":
            SubNameToId = 11;
            break;
        case "影片製作":
            SubNameToId = 12;
            break;
        case "聲音製作":
            SubNameToId = 13;
            break;
        case "音樂製作":
            SubNameToId = 14;
            break;
        case "手做":
            SubNameToId = 15;
            break;
        case "Live2D":
            SubNameToId = 16;
            break;
        case "設計":
            SubNameToId = 17;
            break;
    }

    //if (LimitInTime.checked == true) {
    //    var CommissionToData = {
    //        Title: $("#CommisionName").val(),
    //        SubtitleId: SubNameToId,
    //        OverDate: null,
    //        Description: getQuillContent(),
    //        PriceMin: $("#MinCharge").val(),
    //        PriceMax: $("#MaxCharge").val()
    //    };
    //} else {
    //    var CommissionToData = {
    //        Title: $("#CommisionName").val(),
    //        SubtitleId: SubNameToId,
    //        OverDate: $("#TimeLimit").val(),
    //        Description: getQuillContent(),
    //        PriceMin: $("#MinCharge").val(),
    //        PriceMax: $("#MaxCharge").val()
    //    };
    //}
    var PostToData = {
        Title: $("#postTitle").val(),
        SubtitleId: SubNameToId,
        Description: getQuillContent()
    }

    console.log(PostToData);

    $.ajax({
        url: "/Creator/CreatePost",
        method: "post",
        data: PostToData,
        success: function (response) {
            //sendImageDataURLToBackend();
            alert("作品發布成功");
        },
        error: function (xhr, status, error) {
            // 處理錯誤 
            alert("作品發布失敗");
        }
    });
}


// 將圖片轉成 dataURL
//$("#ImageFile").change(function () {
//    $("#preview-ImageFile").html(""); // 清除預覽
//    readURL(this);
//});

//function readURL(input) {
//    if (input.files && input.files.length >= 0) {
//        for (var i = 0; i < input.files.length; i++) {
//            var reader = new FileReader();
//            reader.onload = function (e) {
//                var img = $("<img width='300' height='200'>").attr('src', e.target.result);
//                $("#preview-ImageFile").append(img);
//                // 将每张图片的DataURL存储在数组中
//                imageDataURLs.push(e.target.result);
//            }
//            reader.readAsBinaryString(input.files[i]); // 讀成Binary
//        }
//        // 将存储多张图片的DataURL的数组传递给其他函数或发送到后端
//        console.log(imageDataURLs);
//    } else {
//        var noPictures = $("<p>目前沒有圖片</p>");
//        $("#preview-ImageFile").append(noPictures);
//    }
//}

//function readfile(input) {
//    if (input.files && input.files.length >= 0) {
//        for (var i = 0; i < input.files.length; i++) {
//            var reader = new FileReader();
//            reader.onload = function (e) {
//                var img = $("<img width='300' height='200'>").attr('src', e.target.result);
//                $("#preview-ImageFile").append(img);
//                // 将每张图片的DataURL存储在数组中
//                imageDataURLs.push(e.target.result);
//            }
//            reader.readAsBinaryString(input.files[i]); // 讀成Binary
//        }
//        // 将存储多张图片的DataURL的数组传递给其他函数或发送到后端
//        console.log(imageDataURLs);
//    } else {
//        var noPictures = $("<p>目前沒有圖片</p>");
//        $("#preview-ImageFile").append(noPictures);
//    }
//}

// 上傳多張圖片進後端
//function sendImageDataURLsToBackend() {
//    console.log(imageDataURLs);
//    for (var i = 0; i < imageDataURLs.length; i++) {
//        $.ajax({
//            url: "/Creator/CreateContentExImg",
//            method: "post",

//            data: {
//                ImageURL: imageDataURLs[i],
//            },
//            success: function (response) {
//            },
//            error: function () {
//                alert("作品圖片上傳失敗");
//            }
//        })
//    }
//}

function getQuillContent() {
    // 拿到編輯器內容 Delta
    const QuillContent = quill.getContents();
    // Delta 轉 Json
    var DeltaJson = JSON.stringify(QuillContent);
    console.log(DeltaJson);
    return DeltaJson;
}