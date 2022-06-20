app.ajaxForm.init = function (options) {
    var $form;
    if (options.$container.is('form'))
        $form = options.$container;
    else
        $form = options.$container.find('form:first');

    if (!$form.length)
        return;

    var formtype = $form.attr('data-form-type');

    if (formtype === 'details')
        return;

    if (typeof options.initCallback == 'function')
        options.initCallback();

    // Assign to local variable function and set global variable null to perform local validation so will not affect other app.ajaxForm.init by app.window.form
    var globalValidationCallback = null;
    if (typeof app.ajaxForm.validationCallback == 'function') {
        globalValidationCallback = app.ajaxForm.validationCallback;
        app.ajaxForm.validationCallback = null;
    }

    // Assign to local variable function and set global variable null to perform local callback so will not affect other app.ajaxForm.init by app.window.form
    var globalCallback = null;
    if (typeof app.ajaxForm.callback == 'function') {
        globalCallback = app.ajaxForm.callback;
        app.ajaxForm.callback = null;
    }

    // Assign to local variable function and set global variable null to perform local callback so will not affect other app.ajaxForm.init by app.window.form
    var globalContinueAddingCallback = null;
    if (typeof app.ajaxForm.continueAddingCallback == 'function') {
        globalContinueAddingCallback = app.ajaxForm.continueAddingCallback;
        app.ajaxForm.continueAddingCallback = null;
    }

    // Assign to local variable function and set global variable null to perform local callback so will not affect other app.ajaxForm.init by app.window.form
    var globalContinueEditingCallback = null;
    if (typeof app.ajaxForm.continueEditingCallback == 'function') {
        globalContinueEditingCallback = app.ajaxForm.continueEditingCallback;
        app.ajaxForm.continueEditingCallback = null;
    }

    var $winForm = $form.data('$winForm');
    var isWinForm = $winForm && $winForm.attr('id').indexOf('app_window_form') >= 0;

    var autoClose = true;
    if (options.autoClose !== undefined && options.autoClose !== null && options.autoClose === false)
        autoClose = false;

    var $btnSubmit;
    if (options.submitElement) {
        $btnSubmit = $form.find(options.submitElement);
        if (!$btnSubmit.length)
            $btnSubmit = $(options.submitElement);
    } else {
        $btnSubmit = $form.find('button[type="submit"]:not([data-action="continueAdding"]):not([data-action="continueEditing"]):not([data-action="continueSubmitting"])');
    }

    var continueAdding = false;
    var $continueAdding;
    if (options.continueAddingElement) {
        $continueAdding = $form.find(options.continueAddingElement);
        if (!$continueAdding.length)
            $continueAdding = $(options.continueAddingElement);
    } else {
        $continueAdding = $form.find('button[data-action="continueAdding"]');
    }

    var continueEditing = false;
    var $continueEditing;
    if (options.continueEditingElement) {
        $continueEditing = $form.find(options.continueEditingElement);
        if (!$continueEditing.length)
            $continueEditing = $(options.continueEditingElement);
    } else {
        $continueEditing = $form.find('button[data-action="continueEditing"]');
    }

    var continueSubmitting = false;
    var $continueSubmitting;
    if (options.continueSubmittingElement) {
        $continueSubmitting = $form.find(options.continueSubmittingElement);
        if (!$continueSubmitting.length)
            $continueSubmitting = $(options.continueSubmittingElement);
    } else {
        $continueSubmitting = $form.find('button[data-action="continueSubmitting"]');
    }

    // Ajax form
    if (formtype && formtype === 'ajax') {
        // For attach submit to non-submit button
        $btnSubmit.click(function (e) {
            e.preventDefault();
            continueAdding = false;
            continueEditing = false;
            continueSubmitting = false;
            $form.data('SubmitType', '');
            $form.data('SubmitType', 'submit');
            $form.submit();
        });
        if ($continueAdding.length) {
            $continueAdding.click(function (e) {
                e.preventDefault();
                continueAdding = true;
                $form.data('SubmitType', '');
                $form.data('SubmitType', 'continueAdding');
                $form.submit();
            });
        }
        if ($continueEditing.length) {
            $continueEditing.click(function (e) {
                e.preventDefault();
                continueEditing = true;
                $form.data('SubmitType', '');
                $form.data('SubmitType', 'continueEditing');
                $form.submit();
            });
        }
        if ($continueSubmitting.length) {
            $continueSubmitting.click(function (e) {
                e.preventDefault();
                continueSubmitting = true;
                $form.data('SubmitType', '');
                $form.data('SubmitType', 'continueSubmitting');
                $form.submit();
            });
        }

        // Assign to local variable function and set global variable null to perform local validation so will not affect other app.ajaxForm.init by app.window.form
        var globalExtraData = null;
        if (app.ajaxForm.extraData != null) {
            globalExtraData = app.ajaxForm.extraData;
            app.ajaxForm.extraData = null;
        }

        var validator = $form.initFormValidation();
        if (validator) {
            validator.settings.submitHandler = function (form) {
                if ($btnSubmit.length)
                    $btnSubmit.prop('disabled', true);
                if ($continueAdding.length)
                    $continueAdding.prop('disabled', true);
                if ($continueEditing.length)
                    $continueEditing.prop('disabled', true);

                var $spinSubmit = $btnSubmit.find('i.spin-submit');
                var $spinSubmitOthers = $spinSubmit.siblings();
                var $spinContinueAdding, $spinContinueAddingOthers;
                var $spinContinueEditing, $spinContinueEditingOthers;
                if (continueAdding) {
                    $spinContinueAdding = $continueAdding.find('i.spin-submit');
                    $spinContinueAddingOthers = $spinContinueAdding.siblings();
                }
                if (continueEditing) {
                    $spinContinueEditing = $continueEditing.find('i.spin-submit');
                    $spinContinueEditingOthers = $spinContinueEditing.siblings();
                }

                if ($form.valid()
                    && (options.validationCallback === undefined || options.validationCallback == null
                        || (typeof options.validationCallback == 'function' && options.validationCallback() === true))
                    && (globalValidationCallback == null
                        || globalValidationCallback())) {
                    if (CKEDITOR) {
                        for (instance in CKEDITOR.instances)
                            CKEDITOR.instances[instance].updateElement();
                    }
                    $.ajax({
                        type: 'POST',
                        data: $form.serialize(),
                        dataType: 'json',
                        url: options.url,
                        beforeSend: function (jqXhr, settings) {
                            if (continueAdding) {
                                $spinContinueAddingOthers.hide();
                                $spinContinueAdding.show();
                            } else if (continueEditing) {
                                $spinContinueEditingOthers.hide();
                                $spinContinueEditing.show();
                            } else {
                                $spinSubmitOthers.hide();
                                $spinSubmit.css('display', 'inline-block');
                            }

                            if ($btnSubmit.length)
                                $btnSubmit.prop('disabled', true);
                            if ($continueAdding.length)
                                $continueAdding.prop('disabled', true);
                            if ($continueEditing.length)
                                $continueEditing.prop('disabled', true);

                            if (options.extraData !== undefined && options.extraData != null) {
                                var objExtraData;
                                if (typeof options.extraData === 'function') {
                                    objExtraData = options.extraData();
                                } else if (typeof options.extraData === 'object') {
                                    objExtraData = options.extraData;
                                }
                                if (objExtraData)
                                    settings.data += '&' + $.param(objExtraData);
                            }
                            if (globalExtraData != null) {
                                var objGlobalExtraData;
                                if (typeof globalExtraData === 'function') {
                                    objGlobalExtraData = globalExtraData();
                                } else if (typeof extraData === 'object') {
                                    objGlobalExtraData = globalExtraData;
                                }
                                if (objGlobalExtraData)
                                    settings.data += '&' + $.param(objGlobalExtraData);
                            }
                        },
                        success: function (data, textStatus, jqXhr) {
                            if (!data)
                                return;
                            if (data.isRedirect) {
                                window.location.href = data.redirectUrl;
                                return;
                            }
                            if (data.success) {
                                if (continueAdding) { // Continue adding form
                                    if (!_.isEmpty(data.message))
                                        notify({ title: data.title, text: data.message, type: 'success' });

                                    var clientFormId = options.$container.attr('data-client-form-id');
                                    var mergedData = $.extend(options.extraData, { ClientFormId: clientFormId });

                                    $.ajax({
                                        type: 'GET',
                                        dataType: 'html',
                                        data: mergedData,
                                        url: options.url,
                                        beforeSend: function (jqXhr, settings) {
                                            if (isWinForm)
                                                app.ui.loader(options.$container.closest('.k-widget.k-window'), true);
                                            else
                                                app.ui.loader(options.$container, true);
                                        },
                                        success: function (data, textStatus, jqXhr) {
                                            var $formHtml = $(data);
                                            var $form = $formHtml.find('form:first');
                                            if (!$form.attr('data-client-form-id'))
                                                $form.attr('data-client-form-id', clientFormId);

                                            if (isWinForm) {
                                                $form.data('$winForm', $winForm);
                                                $form.data('winForm', $winForm.data("kendoWindow"));

                                                options.$container.data("kendoWindow").content($formHtml);
                                                app.window.form.initWindow(options.$container);
                                            } else {
                                                options.$container.html(data);
                                                $form.initFormScroll();
                                            }

                                            if (options.initAjaxForm === undefined || options.initAjaxForm == null || options.initAjaxForm)
                                                options.$container.initAjaxForm(options);

                                            $winForm.focusFirst();

                                            if (isWinForm)
                                                app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                                            else
                                                app.ui.loader(options.$container, false);
                                            return;
                                        },
                                        error: function (jqXhr, textStatus, errorThrown) {
                                            if (isWinForm)
                                                app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                                            else
                                                app.ui.loader(options.$container, false);
                                        }
                                    });

                                    if (typeof options.continueAddingCallback == 'function')
                                        options.continueAddingCallback({ data: data, $container: options.$container, $form: $form });
                                    if (typeof globalContinueAddingCallback == 'function')
                                        globalContinueAddingCallback({ data: data, $form: $form });
                                } else if (continueEditing) { // Continue editing form
                                    if (!_.isEmpty(data.message))
                                        notify({ title: data.title, text: data.message, type: 'success' });

                                    if (typeof options.continueEditingCallback == 'function')
                                        options.continueEditingCallback({ data: data, $container: options.$container, $form: $form });
                                    if (typeof globalContinueEditingCallback == 'function')
                                        globalContinueEditingCallback({ data: data, $form: $form });
                                } else { // Non continue adding form & non continue editing form
                                    if (autoClose && options.$container.data('kendoWindow')) {
                                        options.$container.data('kendoWindow').close();
                                    }
                                    if (!_.isEmpty(data.message))
                                        notify({ title: data.title, text: data.message, type: 'success' });
                                }

                                if (typeof options.callback == 'function')
                                    options.callback({ data: data, $form: $form });
                                if (typeof globalCallback == 'function')
                                    globalCallback({ data: data, $form: $form });

                                if (continueAdding) {
                                    $spinContinueAdding.hide();
                                    $spinContinueAddingOthers.show();
                                } else if (continueEditing) {
                                    $spinContinueEditing.hide();
                                    $spinContinueEditingOthers.show();
                                } else {
                                    $spinSubmit.hide();
                                    $spinSubmitOthers.show();
                                }

                                // If kendo window is closing, wait for it to be closed to prevent multiple click submit
                                setTimeout(function () {
                                    if ($btnSubmit.length)
                                        $btnSubmit.prop('disabled', false);
                                    if ($continueAdding.length)
                                        $continueAdding.prop('disabled', false);
                                    if ($continueEditing.length)
                                        $continueEditing.prop('disabled', false);
                                }, 100);
                            } else {
                                if (!_.isEmpty(data.message))
                                    notify({ title: data.title, text: data.message, type: 'error' });
                                if (continueAdding) {
                                    $spinContinueAdding.hide();
                                    $spinContinueAddingOthers.show();
                                } else if (continueEditing) {
                                    $spinContinueEditing.hide();
                                    $spinContinueEditingOthers.show();
                                } else {
                                    $spinSubmit.hide();
                                    $spinSubmitOthers.show();
                                }

                                if ($btnSubmit.length)
                                    $btnSubmit.prop('disabled', false);
                                if ($continueAdding.length)
                                    $continueAdding.prop('disabled', false);
                                if ($continueEditing.length)
                                    $continueEditing.prop('disabled', false);
                            }
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            if (continueAdding) {
                                $spinContinueAdding.hide();
                                $spinContinueAddingOthers.show();
                            } else if (continueEditing) {
                                $spinContinueEditing.hide();
                                $spinContinueEditingOthers.show();
                            } else {
                                $spinSubmit.hide();
                                $spinSubmitOthers.show();
                            }

                            if ($btnSubmit.length)
                                $btnSubmit.prop('disabled', false);
                            if ($continueAdding.length)
                                $continueAdding.prop('disabled', false);
                            if ($continueEditing.length)
                                $continueEditing.prop('disabled', false);
                        }
                    });
                } else {
                    if (continueAdding) {
                        $spinContinueAdding.hide();
                        $spinContinueAddingOthers.show();
                    } else if (continueEditing) {
                        $spinContinueEditing.hide();
                        $spinContinueEditingOthers.show();
                    } else {
                        $spinSubmit.hide();
                        $spinSubmitOthers.show();
                    }

                    if ($btnSubmit.length)
                        $btnSubmit.prop('disabled', false);
                    if ($continueAdding.length)
                        $continueAdding.prop('disabled', false);
                    if ($continueEditing.length)
                        $continueEditing.prop('disabled', false);
                }
                return;
            };
        } else { // Form doesn't have validation, such as: Deletes, Authorize forms, ... 
            $form.submit(function (e) {
                if ($btnSubmit.length)
                    $btnSubmit.prop('disabled', true);
                e.preventDefault();

                var $spinSubmit = $btnSubmit.find('i.spin-submit');
                var $spinSubmitOthers = $spinSubmit.siblings();
                var $spinContinueSubmitting, $spinContinueSubmittingOthers;
                if (continueSubmitting) {
                    $spinContinueSubmitting = $continueSubmitting.find('i.spin-submit');
                    $spinContinueSubmittingOthers = $spinContinueSubmitting.siblings();
                }

                if ((options.validationCallback === undefined || options.validationCallback == null
                        || (typeof options.validationCallback == 'function' && options.validationCallback() === true))
                    && (globalValidationCallback == null
                        || globalValidationCallback())) {
                    if (CKEDITOR) {
                        for (instance in CKEDITOR.instances)
                            CKEDITOR.instances[instance].updateElement();
                    }
                    $.ajax({
                        type: 'POST',
                        data: $form.serialize(),
                        dataType: 'json',
                        url: options.url,
                        beforeSend: function (jqXhr, settings) {
                            if (continueSubmitting) {
                                $spinContinueSubmittingOthers.hide();
                                $spinContinueSubmitting.show();
                            } else {
                                $spinSubmitOthers.hide();
                                $spinSubmit.css('display', 'inline-block');
                            }

                            if ($btnSubmit.length)
                                $btnSubmit.prop('disabled', true);
                            if ($continueSubmitting.length)
                                $continueSubmitting.prop('disabled', true);

                            if (options.extraData !== undefined && options.extraData != null) {
                                var objExtraData;
                                if (typeof options.extraData === 'function') {
                                    objExtraData = options.extraData();
                                } else if (typeof options.extraData === 'object') {
                                    objExtraData = options.extraData;
                                }
                                if (objExtraData)
                                    settings.data += '&' + $.param(objExtraData);
                            }
                            if (globalExtraData != null) {
                                var objGlobalExtraData;
                                if (typeof globalExtraData === 'function') {
                                    objGlobalExtraData = globalExtraData();
                                } else if (typeof extraData === 'object') {
                                    objGlobalExtraData = globalExtraData;
                                }
                                if (objGlobalExtraData)
                                    settings.data += '&' + $.param(objGlobalExtraData);
                            }
                        },
                        success: function (data, textStatus, jqXhr) {
                            if (!data)
                                return;
                            if (data.isRedirect) {
                                window.location.href = data.redirectUrl;
                                return;
                            }
                            if (data.success) {
                                if (continueSubmitting) {
                                    $spinContinueSubmitting.hide();
                                    $spinContinueSubmittingOthers.show();

                                    if ($btnSubmit.length)
                                        $btnSubmit.prop('disabled', false);
                                    if ($continueSubmitting.length)
                                        $continueSubmitting.prop('disabled', false);
                                } else {
                                    if (autoClose && options.$container.data('kendoWindow')) {
                                        options.$container.data('kendoWindow').close();
                                    }
                                    if (!_.isEmpty(data.message))
                                        notify({ title: data.title, text: data.message, type: 'success' });
                                }

                                if (typeof options.callback == 'function')
                                    options.callback(data);
                                if (typeof globalCallback == 'function')
                                    globalCallback({ data: data, $form: $form });

                                if (continueSubmitting) {
                                    $spinContinueSubmitting.hide();
                                    $spinContinueSubmittingOthers.show();
                                } else {
                                    $spinSubmit.hide();
                                    $spinSubmitOthers.show();
                                }

                                // If kendo window is closing, wait for it to be closed to prevent multiple click submit
                                setTimeout(function () {
                                    if ($btnSubmit.length)
                                        $btnSubmit.prop('disabled', false);
                                    if ($continueSubmitting.length)
                                        $continueSubmitting.prop('disabled', false);
                                }, 100);
                            } else {
                                if (!_.isEmpty(data.message))
                                    notify({ title: data.title, text: data.message, type: 'error' });

                                if (continueSubmitting) {
                                    $spinContinueSubmitting.hide();
                                    $spinContinueSubmittingOthers.show();
                                } else {
                                    $spinSubmit.hide();
                                    $spinSubmitOthers.show();
                                }

                                if ($btnSubmit.length)
                                    $btnSubmit.prop('disabled', false);
                                if ($continueSubmitting.length)
                                    $continueSubmitting.prop('disabled', false);
                            }
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            if (continueSubmitting) {
                                $spinContinueSubmitting.hide();
                                $spinContinueSubmittingOthers.show();
                            } else {
                                $spinSubmit.hide();
                                $spinSubmitOthers.show();
                            }

                            if ($btnSubmit.length)
                                $btnSubmit.prop('disabled', false);
                            if ($continueSubmitting.length)
                                $continueSubmitting.prop('disabled', false);
                        }
                    });
                }
                return;
            });
        }
    } else { // Non-ajax form (such as: chosen form)
        $form.initFormValidation();
        $form.submit(function (e) {
            if ($btnSubmit.length)
                $btnSubmit.prop('disabled', true);
            e.preventDefault();
            if ($form.valid()
                && (options.validationCallback === undefined || options.validationCallback == null
                    || (typeof options.validationCallback == 'function' && options.validationCallback() === true))
                && (globalValidationCallback == null
                    || globalValidationCallback())) {
                if (CKEDITOR) {
                    for (instance in CKEDITOR.instances)
                        CKEDITOR.instances[instance].updateElement();
                }
                if (typeof options.callback == 'function')
                    options.callback();
                if (typeof globalCallback == 'function')
                    globalCallback({ data: data, $form: $form });
            }

            if ($btnSubmit.length)
                $btnSubmit.prop('disabled', false);
        });
    }
}

app.ajaxForm.reload = function (options) {
    var $form;
    if (options.$container.is('form'))
        $form = options.$container;
    else
        $form = options.$container.find('form:first');

    if (!$form.length)
        return;

    var $winForm = $form.data('$winForm');
    var isWinForm = $winForm && $winForm.attr('id').indexOf('app_window_form') >= 0;
    var clientFormId = $winForm.attr('data-client-form-id');
    var mergedData = $.extend(options.extraData, { ClientFormId: clientFormId });
    // delay để dùng khi bật form liên tiếp thì form sau các Control được khởi tạo đúng (có nhiều Control trùng id, name với nhau nên sẽ chỉ có Control được find đầu tiên mới khởi tạo)
    // sau khi form trước đã destroy (vì khi close window có một khoảng thời gian thực hiện animation).
    // Nếu không dùng delay thì dùng cách đặt id khác nhau cho Control để khi bật nhiều form thì các Control được khởi tạo đúng.
    // Hoặc setTimeout cho các công việc được thực hiện ngay sau lệnh close window (nhưng sẽ phải xử lý loader cho mượt).
    var delay = 0;
    if (options.delay) {
        delay = options.delay;
        if (isWinForm)
            app.ui.loader(options.$container.closest('.k-widget.k-window'), true);
        else
            app.ui.loader(options.$container, true);
    }

    setTimeout(function () {
        $.ajax({
            type: 'GET',
            dataType: 'html',
            data: mergedData,
            url: options.url,
            beforeSend: function (jqXhr, settings) {
                if (!options.delay) {
                    if (isWinForm)
                        app.ui.loader(options.$container.closest('.k-widget.k-window'), true);
                    else
                        app.ui.loader(options.$container, true);
                }
            },
            success: function (data, textStatus, jqXhr) {
                if (!data) {
                    if (isWinForm)
                        app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                    else
                        app.ui.loader(options.$container, false);
                    return null;
                }

                if (isJSON(data)) {
                    if (!data.success) {
                        if (!_.isEmpty(data.message))
                            notify({ title: data.title, text: data.message, type: 'error' });
                    }

                    if (isWinForm)
                        app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                    else
                        app.ui.loader(options.$container, false);
                    return null;
                } else {
                    if (isWinForm) {
                        if (options.options !== undefined && options.options != null)
                            $winForm.data("kendoWindow").setOptions(options.options);
                    }

                    var $formHtml = $(data);
                    var $form = $formHtml.find('form:first');
                    if (!$form.attr('data-client-form-id'))
                        $form.attr('data-client-form-id', clientFormId);

                    if (isWinForm) {
                        $form.data('$winForm', $winForm);
                        $form.data('winForm', $winForm.data("kendoWindow"));

                        $winForm.data("winForm").content($formHtml);
                        app.window.form.initWindow($winForm);
                    } else {
                        options.$container.html(data);
                        $form.initFormScroll();
                    }

                    if (options.initAjaxForm === undefined || options.initAjaxForm == null || options.initAjaxForm)
                        $winForm.initAjaxForm({
                            url: options.actionUrl || options.url,
                            extraData: options.extraData,
                            initAjaxForm: options.initAjaxForm,
                            autoClose: options.autoClose,
                            submitElement: options.submitElement,
                            continueAddingElement: options.continueAddingElement,
                            continueEditingElement: options.continueEditingElement,
                            errorDisplayMode: options.errorDisplayMode,
                            initCallback: options.initCallback,
                            validationCallback: options.validationCallback,
                            callback: options.callback,
                            continueEditingCallback: options.continueEditingCallback
                        });

                    $winForm.focusFirst();

                    if (isWinForm)
                        app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                    else
                        app.ui.loader(options.$container, false);
                    return options.$container;
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                if (isWinForm)
                    app.ui.loader(options.$container.closest('.k-widget.k-window'), false);
                else
                    app.ui.loader(options.$container, false);
            }
        });
    }, delay);
}
