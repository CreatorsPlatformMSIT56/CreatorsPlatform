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
            commentGet(); // 實時顯示留言還沒寫
        },
        error: function () {
            alert('fail');
        }
    });
}