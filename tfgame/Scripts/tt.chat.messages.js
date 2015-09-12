ChatMessageModule = (function() {
	var formatters = [];
	var reservedText = [];
	var pub = {};

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
		message: "  ------\n,'   SERVER PUBLISH HAPPENING SOON.       `.\n|     (Brace for derp.)        \\\n"
	}

	formatters['RegularText'] = function (model) {
		var user = model.User;

	    var userName = $('<strong></strong>')
	        .text(user + ': ')
	        .doubleTap(function(e) { ChatModule.onUserDoubleTapped(user, e); });

	    if (model.IsStaff)
	    	userName.addClass('adminFont');
	    else
	    	userName.css('color', model.Color);

	    var image = '';
	    if (ChatModule.config.imagesEnabled) {
	        image = $('<img />').attr('src', model.Pic);
	    }

	    return $('<li></li>')
	        .html(linkify(model.Message) + ' ')
            .prepend(userName)
            .prepend(image)
            .append($('<span></span>').text(moment(model.TimeStamp).format('h:mm:ss A')).addClass('timeago'));
    }

	formatters['ReservedText'] = function(model) {
	    var options = reservedText[model.Message];
		
	    if (options.link !== undefined) {
	        var link = $('<a>').bind('click', options.link, openLink);

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

	    return $('<li></li>').append($('<pre></pre>').text(options.message));
	}

	pub.formatMessage = function (model) {
		return formatters[model.MessageType](model);
	}

	$("#discussion").on('click', '.chatlink', function () {
	    openLink({ data: { url: $(this).data('url') } });
	});

    return pub;
})();