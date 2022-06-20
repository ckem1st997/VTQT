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
