window.addEventListener('load', function () {
    var myCarousel = document.querySelector('#SmallTourCarousel');
    var carousel = new bootstrap.Carousel(myCarousel, {
        interval: false,
    });
    
});

divElement.addEventListener('scroll', function () {
    let divElement = document.getElementById('Userlist');
    // 判断用户是否滚动到底部
    if (Math.ceil(divElement.scrollTop) + divElement.clientHeight >= divElement.scrollHeight) {
        console.log('User has scrolled to the bottom.');
        // 在这里执行您希望在滚动到底部时执行的操作
    }
});
function ddddd() {
    console.log($('input[name="input_Filter"]:checked').val());
}
$('input[name="input_Filter"]').on("change", function () {
    var selectedOption = $('input[name="input_Filter"]:checked').val(); // 获取选中的选项的值
    window.location.href = "/Vicky/Search";
    filterResults(selectedOption); // 根据选项筛选结果
});
function filterResults(option, pageNumber = 1) {
    var ascending = option;
    $.ajax({
        type: "POST",
        url: `/Vicky/GetSubtitle/${ascending}?page=${pageNumber}&pageSize=3`,
        data: { subtitleId: option },
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
            })
            $('#_pageNavigation').empty();
            var pageNavigation = '<ul class="pagination">';
            pageNavigation += `<li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
                        <a class="page-link" href="#" tabindex="-1" onclick="filterResults(${option}, ${pageNumber - 1})">Previous</a>
                  </li>`;
            for (let i = 1; i <= data.totalPages; i++) {
                pageNavigation += `<li class="page-item ${pageNumber == i ? "active" : ""}">
                            <a class="page-link" href="#"onclick="filterResults(${option}, ${i})">${i}</a>
                          </li>`;
            }
            pageNavigation += `<li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
                        <a class="page-link" href="#" onclick="filterResults(${option}, ${pageNumber + 1})">Next</a>
                      </li>
                    </ul>`;
            $('#_pageNavigation').append(pageNavigation);

        },
        error: function (error) {
            console.error(error);
        }
    });
}