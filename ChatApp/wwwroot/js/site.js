// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// jQuery for friend drawer toggle
$(document).ready(function () {
    $(".list-group-item").on("click", function () {
        $(".chat-bubble").fadeOut("slow").fadeIn("slow");
    });
});
