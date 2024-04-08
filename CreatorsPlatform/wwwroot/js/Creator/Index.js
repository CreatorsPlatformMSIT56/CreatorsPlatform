console.log(123);
$(function Follow() {
    console.log(123);
    if (MembersOnline) {
        $("#FollowBtn").prop("style", "display:none;");
        $(".NameAndFollow").append(`<button class="btn ms-3 btn-primary" id="UnFollowBtn">Followed</button>`);
    } else {
        window.location.href = '../../../yhu/Login';
    }
});
