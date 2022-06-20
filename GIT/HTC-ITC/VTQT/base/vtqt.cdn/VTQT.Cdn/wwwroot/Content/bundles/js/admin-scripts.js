admin.apps.init = function () {
    admin.apps.overriddenAppSetting.init();
}

admin.apps.overriddenAppSetting.init = function () {
    // Using Bootstrap Switch with input (checkbox, radio) has class "make-switch".
    // But it will show checkbox for a short time before show Bootstrap Switch
    //$('input.multi-app-override-option').on('switchChange.bootstrapSwitch', function (event, state) {
    //    admin.apps.overriddenAppSetting.checkValue(this);
    //});

    // check overridden app settings
    $('input.multi-app-override-option').each(function (index, elem) {
        admin.apps.overriddenAppSetting.checkValue(elem);
    });
}

admin.apps.overriddenAppSetting.checkboxCheck = function (obj, checked) {
    if (checked)
        $(obj).attr('checked', 'checked');
    else
        $(obj).removeAttr('checked');
}
admin.apps.overriddenAppSetting.checkAllValue = function (obj) {
    $('input.multi-app-override-option').each(function (index, elem) {
        admin.apps.overriddenAppSetting.checkboxCheck(elem, obj.checked);
        admin.apps.overriddenAppSetting.checkValue(elem);
    });
}
admin.apps.overriddenAppSetting.checkValue = function (checkbox) {
    var parentSelector = $(checkbox).attr('data-parent-selector').toString(),
			parent = (parentSelector.length > 0 ? $(parentSelector) : $(checkbox).closest('.onoffswitch-container').parent()),
			checked = $(checkbox).is(':checked');

    // Using Bootstrap Switch with input (checkbox, radio) has class "make-switch".
    // But it will show checkbox for a short time before show Bootstrap Switch
    //var parentSelector = $(checkbox).attr('data-parent-selector').toString(),
    //		parent = (parentSelector.length > 0 ? $(parentSelector) : $(checkbox).closest('.bootstrap-switch-wrapper').parent()),
    //		checked = $(checkbox).is(':checked');

    parent.find(':input:not([type=hidden])').each(function (index, elem) {
        if ($(elem).is('select')) {
            //$(elem).select2(checked ? 'enable' : 'disable'); // Old
            $(elem).prop('disabled', checked ? false : true);
        }
        else if (!$(elem).hasClass('multi-app-override-option')) {
            if (checked)
                $(elem).removeAttr('disabled');
            else
                $(elem).attr('disabled', 'disabled');

            // Update Uniform
            app.plugins.uniform.update(elem);
        }
    });
}

admin.appSelector.init = function () {
    admin.appSelector.$this = $(admin.appSelector.selector);
    admin.appSelector.$content = $(admin.appSelector.contentSelector);
    admin.appSelector.$this.click(function () {
        admin.appSelector.$this.popModal({
            customClass: 'app-selector-popModal',
            html: admin.appSelector.$content,
            placement: 'bottomLeft',
            showCloseBut: false,
            onDocumentClickClose: true,
            onDocumentClickClosePrevent: '',
            overflowContent: false,
            inline: false,
            asMenu: false,
            beforeLoadingContent: '',
            onOkBut: function () { },
            onCancelBut: function () { },
            onLoad: function () { },
            onClose: function () { }
        });
    });
}

admin.breadcrumb.init = function (appType, appControllerName, appActionName) {
    if (app.action.isCommon) {
        app.action.isCommon = false;
        return;
    }

    var $breadcrumb = $('.page-bar .page-breadcrumb');
    if (!$breadcrumb.length)
        return;
    $.ajax({
        type: 'GET',
        data: { appType: appType, appControllerName: appControllerName, appActionName: appActionName },
        dataType: 'json',
        url: admin.urls.breadcrumb,
        beforeSend: function (jqXhr, settings) {
            $breadcrumb.html('<i class="fa fa-spinner fa-spin"></i>');
        },
        success: function (data) {
            $breadcrumb.html('');

            if (!data || !data.length)
                return;

            data.reverse();
            document.title = data[data.length - 1].Name;
            $(data).each(function (i, x) {
                $breadcrumb.append(
                    '<li>' +
                        (x.Icon ? '<i class="' + x.Icon + '"></i>&nbsp;' : '') +
                        (i < data.length - 1
                            ? (x.ActionUrl ? '<a href="{0}" title="{1}">'.format(x.ActionUrl, x.Description) + x.Name + '</a>' : '<span title="{0}">{1}</span>'.format(x.Description, x.Name))
                            : '<a href="{0}" title="{1}">{2}</a>'.format(window.location.pathname + admin.breadcrumb.queryString, x.Description, x.Name)) +
                        (data.length > 1 && i < data.length - 1 ? '&nbsp;<i class="flaticon2-right-arrow"></i>&nbsp;' : '') +
                    '</li>');
            });

            admin.breadcrumb.queryString = '';
        },
        error: function (jqXhr, textStatus, errorThrown) {
            admin.breadcrumb.queryString = '';
        }
    });
}

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

admin.pageToolbar.actions.initCreate = function ($toolbar, callback) {
    $toolbar.find('a[data-action="create"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initDetails = function ($toolbar, callback) {
    $toolbar.find('a[data-action="details"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initEdit = function ($toolbar, callback) {
    $toolbar.find('a[data-action="edit"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initDeletes = function ($toolbar, callback) {
    $toolbar.find('a[data-action="deletes"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initCancel = function ($toolbar, callback) {
    $toolbar.find('a[data-action="cancel"]').click(function (e) {
        callback();
    });
}
admin.pageToolbar.actions.initActivate = function ($toolbar, callback) {
    $toolbar.find('a[data-action="activate"], a[data-action="deactivate"]').click(function (e) {
        var $this = $(this);
        var action = $this.attr('data-action');
        var active = action === 'activate' ? true : false;

        callback(active);
    });
}
admin.pageToolbar.actions.initPublish = function ($toolbar, callback) {
    $toolbar.find('a[data-action="publish"], a[data-action="unpublish"]').click(function (e) {
        var $this = $(this);
        var action = $this.attr('data-action');
        var publish = action === 'publish' ? true : false;

        callback(publish);
    });
}


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