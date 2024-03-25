$("#LoginList").on("submit", function (e) {
    e.preventDefault();
    var UserData = $('#LoginList').serializeArray();
    $.ajax({
        url: '/yhu/Login',
        method: 'POST',
        data: JSON.stringify(UserData),
        contentType: 'application/json',
        success: function (response) {
            switch (response) {
                case "PersonalUser":
                    window.location.href = "/yhu/PersonalUser";
                    break;
                case "EmailCheck":
                    $("#EmailError").css("display", "block");
                    break;
                case "PasswdCheck":
                    $("#EmailError").css("display", "none");
                    $("#PasswdError").css("display", "block");
                    break;
                default:
                    // 如果没有任何情况匹配，则执行默认操作
                    break;
            }
        }
    });
});
