app.handlers.init = function () {
    $(function () {
        app.handlers.window.resize();
    });
}
app.handlers.window.resize = function () {
    $(window).on('resize', function (e) {
        // Fix validation tooltip (qTip2) hiện ở vị trí không đúng trong window size mới sau khi resize window
        delay('app.handlers.window.resize__app.qtip.hideAll', function () {
            app.qtip.hideAll();
        }, 100);

        // Resize Kendo Windows for each single main Tab scroll
        delay('app.handlers.window.resize__app.tab.handlers.resizeWindowsForEachSingleMainTabScroll', function () {
            app.tab.handlers.resizeWindowsForEachSingleMainTabScroll();
        }, 100);

        // Resize Kendo Windows
        delay('app.handlers.window.resize__app.window.handlers.resize', function () {
            app.window.handlers.resize();
        }, 100);

        // Resize Kendo Grids
        delay('app.handlers.window.resize__app.grid.handlers.resize', function () {
            app.grid.handlers.resize();
        }, 100);

        delay('app.handlers.window.resize__app.plugins.split.handlers.resize', function () {
            app.plugins.split.handlers.resize();
        }, 0);
    });
}