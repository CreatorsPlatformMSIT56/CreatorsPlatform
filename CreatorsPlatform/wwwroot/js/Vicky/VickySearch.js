
//委託按鈕觸發判斷
var isCommissionOnClicked = false;
var isWorkOnClicked = false;
//篩選按鈕判斷
var subtitleClicked = false;
//輸入的值
var searchKeyValue = $("#searchKey").data("searchkey");
var sortOrder = "ascending";
//年月日
function formatDate(date) {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}/${month}/${day}`;
}

function WorkOn(pageNumber = 1) {
    // 屏蔽委托排序与作品B排序按钮
    _button_container_TimeSort.style.display = "none";
    _button_container_TimeSortB.style.display = "block";
    _button_container_PriceSort.style.display = "none";
    _button_container_PriceSortB.style.display = "none";
    var buttonClicked = "WorkOn";
    isWorkOnClicked = true;
    isCommissionOnClicked = false;
    subtitleClicked = false;
    sortOrder = "ascending"
    $.ajax({
        url: `/Vicky/WorkOn/${sortOrder}?page=${pageNumber}&pageSize=9`,
        method: 'post',
        data: { sortOrder: sortOrder },
        success: function (data) {
            // 重置單選按鈕選擇狀態
            $('input[name="input_Filter"]').prop('checked', false);
            // 清空搜尋框內容
            $('#searchInput').val('');
            //創作者卡片
            $("#_creatorCard").remove();
            //主要內容
            $("#_worksConent").empty();
            //搜尋筆數
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            //頁數
            $('#_pageNavigation').empty();

            //篩選的筆數
            if (buttonClicked != null) {
                for (let i = 1; i <= 16; i++) {
                    $(`#filterAmount${i}`).empty();
                    $(`#filterAmount${i}`).html(`<i>${data.count[i].count}</i>`);
                }
            }


            data.workList.forEach(function (work) {
                let workHtml =
                    `
                    <div class="col-4  border _card_B">
                    <a href="/Creator/GetPost?id=${work.contentsID}"><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								    <a href="/Creator/GetPost?id=${work.contentsID}" class="fs-5">${work.title}</a>
                                <div class=" d-flex justify-content-between">
                                	<h4 class="text-muted">${formatDate(work.uploadDate)}</h4>
                                    <a href="/Creator/Index?id=${work.creatorID}" class="fs-5">${work.userName}</a>
                                </div>				
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
    sortOrder = "descending";
    subtitleClicked = false;
    $.ajax({
        url: `/Vicky/WorkOn/${sortOrder}?page=${pageNumber}&pageSize=9`,
        // 发送带有页码和页面大小的请求
        method: 'post',
        data: { sortOrder: sortOrder },
        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            data.workList.forEach(function (work) {
                let workHtml =
                    `
                        <div class="col-4  border _card_B">
                    <a href="/Creator/GetPost?id=${work.contentsID}"><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								    <a href="/Creator/GetPost?id=${work.contentsID}" class="fs-5">${work.title}</a>
                                <div class=" d-flex justify-content-between">
                                	<h4 class="text-muted">${formatDate(work.uploadDate)}</h4>
                                    <a href="/Creator/Index?id=${work.creatorID}" class="fs-5">${work.userName}</a>
                                </div>				
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
    var buttonClicked = "CommissionOn";
    isCommissionOnClicked = true;
    isWorkOnClicked = false;
    subtitleClicked = false;
    sortOrder = "ascending"
    $.ajax({
        url: `/Vicky/CommissionOn/${sortOrder}?page=${pageNumber}&pageSize=9`,
        method: 'post',
        data: { sortOrder: sortOrder, buttonClicked: buttonClicked },

        success: function (data) {
            $('input[name="input_Filter"]').prop('checked', false);
            $('#searchInput').val('');
            $("#_creatorCard").remove();
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();

            if (buttonClicked != null) {
                for (let i = 1; i <= 16; i++) {
                    $(`#filterAmount${i}`).empty();
                    $(`#filterAmount${i}`).html(`<i>${data.count[i].count}</i>`);
                }
            }

            data.workList.forEach(function (commission) {
                let commissionHtml =
                    `
                    <div class="col-4  border _card_B">
                    <a href="/Creator/GetCommission?id=${commission.commissionID}"><img src="${commission.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="/Creator/GetCommission?id=${commission.commissionID}" class="fs-5">${commission.commissionTitle}</a>
                                <div class=" d-flex justify-content-between">
                                     <h4 class="text-muted ">${commission.priceMin} ~ ${commission.priceMax}</h4>
                                     <a href="/Creator/Index?id=${commission.creatorID}" class="fs-5">${commission.userName}</a>
                                </div>				
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
    sortOrder = "descending";
    subtitleClicked = false;

    $.ajax({
        url: `/Vicky/CommissionOn/${sortOrder}?page=${pageNumber}&pageSize=9`,
        method: 'post',
        data: { sortOrder: sortOrder },

        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();
            data.workList.forEach(function (commission) {
                let commissionHtml =
                    `
                    <div class="col-4  border _card_B">
                    <a href="/Creator/GetCommission?id=${commission.commissionID}"><img src="${commission.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="/Creator/GetCommission?id=${commission.commissionID}" class="fs-5">${commission.commissionTitle}</a>
                                <div class=" d-flex justify-content-between">
                                     <h4 class="text-muted ">${commission.priceMin} ~ ${commission.priceMax}</h4>
                                     <a href="/Creator/Index?id=${commission.creatorID}" class="fs-5">${commission.userName}</a>
                                </div>				
							</div>
                    </div>
                    `;
                $('#_worksConent').append(commissionHtml);
            });
            let pageNavigation = `
			 <ul class="pagination">
                    <li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="CommissionLow(${pageNumber - 1})">Previous</a>
                    </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `
                <li class="page-item ${pageNumber == i ? "active" : ""}"><a class="page-link" href="#" onclick="CommissionLow(${i})">${i}</a></li>`;
            }
            pageNavigation += `
                    <li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="CommissionLow(${pageNumber + 1})">Next</a>
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
    if (subtitleClicked) {
        _button_container_TimeSort.style.display = "none";
        _button_container_TimeSortB.style.display = "block";
        isWorkOnClicked = true;
        sortOrder = "ascending";
        subtitleChange();
    } else {
        WorkOn();
    }
}
_button_container_TimeSortB.onclick = function () {
    if (subtitleClicked) {
        _button_container_TimeSort.style.display = "block";
        _button_container_TimeSortB.style.display = "none";
        isWorkOnClicked = true;
        sortOrder = "descending";
        subtitleChange();
    } else {
        WorkLow();
    }

}
_button_container_PriceSort.onclick = function () {

    if (subtitleClicked) {
        _button_container_PriceSortB.style.display = "block";
        _button_container_PriceSort.style.display = "none";

        isCommissionOnClicked = true;
        sortOrder = "ascending";
        subtitleChange();
    } else {
        CommissionOn();
    }
}
_button_container_PriceSortB.onclick = function () {
    if (subtitleClicked) {
        _button_container_PriceSortB.style.display = "none";
        _button_container_PriceSort.style.display = "block";
        isCommissionOnClicked = true;
        sortOrder = "descending";
        subtitleChange();
    } else {
        CommissionLow();
    }
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
function filterResults(option, searchKey, pageNumber = 1, buttonClicked) {
    subtitleClicked = true;
    if (searchKey != null) {
        searchKey = searchKey;
    }
    if (searchKey == null || searchKey == '') {
        searchKey = ' ';
    }
    if (isCommissionOnClicked) {
        buttonClicked = "CommissionOn";
    }
    if (isWorkOnClicked) {
        buttonClicked = "WorkOn";
    }

    $.ajax({
        type: "POST",
        url: `/Vicky/GetSubtitle/${option}/${searchKey}?page=${pageNumber}&pageSize=9`,
        data: { subtitleId: option, searchKey: searchKey, sortOrder: sortOrder, buttonClicked: buttonClicked },

        success: function (data) {
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();


            data.workList.forEach(function (work) {
                let workHtml =
                    `
                   <div class="col-4  border _card_B">
                    ${work.isCommission == true ? `<a href="/Creator/GetCommission?id=${work.commissionID}"><img src="${work.commissionImage}" class="img_Card ard-img-top img-fluid" alt="..."></a>` :
                        `<a href="/Creator/GetPost?id=${work.contentsID}"><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>`}
							<div class="card-body text-center position-relative">
								${work.isCommission == true ? `<a href="/Creator/GetCommission?id=${work.commissionID}" class="fs-4">${work.title}</a>` : `<a href="/Creator/GetPost?id=${work.contentsID}" class="fs-4">${work.title}</a>`}
								<div class=" d-flex justify-content-between">
                                    <h4 class="text-muted">${work.isCommission == true ? `${work.priceMin}~${work.priceMax}` : `${formatDate(work.uploadDate)}`}</h4>
                                    <a href="/Creator/Index?id=${work.creatorID}" class="fs-5">${work.userName}</a>
                                </div>
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
            pageNavigation += `<li class="page-item ${((pageNumber == data.totalPages) || (data.totalPages == 0)) ? "disabled" : ""}">
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

function tagResults(tagId, pageNumber = 1) {
    $.ajax({
        method: "get",
        url: `/Vicky/GetTags/${tagId}?page=${pageNumber}&pageSize=9`,
        data: { tagId: tagId },
        success: function (data) {
            // 更新页面内容
            $("#_worksConent").empty();
            $("#_searchAmount").empty();
            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
            $('#_searchAmount').append(dataLength);
            $('#_pageNavigation').empty();

            data.workList.forEach(function (work) {
                let workHtml =
                    `
                    <div class="col-4  border _card_B">
                        <a href="/Creator/GetPost?id=${work.contentsID}"><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
							<div class="card-body text-center position-relative">
								<a href="/Creator/GetPost?id=${work.contentsID}" class="fs-4">${work.title}</a>
                                <div class=" d-flex justify-content-between">
									<h4 class="text-muted">${formatDate(work.uploadDate)}</h4>
									<a href="/Creator/Index?id=${work.creatorID}" class="fs-5">${work.userName}</a>
								</div>
							</div>
                    </div>
                    `;
                $('#_worksConent').append(workHtml);
            })
            $('#_pageNavigation').empty();
            var pageNavigation = '<ul class="pagination">';
            pageNavigation += `<li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="tagResults(${tagId}, ${pageNumber - 1})">Previous</a>
                  </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `<li class="page-item ${pageNumber == i ? "active" : ""}">
                            <a class="page-link" href="#"onclick="tagResults(${tagId}, ${i})">${i}</a>
                          </li>`;
            }
            pageNavigation += `<li class="page-item ${((pageNumber == data.totalPages) || (data.totalPages == 0)) ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="tagResults(${tagId}, ${pageNumber + 1})">Next</a>
                      </li>
                    </ul>`;
            $('#_pageNavigation').append(pageNavigation);
        },
        error: function (error) {
            console.error(error);
        }
    });
}

if (tagIdValue !== 0) {
    tagResults(tagIdValue, pageNumber = 1);
} 