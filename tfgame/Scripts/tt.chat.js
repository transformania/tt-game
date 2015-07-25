var ChatModule = (function () {
    var unreadCount = 0;
    var antiSpamEnabled = false;
    var roomName = '';

    var pub= {};

    pub.chat = $.connection.chatHub;

    /* Private methods */

    function updateTitle(title) {
        $(document).attr('title', title);
    }
    
    /* Event handlers */

    function onGainFocus() {
        updateTitle(roomName);
        unreadCount = 0;
    }

    function onSendMessage() {
        if (antiSpamEnabled === false) {
            pub.chat.server.send($('#displayname').val(), $('#message').val());
            $('#message').val('').focus();
        }

        antiSpamEnabled = true;

        window.setInterval(function() { antiSpamEnabled = false; }, 3000);
    }

    function onChatHubStarted() {
        pub.chat.server.joinRoom(roomName);
        pub.chat.state.toRoom = roomName;

        $("#disconnected").hide();

        $(window).focus(onGainFocus);
        $('#sendmessage').click(onSendMessage);

        $(document).keypress(function(e) { 
            if (e.which === 13) {
                onSendMessage();
            }
        });
    }

    function onChatDisconnected() {
        $('#connectionStatus').text('Your chat is currently disconnected from the server.  It will attempt to automatically reconnect.').show();

        setTimeout(function () {
            $.connection.hub.start();
        }, 10000);
    }

    /* Public methods */

    pub.initialize = function (options) {
        roomName = options.roomName;

        $.connection.hub.start().done(onChatHubStarted);
        $.connection.hub.disconnected(onChatDisconnected);
    }

    pub.onUserDoubleTapped = function (name, e) {
        $('#message').val("@" + name + "  " + $('#message').val());
        $('#message').focus();

        e.preventDefault();
    }

    return pub;
})();