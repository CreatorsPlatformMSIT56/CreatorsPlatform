$(function () {
    // 上傳圖片並且預覽功能
    $("#progressbarTWInput").change(function () {

        readURL(this);

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

    // 讓參加活動的投稿的圖片與標題，點擊時可以顯示Modal
    $(".EventPostImgPart, .EventPostTitle").attr({ 'data-bs-toggle': 'modal', 'data-bs-target': '#EventPostModal' })

});
