//#region @typedef

//#region app.ajaxForm

/**
 * Options for initialize ajax for single form.
 * @typedef {Object} app.ajaxForm.init.options
 * @property {Object} $container - jQuery container element of ajax form.
 * @property {String} url - Action url of ajax form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {Boolean=} [autoClose=true] - Tự động close window khi response success.
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
 * @property {Object} $container - jQuery container element of ajax form.
 * @property {String} url - Url for get ajax form.
 * @property {Number=} [delay=0] - Delay of windown form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form/window form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {String=} actionUrl - Action url of ajax form.
 * @property {Boolean=} [autoClose=true] - Tự động close window khi response success.
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
 * Ajax form callback.
 * @callback app.ajaxForm.init.options.callback
 * @param {app.ajaxForm.init.options.callback.data=} data - Callback data.
 */

/**
 * Ajax form callback data.
 * @typedef {Object} app.ajaxForm.init.options.callback.data
 * @property {*=} data - The data returned from the server via ajax request.
 * @property {Object} $form - Ajax form jQuery element.
 */

//#endregion

//#region app.form

/**
 * Options for form validation.
 * @typedef {Object} app.form.initFormValidation.options
 * @property {Object} $form - Form jQuery object.
 * @property {enums.form.ErrorDisplayMode} [errorDisplayMode=enums.ajaxForm.ErrorDisplayMode.ShowFirstError] - Error display mode.
 */

//#endregion

//#region app.ui

/**
 * Options for loader.
 * @typedef {Object} app.ui.loader.options
 * @property {Number=} delay - Delay of loader.
 * @property {Number=} speed - Speed of loader.
 * @property {Number=} timeout - Timeout of loader.
 * @property {Boolean=} white - White loader background.
 * @property {Boolean=} small - Loader is small size or not.
 * @property {Number=} size - Size of loader.
 * @property {String=} message - Loader message.
 * @property {String=} cssClass - Loader css class.
 * @property {Function=} callback - Loader callback.
 * @property {Boolean=} show - Show loader immediately after initial.
 * @property {Boolean=} _global - Loader is global or not.
 */

//#endregion

//#region app.window

/**
 * Options for window form.
 * @typedef {Object} app.window.form.open.options
 * @property {String} url - Url for get ajax form.
 * @property {Object=} options - Options for Kendo Window.
 * @property {Number=} [delay=0] - Delay of windown form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form/window form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {String=} actionUrl - Action url of ajax form.
 * @property {Boolean=} [autoClose=true] - Tự động close window khi response success.
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
 * Options for window alert.
 * @typedef {Object} app.window.alert.open.options
 * @property {String=} title - Title of window alert.
 * @property {String} text - Content of window alert.
 * @property {String} type - Type of window alert.
 * @property {Object=} options - Options for Kendo Window.
 */

/**
 * Options for window confirm.
 * @typedef {Object} app.window.confirm.open.options
 * @property {String=} title - Title of window confirm.
 * @property {String} text - Content of window confirm.
 * @property {String} type - Type of window confirm.
 * @property {Object=} options - Options for Kendo Window.
 * @property {Function} callback - Callback for window confirm.
 */

/**
 * Options for window deletes.
 * @typedef {Object} app.window.deletes.open.options
 * @property {String} url - Action url of window deletes form.
 * @property {Number[]|String[]} ids - Array of Id.
 * @property {String=} title - Title of window deletes.
 * @property {String} text - Content of window deletes.
 * @property {(Object=|Function=)} extraData - Extra data for window deletes form.
 * @property {Object=} options - Options for Kendo Window.
 * @property {app.ajaxForm.init.options.callback=} callback - Callback for window deletes.
 */

/**
 * Options for window org mapping.
 * @typedef {Object} app.window.orgMap.open.options
 * @property {String=} title - Title of window confirm.
 * @property {String} text - Content of window confirm.
 * @property {String} type - Type of window confirm.
 * @property {Object=} options - Options for Kendo Window.
 * @property {Function} callback - Callback for window confirm.
 */

/**
 * Options for resize Kendo Window for scroll.
 * @typedef {Object} app.window.handlers.resizeForScroll.options
 * @property {Object} $kWindow - jQuery container element of Kendo Window.
 * @property {Boolean=} [center=true] - Specify whether centers Kendo Window or not.
 * @property {Boolean=} [formScrollParts=false] - Specify whether resize Kendo Window based on list of ".element-scroll-part" height or not.
 */

//#endregion

//#endregion

var app = {
    layout: {
        appInitScriptSelector: '#__appInitScript',
        scriptsSectionSelector: '#__scriptsSection'
    },
    action: {
        isCommon: false,
        /**
         * Relative url để active menu chỉ định trong trường hợp Action ko show ở menu mà vẫn có breadcrumb (dùng trong hàm activeMenu & activeMobileMenu).
         */
        urlActiveMenu: ''
    },
    route: {
        appId: '',
        appType: '',
        appAssemblyAreaName: '',
        areaName: '',
        appControllerName: '',
        appActionName: ''
    },
    init: function () { },
    initComponents: function () { },
    initAjax: function () { },
    ajax: {
        init: function () { },
        setup: function () { },
        handleUnauthorizedRequest: function (jqXhr) { },
        statusCodeHandler: {}
    },
    ajaxForm: {
        /**
		 * Initialize ajax for single form.
		 * @param {app.ajaxForm.init.options} options - Options for initialize ajax form.
		 */
        init: function (options) { },
        /**
         * Global extra data for ajax form.
         * @type {(Object|Function)}
         * @returns {Object} - Extra data object.
         */
        extraData: null,
        /**
         * Global validation callback for ajax form.
         * @type {Function}
         * @returns {Boolean} - Validation result.
         */
        validationCallback: null,
        /**
         * Reload ajax form.
         * @param {app.ajaxForm.reload.options} options - Options for reload ajax form.
         */
        reload: function (options) { }
    },
    form: {
        init: function () { },
        initComponents: function () { },
        antiForgeryToken: {
            selector: '#app_form_antiForgeryToken_container',
            pjaxSelector: '#pjax_app_form_antiForgeryToken_container',
            $container: {},
            init: function () { },
            cookieName: '',
            value: ''
        },
        getAntiForgeryToken: function () { },
        focusFirst: function ($container) { },
        initFormCommit: function () { },
        initFormScroll: function ($container) { },
        /**
         * Initialize form validation.
         * @param {app.form.initFormValidation.options=} options - Options for form validation.
         * @returns {Object} - Form validator.
         */
        initFormValidation: function (options) { },
        initLabel: function () { },
        initLabelHint: function ($container) { }
    },
    grid: {
        editingColumnField: function (e) { },
        editingColumnIndex: function (e) { },
        handlers: {
            columnMenuInit: function (e) { },
            resize: function () { }
        },
        formats: {
            time: function (time, format) { }
        }
    },
    notification: {
        init: function () { }
    },
    pjax: {
        init: function () { },
        defaults: {
            timeout: 60000,
            container: '.page-content, [data-pjax="true"]'
        }
    },
    plugins: {
        datetimepicker: {
            hideAll: function () { }
        },
        fancytree: {
            initCheckAll: function ($element) { },
            strings: {},
            handlers: {
                select: function (event, data) { }
            }
        },
        iconpicker: {
            /**
             * Initialize iconpicker for specify element.
             * @param {Object} $element - jQuery element.
             * @param {String=} icon - Set selected icon.
             */
            init: function ($element, icon) { },
            clear: function ($element) { },
            hideAll: function () { }
        },
        select2: {
            init: function ($context) { },
            /**
             * Clear select for specify element.
             * @param {Object} $element - jQuery element.
             * @param {(Array=|Object=)} eventParams - Event parameters.
             */
            clearSelect: function ($element, eventParams) { }
        },
        split: {
            handlers: {
                resize: function () { }
            }
        },
        tab: {
            init: function ($context) { },
            initAutoSelection: function ($context) { },
            initAjaxTabs: function ($context) { },
            initTabShown: function ($context) { },
            initTabSroll: function ($context) { }
        },
        uniform: {
            handles: function () { },
            init: function ($elements) { },
            update: function ($elements) { }
        },
        ace: {
            completers: {
                // TODO-XBase-Attendance-FormulaEditor: bổ sung completer cho Toán tử, hàm Excel
                // Phần tử công thức công
                timeFmlElems: []
            },
            getCompleter: function (completer) { }
        },
        applyCommonPlugins: function ($context) { }
    },
    qtip: {
        hideAll: function () { },
        destroyAll: function () { }
    },
    tab: {
        handlers: {
            resizeWindowsForEachSingleMainTabScroll: function () { },
            /**
             * Resize Kendo Window for each single main Tab scroll.
             * @param {Object} $kWindow - jQuery Kendo Window element.
             */
            resizeWindowForEachSingleMainTabScroll: function ($kWindow) { }
        }
    },
    templates: {
        grid: {
            bool: function (activate) { }
        }
    },
    ui: {
        def: {
            debounce: {
                timeout: 250
            }
        },
        /**
         * Show loading animation on specify container.
         * @param {Object} $container - jQuery container of loader.
         * @param {Boolean} toggle - If true: show the loader, false: hide the loader.
         * @param {app.ui.loader.options=} options - Options for loader.
         * @returns {Object} - Loader instance.
         */
        loader: function ($container, toggle, options) { }
    },
    urls: {
        breadcrumb: '',
        sessionExpired: ''
    },
    validator: {
        init: function () { },
        initComponents: function () { },
        setDefaults: function () { }
    },
    window: {
        form: {
            /**
             * {0}: Client form Id.
             */
            idFormat: 'app_window_form_{0}',
            initWindow: function ($winForm) { },
            /**
             * Open window form.
             * @param {app.window.form.open.options} options - Options for window form.
             * @returns {Object} - jQuery window form element.
             */
            open: function (options) { },
            destroyAll: function () { }
        },
        alert: {
            selector: '#app_window_alert',
            $this: {},
            contentHtml: '',
            init: function () { },
            /**
             * Open window alert.
             * @param {app.window.alert.open.options} options - Options for window alert.
             */
            open: function (options) { },
            close: function () { }
        },
        confirm: {
            selector: '#app_window_confirm',
            $this: {},
            contentHtml: '',
            init: function () { },
            /**
             * Open window confirm.
             * @param {app.window.confirm.open.options} options - Options for window confirm.
             */
            open: function (options) { },
            close: function () { }
        },
        deletes: {
            selector: '#app_window_deletes',
            $this: {},
            contentHtml: '',
            init: function () { },
            initWindow: function () { },
            /**
             * Open window deletes.
             * @param {app.window.deletes.open.options} options - Options for window deletes.
             */
            open: function (options) { },
            close: function () { }
        },
        handlers: {
            fixTop: function ($wrapper, top) { },
            resize: function () { },
            /**
             * Resize scroll height for Kendo Window.
             * @param {app.window.handlers.resizeForScroll.options} options - Options for resize Kendo Window for scroll.
             */
            resizeForScroll: function (options) { }
        }
    },
    handlers: {
        init: function () { },
        window: {
            resize: function () { }
        }
    }
};

//#region UserControls

// Employee
var _chooseEmpsList = [];
var _chooseEmpInput = [];
var _chooseEmpInfo = [];
var _chooseSignerInfo = [];

var _chooseEmp = {};
var _chooseEmps = {};

// Organizational Unit
var _chooseOrgInput = [];

var _chooseOrg = {};

//#endregion
