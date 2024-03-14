
var CurrentMsg = 0;
var UserCurrentMsgtype = "newmsg";
var _UserCurrentMsgtype = "newmsg";
var IconRecordOld = "NewMesg";

function MessagUpdata(UserCurrentMsgtype){
    CurrentMsg = 0;
    $("#Messages").empty();
    $.ajax({
        url: '/yhu/PersonalUser',
        method: 'POST',
        data: {
            _CurrentMsg: CurrentMsg,
            tapy:UserCurrentMsgtype,
        }, success: function (response) {
            for (i = CurrentMsg; i < response.length; i++) {
                let MsgHtml =
                    `<li>
             <p>${response[i].nickname}</p>
             <p>${response[i].description}</p>
             <img src="${response[i].imageURL}" alt="">
             <p>${response[i].title}</p>
            </li>`;
                $('#Messages').append(MsgHtml);
            }
        }
    })
    
};
function Messagloading(UserCurrentMsgtype, CurrentMsg) {
    CurrentMsg += 2;
    $.ajax({
        url: '/yhu/PersonalUser',
        method: 'POST',
        data: {
            _CurrentMsg: CurrentMsg,
            tapy: UserCurrentMsgtype
        },
        success: function (response) {
            for (i = CurrentMsg; i < response.length; i++) {
                let MsgHtml =
                    `<li>
             <p>${response[i].nickname}</p>
             <p>${response[i].description}</p>
             <img src="${response[i].imageURL}" alt="">
             <p>${response[i].title}</p>
            </li>`;
                $('#Messages').append(MsgHtml);
            }
        }
    });
};
$("#Sidebar").on("mouseover",
    function () {
        $("#Main").removeClass("MainClose").addClass("MainOpen");
        $("#Sidebar").removeClass("SidebarClose").addClass("SidebarOpen");
    }
);
$("#Sidebar").on("mouseout",
    function () {
        $("#Main").removeClass("MainOpen").addClass("MainClose");
        $("#Sidebar").removeClass("SidebarOpen").addClass("SidebarClose");
    }
);

//$("#Sidebar").hover(
//    function Open() {
//        $("#Main").removeClass().addClass("MainOpen");
//        $("#Sidebar").removeClass().addClass("SidebarOpen");
//    }, function Close() {
//        $("#Main").removeClass().addClass("MainClose");
//        $("#Sidebar").removeClass().addClass("SidebarClose");
//    }
//);
$("#NewMesg").on("mouseover",
    function () {
        $("#NewMesgIcon").css("fill", "#3498DB");
    }
);
$("#NewMesg").on("mouseout",
    function () {
        $("#NewMesgIcon").css("fill", "currentColor");
    }
);
$("#SubscriptionMesg").on("mouseover",
    function () {
        $("#SubscriptionMesgIcon").css("fill", "#FADADD");
    });
$("#SubscriptionMesg").on("mouseout",
    function () {
        $("#SubscriptionMesgIcon").css("fill", "currentColor");
    });
$("#Activity").on("mouseover",
    function () {
        $("#ActivityIcon").css("fill", "#FFDAB9");
    });
$("#Activity").on("mouseout",
    function () {
        $("#ActivityIcon").css("fill", "currentColor");
    });
function UserIconColor(ToggleIcon) {
    let Color;
    $("#" + IconRecordOld).removeClass("border-right-0");
    $("#" + ToggleIcon).addClass("border-right-0");
    switch (ToggleIcon) {
        case "NewMesg":
            Color = "#3498DB";
            break;
        case "SubscriptionMesg":
            Color = "#FADADD";
            break;
        case "Activity":
            Color = "#FFDAB9";
            break;
    }


    $("#" + IconRecordOld + "Icon").css("fill", "currentColor");
    $("#" + ToggleIcon + "Icon").css("fill", Color);

    IconRecordOld = ToggleIcon;
};

    //
    // IconFill(ToggleIcon) {
   
    //});
/*};*/
MessagUpdata(_UserCurrentMsgtype, CurrentMsg);

//$("#Main").css({
//    "transition": "0.5s",
//    "transform": "scaleX(0.8)",
//    "transform-origin": "left"
//});
//$("#Sidebar").css({
//    "transition": "0.5s",
//    "transform": "scaleX(1)"
//});
//$("#Main").css({
//    "transition": "0.5s",
//    "transform": "scaleX(1)",
//    "transform-origin": "left"
//});
//$("#Sidebar").css({
//    "transition": "0.3s",
//    "transform": "scaleX(0.1)"
//});
