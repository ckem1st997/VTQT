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
