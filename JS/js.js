
document.addEventListener("DOMContentLoaded", function () {
    let navbar = document.querySelector('.navbar');
    let searchForm = document.querySelector('.search-form');

    document.querySelector('#menu-btn').onclick = () => {
        navbar.classList.toggle('active');
        searchForm.classList.remove('active');
    }

    document.querySelector('#search-btn').onclick = () => {
        searchForm.classList.toggle('active');
        navbar.classList.remove('active');
    }

    // Thêm sự kiện "click" toàn cục cho màn hình
    document.addEventListener("click", function (event) {
        // Kiểm tra xem sự kiện click có xảy ra bên ngoài navbar, searchForm, và các phần tử liên quan
        if (!navbar.contains(event.target) && !searchForm.contains(event.target) && event.target.id !== "menu-btn" && event.target.id !== "search-btn") {
            navbar.classList.remove('active');
            searchForm.classList.remove('active');
        }
    });
});



var prevScrollpos = window.scrollY;
window.onscroll = function () {
    var currentScrollPos = window.scrollY;
    if (prevScrollpos > currentScrollPos) {
        document.getElementById("header").style.top = "0";
    } else {
        document.getElementById("header").style.top = "-105px";
    }
    prevScrollpos = currentScrollPos;
}


document.addEventListener("DOMContentLoaded", function () {
    function setImageClickHandler(featuredClass, bigClass) {
        document.querySelectorAll(featuredClass).forEach(featuredImage => {
            featuredImage.addEventListener('click', () => {
                var src = featuredImage.getAttribute('src');
                document.querySelector(bigClass).src = src;
            });
        });
    }

    setImageClickHandler('.featured-image-1', '.big-image-1');
    setImageClickHandler('.featured-image-2', '.big-image-2');
    setImageClickHandler('.featured-image-3', '.big-image-3');
});


document.addEventListener("DOMContentLoaded", function () {
    let currentSlide = 0;
    const slides = document.querySelectorAll(".home .slide-container");

    function showSlide(n) {
        slides[currentSlide].classList.remove("active");
        currentSlide = (n + slides.length) % slides.length;
        slides[currentSlide].classList.add("active");
    }

    function nextSlide() {
        showSlide(currentSlide + 1);
    }

    function prevSlide() {
        showSlide(currentSlide - 1);
    }

    // Set an interval to automatically advance the slides every few seconds
    const slideInterval = setInterval(nextSlide, 5000); // Change slide every 5 seconds

    // Add event listeners to the previous and next buttons
    document.getElementById("prev").addEventListener("click", function () {
        prevSlide();
        clearInterval(slideInterval); // Reset the interval
    });

    document.getElementById("next").addEventListener("click", function () {
        nextSlide();
        clearInterval(slideInterval); // Reset the interval
    });
});





document.addEventListener("DOMContentLoaded", function () {
    var loginform = document.getElementById("loginform");
    var regform = document.getElementById("regform");
    var Indicator = document.getElementById("Indicator");
});

function login() {
    regform.style.transform = "translateX(0px)";
    loginform.style.transform = "translateX(0px)";
    Indicator.style.transform = "translateX(65px)";
}

function register() {
    regform.style.transform = "translateX(380px)";
    loginform.style.transform = "translateX(380px)";
    Indicator.style.transform = "translateX(-60px)";
}



document.addEventListener("DOMContentLoaded", function () {
    var breadcrumbs = document.getElementById("breadcrumbs");
    var currentUrl = window.location.href;
    var urlParts = currentUrl.split("/");
    urlParts.shift();

    if (urlParts[urlParts.length - 1] === "") {
        urlParts.pop();
    }

    var breadcrumbPath = "";
    urlParts.forEach(function (part, index) {
        breadcrumbPath += "/" + part;

        if (index === 0) {
            return;
        }

        var formattedPart = part.charAt(0).toUpperCase() + part.slice(1);

        var breadcrumb = document.createElement("a");
        breadcrumb.href = breadcrumbPath;
        breadcrumb.textContent = formattedPart;
        breadcrumbs.appendChild(breadcrumb);

        if (index < urlParts.length - 1) {
            var separator = document.createTextNode(" > ");
            breadcrumbs.appendChild(separator);
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    const imgItems = document.querySelectorAll('.img-select .img-item');
    const imgShowcase = document.querySelector('.img-showcase');
    let imgId = 0;

    imgItems.forEach((imgItem, index) => {
        imgItem.addEventListener('click', (event) => {
            event.preventDefault();
            imgId = index;
            slideImage();
        });
    });

    function slideImage() {
        const displayWidth = imgShowcase.clientWidth;
        imgShowcase.style.transform = `translateX(${-imgId * displayWidth}px)`;
    }

    window.addEventListener('resize', slideImage);
});






