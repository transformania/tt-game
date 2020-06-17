$(document).ready(function () {
    $('#bufftable tr').each(function () {
        $(this).find('td').each(function () {

            if ($(this).index() > 0.0) {
                var num = parseFloat($(this).html());
                if (num > 0) {
                    $(this).addClass("good");
                } else if (num < 0.0) {
                    $(this).addClass("bad");
                } else {
                    $(this).addClass("zero");
                }
            }

        })
    })
});
