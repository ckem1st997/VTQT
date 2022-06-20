app.pjax.init = function () {
    $.pjax.defaults.timeout = app.pjax.defaults.timeout;
    $.pjax.defaults.container = app.pjax.defaults.container;
    $(document).pjax('a:not(.no-pjax)', app.pjax.defaults.container, { timeout: app.pjax.defaults.timeout }); // Pjax
    $(document).on('pjax:click', function (options) {
        app.qtip.destroyAll();
    });

    $(document).on('pjax:success', function (data, status, xhr, options) {
        $('.dropdown-toggle').dropdown();
        $('.dropdown-toggle[data-hover="dropdown"]').dropdownHover();
    });
    $(function () {
        $(app.pjax.defaults.container)
            .bind('pjax:start', function () {
                $('#ajax-busy').addClass('busy');
            })
            .bind('pjax:end', function () {
                window.setTimeout(function () {
                    $('#ajax-busy').removeClass('busy');
                }, 350);
            });
    });
    /* Pjax: customize for prevent Popstate cache for Back/Forward browser button works correctly */
    $(document).on('pjax:popstate', function (e) {
        e.preventDefault();

        app.window.alert.close();
        app.window.confirm.close();
        app.window.deletes.close();
        app.window.form.destroyAll();
    });
}
