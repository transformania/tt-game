var ChatUserListModule = (function() {
	var lowActivityThreshold = 180000; // time before name turns from green to red without activity.  In milliseconds.
	var onlineChatters;

    var pub = {};

    function renderUserListItem(item, isDonator) {

		var output = $('<span />', { text: ' ' + item.User }).addClass('userlistRow');

		if (isDonator === true) {
			$("<span />", { title: 'This player supports this game on Patreon monthly.' })
                .addClass('icon icon-donate')
                .prependTo(output);
		}

		if (Date.now() - item.LastActivity < lowActivityThreshold)
			output.addClass('good');
		else
			output.addClass('bad');

		output.doubleTap(function (e) { ChatModule.onUserDoubleTapped($(this).text(), e); });

		return output.append("<br>");
	}

	function renderUserList() {

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

	function updateUserList(userList) {
		onlineChatters = userList;

		renderUserList();
	};

	ChatModule.chat.client.updateUserList = function (userList) {
		updateUserList(userList);
	};

    return pub;
})();