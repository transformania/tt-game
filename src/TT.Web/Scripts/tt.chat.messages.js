ChatMessageModule = (function() {
    var formatters = [];
	var reservedText = [];
	var pub = {};

	function canRender(model) {
	    if (ConfigModule.chat.ignoreList === undefined)
	        return true;

	    var ignoreList = ConfigModule.chat.ignoreList.slice(0);

	    if (ignoreList.length === 0)
	        return true;

	    var ignores = ignoreList.length;

	    for (var i = 0; i < ignores; i++)
	        if (model.User.indexOf(ignoreList[i]) >= 0 || model.Message.indexOf(ignoreList[i]) >= 0)
	            return false;

	    return true;
	}

	function openLink(event) {
		var message = event.data.message;

	    if (message === undefined)
	    	message = 'Are you sure you want to go to ' + event.data.url + '?  Be aware that this may be an external link that leads outside of Transformania Time, so be careful to only proceed if intended.';

	    var confirm = window.confirm(message);

	    if (confirm === true)
	        window.open(event.data.url);
	}

	function linkify(input) {
	    if (input === undefined || input == null)
	        return input;

	    var pattern = /((https?\:\/\/)|(www\.))(\S+)(\w{2,4})(:[0-9]+)?(\/|\/([\w#!:.?+=&%@@!\-\/]))?/gi;
		
	    var output = input.replace(pattern,
			function (url) {

				var fullUrl = url.match('^https?:\/\/') ? url : 'http://' + url;
				return "<a class='chatlink' data-url='" + fullUrl + "'>" + url + "</a>";
			});

		return output;
	}

	function applyHighlightToMessage(message) {
        var replacement = "<span style='background: #FFE9D5; color: " + ChatModule.currentPlayerChatColor + ";'><strong>" + ChatModule.currentPlayer + "</strong></span>";
        return message.replace(ChatModule.currentPlayer, replacement);
	}

	function renderImage(pic) {
	    var image = '';

	    if (ConfigModule.chat.imagesEnabled  && pic !== undefined)
	        image = $('<img />').attr('src', pic);

	    return image;
	}

    function renderTimestamp(timestamp) {
        return $('<span></span>').text(moment(timestamp).format('h:mm:ss A')).addClass('timeago');
    }

    function renderActionText(message, messageClass, model, useUserColour) {
        var text = $('<span></span>')
            .addClass(messageClass)
            .append(applyHighlightToMessage(linkify(message)))
            .doubleTap(function (e) { ChatModule.onUserDoubleTapped(model.User, e); })
            .prepend(model.IsStaff ? $('<span class="adminFont"></span>').text(model.User) : model.User);

        if (useUserColour)
            text.css('color', model.Color);

        return $('<li></li>').append(
            $('<strong></strong>').prepend(renderImage(model.Pic)).append(text)
        ).append(renderTimestamp(model.Timestamp));
    }

    reservedText['[luxa]'] = {
        link: {
            url: 'https://www.picarto.tv/live/channel.php?watch=Luxianne',
            message: "Are you sure that you want to watch Luxianne's stream?"
        },
        image: {
            width: '463px',
            height: '49px',
            src: '//i.imgur.com/khaIWKZ.jpg'
        }
    };

    reservedText['[blanca]'] = {
    	link: {
    		url: 'https://www.picarto.tv/live/channel.php?watch=Dandelines',
    		message: "Are you sure that you want to watch Blanca's stream?",
    		text: 'Blanca is streaming now'
    	}
    };

    reservedText['[poll]'] = {
    	link: {
    		url: 'http://goo.gl/forms/Zu0JeZBLn9',
    		text: 'Transformation Poll - http://goo.gl/forms/Zu0JeZBLn9'
    	},
    	message: 'Please take part in our Transformation Preferences Poll. '
    };

    reservedText['[fp]'] = {
        message: "  .-'---`-.\n,'          `.\n|             \\\n|              \\\n\\           _  \\\n,\\  _    ,'-,/-)\\\n( * \\ \\,' ,' ,'-)\n `._,)     -',-')\n   \\/         ''/\n    )        / /\n   /       ,'-",
    }

    reservedText['[sd]'] = {
        message: "  ------\n,'   SERVER PUBLISH HAPPENING SOON.       `.\n|     (Brace for derp.)        \\\n",
    }

    reservedText['[qb]'] = {
        message: " ／人◕ ‿‿ ◕人＼ { Dᴏ ʏᴏᴜ ᴡᴀɴɴᴀ ᴍᴀᴋᴇ ᴀ ᴄᴏɴᴛʀᴀᴄᴛ? }",
    };

	formatters['RegularText'] = function (model) {
		var user = model.User;

	    var userName = $('<strong></strong>')
	        .text(user + ': ')
	        .doubleTap(function(e) { ChatModule.onUserDoubleTapped(user, e); });

	    if (model.IsStaff)
	    	userName.addClass('adminFont');
	    else
	    	userName.css('color', model.Color);

	    var message = linkify(model.Message);

	    return $('<li></li>')
	        .prepend(userName)
            .prepend(renderImage(model.Pic))
            .append(applyHighlightToMessage(message) + ' ')
            .append(renderTimestamp(model.Timestamp));
    }

	formatters['ReservedText'] = function(model) {
	    var options = reservedText[model.Message];
		
	    if (options.link === undefined)
	        return $('<li></li>').append($('<pre></pre>').text(options.message));

	    var link = $('<a>').on('click', options.link, openLink);

	    if (options.link.text !== undefined)
	        link.text(options.link.text);

	    if (options.image !== undefined) {
	        var img = $('<img />')
                .css('width', options.image.width)
                .css('height', options.image.height)
                .attr('src', options.image.src);

	        link.append(img);
	    }

	    return $('<li></li>')
        .text(options.message !== undefined ? options.message : '')
        .append(link);
	}

	formatters['DmMessage'] = function(model) {
	    return renderActionText(' [DM]:' + model.Message + ' ', 'dm', model, false);
	}

	formatters['DmAction'] = function (model) {
	    return renderActionText(' ' + model.Message + ' ', 'dm', model, false);
	}

	formatters['DieRoll'] = function (model) {
	    return renderActionText(model.Message + ' ', 'roll', model, false);
	}

	formatters['Action'] = function(model) {
        return renderActionText(model.Message + ' ', "me", model, true);
	}

	formatters['Notification'] = function (model) {
	    return renderActionText(model.Message + ' ', "enterMsg", model, false);
	}

	pub.formatMessage = function (model) {
	    if (!canRender(model))
	        return '';

		return formatters[model.MessageType](model);
	}

    $("#discussion").on('click', '.chatlink', function () {
	    openLink({ data: { url: $(this).data('url') } });
	});

    return pub;
})();