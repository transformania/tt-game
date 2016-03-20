var ChatOperationsModule = (function() {
	
	var pub = {};

	pub.initialize = function () {
	    $('#createChatRoomModal').on('shown.bs.modal', function () {
	        $('#newRoomName').focus();
	    });

		$("#createChatRoomForm").submit(function (event) {
			event.preventDefault();

			var payload = { RoomName: $('#newRoomName').val().replace(/ /g, '_') };

			ApiModule.apiPost('chatroom', payload, function (data) {
			    showFlash('Chat room \'' + data.Name + '\' created successfully', 'result');
			}, function(data) {
			    showFlash('Could not create chat room: ' + data.Message, 'error');
			});

			$('#createChatRoomModal').modal('hide');
			$('#newRoomName').val('');
		});

		$('#listChatRoomsModal').on('show.bs.modal', function () {
		    var roomList = $('#roomList');
		    roomList.html('<strong>Loading...</strong>');

	        ApiModule.apiGet('chatroom', {}, function(data) {
	            var list = $("<ul></ul>");
	            list.addClass('chatRoomList');

	            var roomCount = data.length;
	            for (var i = 0; i < roomCount; i++) {
	                list.append('<li><strong>' + data[i].Name + '</strong><span>' + data[i].UserCount + '</span><br />' + data[i].Topic + '</li>');
	            }

	            roomList.html(list);
	        });
	    });
	}

    function showFlash(message, messageClass) {
        $('#infoBar').html('<p class="infoMessage '+messageClass+'">' + message + '</p>');
        $('#infoBar').fadeIn('fast');
        $('#infoBar').click(function () { $('#infoBar').fadeOut('fast') });
    }

	return pub;
})();