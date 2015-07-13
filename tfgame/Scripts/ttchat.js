ChatModule = (function() {
    var lowActivityThreshold = 180000; // time before name turns from green to red without activity.  In milliseconds.
    var onlineChatters;

    var pub = {};
    
    pub.updateUserList = function(userList) {
        onlineChatters = userList;

        pub.renderUserList();
    };

    pub.renderUserList = function() {

        if (onlineChatters === undefined)
            return;

        var i;
        var onlinePanel = $(".userlist-chat");
        onlinePanel.html("");
        
        for (i = 0; i < onlineChatters.Staff.length; i++) {
            onlinePanel.append(renderUserListItem(onlineChatters.Staff[i], false)); // admins shouldn't show a donator star
        }

        for (i = 0; i < onlineChatters.Donators.length; i++) {
            onlinePanel.append(renderUserListItem(onlineChatters.Donators[i], true));
        }

        for (i = 0; i < onlineChatters.Users.length; i++) {
            onlinePanel.append(renderUserListItem(onlineChatters.Users[i], false));
        }
    }

    pub.autoRender = function() {
        window.setInterval(pub.renderUserList(), 10000);
    }

    var renderUserListItem = function(item, isDonator) {

        var output = $('<span />', { text : ' '+item.User}).addClass('userlistRow');

        if (isDonator === true) 
        {
            $("<span />", { title: 'This player supports this game on Patreon monthly.' })
                .addClass('icon icon-donate')
                .prependTo(output);
        }

        if (Date.now() - item.LastActivity < lowActivityThreshold)
            output.addClass('good');
        else
            output.addClass('bad');

        // assign double click to print out the @ reference
        output.dblclick(function () {
            $('#message').val("@" + $(this).text() + "  " + $('#message').val());
            $('#message').focus();
        });

        return output.append("<br />");
    }

    return pub;

})();

