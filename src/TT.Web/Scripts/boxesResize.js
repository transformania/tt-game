$("#fullDetails").hide();

$("#fullDetails").on('click', function () {
    $("#fullDetails").hide();
});

$(".detailLink").on('click', function () {
    $("#fullDetails").show();
});