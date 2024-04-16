// 定義空陣列放資料
var base64Data = [];
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

    //console.log('uint8ArrayData[0]', typeof uint8ArrayData[0]);
    //console.log('Title', typeof $("#postTitle").val());
    //console.log('ImageUrl: ', typeof $("#ImageFile")[0].files[0]);
    //console.log('SubtitleId', SubNameToId);
    //console.log('Description', getQuillContent());

    var PostToData = {
        Title: $("#postTitle").val(), // 標題
        CategoryIdstring: $("#categorySelect").val(), // 主分類
        SubtitleId: SubNameToId, // 子分類
        Imagebase64: base64Data[0], // 取用base64 string
        Description: getQuillContent() // 作品描述
    }
    /*console.clear();*/
    console.log('PostToData: ', PostToData);
    $.ajax({
        url: "/Creator/CreatePost",
        method: "post",
        data: PostToData,
        success: function (response) {
            //sendImageDataURLToBackend();
            console.log(response);
            alert("作品發布成功");
            window.location.href = `/Creator/GetPost/${response.contentId}`;
        }
    });
}

function previewImage() {
    const fileInput = document.getElementById('ImageFile');
    const previewImage = document.getElementById('preview-ImageFile');
    
    if (fileInput.files && fileInput.files[0]) {
        const reader = new FileReader();
        // 預覽圖片
        reader.onload = function (e) {
            previewImage.src = e.target.result;

            // 先轉 base64 進後端 再轉換為 byte[]
            var base64 = e.target.result.split(',')[1];
            base64Data.push(base64);
            console.log(base64Data);
        }
        reader.readAsDataURL(fileInput.files[0]);
    }
    //console.log('imageFileData: ', imageFileData);
    //console.log('uint8ArrayData: ', uint8ArrayData);
    //console.log('uint8ArrayData[0]: ', uint8ArrayData[0]);

}

function getQuillContent() {
    // 拿到編輯器內容 Delta
    const QuillContent = quill.getContents();
    // Delta 轉 Json
    var DeltaJson = JSON.stringify(QuillContent);
    console.log(DeltaJson);
    return DeltaJson;
}