

$(document).ready(function () {

   

    $(function () {
        var link = $.connection.noticeHub;
        link.client.receiveNotice = function (data) {
            
            alert(data);

        };


        $.connection.hub.start().done(function () {
            link.server.connect();
          //  chat.server.joinRoom("@ViewBag.ChatName");
        });

    });

});