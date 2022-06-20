admin.init = function () {
    admin.initComponents();
}
admin.initComponents = function () {
    admin.appSelector.init();
    
    admin.hMenu.init();
    admin.hMenu.activates();
    admin.vMenu.init();
    admin.vMenu.activates();
    admin.mobileMenu.init();
    admin.mobileMenu.activatesMenu();
    //admin.breadcrumb.init(app.route.appId, app.route.appAssemblyAreaName, app.route.appControllerName, app.route.appActionName);
    admin.breadcrumb.init(app.route.appType, app.route.appControllerName, app.route.appActionName);

    // check overridden app settings
    admin.apps.init();

    admin.btnClearCache.init();
}
admin.initPjax = function () {
    admin.hMenu.activates();
    admin.vMenu.activates();
    admin.mobileMenu.activatesMenu();
    // Custom Pjax: Move to above the RenderBody section to render breadcrumb before pjax page load
    //admin.breadcrumb.init(app.route.appId, app.route.appAssemblyAreaName, app.route.appControllerName, app.route.appActionName);

    // check overridden app settings
    admin.apps.init();
}

admin.btnClearCache.init = function () {
    admin.btnClearCache.$this = $(admin.btnClearCache.selector);
    admin.btnClearCache.$this.click(admin.btnClearCache.click);
}
admin.btnClearCache.click = function (e) {
    var returnUrl = window.location.pathname + window.location.search;
    var url = admin.urls.clearCache + '?previousUrl=' + returnUrl;
    window.location.href = url;
}