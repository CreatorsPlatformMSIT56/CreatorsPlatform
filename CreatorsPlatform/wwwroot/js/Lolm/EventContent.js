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

    PostSet();

    //PostMadl上的愛心按鈕
    $(".LikeModalBtn").on("click", function () {
        var CheckedPostId = parseInt(NowCheckedPost);
        var TheSameSmallPostId = "#Post" + NowCheckedPost;
        console.log(TheSameSmallPostId);
        $(TheSameSmallPostId).find(".LikeBtn").toggleClass("LikeChecked");
        $(this).toggleClass("LikeChecked");
        if ($(this).hasClass("LikeChecked")) {
            $(this).children('p').text(parseInt($(this).children('p').text()) + 1);
            $(TheSameSmallPostId).find(".LikeBtn").children('p').text(parseInt($(TheSameSmallPostId).find(".LikeBtn").children('p').text()) + 1);
        } else {
            $(this).children('p').text(parseInt($(this).children('p').text()) - 1);
            $(TheSameSmallPostId).find(".LikeBtn").children('p').text(parseInt($(TheSameSmallPostId).find(".LikeBtn").children('p').text()) - 1);
        }
        var PostLike = parseInt($(this).children('p').text());
        //console.log(PostLike);
        $.ajax({
            url: "/Lolm/PostLikeChange",
            method: "POST",
            data: { LikeChange: PostLike, TheCheckedPostId: CheckedPostId },
            success: function () {
                alert('OK');
            },
            error: function () {
                alert('按愛心失敗');
            }
        });
    });
});

// 排序功能
$("#PostOrderBy").change(PostOrderBy);

function PostOrderBy() {
    var OderByOption = $(this).val();
    $.ajax({
        url: "/Lolm/PostOrderBy",
        method: "get",
        data: { OrderByWhat: OderByOption, id: id },
        success: function (Response) {
            var htmlContent = "";

            // 遍历响应数据并生成 HTML 内容
            Response.forEach(function (item) {
                if (item.imageUrl != null && item.imageSample == false) {
                    htmlContent += '<div class="col EventPost" id="Post' + item.eventImageId + '">' +
                        '<div class="EventPostImgPart">' +
                        '<div><img class="img-fluid p-1" src="' + item.imageUrl + '" /></div>' +
                        '</div>' +
                        '<div class="d-flex justify-content-between align-items-start px-1">' +
                        '<div><span class="EventPostTitle"><b>' + item.imgTitle + '</b></span></div>' +
                        '<div class="d-flex align-items-center LikeBtn">' +
                        '<div class="d-flex justify-content-center align-items-center me-2">' +
                        '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-heart" viewBox="0 0 16 16">' +
                        '<path d="m8 2.748-.717-.737C5.6.281 2.514.878 1.4 3.053c-.523 1.023-.641 2.5.314 4.385.920 1.815 2.834 3.989 6.286 6.357 3.452-2.368 5.365-4.542 6.286-6.357.955-1.886.838-3.362.314-4.385C13.486.878 10.4.28 8.717 2.01zM8 15C-7.333 4.868 3.279-3.04 7.824 1.143q.09.083.176.171a3 3 0 0 1 .176-.17C12.72-3.042 23.333 4.867 8 15" />' +
                        '</svg>' +
                        '</div>' +
                        '<p class="mb-0">' + item.evePostLike + '</p>' +
                        '</div>' +
                        '</div>' +
                        '<div class="d-flex align-items-center px-1 EventParticipant">' +
                        '<a class="d-flex align-items-center text-secondary" href="#"><img class="SmallSizeHeadShot" src="data:image/*;base64,' + item.imgCreAvatar + '" alt="Alternate Text" />' + item.imgCreName + '</a>' +
                        '</div>' +
                        '</div>';
                }
            });

            // 将生成的 HTML 内容设置到 .EventPostSection 容器中
            $(".EventPostSection").html(htmlContent);

            PostSet();
        },
        error: function () {
            alert('Order By fail');
        }
    })
}

function PostSet() {
    // 讓參加活動的投稿的圖片與標題，點擊時可以顯示Modal
    $(".EventPostImgPart, .EventPostTitle").attr({ 'data-bs-toggle': 'modal', 'data-bs-target': '#EventPostModal' });

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
                EventPostId: ThePostId
            },
            success: function (ThePostModel) {
                $("#exampleModalLabel").text(ThePostModel.imgTitle);
                $("#PostDescription").text(ThePostModel.imgDes);
                $(".EventPostModalImg").prop("src", e => (e = ThePostModel.imageUrl));
                //var PostCreatorAvatar64 = hexToDataURL(ThePostModel.imgCreAvatar);
                $("#PostCreatorAvatar").prop("src", e => (e = "data: image / jpg; base64," + ThePostModel.imgCreAvatar));
                $("#PostCreatorName").text(ThePostModel.imgCreName);
                $("#PostLikeInt").text(ThePostModel.evePostLike);
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
        // 更新Modal內容時，如果有檢查有沒有按過讚，套用樣式(愛心數不用改，包含在上一行了))
        if ($(`#Post${NowCheckedPost}`).find(".LikeBtn").hasClass("LikeChecked")) {
            $(".LikeModalBtn").addClass("LikeChecked");
        } else {
            $(".LikeModalBtn").removeClass("LikeChecked");
        }
    });


    // Post上的愛心按鈕
    $(".LikeBtn").on("click", function () {
        var CheckedPostId = parseInt(WhatPost(this));
        $(this).toggleClass("LikeChecked");
        if ($(this).hasClass("LikeChecked")) {
            $(this).children('p').text(parseInt($(this).children('p').text()) + 1);
        } else {
            $(this).children('p').text(parseInt($(this).children('p').text()) - 1);
        }
        var PostLike = parseInt($(this).children('p').text());
        //console.log(PostLike);
        $.ajax({
            url: "/Lolm/PostLikeChange",
            method: "POST",
            data: { LikeChange: PostLike, TheCheckedPostId: CheckedPostId },
            success: function () {
                //alert('OK');
            },
            error: function () {
                alert('按愛心失敗');
            }
        });
    });

}