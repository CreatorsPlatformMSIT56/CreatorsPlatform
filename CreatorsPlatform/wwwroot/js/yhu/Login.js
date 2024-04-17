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
                case "EmailCheck":
                    $("#EmailError").css("display", "block");
                    break;
                case "PasswdCheck":
                    $("#EmailError").css("display", "none");
                    $("#PasswdError").css("display", "block");
                    break;
                default:
                    //$("#MemberSection").empty();
                    //$("#MemberSection").append(`
                    //  <a href="/yhu/PersonalUser" class="MemberDropdown">
                    //  ${response != undefined ? `<img style="width: 65px; height: 65px;" src="data:image/png;base64,${response.avatar}" class="rounded-2" alt="...">`
                    //    :` <svg xmlns="http://www.w3.org/2000/svg" width="65" height="65" fill="currentColor" class="bi bi-person-circle" viewBox="0 0 16 16">
                    //            <path d="M11 6a3 3 0 1 1-6 0 3 3 0 0 1 6 0" />
                    //            <path fill-rule="evenodd" d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8m8-7a7 7 0 0 0-5.468 11.37C3.242 11.226 4.805 10 8 10s4.757 1.225 5.468 2.37A7 7 0 0 0 8 1" />
                    //        </svg>` } 
                    //    </a>
                    //    <ul class="MemberDropdownList">
                    //        <li><a  href="/yhu/Individual" class="p-1">設定</a></li>
                    //        <li><a  onclick="UserLogOut()">登出</a></li>
                    //    </ul>
                    //`);
                    window.location.href = '/yhu/PersonalUser';
            }
        }
    });
});


//切換密碼圖片
document.getElementById('showPasswordBtnLogin').addEventListener('click', function () {
    var passwordIcon = document.getElementById('passwordIconLogin');
    if (passwordIcon.src.includes('lockFill')) {
        passwordIcon.src = "/img/VickyImg/lockUnlock.png";
    } else {
        passwordIcon.src = "/img/VickyImg/lockFill.png";
    }
});

//顯示密碼
$(function () {
    $("#showPasswordBtnLogin").on("click", function () {
        var passwordField = $("#Passwd");
        var passwordError = $("#PasswdError");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            passwordError.hide();
        } else {
            passwordField.attr("type", "password");
        }
    });
});


//沒點擊時看見是密碼 點擊後會變成黑點點

function showPlainText(input) {
    if (input.value === '密碼') {
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