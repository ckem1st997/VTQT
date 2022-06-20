app.ajax.init = function () {
    app.ajax.setup();
}
app.ajax.setup = function () {
    $.ajaxSetup({
        cache: false,
        statusCode: app.ajax.statusCodeHandler
    });
}
app.ajax.handleUnauthorizedRequest = function (jqXhr) {
    var xRespondedJson = jqXhr.getResponseHeader('X-Responded-JSON');
    if (xRespondedJson !== null) {
        var obj = JSON.parse(xRespondedJson);
        if (!obj)
            return true;

        if (obj.status === 401) {
            window.location.href = app.urls.sessionExpired + '?ReturnUrl=' + encodeURIComponent(window.location.pathname + window.location.search);
            return false;
        }
        if (obj.status === 403) {
            app.ajax.statusCodeHandler[403]();
            return false;
        }
    }

    return true;
}
