var ImgDataURL;
var EventIdSaver = EventIdStocker; 
$(function () {
    // 上傳圖片並且預覽功能
    $("#progressbarTWInput").change(function () {

        readURL(this);

    });
    function readURL(input) {

        if (input.files && input.files[0]) {

            var reader = new FileReader();

            reader.onload = function (e) {
                ImgDataURL = e.target.result;
                $("#preview_progressbarTW_img").attr('src', ImgDataURL);

            }

            reader.readAsDataURL(input.files[0]);

        }

    }

    // 讓參加活動的投稿的圖片與標題，點擊時可以顯示Modal
    $(".EventPostImgPart, .EventPostTitle").attr({ 'data-bs-toggle': 'modal', 'data-bs-target': '#EventPostModal' });

    //Quill設定
    /*< !--Initialize Quill editor-- >*/
    

    // 參加活動投稿OKbtn
    $("#okButton").on("click", function () {
        var EventPostData = {
            ImageUrl: ImgDataURL,
            ImageSample: false,
            Description: $("#EventPostContentTextBox").val(),
            ImageTitle: $("#titleTextBox").val(),
            EventID: EventIdSaver
        }
        $.ajax({
            url: "/Lolm/CreateEventPost",
            method: "post",
            data: EventPostData,
            success: function (response) {
                alert(response);
            },
            error: function () {
                alert('新增失敗');
            }
        })
    })
});