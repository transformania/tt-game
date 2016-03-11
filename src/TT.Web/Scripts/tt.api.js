var ApiModule = (function () {
    var apiBasePath = '/api/';
    
    var pub = {};

    pub.apiPost = function(resource, payload, success, failure) {
        $.ajax({
            method: 'PUT',
            url: apiBasePath + resource,
            dataType: "json",
            data: payload
        })
         .done(success)
         .fail(function(data) { failure(JSON.parse(data.responseText)); });
    }

    return pub;
})();