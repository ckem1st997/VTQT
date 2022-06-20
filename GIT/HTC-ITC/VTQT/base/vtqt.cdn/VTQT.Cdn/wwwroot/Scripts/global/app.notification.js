//#region @typedef

/**
 * Options for notification.
 * @typedef {Object} notify.options
 * @property {String=} title - The notice's title.
 * @property {String} text - The notice's text.
 * @property {String} type - Type of the notice. "notice", "info", "success", or "error".
 * @property {Boolean=} hide - After a delay, remove the notice.
 * @property {Number=} delay - Delay in milliseconds before the notice is removed.
 */

//#endregion

app.notification.init = function () {
    var stackBottomCenter = { "dir1": "up", "dir2": "right", "firstpos1": 100, "firstpos2": 10 };
    PNotify.prototype.options.styling = "fontawesome";
    PNotify.prototype.options.addclass = "stack-bottomcenter";
    PNotify.prototype.options.stack = stackBottomCenter;
    PNotify.prototype.options.delay = 5000;
    $.extend(PNotify.styling.fontawesome, {
        pin_up: "fa fa-thumb-tack fa-rotate-90",
        pin_down: "fa fa-thumb-tack"
    });
    $.extend(PNotify.prototype.options.buttons, {
        labels: { close: "", stick: "" }
    });
}

/**
 * Display notification.
 * @param {notify.options} options - Options for notification.
 * @returns {Object} - Notification object.
 */
function notify(options) {
    var notice = new PNotify({
        title: options.title,
        text: options.text,
        type: options.type,
        hide: options.hide !== undefined && options.hide != null ? options.hide : !(options.type !== "success" && options.type !== "info"),
        delay: options.delay !== undefined && options.delay != null ? options.delay : (options.type === "success" ? 2000 : PNotify.prototype.options.delay)
    });

    return notice;
}
