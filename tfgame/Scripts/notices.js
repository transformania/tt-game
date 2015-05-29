


$(document).ready(function () {

    if ($("#notificationBox").html().length > 10) {
        $("#notificationBox").show();
        $("#dismissNotficationBox").show();
    }
    else {
        $("#notificationBox").hide();
        $("#dismissNotficationBox").hide();
    }

    if (donatorTier >= 1) {

        $(function () {
            var link = $.connection.noticeHub;
            link.client.receiveNotice = function (data) {

                //alert(data);
                //   console.log(data);

                var notice = JSON.parse(data);

                $("#notificationBox").show();
                $("#dismissNotficationBox").show();
                $("#notificationBox").prepend(notice.jMessage + "</br></br>");

                if (notice.type == "attack") {
                    var wpWidth = notice.wp / notice.maxwp * 100;
                    var mnWidth = notice.mana / notice.maxmana * 100;
                    $("#wp_amt1").css("width", wpWidth + "%");
                    $("#mn_amt1").css("width", mnWidth + "%");
                    $("#wp_amt2").css("width", wpWidth + "%");
                    $("#mn_amt2").css("width", mnWidth + "%");


                    $("#wp_num1").html(notice.wp);
                    $("#wp_num2").html(notice.wp);

                    // $("body").css("background-color", "darkred");
                    backgroundPulse();
                }


            };


            $.connection.hub.start().done(function () {
                link.server.connect();

                $("#liveConnectionNotice").addClass("noticeOn");
                $("#liveConnectionNotice").removeClass("noticeOff");

            });

            $.connection.hub.disconnected(function () {
                $("#liveConnectionNotice").addClass("noticeOff");
                $("#liveConnectionNotice").removeClass("noticeOn");
            });




        });
    }

});

function backgroundPulse() {
    $("body").animate({ backgroundColor: "red" }, 1000).animate({ backgroundColor: "black" }, 1000, backgroundPulse);
}