var ChatOperationsModule = (function() {
	
	var pub = {};

	pub.initialize = function () {
		$("#createChatRoomForm").submit(function (event) {
			event.preventDefault();

			var payload = { RoomName:$('#newRoomName').val() }

			ApiModule.apiPost('chatroom', payload, function (data) {
		        console.table(data);
		    });

			$('#createChatRoomModal').modal('hide');
			$('#newRoomName').val('');
		});
	}

	return pub;
})();