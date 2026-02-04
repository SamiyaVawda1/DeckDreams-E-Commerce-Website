// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function redirectToAboutPage() {
    window.location.href = "/Home/About";
}

function redirectToProductsPage() {
    window.location.href = "/Product/Index";
}

//for the show password feature on register page
function togglePassword() {
    var passwordField = document.getElementById("passWord");
    if (passwordField) {
        if (passwordField.type === "password") {
            passwordField.type = "text";
        } else {
            passwordField.type = "password";
        }
    } else {
        console.error("Password field not found!");
    }
}

function LoginPopUp() {
    window.alert("Please log in to view cart items!");
}

function directToCheckout() {
    window.location.href = '/Order/Create';
}