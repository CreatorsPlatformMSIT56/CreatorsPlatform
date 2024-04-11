$("#Register").on("submit", function (e) {
    e.preventDefault();
    var UserData = $('#Register').serializeArray();
    let Passwd = UserData[2].value;
    let PasswdCheck = UserData[3].value;
    if (Passwd == PasswdCheck) {
        $.ajax({
            url: '/yhu/SignupData',
            method: 'POST',
            data: JSON.stringify(UserData),
            contentType: 'application/json',
            success: function (response) {
                switch (response) {
                    case "Email&NameCheck":
                        $("#EmailError").css("display", "block");
                        $("#UserNameError").css("display", "block");
                        break;
                    case "NameCheck":
                        $("#UserNameError").css("display", "block");
                        break;
                    case "EmailCheck":
                        $("#EmailError").css("display", "block");
                        break;
                    case "CheckOk":
                        $("#Userlist").css("display", "none");
                        $("#UserAdd").css("display", "block");   
                        let Name = $("#UserName").val();
                        $("#NewUser").text(`歡迎  ${Name}!!`);
                        break;
                    default:
                        // 如果没有任何情况匹配，则执行默认操作
                        break;
                }
            }
        });
    } else {
        $("#PasswdError").css("display", "block");
    };
   
});

$(function () {
    $("#showPasswordBtn").on("click", function () {
        var passwordField = $("#UserPasswd");
        var passwordFieldCheck = $("#UserPasswdCheck");
        var passwordError = $("#PasswdError");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            passwordError.hide();
        } else {
            passwordField.attr("type", "password");
        }
        if (passwordFieldCheck.attr("type") === "password") {
            passwordFieldCheck.attr("type", "text");
            passwordError.hide();
        } else {
            passwordFieldCheck.attr("type", "password");
        }
    });
});