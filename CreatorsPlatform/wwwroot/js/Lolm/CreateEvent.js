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

}