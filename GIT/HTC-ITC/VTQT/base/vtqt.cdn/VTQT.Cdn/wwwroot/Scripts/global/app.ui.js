app.ui.loader = function ($container, toggle, options) {
    var defaultOptions = {
        delay: 0,
        speed: 150,
        timeout: 0,
        white: true,
        small: true,
        message: "",
        cssClass: null,
        callback: null,
        show: false,
        // internal
        _global: false
    };
    var opts = $.extend(defaultOptions, options);
    var throbber = $container.data("throbber");

    if (throbber) {
        if (toggle)
            throbber.show(options);
        else
            throbber.hide();
    } else {
        throbber = $container.throbber(opts).data("throbber");
        if (toggle)
            throbber.show();
        else
            throbber.hide();
    }

    return throbber;
}
