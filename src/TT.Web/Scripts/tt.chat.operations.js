var ChatOperationsModule = (function() {
	
	var pub = {};

	pub.initialize = function () {
		$("#createChatRoomForm").submit(function (event) {
			event.preventDefault();

			var payload = { RoomName:$('#newRoomName').val() }

			ApiModule.apiPost('chatroom', payload, function (data) {
			    showFlash('Chat room \'' + data.RoomName + '\' created successfully', 'result');
			}, function(data) {
			    showFlash('Could not create chat room: ' + data.Message, 'error');
			});

			$('#createChatRoomModal').modal('hide');
			$('#newRoomName').val('');
		});
	}

    function showFlash(message, messageClass) {
        $('#infoBar').html('<p class="infoMessage '+messageClass+'">' + message + '</p>');
        $('#infoBar').fadeIn('fast');
        $('#infoBar').click(function () { $('#infoBar').fadeOut('fast') });
    }

	return pub;
})();