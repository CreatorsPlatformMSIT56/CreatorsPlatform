function Follow() {
    //console.log(123);
    if (UserId == 0) {
        alert('請先登入');
        window.location.href = '../../../yhu/Login';
    } else {
        $(".FollowBtn").prop("style", "display:none;");
        $(".NameAndFollow").append('<button class="btn btn-primary UnFollowBtn" onclick="UnFollow()">關注中</button>');
        FollowAjax();
    }
}

function UnFollow() {
    $(".UnFollowBtn").prop("style", "display:none;");
    $(".NameAndFollow").append('<button class="btn FollowBtn" onclick="Follow()">關注</button>');
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
