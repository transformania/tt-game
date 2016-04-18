/*
    This script will create a <div> after any dom element with the the attribute 'SuggestedLength'.  Some text will be rendered beneath
    the element that states if the desired character length has been met where desired length is the value set for the attribute.
*/

$(document).ready(function () {
    bindSuggestedLengths();
});

function bindSuggestedLengths() {

    $("[data-suggested-length]").each(function () {
        var desiredLength = $(this).attr("data-suggested-length");
        var box = $(this);
        box.after("<div style='text-align: right;'></div>");
        setText(box, desiredLength);

        $(this).on('input', function () {
            setText($(this), desiredLength);
        });
    });

    function setText(box, desiredLength) {

        var note = box.next();
        var currentLength = box.val().length;

        var classToUse = "bad";

        if (currentLength >= desiredLength) {
            classToUse = "good";
        }

        note.removeClass("good");
        note.removeClass("bad");
        note.addClass(classToUse);

        if (currentLength>0) {
            note.html(currentLength + "/" + desiredLength + " of minimum suggested length");
        }
    }

}

