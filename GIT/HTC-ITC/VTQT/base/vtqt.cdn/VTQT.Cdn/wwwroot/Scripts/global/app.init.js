app.init = function () {
    app.initComponents();
    app.handlers.init();
}
app.initComponents = function () {
    app.ajax.init();
    app.form.init();
    app.notification.init();
    app.validator.init();

    app.window.alert.init();
    app.window.confirm.init();
    app.window.deletes.init();

    app.plugins.applyCommonPlugins($('body'));
}
app.initAjax = function () {
    app.plugins.applyCommonPlugins($('body'));
}
