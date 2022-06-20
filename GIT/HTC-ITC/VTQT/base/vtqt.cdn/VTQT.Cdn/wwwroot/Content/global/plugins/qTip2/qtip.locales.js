var _qtip = {
    locales: {}
};
_qtip.locales['vi-VN'] = {
    closeTooltip: 'Đóng'
};
_qtip.locales['en-US'] = {
    closeTooltip: 'Close tooltip'
};
_qtip.getResource = function (key) {
    var locale = _qtip.locales[Globalize.culture().name];
    if (locale != null)
        return locale[key];
    return _qtip.locales['en-US'].key;
};