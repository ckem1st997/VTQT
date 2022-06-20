app.form.init = function () {
    app.form.initComponents();
}
app.form.initComponents = function () {
    app.form.antiForgeryToken.init();
    app.form.initFormCommit();
    app.form.initLabel();
    app.form.initLabelHint();
}
app.form.antiForgeryToken.init = function () {
    app.form.antiForgeryToken.$container = $(app.form.antiForgeryToken.selector);
    app.form.antiForgeryToken.value = app.form.antiForgeryToken.$container.find('input[name="' + app.form.antiForgeryToken.cookieName + '"]').val();
}
app.form.getAntiForgeryToken = function () {
    var obj = {
    };
    obj[app.form.antiForgeryToken.cookieName] = app.form.antiForgeryToken.value;
    return obj;
}
app.form.focusFirst = function ($container) {
    var $this = $container;
    if (!$this.is('form')) {
        var $form = $this.find('form');
        if ($form.length)
            $this = $form;
    }
    $this.find('input:not([type="hidden"], [disabled], [disabled="disabled"], [readonly], [readonly="readonly"], [class*="check-all"], [data-action="checkAll"]):first').focus();
}
app.form.initFormCommit = function () {
    // Temp only
    $(".options button[value=save-continue]").click(function () {
        var btn = $(this);
        btn.closest("form").append('<input type="hidden" name="save-continue" value="true" />');
    });

    // publish entity commit messages
    $('.entity-commit-trigger').on('click', function (e) {
        var el = $(this);
        if (el.data('commit-type')) {
            PubSub.publish("entity-commit", {
                type: el.data('commit-type'),
                action: el.data('commit-action'),
                id: el.data('commit-id')
            });
        }
    });
}
app.form.initFormScroll = function ($container) {
    $container.find('.modal-body').scroll(function (e) {
        // Fix validation tooltip (qTip2) hiện ở vị trí không đúng trong form khi scroll
        delay('app.form.initFormScroll__app.qtip.hideAll', function () {
            app.qtip.hideAll();
        }, 150);
        // Custom: Hide all datetimepickers when window resizes
        delay('app.form.initFormScroll__app.plugins.datetimepicker.hideAll', function () {
            app.plugins.datetimepicker.hideAll();
        }, 150);
    });
}
app.form.initFormValidation = function (options) {
    var $form = options.$form;

    if (!$form.is('form'))
        return undefined;
    $.validator.unobtrusive.parse($form);
    var validator = $form.data('validator');
    if (!validator)
        return undefined;

    var errorDisplayMode = options.errorDisplayMode;
    if (!errorDisplayMode)
        errorDisplayMode = enums.form.ErrorDisplayMode.ShowOnFocus;

    validator.settings.errorPlacement = function (error, element) {
        // qTip call
        var $elem = $(element);
        var $target = null, flagTarget = false;

        // Select2
        var $s2 = $elem.next('.select2-container');
        if ($s2.length) {
            if (!error.is(':empty')) {
                flagTarget = true;
                $target = $s2;

                var s2 = $elem.data('select2');
                var $selection = s2.$selection;
                var $dropdown = s2.dropdown.$dropdown;
                var $search = s2.dropdown.$search;
                setTimeout(function () {
                    $selection.addClass('select2-input-validation-error');
                    $dropdown.addClass('select2-input-validation-error');
                    if ($search) {
                        $search.addClass('input-validation-error');
                    }
                });
            }
        }

        // DateTimePicker
        if (flagTarget === false) {
            var $dtPicker = $elem.parent();
            if ($dtPicker.length && $dtPicker.hasClass('date') && $dtPicker.attr('id') === $elem.attr('id') + '-parent') {
                flagTarget = true;
                $target = $dtPicker;
            }
        }

        // Kendo NumericTextBox
        if (flagTarget === false) {
            var $num = $elem.parent();
            if ($num.length && $num.hasClass('k-numeric-wrap')) {
                flagTarget = true;
                $target = $num.parent();
                $num.addClass('span-input-validation-error');
            }
        }

        // If form has portlet and it's collapse => expand portlet
        var $portlet = $form.find('.portlet.form');
        // If form in portlet and portlet is collapse => expand portlet
        if (!$portlet.length)
            $portlet = $form.closest('.portlet.form');
        if ($portlet.length) {
            var $portletBody = $portlet.find('.portlet-body');
            var $a = $portlet.find('.tools > a:last');
            var status = $a.attr('class');
            if (status === 'expand') {
                $portletBody.show('slideDown');
                $a.attr('class', 'collapse');
                $form.valid();
            }
        }

        if (!error.is(':empty')) {
            var api = ($target || $elem).not('.valid').qtip({
                overwrite: false,
                content: {
                    text: error
                },
                position: {
                    target: $target || $elem,
                    my: 'bottom right',
                    at: 'top right',
                    viewport: $(window),
                    adjust: {
                        mouse: false,
                        resize: true,
                        scroll: true,
                        method: 'none' // Don't adjust the tooltip when it goes off-screen/hidden to prevent show it when it has been hidden
                    }
                },
                show: {
                    event: 'click focus focusin mouseenter',
                    solo: errorDisplayMode === enums.form.ErrorDisplayMode.ShowAllErrors ? false : true,
                    delay: 300
                },
                hide: {
                    event: 'unfocus blur focusout'
                },
                style: {
                    classes: 'qtip-red qtip-rounded'
                }
            })
            // If we have a tooltip on this element already, just update its content
            // => Update validation message if element has multiple validations
            .qtip('option', 'content.text', error);

            if (errorDisplayMode === enums.form.ErrorDisplayMode.ShowOnFocus) {
                // Only show error tooltip when focus error element:
                // No codes for perform that
            } else if (errorDisplayMode === enums.form.ErrorDisplayMode.ShowFirstError) {
                // Only show error tooltip for first error element:
                // Find first error element in form and check if current element is the first error element -> show error tooltip
                var $firstElem = $form.find('.input-validation-error:first');
                if ($elem.is($firstElem))
                    api.qtip('show');
            } else if (errorDisplayMode === enums.form.ErrorDisplayMode.ShowAllErrors) {
                // Show all error tooltips of error elements
                api.qtip('show');
            }
        } else {
            $elem.qtip('destroy');
        }
    };
    validator.settings.success = function (error, element) {
        // Hide tooltips on valid elements
        setTimeout(function () {
            $form.find('.valid').qtip('hide');
        });

        var $elem = $(element), flagTarget = false;

        // Select2
        var $s2 = $elem.next('.select2-container');
        if ($s2.length) {
            flagTarget = true;

            var s2 = $elem.data('select2');
            var $selection = s2.$selection;
            var $dropdown = s2.dropdown.$dropdown;
            var $search = s2.dropdown.$search;
            setTimeout(function () {
                $selection.removeClass('select2-input-validation-error');
                $dropdown.removeClass('select2-input-validation-error');
                if ($search) {
                    $search.removeClass('input-validation-error');
                }
            });
        }
    };
    validator.settings.unhighlight = function (element, errorClass, validClass) {
        var $elem = $(element);
        $elem.removeClass('input-validation-error');
        var flagTarget = false;

        // Select2
        var $s2 = $elem.next('.select2-container');
        if ($s2.length) {
            flagTarget = true;

            var s2 = $elem.data('select2');
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

        // Kendo NumericTextBox
        if (flagTarget === false) {
            var $num = $elem.parent();
            if ($num.length && $num.hasClass('k-numeric-wrap')) {
                flagTarget = true;
                var $numWidget = $num.parent();
                $num.removeClass('span-input-validation-error');
                $numWidget.qtip('destroy');
            }
        }
    };
    return validator;
}
app.form.initLabel = function () {
    $(document).on('click', 'label', function (e) {
        var $this = $(this);
        var forElement = $this.attr('for');
        if (!forElement)
            return;
        var $group = $this.closest('.form-group');
        if (!$group.length)
            $group = $this.closest('.adminTitle').parent('tr');
        if (!$group.length)
            $group = $this.closest('div, p');
        if ($group) {
            var $element = $group.find('#' + forElement), $focus = [];
            if (!$element.length || $element.prop('disabled'))
                return;

            if ($element.is('input')) {
                // Kendo Numeric Textbox
                if ($element.attr('data-role') === 'numerictextbox') {
                    $focus = $element.siblings('input.k-input');
                }

                if ($focus.length)
                    $focus.focus();
            } else if ($element.is('select') && $element.data('select2')) {
                // Select2
                var $s2Container = $element.siblings('.select2-container');
                if ($s2Container.length) {
                    $s2Container.click(); // For display validation tooltip via click event
                    $element.select2('open');
                }
            }
        }
    });
}
app.form.initLabelHint = function ($container) {
    var $selector = $container || $(document);
    $selector.find('.form-group a.hint, td.adminTitle a.hint').qtip({
        position: {
            my: 'center right',
            at: 'center left',
            target: 'event',
            viewport: $(window),
            adjust: {
                mouse: false,
                scroll: false
            }
        },
        show: {
            delay: 400
        },
        style: {
            classes: 'qtip-bootstrap qtip-hint'
        }
    });
}