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

//顯示密碼
$(function () {
    $("#showPasswordBtn").on("click", function () {
        var passwordField = $("#UserPasswd");
        var passwordError = $("#PasswdError");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            passwordError.hide();
        } else {
            passwordField.attr("type", "password");
        }
    });
});

$(function () {
    $("#showPasswordCheckBtn").on("click", function () {
        var passwordFieldCheck = $("#UserPasswdCheck");
        var passwordError = $("#PasswdError");
        if (passwordFieldCheck.attr("type") === "password") {
            passwordFieldCheck.attr("type", "text");
            passwordError.hide();
        } else {
            passwordFieldCheck.attr("type", "password");
        }
    });
});

//沒點擊時看見是密碼 點擊後會變成黑點點

function showPlainText(input) {
    if (input.value === '密碼' || input.value === '確認密碼') {
        input.value = '';
        input.type = 'password';
        
    }
}

//沒點擊時看見是密碼 點擊後會變成黑點點
function hidePlainText(input) {
    if (input.value === '') {
        input.type = 'text';
        input.value = '密碼';
    }
}

function hidePlainCheckText(input) {
    if (input.value === '') {
        input.type = 'text';
        input.value = '確認密碼';
    }
}

//點擊切換圖片


document.getElementById('showPasswordBtn').addEventListener('click', function () {
    var passwordIcon = document.getElementById('passwordIcon');
    if (passwordIcon.src.includes('lockFill')) {
        passwordIcon.src = "/img/VickyImg/lockUnlock.png";
    } else {
        passwordIcon.src = "/img/VickyImg/lockFill.png";
    }
});

document.getElementById('showPasswordCheckBtn').addEventListener('click', function () {
    var passwordIcon = document.getElementById('passWordCheckIcon');
    if (passwordIcon.src.includes('lockFill')) {
        passwordIcon.src = "/img/VickyImg/lockUnlock.png";
    } else {
        passwordIcon.src = "/img/VickyImg/lockFill.png";
    }
});
