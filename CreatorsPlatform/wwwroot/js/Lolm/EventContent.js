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

    // 參加活動投稿OKbtn
    $("#okButton").on("click", function () {
        var EventPostData = {
            ImageUrl: ImgDataURL,
            ImageSample: 0,
            Description: $("#EventPostContentTextBox").val(),
            ImageTitle: $("#titleTextBox").val(),
            EventID: EventIdSaver
        }
        $.ajax({
            url: "/Lolm/CreateEventPost",
            method: "post",
            data: EventPostData,
            success: function (response) {
                location.reload();
            },
            error: function () {
                alert('新增失敗');
            }
        })
    })

    // 抓點的是哪個投稿
    function WhatPost(InputEle) {
        var ThatPostPartId = $(InputEle).closest(".EventPost").prop("id");
        var ThePostIdString = ThatPostPartId.substring(4);
        return ThePostIdString;
    }

    // Modal更新 
    function RefrashPostModal(TheEle) {
        var ThePostId = WhatPost(TheEle);
        $.ajax({
            url: "/Lolm/EventPostContent",
            method: "get",
            data: {
                EventAndImgId: ThePostId
            },
            success: function (ThePostModel) {
                $("#exampleModalLabel").text(ThePostModel.imgTitle);
                $("#PostDescription").text(ThePostModel.imgDes);
                $(".EventPostModalImg").prop("src", e => (e = ThePostModel.imageUrl));
                //var PostCreatorAvatar64 = hexToDataURL(ThePostModel.imgCreAvatar);
                $("#PostCreatorAvatar").prop("src", e => (e = "data: image / jpg; base64," + ThePostModel.imgCreAvatar));
                $("#PostCreatorName").text(ThePostModel.imgCreName);
                $("#PostLikeInt").text(ThePostModel.evePostLike)
            },
            error: function () {
                alert("讀取失敗");
            }
        });
        return ThePostId;
    }

    // 當前點擊的post的id
    var NowCheckedPost;

    // 點擊投稿讓Modal內容更新
    $(".EventPostImgPart, .EventPostTitle").on("click", function () {
        NowCheckedPost = RefrashPostModal(this);
    });


    // 愛心按鈕
    $(".LikeBtn").on("click", function () {
        var CheckedPostId = WhatPost(this);
        $(".LikeBtn").toggleClass("LikeChecked");
        if ($(".LikeBtn").hasClass("LikeChecked")) {

        }
    });
});
