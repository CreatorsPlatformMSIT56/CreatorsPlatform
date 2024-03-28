
function ReviseON() {
    $(".ReviseShow").css("display", "block");
    $(".DataShow").css("display", "none");
};
function ReviseOFF() {
    $(".ReviseShow").css("display", "none");
    $(".DataShow").css("display", "block");
};

function readFile(event) {
    const input = event.target;
    const file = input.files[0];
    if (!input || !file) { 
        return;
    }
        
     const FR = new FileReader();
     FR.addEventListener("load", function (evt) {
        let bit64 = evt.target.result;
        
         let base64format = bit64.substring(bit64.indexOf(',') + 1);
         
        $.ajax({
            url: '/yhu/IndividualAvatarUp',
            method: 'POST',
            data: {
                base64: base64format
            },
            success: function (response) {
                $("#Avatar").attr("src", "data:image/png;base64," + response);
            }
        });
    });

    FR.readAsDataURL(file);
}

$("#UserData").on("submit", function (e) {
    e.preventDefault();
    var UserData = $('#UserData').serializeArray();
    console.log(UserData);
    $.ajax({
        url: '/yhu/IndividualDataUP',
        method: 'POST',
        data: JSON.stringify(UserData),
        contentType: 'application/json',
        success: function (response) {
            $("#UserEMail").val(response.email);
            $("#UserName").val(response.name);  
        }
    })
});
function GeneralSettingsReadData() {
    $("#AuthorSettingsDetailed").css("display", "none");
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "GeneralSettings"
        },
        success: function (response) {
            $("#Avatar").attr("src", "data:image/png;base64," + response.avatar);
            $("#UserEMail").val(response.email);
            $("#UserName").val(response.name);  
        }
    });
};
function ConsumptionRecordReadData() {
    $("#AuthorSettingsDetailed").css("display", "none");
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord"
        },
        success: function (response1) {
            let cont = 0;
            $("#ConsumptionRecord_Plan").empty();
            response1.forEach(function () {
                $("#ConsumptionRecord_Plan").addend(`
              <tr>
                        <th scope="row">${cont}</th>
                        <td>${response1[cont].PlanName}</td>
                        <td>${response1[cont].UserName}</td>
                        <td>${response1[cont].Description}</td>
                        <td>${response1[cont].PlanLevel}</td>
                        <td>${response1[cont].PlanPrice}</td>
                        <td>${response1[cont].EndDate}</td>
             </tr>
            `)
                cont++;
            });
          
        }
    });
};
function AuthorSettingsReadData() {
    $("#AuthorSettingsDetailed").css("display", "block");
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "AuthorSettings"
        },
        success: function (response) {
            let cont = 0;
            $("#AuthorSettings_List").empty();
            response.forEach(function () {
                $("#AuthorSettings_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].planName}</td>
                                <td scope="col">${response[cont].description}</td>
                                <td scope="col">${response[cont].planLevel}</td>
                                <td scope="col">${response[cont].planPrice}</td>
                                <td> 
                                    <div id="Plan${response[cont].planLevel}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
                                          <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                                          </svg>
                                    </div>
                              </td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
};
function PlanDataAddShow() {
    $("#UserPlanForm").css("display", "block");
    $("#PlanDataAddButton").css("display", "none");
};
function PlanDataAddClosure() {
    $("#UserPlanForm").css("display", "none");
    $("#PlanDataAddButton").css("display", "flex");
    $("#PlanDataAdd")[0].reset();
};
$("#PlanDataAdd").on("submit", function (e) {
    e.preventDefault();
    var PlanDataAdd = $('#PlanDataAdd').serializeArray();
    console.log(PlanDataAdd);
    $.ajax({
        url: '/yhu/PlanDataAdd',
        method: 'POST',
        data: JSON.stringify(PlanDataAdd),
        contentType: 'application/json',
        success: function (response) {
            $('#AuthorSettings_List').find('tr').eq(response.level - 1).empty();
            $('#AuthorSettings_List').find('tr').eq(response.level - 1)
                .append(`
                                <th scope="col">${response.level}</th>
                                <td scope="col">${response.name}</td>
                                <td scope="col">${response.description}</td>
                                <td scope="col">${response.level}</td>
                                <td scope="col">${response.price}</td>   
                    `);
        }
    })
});

function DataDelete(e) {
    console.log(e.id);
    var match = e.id.match(/\d+$/);

    if (match !== null) {
        // 提取到的数字字符串
        var numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        var Category = input.substring(0, match.index);
        
    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/DataDelete',
        method: 'POST',
        data: {
            type: Category,
            id: TargetID
        },
        success: function (response) {
            switch (Category) {
                case "Plan":
                    break;
            }
        }
    });
};
function WorkRead() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "WorkRead"
        },
        success: function (response) {
            console.log(response[0]);
            let cont = 0;
            $("#Works_List").empty();
            response.forEach(function () {
                $("#Works_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].planName}</td>
                                <td scope="col">${response[cont].description}</td>
                                <td scope="col">${response[cont].planLevel}</td>
                                <td scope="col">${response[cont].planPrice}</td>
                                <td> 
                                    <div id="Plan${response[cont].planLevel}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
                                          <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                                          </svg>
                                    </div>
                              </td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
};
function OrderRead() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData1"
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            $("#Works_List").empty();
            //response.forEach(function () {
            //    $("#Works_List").append(`
            //             <tr>
            //                    <th scope="col">${cont + 1}</th>
            //                    <td scope="col">${response[cont].planName}</td>
            //                    <td scope="col">${response[cont].description}</td>
            //                    <td scope="col">${response[cont].planLevel}</td>
            //                    <td scope="col">${response[cont].planPrice}</td>
            //                    <td> 
            //                        <div id="Plan${response[cont].planLevel}" onclick="DataDelete(this) ">
            //                              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
            //                              <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
            //                              </svg>
            //                        </div>
            //                  </td>
            //            </tr>
            //        `);
            //    cont++;
            //});
        }
    });
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2"
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            $("#Works_List").empty();
            //response.forEach(function () {
            //    $("#Works_List").append(`
            //             <tr>
            //                    <th scope="col">${cont + 1}</th>
            //                    <td scope="col">${response[cont].planName}</td>
            //                    <td scope="col">${response[cont].description}</td>
            //                    <td scope="col">${response[cont].planLevel}</td>
            //                    <td scope="col">${response[cont].planPrice}</td>
            //                    <td> 
            //                        <div id="Plan${response[cont].planLevel}" onclick="DataDelete(this) ">
            //                              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
            //                              <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
            //                              </svg>
            //                        </div>
            //                  </td>
            //            </tr>
            //        `);
            //    cont++;
            //});
        }
    });
};
