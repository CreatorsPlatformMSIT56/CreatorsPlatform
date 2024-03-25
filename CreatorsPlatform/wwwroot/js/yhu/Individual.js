
function ReviseON() {
    $(".ReviseShow").css("display", "block");
    $(".DataShow").css("display", "none");

}
function ReviseOFF() {
    $(".ReviseShow").css("display", "none");
    $(".DataShow").css("display", "block");
}
$("#UserData").on("submit", function (e) {
    e.preventDefault();
    console.log(e);
    var UserData = $('#UserData').serializeArray();
    console.log(UserData);
    $.ajax({
        url: '/yhu/IndividualDataUp',
        method: 'POST',
        data: JSON.stringify(UserData),
        contentType: 'application/json',
        success: function (response) {
            console.log(response);
        }
    });
});
function GeneralSettingsReadData() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "GeneralSettings"
        },
        success: function (response) {
            console.log(response);
            $("#Avatar").attr("src", "data:image/png;base64," + response.avatar);
            $("#UserEMail").val(response.email);
            $("#UserName").val(response.name);  
        }
    });
};
function ConsumptionRecordReadData() {
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "ConsumptionRecord"
        },
        success: function (response1) {
            let cont = 0;
           
            response1.forEach(function () {
                $("#ConsumptionRecord_Plan").addend(`
              <tr>
                        <th scope="row">${cont+1}</th>
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
    $.ajax({
        url: '/yhu/IndividualData',
        method: 'POST',
        data: {
            type: "AuthorSettings"
        },
        success: function (response) {
            let cont = 0;
            console.log(response);
            response.forEach(function () {
                $("#AuthorSettings_List").append(`
                         <tr>
                                <th scope="col">${cont + 1}</th>
                                <th scope="col">${response[cont].planName}</th>
                                <th scope="col">${response[cont].description}</th>
                                <th scope="col">${response[cont].planLevel}</th>
                                <th scope="col">${response[cont].planPrice}</th>
                        </tr>
                    `);
                cont++;
            });
        }
    });
};