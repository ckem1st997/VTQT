app.plugins.getFunction = function (code, argNames) {
    var fn = window, parts = (code || "").split(".");
    while (fn && parts.length) {
        fn = fn[parts.shift()];
    }
    if (typeof (fn) === "function") {
        return fn;
    }
    argNames.push(code);
    return Function.constructor.apply(null, argNames);
}

app.plugins.datetimepicker.hideAll = function () {
    var $pickers = $('[id^="EditorTemplates_DateTime_Container_"]');
    if ($pickers.length) {
        $.each($pickers, function (i, x) {
            var $x = $(x);
            $x.data("DateTimePicker").hide();
        });
    }
}

app.plugins.fancytree.initCheckAll = function ($ctx) {
    if (!$ctx)
        $ctx = $(document);

    $ctx.on('click', 'table.fancytree-container thead tr th input[type="checkbox"][data-action="checkAll"]', function (e) {
        var $this = $(this);
        var $tree = $this.closest('table.fancytree-container');
        var tree = $tree.fancytree("getTree");
        if (!tree)
            return;

        var checked = $(this).prop('checked');
        tree.visit(function (node) {
            node.setSelected(checked);
        });
    });
}
//TODO: XBase Resources
app.plugins.fancytree.strings = {
    loading: window.Res['Common.Loading'],
    loadError: window.Res['Common.LoadError'],
    moreData: window.Res['Common.MoreData'],
    noData: window.Res['Common.NoData']
};
app.plugins.fancytree.handlers.selectOnCheckAll = function (event, data) {
    var tree = data.tree;
    var $div = tree.$div;
    var $checkAll = $div.find('thead tr th input[type="checkbox"][data-action="checkAll"]');
    if (!$checkAll.length)
        return;

    if (tree.getSelectedNodes().length < tree.count()) {
        $checkAll.prop('checked', false);
        $checkAll.parent('span').removeClass('checked');
    } else {
        $checkAll.prop('checked', true);
        $checkAll.parent('span').addClass('checked');
    }
}

app.plugins.iconpicker.init = function ($element, icon) {
    icon = icon ? icon : '';
    $element.iconpicker({
        arrowClass: 'btn-default',
        arrowPrevIconClass: 'fa fa-arrow-left',
        arrowNextIconClass: 'fa fa-arrow-right',
        cols: 5,
        icon: icon,
        iconClassFix: 'fa fa-',
        iconset: 'fontawesome',
        //labelHeader: '{0} / {1}',
        //labelFooter: '{0} - {1} of {2}',
        labelFooter: '{0} - {1} / {2}',
        placement: 'right',
        rows: 5,
        search: true,
        searchText: window.Res['Common.Search'],
        selectedClass: 'btn-primary',
        unselectedClass: ''
    });
    if (!icon)
        app.plugins.iconpicker.clear($element);
}
app.plugins.iconpicker.clear = function ($element) {
    $element.find('i').attr('class', '');
    $element.find('input[type="hidden"]').val('');
}
app.plugins.iconpicker.hideAll = function () {
    var $popovers = $('.iconpicker-popover');
    if ($popovers.length) {
        $.each($popovers, function (i, x) {
            var $x = $(x);
            var popover = $x.data('bs.popover');
            if (popover)
                popover.hide();
        });
    }
}

app.plugins.select2.init = function ($ctx) {
    $.fn.select2.defaults.set("theme", "bootstrap");
    $ctx.find('select:not(.noskin, .nostyle, [data-auto-init="false"], [data-auto-init="False"], [data-role="dropdownlist"]), input:hidden[data-select]:not([data-auto-init="false"], [data-auto-init="False"])')
		.not('.k-grid select')
		.selectWrapper();
}
app.plugins.select2.clearSelect = function ($element) {
    $element
        .val('')
        .trigger('change')
        .empty()
        .append($('<option></option>', {
            value: '',
            text: ''
        }));

    var $s2 = $element.parent().find('.select2-container');
    if ($s2.length) {
        var s2 = $element.data('select2');
        if (s2) {
            var $selection = s2.$selection;
            var $dropdown = s2.dropdown.$dropdown;
            var $search = s2.dropdown.$search;
            setTimeout(function () {
                $s2.qtip('destroy');
                $selection.removeClass('select2-input-validation-error');
                $dropdown.removeClass('select2-input-validation-error');
                if ($search) {
                    $search.removeClass('input-validation-error');
                }
            });
        }
    }
}

app.plugins.split.handlers.resize = function () {
    var $splits = $('.splitter');
    $.each($splits, function (i, x) {
        var $x = $(x);
        var split = $(x).data('splitter');
        if (split)
            split.trigger('splitter.resize');
    });
}

app.plugins.tab.init = function ($ctx) {
    app.plugins.tab.initAutoSelection($ctx);
    app.plugins.tab.initAjaxTabs($ctx);
    app.plugins.tab.initTabShown($ctx);
    app.plugins.tab.initTabScroll($ctx);
}
app.plugins.tab.initAutoSelection = function ($ctx) {
    // tab strip smart auto selection
    $ctx.find('.tabs-autoselect ul.nav a[data-toggle=tab]').on('shown.bs.tab', function (e) {
        var tab = $(e.target),
            strip = tab.closest('.tabbable'),
            href = strip.data("tabselector-href"),
            hash = tab.attr("href");

        if (hash)
            hash = hash.replace(/#/, "");

        if (href) {
            $.ajax({
                type: "POST",
                url: href,
                async: true,
                data: { navId: strip.attr('id'), tabId: hash, path: location.pathname + location.search },
                global: false
            });
        }
    });
}
app.plugins.tab.initAjaxTabs = function ($ctx) {
    // AJAX tabs
    $ctx.find('.nav a[data-ajax-url]').on('show.bs.tab', function (e) {
        var newTab = $(e.target),
            tabbable = newTab.closest('.tabbable'),
            pane = tabbable.find(newTab.attr("href")),
            url = newTab.data('ajax-url');

        if (newTab.data("loaded") || !url)
            return;

        $.ajax({
            cache: false,
            type: "GET",
            async: false,
            global: false,
            url: url,
            beforeSend: function (xhr) {
                app.plugins.getFunction(tabbable.data("ajax-onbegin"), ["tab", "pane", "xhr"]).apply(this, [newTab, pane, xhr]);
            },
            success: function (data, status, xhr) {
                pane.html(data);
                app.plugins.getFunction(tabbable.data("ajax-onsuccess"), ["tab", "pane", "data", "status", "xhr"]).apply(this, [newTab, pane, data, status, xhr]);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //pane.html('<div class="text-error">Error while loading resource: ' + thrownError + '</div>');
                pane.html('<div class="text-error">{0}: {1}</div>'.format(window.Res['Common.Error'], thrownError));
                app.plugins.getFunction(tabbable.data("ajax-onfailure"), ["tab", "pane", "xhr", "ajaxOptions", "thrownError"]).apply(this, [newTab, pane, xhr, ajaxOptions, thrownError]);
            },
            complete: function (xhr, status) {
                newTab.data("loaded", true);
                var tabName = newTab.data('tab-name') || newTab.attr("href").replace(/#/, "");
                tabbable.append('<input type="hidden" class="loaded-tab-name" name="LoadedTabs" value="' + tabName + '" />');

                app.plugins.getFunction(tabbable.data("ajax-oncomplete"), ["tab", "pane", "xhr", "status"]).apply(this, [newTab, pane, xhr, status]);

                // Resize Kendo Windows for each single main Tab scroll
                app.tab.handlers.resizeWindowsForEachSingleMainTabScroll();
            }
        });
    });
}
app.plugins.tab.initTabShown = function ($ctx) {
    $ctx.find('div.tabbable > ul.nav a[data-toggle=tab]').on('shown.bs.tab', function (e) {
        // Resize Kendo Windows for each single main Tab scroll
        app.tab.handlers.resizeWindowsForEachSingleMainTabScroll();
    });
}
app.plugins.tab.initTabScroll = function ($ctx) {
    $ctx.find('div.tabbable .tab-content .tab-pane').on('scroll', function (e) {
        // Fix validation tooltip (qTip2) hiện ở vị trí không đúng trong form khi scroll
        delay('app.plugins.tab.initTabScroll__app.qtip.hideAll', function () {
            app.qtip.hideAll();
        }, 150);
        // Custom: Hide all datetimepickers when window resizes
        delay('app.plugins.tab.initTabScroll__app.plugins.datetimepicker.hideAll', function () {
            app.plugins.datetimepicker.hideAll();
        }, 150);
    });
}

app.plugins.uniform.handles = function () {
    if (!$().uniform) {
        return;
    }
    var test = $("input[type=checkbox]:not(.nostyle, .toggle, .make-switch, .icheck, .onoffswitch-checkbox, .check-all, .row-checkbox, .k-checkbox), input[type=radio]:not(.nostyle, .toggle, .star, .make-switch, .icheck, .onoffswitch-checkbox)");
    if (test.size() > 0) {
        test.each(function (i, x) {
            var $x = $(x);
            if ($x.parents(".checker").size() === 0) {
                $x.show();
                $x.uniform();
            }
        });
    }
}
app.plugins.uniform.init = function ($elements) {
    if ($elements) {
        $elements.each(function (i, x) {
            var $x = $(x);
            if ($x.parents(".checker").size() === 0) {
                $x.show();
                $x.uniform();
            }
        });
    } else {
        app.plugins.uniform.handles();
    }
}
app.plugins.uniform.update = function ($elements) {
    $.uniform.update($elements); // update the uniform checkbox & radios UI after the actual input control state changed
}

app.plugins.ace.getCompleter = function (completer) {
    return {
        getCompletions: function (editor, session, pos, prefix, callback) {
            //if (prefix.length === 0) { callback(null, []); return }
            callback(null, completer);
        }
    };
};

app.plugins.applyCommonPlugins = function ($ctx) {
    app.plugins.fancytree.initCheckAll();
    app.plugins.select2.init($ctx);
    app.plugins.tab.init($ctx);
    app.plugins.uniform.init();
}
