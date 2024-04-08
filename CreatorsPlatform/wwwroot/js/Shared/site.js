
// 檢查 sessionstorage 是否包含搜索關鍵字並填充輸入框
window.onload = function () {
    var searchkey = sessionstorage.getitem('searchkey');
    if (searchkey) {
        document.queryselector('.searchforminput').value = searchkey;
    }
};

//// 在提交表單時將搜索關鍵字存儲到 sessionStorage 中
//document.querySelector('form').addEventListener('submit', function (event) {
//    var searchKey = document.querySelector('.SearchFormInput').value;
//    sessionStorage.setItem('searchKey', searchKey);
//});

function SaveSearchKey(event) {
    var searchKey = document.getElementById("searchInput").value;
    TempData["SearchKey"] = searchKey;
};
//function UserLogOut() {
//    console.log("ok");
//	//$.ajax({
//	//	url: '/yhu/LogOut',
//	//	method: 'POST',
//	//	data: {
//	//	},
//	//	success: function (response) {
//	//		window.location.href = '/yhu/IMAGINK';
//	//	}
//	//});
//};
