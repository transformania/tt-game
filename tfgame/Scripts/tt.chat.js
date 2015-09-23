var ChatModule = (function () {
    var unreadCount = 0;
    var cooldownActive = false;
    var roomName = '';
    var currentPlayer = '';
    
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

        if (ConfigModule.chat.autoScrollEnabled)
            $('#autoScrollToggle').text('Autoscroll ON');
        else
            $('#autoScrollToggle').text('Autoscroll OFF');
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
    
    /* Event handlers */

    function onGainFocus() {
        updateTitle(roomName);
        unreadCount = 0;
    }

    function onSendMessage() {
        if (cooldownActive === false) {
            pub.chat.server.send($('#message').val());
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

        if (ConfigModule.chat.autoScrollEnabled)
            $('#discussion ').animate({ scrollTop: $('#discussion').prop("scrollHeight") }, 500);

        playAudio(model.Message.indexOf(currentPlayer) > 0);
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

    /* Public methods */

    pub.initialize = function (options) {
        roomName = options.roomName;
        currentPlayer = options.currentPlayer;

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
        if (ConfigModule.chat.autoScrollEnabled === false) {
            ConfigModule.chat.autoScrollEnabled = true;
            $(this).text("Autoscroll ON");
        } else {
            ConfigModule.chat.autoScrollEnabled = false;
            $(this).text("Autoscroll OFF");
        }

        ConfigModule.save();
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