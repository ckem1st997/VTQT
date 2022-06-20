/**
 * Các bước thực hiện
 * - Lấy các giá trị:
 * + tab-pane có scroll height max -> A
 * + Height max trong các tab-pane -> A0
 * + Scroll height max trong các tab-pane -> A1
 * + Chênh lệch height của tab-pane khi hiển thị -> A2 = A1 - A0
 * + Kendo Window height -> B
 * - Nếu B < C && A.isOverflow()
 * + B += A2
 * + Call Resize Kendo Window
 * Note:
 * - Không xử lý được hết các trường hợp, vẫn tồn tại khi chuyển qua các tab thì height bị thay đổi
 * vì khi show tab nào height tab đó sẽ là height của các tab khác (đang hidden), khi resize window thì cũng như vậy.
 * Nên vẫn xuất hiện trường hợp Kendo Window co vào giãn ra khi vừa chọn Tab vừa resize liên tục.
 * Tuy nhiên cũng hiếm khi xảy ra trừ khi user vừa chọn Tab vừa resize window liên tục nên cũng không gây ảnh hưởng đáng kể.
 * - Tồn tại khoảng trắng ở các tab có height nhỏ hơn.
 */
app.tab.handlers.resizeWindowsForEachSingleMainTabScroll = function () {
    var $kWindows = $('.k-widget.k-window');
    if ($kWindows.length) {
        $.each($kWindows, function (i, x) {
            var $kWindow = $(x);

            app.tab.handlers.resizeWindowForEachSingleMainTabScroll($kWindow);
        });
    }
}
app.tab.handlers.resizeWindowForEachSingleMainTabScroll = function ($kWindow) {
    var $tabs = $kWindow.find('div.tabbable');
    if (!$tabs.length)
        return;
    if ($tabs.length > 1) {
        console.log('Can only handles for single main tab in window');
        return;
    }

    var $tab = $tabs.first();
    var $tabPanes = $tab.find('.tab-content .tab-pane'),
        kWindowHeight = $kWindow.outerHeight(),
        windowHeight = $(window).height();
    var $tabPaneActive = $tabPanes.filter('.active');

    var $tabPaneMaxScrollHeight = _.max($tabPanes, function (x) {
        var $tabPane = $(x);
        return $tabPane.prop('scrollHeight');
    });
    $tabPaneMaxScrollHeight = $($tabPaneMaxScrollHeight);

    if (kWindowHeight < windowHeight && $tabPaneMaxScrollHeight.isOverflow()
        || $tabPaneActive.is($tabPaneMaxScrollHeight)) {
        var maxHeightTabPane = $tabPaneMaxScrollHeight.outerHeight();
        var maxScrollHeightTabPane = $tabPaneMaxScrollHeight.prop('scrollHeight');
        var tabPaneHeightPad = maxScrollHeightTabPane - maxHeightTabPane;
        if (tabPaneHeightPad > 4)
            $kWindow.outerHeight($kWindow.outerHeight() + tabPaneHeightPad);

        var $kWindowContent = $kWindow.find('.k-window-content');
        var kWindow = $kWindowContent.data('kendoWindow');
        kWindow.trigger('resize');

        if ($kWindow.length)
            app.window.handlers.resizeForScroll({ $kWindow: $kWindow });
    }
}
