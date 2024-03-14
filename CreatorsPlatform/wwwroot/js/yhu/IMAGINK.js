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