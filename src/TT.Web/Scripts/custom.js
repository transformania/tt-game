$(document).ready(function () {

    // Toggle the visibility of a hidden <div> element from another <div>.
    $('.confirmDiv').click(function () {
        $(this).find('.confirmContent').toggle(); 
    });

    // Use a link to toggle a hidden <div> element.
    $('.confirmLink').click(function (e) {

        // Please don't do squirrelly shit.
        e.preventDefault();

        var getDiv = $(this).data('target');

        // Toggle the visibility of a hidden <div> element while hiding others that may be toggled.
        $('.confirmContent').not(getDiv).hide();
        $(getDiv).toggle();
    });

});