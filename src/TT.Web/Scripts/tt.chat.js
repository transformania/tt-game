var ChatModule = (function () {
    var unreadCount = 0;
    var cooldownActive = false;
    var connected = false;
    var roomName = '';
    var reconnectTimer;
    var connectAttempts = 0;
    var maxConnectAttempts = 20;
    var addingMessages = 0;
    var autoScrollLock = false;
    
    var pub = {};

    var audioModes = {
        all: 'all',
        some: 'some',
        none: 'none'
    };

    var oldAudioModes = {
        0: 'none',
        1: 'some',
        2: 'all'
    };

    var connectionStatus = {
        connectionSlow: "We are currently experiencing difficulties with your connection, chat messages may be delayed",
        reconnecting: "Chat has been disconnected, trying to reconnect...",
        disconnected: "Chat has been disconnected and failed to reconnect, please re-load the page"
    };

    pub.chat = $.connection.chatHub;

    /* Private methods */

    function updateTitle(title) {
        $(document).attr('title', title);
    }

    function doConfig() {
        if (ConfigModule.chat.roomConfig[roomName] === undefined) {
            var audioMode = audioModes.none;

            if (localStorage['chat_sound_' + roomName] !== undefined) {
                audioMode = audioModes[oldAudioModes[localStorage['chat_sound_' + roomName]]];
                localStorage.removeItem(['chat_sound_' + roomName]);
            }

            ConfigModule.chat.roomConfig[roomName] = {
                audioMode: audioMode
            }

            ConfigModule.save();
        }

        setAutoScrollLock(false);
        updateAutoScrollText();
    }

    function setAutoScroll(flag) {
        ConfigModule.chat.autoScrollEnabled = flag;
        updateAutoScrollText();
        ConfigModule.save();
    }

    function setAutoScrollLock(flag) {
        autoScrollLock = flag;
        $('#autoScrollLock').css('opacity', flag ? '1' : '0.4');
    }

    function updateAutoScrollText() {
        if (ConfigModule.chat.autoScrollEnabled) {
            $('#autoScrollToggle').text('Autoscroll ON');
        }
        else {
            $('#autoScrollToggle').text('Autoscroll OFF');
        }
    }

    function playAudio(isAlert) {

        var audioMode = ConfigModule.chat.roomConfig[roomName].audioMode;

        if (audioMode === audioModes.none)
            return;

        if (audioMode === audioModes.some && isAlert === false)
            return;

        var popAudio;

        if (isAlert === true)
            popAudio = new Audio('../../Sounds/pop2.ogg');
        else
            popAudio = new Audio('../../Sounds/pop.ogg');

        popAudio.play();
    }

    function showConnectionStatus(status) {
        $('#connectionStatus').text(status).show();
    }

    function hideConnectionStatus() {
        $('#connectionStatus').hide();
    }

    function connect() {
        $.connection.hub.start().done(onChatHubStarted);
        $.connection.hub.connectionSlow(function () { showConnectionStatus(connectionStatus.connectionSlow); });
        $.connection.hub.reconnecting(onReconnecting);
        $.connection.hub.disconnected(onChatDisconnected);
        $.connection.hub.reconnected(hideConnectionStatus);
    }
    
    /* Event handlers */

    function onGainFocus() {
        if (connected)
            updateTitle(roomName);

        unreadCount = 0;
    }

    function onSendMessage() {
        if (cooldownActive === false) {
            if (document.getElementById('iq') && $('#iq').val() < 140) {
                pub.chat.server.send(mispell.bimbofy($('#message').val(), (1 - ($('#iq').val() - 40)/100)));
            } else {
                pub.chat.server.send($('#message').val());
            }
            $('#message').val('').focus();
        }

        cooldownActive = true;

        window.setInterval(function() { cooldownActive = false; }, 3000);
    }

    function onScrollDiscussion() {
        if (addingMessages == 0 && !autoScrollLock) {
            var panel = $('#discussion');
            setAutoScroll(panel.prop('scrollHeight') < Math.abs(panel.prop('scrollTop')) + panel.prop('clientHeight') + 5);
        }
    }

    function onResizeUserList() {
        $('#discussion').height($('.userlist-chat').height());
    }

    function onResizeWindow() {
        $('.userlist-chat').height($('#discussion').height());
    }

    function onChatHubStarted() {
        pub.chat.server.joinRoom(roomName);
        pub.chat.state.toRoom = roomName;

        $(window).focus(onGainFocus);
        $('#sendmessage').click(onSendMessage);
        $('#discussion').scroll(onScrollDiscussion);

        // resize event doesn't work on div
        $('.userlist-chat').hover(onResizeUserList);
        $('.userlist-chat').mouseup(onResizeUserList);
        $('.userlist-chat').mousemove(onResizeUserList);

        $(window).resize(onResizeWindow);

        $(document).keypress(function(e) { 
            if (e.which === 13) {
                onSendMessage();
            }
        });

        connectAttempts = 0;
        hideConnectionStatus();
        updateTitle(roomName);
        window.clearInterval(reconnectTimer);
        connected = true;
    }

    function onReconnecting() {
        showConnectionStatus(connectionStatus.reconnecting);
        updateTitle('(Reconnecting) ' + roomName);
    }

    function onChatDisconnected() {
        if (connected === false)
            return;

        connected = false;

        reconnectTimer = setInterval(function () {
            if (connected === false && connectAttempts < maxConnectAttempts) {
                connectAttempts++;
                showConnectionStatus(connectionStatus.reconnecting + ' (attempt ' + connectAttempts + '/' + maxConnectAttempts + ')');
                updateTitle('(Reconnecting) ' + roomName);

                connect();
            } else {
                showConnectionStatus(connectionStatus.disconnected);
                updateTitle('(Disconnected) ' + roomName);
                window.clearInterval(reconnectTimer);
            }
        }, 30000);
    }

    function onNewMessage(model) {
        var output = ChatMessageModule.formatMessage(model);

        // if the poster of this message is ignored, do nothing
        if (!output) { 
            return;
        }
        $('#discussion').append($(output));

        if (ConfigModule.chat.autoScrollEnabled) {
            addingMessages++;
            $('#discussion ').animate({ scrollTop: $('#discussion').prop("scrollHeight") }, 500, function () { addingMessages--; } );
        }

        playAudio(model.Message.indexOf(pub.currentPlayer) > 0);

        if (!document.hasFocus()) {
            unreadCount++;
            updateTitle('(' + unreadCount + ') ' + roomName);
        }
    }

    function onNewIgnoreAdded(newIgnore) {
        ConfigModule.chat.ignoreList.push(newIgnore);
        ConfigModule.save();

        $("#ignore").val("");
        alert("Now ignoring:  " + newIgnore);
    }

    function onIgnoreReset() {
        ConfigModule.chat.ignoreList = [];
        ConfigModule.save();

        alert("No longer ignoring anyone.");
    }

    function onIgnoreViewed() {
        $('#discussion ').append('<li><b>You will not see any chat messages that contain the following substrings:</b></li>');
        if (ConfigModule.chat.ignoreList.length > 0) {
            for (var x = 0; x < ConfigModule.chat.ignoreList.length; x++) {
                $('#discussion ').append('<li>' + ConfigModule.chat.ignoreList[x] + ' </li>');
            }
        } else {
            $('#discussion ').append('<li><i>You do not have anyone or anything on your ignore list.</i></li>');
        }
    }

    function onNameChanged(newName) {
        pub.currentPlayer = newName;
    }

    /* Public methods */

    pub.initialize = function (options) {
        roomName = options.roomName;
        pub.currentPlayer = options.currentPlayer;
        pub.currentPlayerChatColor = options.currentPlayerChatColor;

        doConfig();

        connect();

        pub.chat.client.addNewMessageToPage = onNewMessage;
        pub.chat.client.nameChanged = onNameChanged;
    }

    pub.onUserDoubleTapped = function (name, e) {
        $('#message').val($('#message').val() + "@" + name + ' ');
        $('#message').focus();

        e.preventDefault();
    }

    /* Events */

    $("#autoScrollToggle").click(function () {
        setAutoScroll(ConfigModule.chat.autoScrollEnabled === false);
    });

    $("#autoScrollLock").click(function () {
        setAutoScrollLock(autoScrollLock === false);
    });

    $("#toggleImages").click(function () {
        if (ConfigModule.chat.imagesEnabled === false) {
            alert('Images enabled');
            ConfigModule.chat.imagesEnabled = true;
        } else {
            alert('Images disabled');
            ConfigModule.chat.imagesEnabled = false;
        }

        ConfigModule.save();
    });

    $('li.audioConfig').click(function() {
        var newMode = $(this).attr('data-audio-mode');

        ConfigModule.chat.roomConfig[roomName].audioMode = newMode;
        ConfigModule.save();

        var alertText = {
            'all': 'All sounds enabled for this room.',
            'some': 'Some sounds enabled for this room.  You will only hear a sound if somone addresses you in particular.',
            'none': 'All sounds disabled for this room'
        };

        alert(alertText[newMode]);
    });

    $("#showIgnore").click(function () {
        if ($("#ignoreDiv").is(":visible") === false) {
            $("#ignoreDiv").show();
        } else {
            $("#ignoreDiv").hide();
        }
    });

    $("#ignoreAdd").click(function () {
        onNewIgnoreAdded($("#ignore").val());
    });

    $('#ignoreReset').click(function() {
        onIgnoreReset();
    });

    $("#ignoreView").click(function () {
        onIgnoreViewed();
    });

    return pub;
})();