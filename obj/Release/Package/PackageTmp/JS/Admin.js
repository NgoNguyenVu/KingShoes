document.addEventListener("DOMContentLoaded", function () {
    // Spinner
    var spinner = function () {
        setTimeout(function () {
            var spinner = document.getElementById('spinner');
            if (spinner) {
                spinner.classList.remove('show');
            }
        }, 1);
    };
    spinner();

    // Back to top button
    window.addEventListener("scroll", function () {
        if (window.scrollY > 300) {
            var backToTopButtons = document.querySelectorAll('.back-to-top');
            backToTopButtons.forEach(function (button) {
                button.style.display = 'block';
            });
        } else {
            var backToTopButtons = document.querySelectorAll('.back-to-top');
            backToTopButtons.forEach(function (button) {
                button.style.display = 'none';
            });
        }
    });

    var backToTopButtons = document.querySelectorAll('.back-to-top');
    backToTopButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    });

    // Sidebar Toggler
    var sidebarToggler = document.querySelector('.sidebar-toggler');
    var sidebar = document.querySelector('.sidebar');
    var content = document.querySelector('.content');

    sidebarToggler.addEventListener('click', function () {
        sidebar.classList.toggle("open");
        content.classList.toggle("open");
    });

    var currentUrl = window.location.pathname;
    var links = document.querySelectorAll(".navbar-nav .nav-link");

    for (var i = 0; i < links.length; i++) {
        if (links[i].getAttribute("href") === currentUrl) {
            links[i].classList.add("active");
            break; // Stop after the first match
        }
    }

   
});
