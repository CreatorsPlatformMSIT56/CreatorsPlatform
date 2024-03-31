
window.addEventListener('load', function () {
    var myCarousel = document.querySelector('#SmallTourCarousel');
    var carousel = new bootstrap.Carousel(myCarousel, {
        interval: false,
	});
	
});
function CreatorsChange(Category) {
    console.log(Category);
	$.ajax({
		url: '/yhu/CreatorsChange',
		method: 'POST',
		data: {
			data: Category
		}, //UserlistDetail
		success: function (response) {
			console.log(response);
			let cont = 0;
			$("#UserlistDetail").empty();
			response.forEach(function () {
				$("#UserlistDetail").append(`
			<div id="uid${response[cont].userId}" class="row" onclick="WorkChanges(this)">
				<div class="col-md-4 d-flex align-items-center justify-content-center">
						<img src="data:image/png;base64,${response[cont].avatar}" class="UserIcon rounded-2" alt="...">
				</div>
				<div class="col-md-8">
					<div class="card-body">
						<h5 class="card-title">${response[cont].userName}</h5>
						<p class="card-text">${response[cont].description}</p>
					</div>
				</div>
		 </div>
		 <hr />
            `);
				cont++;
			});
			IconCssRest();
			const { compile } = require("sizzle");
        }
    });
}

function WorkChanges(e) {
	let match = e.id.match(/\d+/);
	let intValue = parseInt(match[0], 10);
	$.ajax({
		url: '/yhu/WorkChanges',
		method: 'POST',
		data: { data:intValue }, //UserlistDetail
		success: function (response) {
			console.log(response[0]);
			//---------------------------
			$("#Woklist1img").empty();
			$("#Woklist1text").empty();
			//---------------------------
			$("#Woklist2img").empty();
			$("#Woklist2text").empty();
			//---------------------------
			$("#Woklist3img").empty();
			$("#Woklist3text").empty();
			//---------------------------
			$("#Woklist1img").append(
				`<div class="ToMiddle">
					${response[0].imageUrl != undefined ? `<img src="data:image/png;base64,${response[0].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
					`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16" class="d-block WorkImg">
						<path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
					</svg>` }
				</div>`)
			$("#Woklist1text").append(
				`<h5>${response[0].title != undefined ? response[0].title : ` `}</h5>
				 <p>${response[0].uploadDate != undefined ? response[0].uploadDate : ` ` }</p>
						`);
			//----------------------------
			$("#Woklist2img").append(
				`<div class="ToMiddle">
						${response[1].imageUrl != undefined ? `<img src="data:image/png;base64,${response[1].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
					`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16" class="d-block WorkImg">
							<path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
					</svg>` }
				</div>`);
			$("#Woklist2text").append(
				`<h5>${response[1].title != undefined ? response[1].title : ` `}</h5>
				  <p>${response[1].uploadDate != undefined ? response[1].uploadDate : ` `}</p>`);
			//-----------------------------
			$("#Woklist3img").append(
				`<div class="ToMiddle">
					${response[2].imageUrl != undefined ? `<img src="data:image/png;base64,${response[2].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
					`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16" class="d-block WorkImg">
							<path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
					</svg>` }
				</div>`);
			$("#Woklist3text").append(
				`<h5>${response[2].title != undefined ? response[2].title : ` `}</h5>
				 <p>${response[2].uploadDate != undefined ? response[2].uploadDate : ` ` }</p>`);
			ImgCssRest();
        }
    });
}
function IconCssRest() {
	$(".UserIcon").css({
		"height": "12vh",
		"width": "12vh",
		"max-height": "12vh",
		"max-width": "12vh",
		"object-fit": "cover"
	});
}
function ImgCssRest() {
	$(".ToMiddle").css({
		"display": "flex",
		"justify-content": "center",
		"align-items": "center"
	});
	$(".WorkImg").css({
		"height": "50vh",
		"max- height": "50vh"
	});
	$(".d-block").css({
		"display":" block!important"
	});

}
//function MyCssRest() {
//	var existingLinks = document.querySelectorAll('link[href]');

//	 將要添加的 CSS 文件路徑存儲在一個數組中
//	var cssFiles = [
//		'https://cdn.jsdelivr.net/npm/quill@2.0.0-rc.2/dist/quill.snow.css',
//		'~/lib/bootstrap/dist/css/bootstrap.min.css',
//		'~/css/Shared/site.css',
//		'~/CreatorsPlatform.styles.css',
//		'https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css',
//		'/css/yhu/IMAGINK.css'
//	];

//	var existingLinks = document.querySelectorAll('link[href]');
//	existingLinks.forEach(function (link) {
//		var href = link.getAttribute('href');
//		if (cssFiles.includes(href)) {
//			link.parentNode.removeChild(link);
//		}
//	});

//	 將每個 CSS 文件添加為一個新的 link 標籤
//	cssFiles.forEach(function (cssFile) {
//		var link = document.createElement('link');
//		link.rel = 'stylesheet';
//		link.href = cssFile;
//		document.head.appendChild(link);
//	});
//}


//$('input[name="input_Filter"]').on("change", function () {
//    var selectedOption = $('input[name="input_Filter"]:checked').val(); // 获取选中的选项的值
//    window.location.href = "/Vicky/Search";
//    filterResults(selectedOption); // 根据选项筛选结果
//});
//function filterResults(option, pageNumber = 1) {
//    var ascending = option;
//    $.ajax({
//        type: "POST",
//        url: `/Vicky/GetSubtitle/${ascending}?page=${pageNumber}&pageSize=3`,
//        data: { subtitleId: option },
//        success: function (data) {
//            // 更新页面内容
//            $("#_worksConent").empty();
//            $("#_searchAmount").empty();
//            let dataLength = `<i>顯示${data.workList.length}筆結果</i>`;
//            $('#_searchAmount').append(dataLength);
//            $('#_pageNavigation').empty();

//            data.workList.forEach(function (work) {
//                let workHtml =
//                    `
//                    <div class="col-4 border d-flex justify-content-center card _card_B" >
//                    <a href=""><img src="data:image/*;base64,${work.imageUrl}" class="img_Card ard-img-top img-fluid" alt="..."></a>
//							<div class="card-body text-center position-relative">
//								<a href="" class="fs-4">${work.title}</a>
//								<p class="position-absolute top-50 start-0">
//									<small class="text-muted fs-5">${formatDate(work.uploadDate)}</small>
//								</p>
//								<a href="" class="fs-5 position-absolute top-50 end-0">${work.userName}</a>
//							</div>
//                    </div>
//                    `;
//                $('#_worksConent').append(workHtml);
//            })
//            $('#_pageNavigation').empty();
//            var pageNavigation = '<ul class="pagination">';
//            pageNavigation += `<li class="page-item ${pageNumber == 1 ? "disabled" : ""}">
//                        <a class="page-link" href="#" tabindex="-1" onclick="filterResults(${option}, ${pageNumber - 1})">Previous</a>
//                  </li>`;
//            for (let i = 1; i <= data.totalPages; i++) {
//                pageNavigation += `<li class="page-item ${pageNumber == i ? "active" : ""}">
//                            <a class="page-link" href="#"onclick="filterResults(${option}, ${i})">${i}</a>
//                          </li>`;
//            }
//            pageNavigation += `<li class="page-item ${pageNumber == data.totalPages ? "disabled" : ""}">
//                        <a class="page-link" href="#" onclick="filterResults(${option}, ${pageNumber + 1})">Next</a>
//                      </li>
//                    </ul>`;
//            $('#_pageNavigation').append(pageNavigation);

//        },
//        error: function (error) {
//            console.error(error);
//        }
//    });
//}