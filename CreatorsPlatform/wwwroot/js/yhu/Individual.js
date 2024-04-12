
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
            $("#UserEMailText").text(response.email);
            $("#UserNameText").text(response.name);
            $(".ReviseShow").css("display", "none");
            $(".DataShow").css("display", "block");
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
            if (response.avatar != undefined) {
                $("#Avatar").attr("src", "data:image/png;base64," + response.avatar);
            } else {
                $("#Avatar").attr("src", "/img/Shared/person-circle.svg");
            }
           
            $("#UserEMail").val(response.email);
            $("#UserName").val(response.name);  
        }
    });
};
function ConsumptionRecordReadData() {
    $("#AuthorSettingsDetailed").css("display", "none");
    PlanCons();
    $("#ConsumptionRecord_Entrust").empty();
    Order1();
    Order2();
    Order3();
    Order4();
    Order5();
    Order6();
};
function PlanCons() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 1
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            $("#ConsumptionRecord_Plan").empty();
            response.forEach(function () {
                $("#ConsumptionRecord_Plan").append(`
              <tr>
                        <th scope="row">${(cont + 1)}</th>
                        <td>${response[cont].planName}</td>
                        <td>${response[cont].userName}</td>
                        <td>${response[cont].description}</td>
                        <td>${response[cont].planLevel}</td>
                        <td>${response[cont].planPrice}</td>
                        <td>${response[cont].endDate}</td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function Order1() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 2
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr class="${response[cont].commissionId}">
                        <th scope="row">${(cont + 1)}</th>
                        <td>${response[cont].title}</td>
                        <td>${response[cont].userName}</td>
                        <td>待確認</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>        
                        <td> </td>
             </tr>
            `)
                cont++;
            });
        }
    });
};
function Order2() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 3
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr>
                        <th scope="row">${cont}</th>
                        <td>${response[cont].title}</td>
                        <td>${response[cont].userName}</td>
                        <td>${response[cont].price}</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>
                        <td>
                                   <div onclick="StatusReplyOptions(this)"  id="Order${response[cont].commissionOrderId}" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                             查看
                                   </div>
                                   <div id="Order${response[cont].commissionOrderId}Des" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                             ${response[cont].description}
                                   </div>
                                     <div id="Order${response[cont].commissionOrderId}price" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                             ${response[cont].price}
                                   </div>
                       </td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function Order3() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 4
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr class="${response[cont].commissionId}">
                        <th scope="row">${cont}</th>
                        <td>${response[cont].title}</td>
                         <td>${response[cont].userName}</td>
                        <td>${response[cont].price}</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>
                        <td> </td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function Order4() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 5
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr class="${response[cont].commissionId}">
                        <th scope="row">${cont}</th>
                        <td>${response[cont].title}</td>
                         <td>${response[cont].userName}</td>
                        <td>${response[cont].price}</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>
                        <td> </td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function Order5() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 6
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr class="${response[cont].commissionId}">
                        <th scope="row">${cont}</th>
                        <td>${response[cont].title}</td>
                         <td>${response[cont].userName}</td>
                        <td>${response[cont].price}</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>
                        <td> </td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function Order6() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord",
            step: 7
        },
        success: function (response) {
            let cont = 0;
            response.forEach(function () {
                $("#ConsumptionRecord_Entrust").append(`
              <tr class="${response[cont].commissionId}">
                        <th scope="row">${cont}</th>
                        <td>${response[cont].title}</td>
                         <td>${response[cont].userName}</td>
                        <td>${response[cont].price}</td>
                        <td>${response[cont].workStatus}</td>
                        <td>${response[cont].orderDate}</td>
                        <td> </td>
             </tr>
            `)
                cont++;
            });

        }
    });
};
function StatusReplyOptions(e) {

    $("#StatusReply").empty();
    $("#OrderDescriptionAuthor").empty();
    let Description = "#"+e.id +"Des"
    let DescriptionText = $(Description).text();
    let price = "#" + e.id + "price"
    let priceText = $(price).text();
    $("#OrderDescriptionAuthor").append(`
       <p>${DescriptionText}</p>
        <hr>
       <p class="text-end">$${priceText}</p>
    `);
    $("#StatusReply").append(`
       <button onclick="FanStatusReply('${e.id}','false')" type="button" class="btn btn-secondary" data-bs-dismiss="modal">拒絕</button>
       <button onclick="FanStatusReply('${e.id}','true')" type="button" class="btn btn-primary" data-bs-dismiss="modal">接受</button>
    `);
};
function FanStatusReply(x, y) {
    let match = x.match(/\d+$/);

    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = x;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/FanStatusReply',
        method: 'POST',
        data: {
            id: TargetID,
            Reply: y
        },
        success: function (response) {
            $("#ConsumptionRecord_Entrust").empty();
            Order1();
            Order2();
            Order3();
            Order4();
            Order5();
            Order6();
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
            $("#WorkData").addClass("d-none");
            $("#OrderData").addClass("d-none");
            $("#EventData").addClass("d-none");
            $("#PlanData").removeClass("d-none");
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
                                     <div class="Plan${response[cont].planId}" onclick="PlanDataChangeShow(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Plan${response[cont].planId}" onclick="DataDelete(this) ">
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
function PlanDataChangeShow(e) {
    let match = e.className.match(/\d+$/);

    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = e.className;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $("#UserPlanForm").css("display", "block");
    $("#PlanDataAddButton").css("display", "none");
    $.ajax({
        url: '/yhu/PlanRowRecord',
        method: 'POST',
        data: {
            type: Category,
            id: TargetID
        },
        success: function (response) {
            console.log(response);
            $("#PlanName").val(response[0].planName);
            $("#PlanPrice").val(response[0].planPrice);
            $("#PlanLevel").val(response[0].planLevel);
            $("#PlanDescription").val(response[0].description);
        }
    })
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
            console.log(response);
            let cont = 0;
            $('#AuthorSettings_List').empty();
            response.forEach(function () {
                $('#AuthorSettings_List')
                    .append(`
                        <tr>
                                <th scope="col">${response[cont].planLevel}</th>
                                <td scope="col">${response[cont].planName}</td>
                                <td scope="col">${response[cont].description}</td>
                                <td scope="col">${response[cont].planLevel}</td>
                                <td scope="col">${response[cont].planPrice}</td>
                                <td>
                                     <div class="Plan${response[cont].planId}" onclick="PlanDataChangeShow(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Plan${response[cont].planId}" onclick="DataDelete(this) ">
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
    })
});

function DataDelete(e) {

    let match = e.className.match(/\d+$/);

    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = e.className;
        var Category = input.substring(0, match.index);
        /*alert(Category);*/
        
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
            console.log(response);
            switch (Category) {
                case "Plan":
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
                                     <div class="Plan${response[cont].planId}" onclick="PlanDataChangeShow(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Plan${response[cont].planId}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
                                          <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                                          </svg>
                                    </div>
                              </td>
                        </tr>
                    `);
                        cont++;
                    });
                    break;

                case "Works":
                    let Workcont = 0;
                    $("#Works_List").empty();
                    response.forEach(function () {
                        $("#Works_List").append(`
                         <tr>
                                <th scope="col">${Workcont + 1}</th>
                                <td scope="col">${response[Workcont].title}</td>
                                <td scope="col">${response[Workcont].description}</td>
                                <td scope="col">${response[Workcont].planLevel}</td>
                                <td>
                                     <div class="Works${response[Workcont].contentId}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Works${response[Workcont].contentId}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
                                          <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                                          </svg>
                                    </div>
                              </td>
                        </tr>
                    `);
                        Workcont++;
                    });
                    break;

                case " OrderOne":
                    break;
                case " OrderTow":
                    break;
                case "Event":
                    let Evecont = 0;
                    console.log(response);
                    $("#Event_List").empty();
                    response.forEach(function () {
                        $("#Event_List").append(`
         <tr>
                <th scope="col">${Evecont + 1}</th>
                <td scope="col">${response[Evecont].eventName}</td>
                <td scope="col">${response[Evecont].startDate}</td>
                <td scope="col">${response[Evecont].endDate}</td>
                <td>
                     <div class="Event${response[Evecont].eventId}" onclick="DataDelete(this) ">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                          </svg>
                    </div>
                </td>
                <td> 
                    <div class="Event${response[Evecont].eventId}" onclick="DataDelete(this) ">
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
                          <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
                          </svg>
                    </div>
              </td>
        </tr>
    `);
                        Evecont++;
                    });
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
            type: "WorkData"
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            $("#PlanData").addClass("d-none");
            $("#OrderData").addClass("d-none");
            $("#EventData").addClass("d-none");
            $("#WorkData").removeClass("d-none"); 
            $("#Works_List").empty();
            response.forEach(function () {
                $("#Works_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].description}</td>
                                <td scope="col">${response[cont].planLevel}</td>
                                <td>
                                     <div class="Works${response[cont].contentId}" onclick="">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Works${response[cont].contentId}" onclick="DataDelete(this) ">
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
    $("#PlanData").addClass("d-none");
    $("#EventData").addClass("d-none");
    $("#WorkData").addClass("d-none"); 
    $("#OrderData").removeClass("d-none"); 
    Order1Read();
    $("#EntrustOrders_List").empty();
    Order2Read1();
    Order2Read2();
    Order2Read3();
    Order2Read4();
    Order2Read5();
    Order2Read6();
};
function Order1Read() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData1"
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            $("#Entrust_List").empty();
            response.forEach(function () {
                $("#Entrust_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title != null ? response[cont].title : "未設定"}</td>
                                <td scope="col">${response[cont].description != null ? response[cont].description : "未設定"}</td>
                                <td scope="col">${response[cont].priceMin != null ? response[cont].priceMin : "未設定"}</td>
                                <td scope="col">${response[cont].priceMax != null ? response[cont].priceMax : "未設定"}</td>
                                <td>
                                     <div class="OrderOne${response[cont].cont + 1}" onclick="DataDelete(this) ">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </div>
                                </td>
                                <td> 
                                    <div class="OrderOne${response[cont].cont + 1}" onclick="DataDelete(this) ">
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
function Order2Read1() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
             step: 1
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">未設定</td>
                                <td>
                                   <div onclick="StatusReplyOptionsCreator1(this)"  id="CreatorOrder${response[cont].commissionOrderId}" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        查看
                                    </div>
                                    <div id="CreatorOrder${response[cont].commissionOrderId}Des" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        ${response[cont].description}
                                    </div>
                                </td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}
function Order2Read2() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
             step: 2
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">${response[cont].price}</td>
                                <td scope="col"></td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}
function Order2Read3() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
            step: 3
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">${response[cont].price}</td>
                                 <td>
                                    <div onclick="StatusReplyOptionsCreator2(this)"  id="CreatorOrder${response[cont].commissionOrderId}"  type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                            開始製作
                                    </div>
                                      <div id="CreatorOrder${response[cont].commissionOrderId}Des" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        ${response[cont].description}
                                    </div>
                                     <div id="CreatorOrder${response[cont].commissionOrderId}Price" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        ${response[cont].price}
                                    </div>
                                 </td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}
function Order2Read4() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
            step: 4
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">${response[cont].price}</td>
                               <td>
                                     <div  onclick="StatusReplyOptionsCreator3(this)"  id="CreatorOrder${response[cont].commissionOrderId}" type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        完成提交
                                    </div>
                                      <div id="CreatorOrder${response[cont].commissionOrderId}Des" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        ${response[cont].description}
                                    </div>
                                     <div id="CreatorOrder${response[cont].commissionOrderId}Price" type="button" class="btn btn-primary d-none" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                                        ${response[cont].price}
                                    </div>
                               </td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}
function Order2Read5() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
            step: 5
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">${response[cont].price}</td>
                                <td scope="col"></td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}
function Order2Read6() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "OrderData2",
            step: 6
        },
        success: function (response) {
            console.log(response);
            let cont = 0;
            response.forEach(function () {
                $("#EntrustOrders_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].title}</td>
                                <td scope="col">${response[cont].userName}</td>
                                <td scope="col">${response[cont].orderDate}</td>
                                <td scope="col">${response[cont].workStatus}</td>
                                <td scope="col">${response[cont].price}</td>
                                <td scope="col"></td>
                        </tr>
                    `);
                cont++;
            });
        }
    });
}

function StatusReplyOptionsCreator1(e) {
    $("#OrderDescriptionAuthor").empty();
    let Description = "#" + e.id + "Des";
    let DescriptionText = $(Description).text();
    $("#OrderDescriptionAuthor").append(`
       <p>${DescriptionText}</p>
        <hr>
      <p class="text-end">$<input id="PriceSet" type="text" name="CardNumber" id="cardNumber" pattern="^[1-9][0-9]*$" required></p>                
    `);
    $("#StatusReply").empty();
    $("#StatusReply").append(`
       <button onclick="CreatorStatusReply1('${e.id}','false')" type="button" class="btn btn-secondary" data-bs-dismiss="modal">拒絕</button>
       <button onclick="CreatorStatusReply1('${e.id}','true')" type="button" class="btn btn-primary" data-bs-dismiss="modal">接受</button>
    `);
};
function StatusReplyOptionsCreator2(e) {
    $("#OrderDescriptionAuthor").empty();
    let Description = "#" + e.id + "Des";
    let DescriptionText = $(Description).text();
    let Price = "#" + e.id + "Price";
    let PriceText = $(Price).text();
    $("#OrderDescriptionAuthor").append(`
       <p>${DescriptionText}</p>
        <hr>
      <p class="text-end">$${PriceText}</p>                
    `);
    $("#StatusReply").empty();
    $("#StatusReply").append(`
       <button onclick="CreatorStatusReply2('${e.id}','false')" type="button" class="btn btn-secondary" data-bs-dismiss="modal">製作拒絕</button>
       <button onclick="CreatorStatusReply2('${e.id}','true')" type="button" class="btn btn-primary" data-bs-dismiss="modal">開始製作</button>
    `);
};
function StatusReplyOptionsCreator3(e) {
    $("#OrderDescriptionAuthor").empty();
    let Description = "#" + e.id + "Des";
    let DescriptionText = $(Description).text();
    let Price = "#" + e.id + "Price";
    let PriceText = $(Price).text();
    $("#OrderDescriptionAuthor").append(`
       <p>${DescriptionText}</p>
        <hr>
      <p class="text-end">$${PriceText}</p>                
    `);
    $("#StatusReply").empty();
    $("#StatusReply").append(`
       <button onclick="CreatorStatusReply3('${e.id}','false')" type="button" class="btn btn-secondary" data-bs-dismiss="modal">製作取消</button>
       <button onclick="CreatorStatusReply3('${e.id}','true')" type="button" class="btn btn-primary" data-bs-dismiss="modal">成品提交</button>
    `);
};
function CreatorStatusReply1(x,y) {
    let match = x.match(/\d+$/);
    let Price = $("#PriceSet").val();
    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = x;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/CreatorStatusReply',
        method: 'POST',
        data: {
            id: TargetID,
            Reply: y,
            Price: Price,
        },
        success: function (response) {
            $("#EntrustOrders_List").empty();
            Order2Read1();
            Order2Read2();
            Order2Read3();
            Order2Read4();
            Order2Read5();
            Order2Read6();
        }
    });
};
function CreatorStatusReply2(x, y) {
    let match = x.match(/\d+$/);
    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = x;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/CreatorStatusReply',
        method: 'POST',
        data: {
            id: TargetID,
            Reply: y,
        },
        success: function (response) {
            $("#EntrustOrders_List").empty();
            Order2Read1();
            Order2Read2();
            Order2Read3();
            Order2Read4();
            Order2Read5();
            Order2Read6();
        }
    });
};
function CreatorStatusReply3(x, y) {
    let match = x.match(/\d+$/);
    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = x;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/CreatorStatusReply',
        method: 'POST',
        data: {
            id: TargetID,
            Reply: y,
        },
        success: function (response) {
            $("#EntrustOrders_List").empty();
            Order2Read1();
            Order2Read2();
            Order2Read3();
            Order2Read4();
            Order2Read5();
            Order2Read6();
        }
    });
};
function StatusReplyOptionsCreator(e) {
    $("#StatusReply").empty();
    $("#StatusReply").append(`
       <button onclick="FanStatusReply('${e.id}','false')" type="button" class="btn btn-secondary" data-bs-dismiss="modal">拒絕</button>
       <button onclick="FanStatusReply('${e.id}','true')" type="button" class="btn btn-primary" data-bs-dismiss="modal">接受</button>
    `);
};
function CreatorStatusReply(x,y) {
    let match = x.match(/\d+$/);

    if (match !== null) {
        // 提取到的数字字符串
        let numberString = match[0];
        var TargetID = parseInt(numberString, 10);
        // 提取数字之前的部分
        let input = x;
        var Category = input.substring(0, match.index);

    } else {
        console.log("找不到数字部分。");
    }
    $.ajax({
        url: '/yhu/FanStatusReply',
        method: 'POST',
        data: {
            id: TargetID,
            Reply: y
        },
        success: function (response) {
            $("#EntrustOrders_List").empty();
            Order2Read1();
            Order2Read2();
            Order2Read3();
            Order2Read4();
            Order2Read5();
            Order2Read6();
        }
    });
};
function EventRead() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "EventData"
        },
        success: function (response) {
            console.log(response[0]);
            $("#PlanData").addClass("d-none");
            $("#OrderData").addClass("d-none");
            $("#WorkData").addClass("d-none");
            $("#EventData").removeClass("d-none"); 
            let cont = 0;
            $("#Event_List").empty();
            response.forEach(function () {
                $("#Event_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <td scope="col">${response[cont].eventName}</td>
                                <td scope="col">${response[cont].startDate}</td>
                                <td scope="col">${response[cont].endDate}</td>
                                <td><div>
                                     <a class="Event${response[cont].eventId}" onclick="window.location.href='/Lolm/EventEdit/${response[cont].eventId}'">
                                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-feather" viewBox="0 0 16 16">
                                            <path d="M15.807.531c-.174-.177-.41-.289-.64-.363a3.8 3.8 0 0 0-.833-.15c-.62-.049-1.394 0-2.252.175C10.365.545 8.264 1.415 6.315 3.1S3.147 6.824 2.557 8.523c-.294.847-.44 1.634-.429 2.268.005.316.05.62.154.88q.025.061.056.122A68 68 0 0 0 .08 15.198a.53.53 0 0 0 .157.72.504.504 0 0 0 .705-.16 68 68 0 0 1 2.158-3.26c.285.141.616.195.958.182.513-.02 1.098-.188 1.723-.49 1.25-.605 2.744-1.787 4.303-3.642l1.518-1.55a.53.53 0 0 0 0-.739l-.729-.744 1.311.209a.5.5 0 0 0 .443-.15l.663-.684c.663-.68 1.292-1.325 1.763-1.892.314-.378.585-.752.754-1.107.163-.345.278-.773.112-1.188a.5.5 0 0 0-.112-.172M3.733 11.62C5.385 9.374 7.24 7.215 9.309 5.394l1.21 1.234-1.171 1.196-.027.03c-1.5 1.789-2.891 2.867-3.977 3.393-.544.263-.99.378-1.324.39a1.3 1.3 0 0 1-.287-.018Zm6.769-7.22c1.31-1.028 2.7-1.914 4.172-2.6a7 7 0 0 1-.4.523c-.442.533-1.028 1.134-1.681 1.804l-.51.524zm3.346-3.357C9.594 3.147 6.045 6.8 3.149 10.678c.007-.464.121-1.086.37-1.806.533-1.535 1.65-3.415 3.455-4.976 1.807-1.561 3.746-2.36 5.31-2.68a8 8 0 0 1 1.564-.173"/>
                                          </svg>
                                    </a>
                                    </div>
                                </td>
                                <td> 
                                    <div class="Event${response[cont].eventId}" onclick="DataDelete(this) ">
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


function ChangePwd() {
    var NewPwd = $("#NewPasswd").val();
    var CheckNewPwd = $("#CheckNewPasswd").val();

    if (NewPwd == CheckNewPwd) {
        $.ajax({
            url: '/yhu/ChangePwd',
            method: 'POST',
            data: {
                Password: NewPwd
            },
            success: function (response) {
                alert("密碼變更成功")
                $('#exampleModal').modal('hide');
            }
        });
    } else {
        alert("重新輸入密碼錯誤，請重新確認")
    }
    
}
