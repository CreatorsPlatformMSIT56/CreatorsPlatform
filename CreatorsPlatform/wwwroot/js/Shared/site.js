
// 檢查 sessionStorage 是否包含搜索關鍵字並填充輸入框
window.onload = function () {
    var searchKey = sessionStorage.getItem('searchKey');
    if (searchKey) {
        document.querySelector('.SearchFormInput').value = searchKey;
    }
};

// 在提交表單時將搜索關鍵字存儲到 sessionStorage 中
document.querySelector('form').addEventListener('submit', function (event) {
    var searchKey = document.querySelector('.SearchFormInput').value;
    sessionStorage.setItem('searchKey', searchKey);
});

