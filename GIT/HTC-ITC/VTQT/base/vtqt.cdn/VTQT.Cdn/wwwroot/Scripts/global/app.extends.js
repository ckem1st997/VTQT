//#region @typedef

//#region app.ajaxForm extends

/**
 * Options for initialize ajax form.
 * @typedef {Object} $.fn.initAjaxForm.options
 * @property {String} url - Action url of ajax form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {Object=} submitElement - Submit element selector of form.
 * @property {Object=} continueAddingElement - "Continue adding" element selector of form.
 * @property {Object=} continueEditingElement - "Continue editing" element selector of form.
 * @property {enums.form.ErrorDisplayMode} [errorDisplayMode=enums.ajaxForm.ErrorDisplayMode.ShowFirstError] - Error display mode.
 * @property {Function=} initCallback - Initial callback for ajax form.
 * @property {Function=} validationCallback - Validation callback for ajax form.
 * @property {app.ajaxForm.init.options.callback=} callback - Callback for ajax form.
 * @property {app.ajaxForm.init.options.continueEditingCallback=} continueEditingCallback - "Continue editing" callback for ajax form.
 */

/**
 * Options for reload ajax form.
 * @typedef {Object} app.ajaxForm.reload.options
 * @property {String} url - Url for get ajax form.
 * @property {Number=} [delay=0] - Delay of windown form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form/window form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {String=} actionUrl - Action url of ajax form.
 * @property {Object=} submitElement - Submit element selector of form.
 * @property {Object=} continueAddingElement - "Continue adding" element selector of form.
 * @property {Object=} continueEditingElement - "Continue editing" element selector of form.
 * @property {enums.form.ErrorDisplayMode} [errorDisplayMode=enums.ajaxForm.ErrorDisplayMode.ShowFirstError] - Error display mode.
 * @property {Function=} initCallback - Initial callback for ajax form.
 * @property {Function=} validationCallback - Validation callback for ajax form.
 * @property {app.ajaxForm.init.options.callback=} callback - Callback for ajax form.
 * @property {app.ajaxForm.init.options.continueEditingCallback=} continueEditingCallback - "Continue editing" callback for ajax form.
 */

//#endregion

//#endregion

//#region app.ajaxForm

// app.ajaxForm.init
/**
 * Initialize ajax for single form.
 * @param {$.fn.initAjaxForm.options} options - Options for initialize ajax form.
 */
$.fn.initAjaxForm = function (options) {
    app.ajaxForm.init($.extend(options, { $container: this }));
}

// app.ajaxForm.reload
/**
 * Reload ajax form.
 * @param {$.fn.reloadAjaxForm.options} options - Options for reload ajax form.
 */
$.fn.reloadAjaxForm = function (options) {
    app.ajaxForm.reload($.extend(options, { $container: this }));
}

//#endregion

//#region app.ui

// app.ui.loader
/**
 * Show loading animation on specify container.
 * @param {Boolean} toggle - If true: show the loader, false: hide the loader.
 * @param {app.ui.loader.options=} options - Options for loader.
 * @returns {Object} - Loader instance.
 */
$.fn.loader = function (toggle, options) {
    return app.ui.loader(this, toggle, options);
}

//#endregion

//#region app.plugins

// select2
/**
* Clear select for specify element.
* @param {(Array=|Object=)} eventParams - Event parameters.
*/
$.fn.clearSelect = function (eventParams) {
    app.plugins.select2.clearSelect(this, eventParams);
}

// Fancytree
$.ui.fancytree._FancytreeClass.prototype.selectedIds = function () {
    var selNodes = this.getSelectedNodes();
    var selKeys = $.map(selNodes, function (node) {
        return node.key;
    });

    return selKeys;
};

//#endregion

//#region Form Helper

$.fn.focusFirst = function () {
    app.form.focusFirst(this);
}
$.fn.initFormScroll = function () {
    app.form.initFormScroll(this);
}
/**
 * Initialize form validation.
 * @param {app.form.initFormValidation.options=} options - Options for form validation.
 * @returns {Object} - Form validator. 
 */
$.fn.initFormValidation = function (options) {
    return app.form.initFormValidation($.extend(options, { $form: this }));
}
$.fn.initLabelHint = function () {
    app.form.initLabelHint(this);
}

//#endregion

//#region Kendo UI

$.extend(kendo.ui, {
    progress: function (container, toggle) {
        window.app.ui.loader(container, toggle, { timingFunction: "initial" });
    }
});

//#endregion
