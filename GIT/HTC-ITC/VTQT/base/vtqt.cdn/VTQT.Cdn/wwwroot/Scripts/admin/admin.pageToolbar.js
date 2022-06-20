admin.pageToolbar.actions.initCreate = function ($toolbar, callback) {
    $toolbar.find('a[data-action="create"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initDetails = function ($toolbar, callback) {
    $toolbar.find('a[data-action="details"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initEdit = function ($toolbar, callback) {
    $toolbar.find('a[data-action="edit"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initDeletes = function ($toolbar, callback) {
    $toolbar.find('a[data-action="deletes"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initCancel = function ($toolbar, callback) {
    $toolbar.find('a[data-action="cancel"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initActivate = function ($toolbar, callback) {
    $toolbar.find('a[data-action="activate"], a[data-action="deactivate"]').click(function (e) {
        var $this = $(this);
        var action = $this.attr('data-action');
        var active = action === 'activate' ? true : false;

        callback(active);
    });
}
admin.pageToolbar.actions.initPublish = function ($toolbar, callback) {
    $toolbar.find('a[data-action="publish"], a[data-action="unpublish"]').click(function (e) {
        var $this = $(this);
        var action = $this.attr('data-action');
        var publish = action === 'publish' ? true : false;

        callback(publish);
    });
}
