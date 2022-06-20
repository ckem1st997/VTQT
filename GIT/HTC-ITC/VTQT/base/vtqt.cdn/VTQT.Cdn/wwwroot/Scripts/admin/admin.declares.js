var admin = {
    init: function () { },
    initComponents: function () { },
    initPjax: function () { },
    apps: {
        init: function () { },
        overriddenAppSetting: {
            selector: 'input.multi-app-override-option',
            init: function () { },
            checkboxCheck: function (obj, checked) { },
            checkAllValue: function (obj) { },
            checkValue: function (element) { }
        }
    },
    appSelector: {
        selector: '#admin_appSelector_container',
        contentSelector: '#admin_appSelector_content_container .app-selector-content',
        $this: {},
        $content: {},
        init: function () { }
    },
    breadcrumb: {
        queryString: '',
        init: function (appType, appControllerName, appActionName) { }
    },
    btnClearCache: {
        selector: '#admin_btnClearCache',
        $this: {},
        init: function () { },
        click: function (e) { }
    },
    hMenu: {
        selector: '#admin_hMenu',
        $this: {},
        init: function () { },
        activates: function () { },
        resetActive: function () { }
    },
    vMenu: {
        selector: '#admin_vMenu',
        $this: {},
        init: function () { },
        activates: function () { },
        resetActive: function () { }
    },
    mobileMenu: {
        selector: '#admin_mobileMenu',
        $this: {},
        init: function () { },
        activatesMenu: function () { },
        resetActiveMenu: function () { }
    },
    pageToolbar: {
        actions: {
            initCreate: function ($toolbar, callback) { },
            initEdit: function ($toolbar, callback) { },
            initDeletes: function ($toolbar, callback) { },
            initActivate: function ($toolbar, callback) { },
            initPublish: function ($toolbar, callback) { }
        }
    },
    urls: {
        clearCache: '',
        breadcrumb: ''
    }
};
