
//作品和委託分類



function formatDate(date) {
    //const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    //return new Date(date).toLocaleDateString('en-US', options);
    //改成年月日
    const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    const formattedDate = new Date(date).toLocaleDateString('en-US', options).split('/').reverse().join('-');
    return formattedDate;
}


function WorkOn(pageNumber = 1) {
    // 屏蔽委托排序与作品B排序按钮
    _button_container_TimeSort.style.display = "none";
    _button_container_TimeSortB.style.display = "block";
    _button_container_PriceSort.style.display = "none";
    _button_container_PriceSortB.style.display = "none";

    var descending = "descending";

    $.ajax({
        url: `/Vicky/WorkOn/${descending}?page=${pageNumber}&pageSize=3`, // 发送带有页码和页面大小的请求
        method: 'post',
        success: function (data) {

            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();


            // 遍历获取到的数据，显示在页面上
            data.workList.forEach(function (work) {
                let workHtml =
                    `
                    <div class="col-4 border d-flex justify-content-center card _card_B" >
                    <a href=""><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="" class="fs-4">${work.title}</a>
								<p class="position-absolute top-50 start-0">
									<small class="text-muted fs-5">${formatDate(work.uploadDate)}</small>
								</p>
								<a href="" class="fs-5 position-absolute top-50 end-0">${work.userName}</a>
							</div>
                    </div>
                    `;
                $('#_worksConent').append(workHtml);
            });
            let pageNavigation = `
			 <ul class="pagination">
                    <li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="WorkOn(${pageNumber - 1})">Previous</a>
                    </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `
                <li class="page-item ${pageNumber == i ? "active" : ""}"><a class="page-link" href="#" onclick="WorkOn(${i})">${i}</a></li>`;
            }
            pageNavigation += `
                    <li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="WorkOn(${pageNumber + 1})">Next</a>
                    </li>
                </ul>`;
            $('#_pageNavigation').append(pageNavigation);
        }
    });
}

function WorkLow(pageNumber = 1) {
    // 展示作品B排序按钮，隐藏作品A排序按钮
    _button_container_TimeSort.style.display = "block";
    _button_container_TimeSortB.style.display = "none";

    var ascending = "ascending";

    $.ajax({
        url: `/Vicky/WorkOn/${ascending}?page=${pageNumber}&pageSize=3`, // 发送带有页码和页面大小的请求
        method: 'post',
        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            data.workList.forEach(function (work) {
                let workHtml =
                    `
                    <div class="col-4 border d-flex justify-content-center card _card_B" >
                        <a href=""><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
                        <div class="card-body text-center position-relative">
                            <a href="" class="fs-4">${work.title}</a>
                            <p class="position-absolute top-50 start-0">
                                <small class="text-muted fs-5">${formatDate(work.uploadDate)}</small>
                            </p>
                            <a href="" class="fs-5 position-absolute top-50 end-0">${work.userName}</a>
                        </div>
                    </div>
                    `;
                $('#_worksConent').append(workHtml);
            });

            let pageNavigation = `
			 <ul class="pagination">
                    <li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="WorkLow(${pageNumber - 1})">Previous</a>
                    </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `<li class="page-item ${pageNumber == i ? "active" : ""}"><a class="page-link" href="#" onclick="WorkLow(${i})">${i}</a></li>`;
            }
            pageNavigation += `
                    <li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="WorkLow(${pageNumber + 1})">Next</a>
                    </li>
                </ul>`;
            $('#_pageNavigation').append(pageNavigation);
        }
    });
}

function CommissionOn(pageNumber = 1) {
    //顯示委託排序
    _button_container_PriceSortB.style.display = "block";
    _button_container_PriceSort.style.display = "none";
    _button_container_TimeSort.style.display = "none";
    _button_container_TimeSortB.style.display = "none";
    var descending = "descending";

    $.ajax({
        url: `/Vicky/CommissionOn/${descending}?page=${pageNumber}&pageSize=3`,
        method: 'post',

        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            data.workList.forEach(function (commission) {
                console.log(commission.commissionImage);
                let commissionHtml =
                    `
                    <div class="col-4 border d-flex justify-content-center card _card_B" >
                    <a href=""><img src="${commission.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="" class="fs-4">${commission.commissionTitle}</a>
								<p class="position-absolute top-50 start-0">
									<small class="text-muted fs-5">${commission.priceMax} ~ ${commission.priceMin}</small>
								</p>
								<a href="" class="fs-5 position-absolute top-50 end-0">${commission.userName}</a>
							</div>
                    </div>
                    `;
                $('#_worksConent').append(commissionHtml);
            });
            let pageNavigation = `
			 <ul class="pagination">
                    <li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="CommissionOn(${pageNumber - 1})">Previous</a>
                    </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `
                <li class="page-item ${pageNumber == i ? "active" : ""}"><a class="page-link" href="#" onclick="CommissionOn(${i})">${i}</a></li>`;
            }
            pageNavigation += `
                    <li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="CommissionOn(${pageNumber + 1})">Next</a>
                    </li>
                </ul>`;
            $('#_pageNavigation').append(pageNavigation);
        }
    });
};
function CommissionLow(pageNumber = 1) {
    //顯示委託排序
    _button_container_PriceSortB.style.display = "none";
    _button_container_PriceSort.style.display = "block";
    _button_container_TimeSort.style.display = "none";
    _button_container_TimeSortB.style.display = "none";
    var ascending = "ascending";

    $.ajax({
        url: `/Vicky/CommissionOn/${ascending}?page=${pageNumber}&pageSize=3`,
        method: 'post',

        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            data.workList.forEach(function (commission) {
                let commissionHtml =
                    `
                    <div class="col-4 border d-flex justify-content-center card _card_B" >
                    <a href=""><img src="${commission.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="" class="fs-4">${commission.commissionTitle}</a>
								<p class="position-absolute top-50 start-0">
									<small class="text-muted fs-5">${commission.priceMax} ~ ${commission.priceMin}</small>
								</p>
								<a href="" class="fs-5 position-absolute top-50 end-0">${commission.userName}</a>
							</div>
                    </div>
                    `;
                $('#_worksConent').append(commissionHtml);
            });
            let pageNavigation = `
			 <ul class="pagination">
                    <li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="CommissionOn(${pageNumber - 1})">Previous</a>
                    </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `
                <li class="page-item ${pageNumber == i ? "active" : ""}"><a class="page-link" href="#" onclick="CommissionOn(${i})">${i}</a></li>`;
            }
            pageNavigation += `
                    <li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="CommissionOn(${pageNumber + 1})">Next</a>
                    </li>
                </ul>`;
            $('#_pageNavigation').append(pageNavigation);
        }
    });
};

/* 搜尋內容排序按鈕 */
var _button_container_TimeSort = document.getElementById('_button_container_TimeSort');
var _button_container_TimeSortB = document.getElementById('_button_container_TimeSortB');
var _button_container_PriceSort = document.getElementById('_button_container_PriceSort');
var _button_container_PriceSortB = document.getElementById('_button_container_PriceSortB');

_button_container_TimeSort.onclick = function () {
    WorkOn();
}

_button_container_TimeSortB.onclick = function () {
    WorkLow();
}

_button_container_PriceSort.onclick = function () {
    CommissionOn();
}

_button_container_PriceSortB.onclick = function () {
    CommissionLow();
}

//因為會先勾選再刷新
$(function () {
    subtitleChange();
});
//// 当单选按钮被选中时触发
$('input[name="input_Filter"]').on("change", function () {
    subtitleChange();
    //var searchInputValue = document.getElementById('searchInput').value;
    //console.log(searchInputValue);
});
function subtitleChange() {
    var selectedOption = $('input[name="input_Filter"]:checked').val(); // 获取选中的选项的值
    /*    console.log($('input[name="input_Filter"]:checked'));*/
    var searchInputValue = document.getElementById('searchInput').value;
    if (selectedOption == undefined) {
        return
    }
    filterResults(selectedOption, searchInputValue);
}
function filterResults(option, searchKey, pageNumber = 1) {
    var ascending = option;
    if (searchKey != null) {
        searchKey = searchKey;
    }
    if (searchKey == null || searchKey == '') {
        searchKey = ' ';
    }
    $.ajax({
        type: "POST",
        url: `/Vicky/GetSubtitle/${ascending}/${searchKey}?page=${pageNumber}&pageSize=3`,
        data: { subtitleId: option, searchKey: searchKey },
        success: function (data) {
            // 更新页面内容
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            /*if(data.workList.subtitleId)*/

            data.workList.forEach(function (work) {
                let workHtml =
                    `
                    <div class="col-4 border d-flex justify-content-center card _card_B" >
                    ${work.isCommission == true ? `<a href=""><img src="${work.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>` :
                        `<a href=""><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>`}
							<div class="card-body text-center position-relative">
								<a href="" class="fs-4">${work.title}</a>
								<p class="position-absolute top-50 start-0">
									<small class="text-muted fs-5">${work.isCommission == true ? `${work.priceMax}` : `${formatDate(work.uploadDate)}`}</small>
								</p>
								<a href="" class="fs-5 position-absolute top-50 end-0">${work.userName}</a>
							</div>
                    </div>
                    `;
                $('#_worksConent').append(workHtml);
            })
            $('#_pageNavigation').empty();
            var pageNavigation = '<ul class="pagination">';
            pageNavigation += `<li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="filterResults(${option}, '${searchKey}', ${pageNumber - 1})">Previous</a>
                  </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `<li class="page-item ${pageNumber == i ? "active" : ""}">
                            <a class="page-link" href="#"onclick="filterResults(${option}, '${searchKey}', ${i})">${i}</a>
                          </li>`;
            }
            pageNavigation += `<li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="filterResults(${option}, '${searchKey}', ${pageNumber + 1})">Next</a>
                      </li>
                    </ul>`;

            $('#_pageNavigation').append(pageNavigation);

        },
        error: function (error) {
            console.error(error);
        }
    });
}



