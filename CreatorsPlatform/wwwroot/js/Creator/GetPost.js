var userName = UserName;
var membersIcon = MembersIcon;
console.log(userName);
console.log(membersIcon);

$(".LikeBtn").on("click", function () {
    
    //console.log("ContentId: ",ContentId);
    $(this).toggleClass("LikeChecked");
    if ($(this).hasClass("LikeChecked")) {
        $(this).children('span').text(parseInt($(this).children('span').text()) + 1);
    } else {
        $(this).children('span').text(parseInt($(this).children('span').text()) - 1);
    }
    var PostLike = parseInt($(this).children('span').text());
    console.log(PostLike);
    $.ajax({
        url: "/Creator/PostLikeChange",
        method: "POST",
        data: { LikeChange: PostLike, TheCheckedPostId: ContentId },
        success: function () {
            //alert('OK');
        },
        error: function () {
            alert('按愛心失敗');
        }
    });
});


function Follow() {
    //console.log(123);
    if (UserId == 0) {
        alert('請先登入');
        window.location.href = '../../../yhu/Login';
    } else {
        $(".FollowBtn").prop("style", "display:none;");
        $(".UnFollowBtn").prop("style", "display:block;");
        FollowAjax();
    }
}

function UnFollow() {
    $(".UnFollowBtn").prop("style", "display:none;");
    $(".FollowBtn").prop("style", "display:block;");
    FollowAjax();
}

// 關注的ajax
function FollowAjax() {
    $.ajax({
        url: "/Creator/FollowCreator",
        method: "post",
        data: { TheUserId: UserId, TheCreatorId: CreatorId },
        success: function () {
            //alert('ok');
        },
        error: function () {
            alert('fail');
        }
    });
}

// 發送留言AJAX
function PostCommentToSQL() {
    var commentData = {
        //Comment1: "HUH",
        Comment1: $("#comment-text").val(),
        UserId: UserId,
        ContentId: ContentId
    }
    console.log(commentData);
    $.ajax({
        url: "/Creator/PostComment",
        method: "post",
        data: commentData,
        success: function () {
            alert('ok');
            var newPostHtml = `
                    <div class="card mb-3" style="border-color: rgba(0, 0, 0, 0);">
                    <div class="row g-0">
                        <div class="col-md-2 avatar text-center">
                            <img src="data:image/png;base64,${MembersIcon}" alt="" class="img-fluid rounded-circle"
                                 style="max-width:100px; max-height:100px;">
                        </div>
                        <div class="col-md-10">
                            <div class="card-body">
                                <h5 class="card-title mb-1">${UserName}</h5>
                                <p class="card-text mb-2">
                                    ${commentData.Comment1}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            $(".new-comment-pop").append(newPostHtml);
        },
        error: function () {
            alert('fail');
        }
    });
}