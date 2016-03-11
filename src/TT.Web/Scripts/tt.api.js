var ApiModule = (function () {
    var apiBasePath = '/api/';
    
    var pub = {};

    pub.apiPost = function(resource, payload, success, failure) {
        $.ajax({
            method: 'PUT',
            url: apiBasePath + resource,
            data: payload
        })
         .done(success)
         .fail(failure);
    }

    return pub;
})();