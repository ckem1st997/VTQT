admin.hMenu.init = function () {
    admin.hMenu.$this = $(admin.hMenu.selector);
}
admin.hMenu.activates = function () {
    admin.hMenu.resetActive();

    var path = app.action.urlActiveMenu || window.location.pathname;
    var idxLast = path.lastIndexOf('/');
    if (idxLast === path.length - 1)
        path = path.substr(0, idxLast);
    var $a = admin.hMenu.$this.find('a[href="' + path + '"]');
    if ($a.length) {
        var $li = $a.parent('li');
        while ($li.length) {
            var $ul = $li.parent('ul');
            if ($ul.hasClass('navbar-nav')) {
                $li.find('> a').append('<span class="selected"></span>');
            }
            $li.addClass('active');
            $li = $ul.parent('li');
        }
    }

    app.action.urlActiveMenu = '';
}
admin.hMenu.resetActive = function () {
    var $liActive = admin.hMenu.$this.find('li.active');
    $.each($liActive, function (i, x) {
        $(x).removeClass('active');
    });
    var $selected = admin.hMenu.$this.find('span.selected');
    $selected.remove();
}

admin.vMenu.init = function () {
    admin.vMenu.$this = $(admin.vMenu.selector);
}
admin.vMenu.activates = function () {
    admin.vMenu.resetActive();
    var path = app.action.urlActiveMenu || window.location.pathname;
    var idxLast = path.lastIndexOf('/');
    if (idxLast === path.length - 1)
        path = path.substr(0, idxLast);
    var $a = admin.vMenu.$this.find('a[href="' + path + '"]');
    if ($a.length) {
        var $li = $a.parent('li');
        while ($li.length) {
            var $ul = $li.parent('ul');
            if ($ul.hasClass('page-sidebar-menu')) {
                $li.find('> a').append('<span class="selected"></span>');
                $li.addClass('active open');
            } else {
                $li.addClass('active');
            }
            var $arrow = $li.find('> a > span[class*="arrow"]');
            if ($arrow.length)
                $arrow.addClass('open');
            $li = $ul.parent('li');
        }
    }

    app.action.urlActiveMenu = '';
}
admin.vMenu.resetActive = function () {
    var $liActive = admin.vMenu.$this.find('li.active');
    $.each($liActive, function (i, x) {
        $(x).removeClass('active');
    });
    var $selected = admin.vMenu.$this.find('span.selected');
    $selected.remove();
}

admin.mobileMenu.init = function () {
    admin.mobileMenu.$this = $(admin.mobileMenu.selector);
}
admin.mobileMenu.activatesMenu = function () {
    admin.mobileMenu.resetActiveMenu();
    var path = app.action.urlActiveMenu || window.location.pathname;
    var idxLast = path.lastIndexOf('/');
    if (idxLast === path.length - 1)
        path = path.substr(0, idxLast);
    var $a = admin.mobileMenu.$this.find('a[href="' + path + '"]');
    if ($a.length) {
        var $li = $a.parent('li');
        while ($li.length) {
            var $ul = $li.parent('ul');
            if ($ul.hasClass('page-sidebar-menu')) {
                $li.find('> a').append('<span class="selected"></span>');
                $li.addClass('active open');
            } else {
                $li.addClass('active');
            }
            var $arrow = $li.find('> a > span[class*="arrow"]');
            if ($arrow.length)
                $arrow.addClass('open');
            $li = $ul.parent('li');
        }
    }

    app.action.urlActiveMenu = '';
}
admin.mobileMenu.resetActiveMenu = function () {
    var $liActive = admin.mobileMenu.$this.find('li.active');
    $.each($liActive, function (i, x) {
        $(x).removeClass('active');
    });
    var $selected = admin.mobileMenu.$this.find('span.selected');
    $selected.remove();
}
