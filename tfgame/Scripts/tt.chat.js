var ChatModule = (function () {
    var unreadCount = 0;
    var cooldownActive = false;
    var roomName = '';
    
    var pub= {};

    pub.chat = $.connection.chatHub;
    pub.config = {};

    /* Private methods */

    function updateTitle(title) {
        $(document).attr('title', title);
    }

    function doConfig()
    {
        var savedConfig = localStorage['tt.chatConfig'];

        if (savedConfig !== undefined)
            pub.config = JSON.parse(savedConfig);
        else {
            pub.config = {
                imagesEnabled: localStorage['chat_ImagesOn'] !== undefined ? localStorage['chat_ImagesOn'] : true,
                autoScrollEnabled: true,
                ignoreList: {}
            }
        }

        if (pub.config.autoScrollEnabled)
            $('#autoScrollToggle').text('Autoscroll ON');
        else
            $('#autoScrollToggle').text('Autoscroll OFF');
    }
    
    /* Event handlers */

    function onGainFocus() {
        updateTitle(roomName);
        unreadCount = 0;
    }

    function onSendMessage() {
        if (cooldownActive === false) {
            pub.chat.server.send($('#displayname').val(), $('#message').val());
            $('#message').val('').focus();
        }

        cooldownActive = true;

        window.setInterval(function() { cooldownActive = false; }, 3000);
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

    function onNewMessage(model) {
        var output = ChatMessageModule.formatMessage(model);
        $('#discussion').append($(output));

        if (pub.config.autoScrollEnabled)
            $('#discussion ').animate({ scrollTop: $('#discussion').prop("scrollHeight") }, 500);
    }

    function onConfigChanged() {
        localStorage.removeItem('chat_ImagesOn');
        localStorage['tt.chatConfig'] = JSON.stringify(pub.config);
    }

    /* Public methods */

    pub.initialize = function (options) {
        roomName = options.roomName;

        doConfig();

        $.connection.hub.start().done(onChatHubStarted);
        $.connection.hub.disconnected(onChatDisconnected);

        pub.chat.client.addNewMessageToPage = onNewMessage;
    }

    pub.onUserDoubleTapped = function (name, e) {
        $('#message').val("@" + name + "  " + $('#message').val());
        $('#message').focus();

        e.preventDefault();
    }

    /* Events */

    $("#autoScrollToggle").click(function () {
        if (pub.config.autoScrollEnabled === false) {
            pub.config.autoScrollEnabled = true;
            $(this).text("Autoscroll ON");
        } else {
            pub.config.autoScrollEnabled = false;
            $(this).text("Autoscroll OFF");
        }

        onConfigChanged();
    });

    $("#toggleImages").click(function () {
        if (pub.config.imagesEnabled === false) {
            alert('Images enabled');
            pub.config.imagesEnabled = true;
        } else {
            alert('Images disabled');
            pub.config.imagesEnabled = false;
        }

        onConfigChanged();
    });

    return pub;
})();