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
			let UserBg;
			$("#UserlistDetail").empty();
			response.forEach(function () {
				if (cont % 2 == 0) {
					UserBg = "bg-info bg-opacity-10";
				} else {
					UserBg = "";
				}
				$("#UserlistDetail").append(`
				  <div id="uid${response[cont].userId}" class="row border border-2 ${UserBg}" onclick="WorkChanges(this)">
						<div class="col-md-4 d-flex align-items-center justify-content-center">
							<img src="data:image/png;base64,${response[cont].avatar}" class="UserIcon rounded-2 CreaterChangePart" alt="...">
						</div>
						<div class="col-md-8">
							<div class="card-body">
							<a href="/Creator/Index/${response[cont].categoryId}">
								<h5 class="card-title">${response[cont].userName}</h5>
							</a>
								<p class="card-text">${response[cont].description}</p>
							</div>
						</div>
				  </div>				  
            `);
				cont++;
			});
			IconCssRest();
		}
	});
};
function WokLink(x) {
	window.location.href = `/Creator/GetPost/${x}`;
};
function WorkChanges(e) {
	let match = e.id.match(/\d+/);
	let intValue = parseInt(match[0], 10);
	$.ajax({
		url: '/yhu/WorkChanges',
		method: 'POST',
		data: { data: intValue }, //UserlistDetail
		success: function (response) {

			var formattedDates = []; // 创建一个空数组来存储格式化后的日期字符串

			for (var i = 0; i < response.length; i++) {
				// 调用 formatDateString 函数来格式化日期
				var FormatUploadDate = formatDateString(new Date(response[i].uploadDate));
				console.log(FormatUploadDate);

				// 将格式化后的日期字符串添加到数组中
				formattedDates.push(FormatUploadDate);
			};

			function formatDateString(date) {
				// 获取年、月、日
				var year = date.getFullYear();
				var month = date.getMonth() + 1; // 月份从 0 开始，所以要加1
				var day = date.getDate();

				// 获取小时、分钟、秒
				var hours = date.getHours();
				var minutes = date.getMinutes();
				var seconds = date.getSeconds();

				// 格式化月份和日期，确保是单个数字时前面加零
				month = month < 10 ? '0' + month : month;
				day = day < 10 ? '0' + day : day;

				// 格式化小时、分钟、秒，确保是单个数字时前面加零
				hours = hours < 10 ? '0' + hours : hours;
				minutes = minutes < 10 ? '0' + minutes : minutes;
				seconds = seconds < 10 ? '0' + seconds : seconds;

				// 判断上午或下午
				var period = hours >= 12 ? '下午' : '上午';

				// 将小时转换为12小时制
				hours = hours % 12;
				hours = hours ? hours : 12; // 如果小时数为0，则转换为12

				// 拼接格式化后的日期和时间字符串
				var formattedDate = year + '/' + month + '/' + day + ' ' + period + ' ' + hours + ':' + minutes + ':' + seconds;

				return formattedDate;
			}


			console.log(typeof response[0].uploadDate + ' ' + response[0].uploadDate);

			console.log(response);
			//---------------------------
			$("#Woklist1img").empty();
			//---------------------------
			$("#Woklist2img").empty();
			//---------------------------
			$("#Woklist3img").empty();
			//---------------------------
			$("#Woklist1img").append(
				
				`<div onclick = "WokLink(${response[0].contentId != undefined ? `${response[0].contentId}` : `1`})" class="ToMiddle image-container">
							 ${response[0].imageUrl != undefined ? `<img src="data:image/png;base64,${response[0].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
							`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16">
							 <path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
						     </svg>` }
				   </div>
				 <div id="Woklist1text" class="carousel-caption d-none d-md-block TextDiv">
							<h3 class="text-center fw-bold GrayText">${response[0].title != undefined ? response[0].title : ` `}</h5>
							 <h5 class="text-end pe-3 GrayText">${formattedDates[0] != undefined ? formattedDates[0] : ` `}</p>
				</div> 
				 `	);
			//----------------------------
			$("#Woklist2img").append(
				`<div onclick = "WokLink(${response[1].contentId != undefined ? `${response[1].contentId}` : `1`})" class="ToMiddle image-container">
							${response[1].imageUrl != undefined ? `<img src="data:image/png;base64,${response[1].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
							`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16" class="d-block WorkImg">
							<path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
							</svg>` }
					</div>
				<div id="Woklist2text" class="carousel-caption d-none d-md-block TextDiv">
					<h3 class="text-center fw-bold GrayText">${response[1].title != undefined ? response[1].title : ` `}</h5>
					 <h5 class="text-end pe-3 GrayText">${formattedDates[1] != undefined ? formattedDates[1] : ` `}</p>
				</div>`
			);
			//-----------------------------
			$("#Woklist3img").append(
				`<div onclick = "WokLink(${response[2].contentId != undefined ? `${response[2].contentId}` : `1`})" class="ToMiddle image-container">
						${response[2].imageUrl != undefined ? `<img src="data:image/png;base64,${response[2].imageUrl}" class="d-block WorkImg" alt="eoijwpeifjsj"> ` :
						`<svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" class="bi bi-brush d-block WorkImg" viewBox="0 0 16 16" class="d-block WorkImg">
								<path d="M15.825.12a.5.5 0 0 1 .132.584c-1.53 3.43-4.743 8.17-7.095 10.64a6.1 6.1 0 0 1-2.373 1.534c-.018.227-.06.538-.16.868-.201.659-.667 1.479-1.708 1.74a8.1 8.1 0 0 1-3.078.132 4 4 0 0 1-.562-.135 1.4 1.4 0 0 1-.466-.247.7.7 0 0 1-.204-.288.62.62 0 0 1 .004-.443c.095-.245.316-.38.461-.452.394-.197.625-.453.867-.826.095-.144.184-.297.287-.472l.117-.198c.151-.255.326-.54.546-.848.528-.739 1.201-.925 1.746-.896q.19.012.348.048c.062-.172.142-.38.238-.608.261-.619.658-1.419 1.187-2.069 2.176-2.67 6.18-6.206 9.117-8.104a.5.5 0 0 1 .596.04M4.705 11.912a1.2 1.2 0 0 0-.419-.1c-.246-.013-.573.05-.879.479-.197.275-.355.532-.5.777l-.105.177c-.106.181-.213.362-.32.528a3.4 3.4 0 0 1-.76.861c.69.112 1.736.111 2.657-.12.559-.139.843-.569.993-1.06a3 3 0 0 0 .126-.75zm1.44.026c.12-.04.277-.1.458-.183a5.1 5.1 0 0 0 1.535-1.1c1.9-1.996 4.412-5.57 6.052-8.631-2.59 1.927-5.566 4.66-7.302 6.792-.442.543-.795 1.243-1.042 1.826-.121.288-.214.54-.275.72v.001l.575.575zm-4.973 3.04.007-.005zm3.582-3.043.002.001h-.002z" />
						</svg>` }
					</div>
				<div id="Woklist3text" class="carousel-caption d-none d-md-block TextDiv">
					<h3 class="text-center fw-bold GrayText">${response[2].title != undefined ? response[2].title : ` `}</h5>
				    <h5 class="text-end pe-3 GrayText">${formattedDates[2] != undefined ? formattedDates[2] : ` `}</p>
				</div>`
			);
			ImgCssRest();
		}
	});
};


function IconCssRest() {
	$(".UserIcon").css({
		"height": "12vh",
		"width": "12vh",
		"max-height": "12vh",
		"max-width": "12vh",
		"object-fit": "cover"
	});
};
function ImgCssRest() {
	$(".ToMiddle").css({
		"display": "flex",
		"justify-content": "center",
		"align-items": "center"
	});
	$(".WorkImg").css({
		"height": "100%",
		"max- height": "100%",
		"object-fit": "cover"
	});
	$(".image-container").css({
		"height": "100vh",/* 设置容器高度 */
		"overflow": "hidden" /* 超出容器部分隐藏 */
	});

};

//function CarouselChanges(response) {
	
//};
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
//divElement.addEventListener('scroll', function () {
//    let divElement = document.getElementById('Userlist');
//    // 判断用户是否滚动到底部
//    if (Math.ceil(divElement.scrollTop) + divElement.clientHeight >= divElement.scrollHeight) {
//        console.log('User has scrolled to the bottom.');
//        // 在这里执行您希望在滚动到底部时执行的操作
//    }
//});


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