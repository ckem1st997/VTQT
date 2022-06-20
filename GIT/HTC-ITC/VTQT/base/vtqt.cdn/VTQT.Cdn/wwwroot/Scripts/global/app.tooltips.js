app.qtip.hideAll = function () {
    var $qtips = $('div.qtip');
    if ($qtips.length) {
        $.each($qtips, function (i, x) {
            var $x = $(x);
            $x.qtip('hide');
        });
    }
}
app.qtip.destroyAll = function () {
    var $qtips = $('div.qtip');
    if ($qtips.length) {
        $.each($qtips, function (i, x) {
            var $x = $(x);
            $x.qtip('destroy', true);
        });
    }
}