//#region app.window.form

app.window.form.initWindow = function ($winForm) {
    var $form = $winForm.find('form:first');
    var confirmClose = $form.attr('data-form-confirmclose');

    $form.find('.form-actions button[data-dismiss="modal"]').off('click').on('click', function (e) {
        if (confirmClose) {
            app.window.confirm.open({
                title: window.Res['Common.Windows.Forms.Close.Confirm.Default.Title'], text: window.Res['Common.Windows.Forms.Close.Confirm.DataMightBeChanged'],
                callback: function () {
                    $winForm.data("kendoWindow").close();
                }
            });
        } else {
            setTimeout(function () {
                $winForm.data("kendoWindow").close();
            }, 150);
        }
    });

    var $title = $form.find('.modal-title');
    if ($title.length) {
        $winForm.data("kendoWindow").title($title.html());
        var $winContainer = $winForm.parent();
        $winContainer.find('.k-header .k-window-title').attr('title', $title.text());
    }

    // Init ajax
    setTimeout(function () {
        App.initAjax();
        app.initAjax();
    }, 100);

    $form.initFormScroll();

    $form.initLabelHint(); // Init label hint
}
app.window.form.open = function (options) {
    var $winForm;
    var clientFormId = Date.now();
    var mergedData = $.extend(options.extraData, { ClientFormId: clientFormId });
    var modal = (options.options !== undefined && options.options != null && options.options.modal) || true;
    var initAjaxForm = options.initAjaxForm === undefined || options.initAjaxForm == null || options.initAjaxForm;
    // delay để dùng khi bật form liên tiếp thì form sau các Control được khởi tạo đúng (có nhiều Control trùng id, name với nhau nên sẽ chỉ có Control được find đầu tiên mới khởi tạo)
    // sau khi form trước đã destroy (vì khi close window có một khoảng thời gian thực hiện animation).
    // Nếu không dùng delay thì dùng cách đặt id khác nhau cho Control để khi bật nhiều form thì các Control được khởi tạo đúng.
    // Hoặc setTimeout cho các công việc được thực hiện ngay sau lệnh close window (nhưng sẽ phải xử lý loader cho mượt).
    var delay = 0;
    if (options.delay) {
        delay = options.delay;
        app.ui.loader($('body'), true);
    }

    setTimeout(function () {
        $.ajax({
            type: 'GET',
            dataType: 'html',
            data: mergedData,
            url: options.url,
            beforeSend: function (jqXhr, settings) {
                if (!options.delay)
                    app.ui.loader($('body'), true);
            },
            success: function (data, textStatus, jqXhr) {
                if (!data) {
                    app.ui.loader($('body'), false);
                    return null;
                }

                if (isJSON(data)) {
                    if (!data.success) {
                        if (!_.isEmpty(data.message))
                            notify({ title: data.title, text: data.message, type: 'error' });
                    }

                    app.ui.loader($('body'), false);
                    return null;
                } else {
                    $winForm = $('<div></div>')
                        .attr('id', app.window.form.idFormat.format(clientFormId))
                        .attr('data-client-form-id', clientFormId)
                        .addClass('winform')
                        .appendTo(document.body);

                    $winForm.kendoWindow({
                        //autoFocus: false,
                        modal: modal,
                        resizable: false,
                        visible: false,
                        actions: ["Close"],
                        open: function (e) {
                            app.window.handlers.fixTop(this.wrapper, options.options.top);
                        },
                        activate: function (e) {
                            $winForm.focusFirst();
                        },
                        close: function (e) {
                            app.plugins.iconpicker.hideAll();

                            if (e.userTriggered) {
                                var $form = $winForm.find('form:first');
                                var confirmClose = $form.attr('data-form-confirmclose');
                                if (confirmClose) {
                                    e.preventDefault();
                                    app.window.confirm.open({
                                        title: window.Res['Common.Windows.Forms.Close.Confirm.Default.Title'], text: window.Res['Common.Windows.Forms.Close.Confirm.DataMightBeChanged'],
                                        callback: function () {
                                            $winForm.data("kendoWindow").close();
                                        }
                                    });
                                }
                            }
                        },
                        deactivate: function (e) {
                            $winForm.find('.input-validation-error,[data-hasqtip]').qtip('destroy', true);

                            // Xóa các vùng Html được gen ra từ Kendo ở cuối body
                            // Xóa Kendo ContextMenu để tránh mở sai (chỉ nhận ContextMenu được mở đầu tiên) khi có nhiều Menu có id trùng nhau ở trong winForm
                            var $ctxMenus = $('ul[data-client-form-id="' + clientFormId + '"]');
                            $.each($ctxMenus, function (i, x) {
                                var $x = $(x);
                                var $ulContainer = $x.parent('.k-animation-container');
                                if ($ulContainer.length)
                                    $ulContainer.remove();
                                else
                                    $x.remove();
                            });

                            $winForm.data("kendoWindow").destroy();
                        },
                        resize: function (e) {
                            app.window.handlers.fixTop(this.wrapper, options.options.top);
                        }
                    });
                    var $kWindow = $winForm.closest('.k-widget.k-window');
                    if (options.options !== undefined && options.options != null)
                        $winForm.data("kendoWindow").setOptions(options.options);

                    var $formHtml = $(data);
                    var $form = $formHtml.find('form:first');
                    if (!$form.attr('data-client-form-id'))
                        $form.attr('data-client-form-id', clientFormId);
                    $form.data('$container', options.container || $winForm);
                    $form.data('$winForm', $winForm);
                    $form.data('winForm', $winForm.data("kendoWindow"));
                    $winForm.data('winForm', $winForm.data("kendoWindow"));

                    $winForm.data("kendoWindow").content($formHtml);
                    app.window.form.initWindow($winForm);

                    if (initAjaxForm)
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

                    app.window.handlers.resizeForScroll({ $kWindow: $kWindow, center: false });

                    $winForm.data("kendoWindow")
                        .center()
                        .open();

                    app.ui.loader($('body'), false);

                    return $winForm;
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                app.ui.loader($('body'), false);
            }
        });
    }, delay);
}
app.window.form.destroyAll = function () {
    var $winForms = $('.winform');
    $.each($winForms, function (i, x) {
        var $x = $(x);
        var winForm = $x.data('kendoWindow');
        if (winForm)
            winForm.destroy();
    });
};

//#endregion

//#region app.window.alert

app.window.alert.init = function () {
    app.window.alert.$this = $(app.window.alert.selector);
    app.window.alert.contentHtml = app.window.alert.$this.clone().html();
}
app.window.alert.open = function (options) {
    if (!options.type)
        options.type = 'default';
    if (!options.title)
        options.title = window.Res['Common.Alert.Default.Title'];
    if (!options.options)
        options.options = {};
    if (!options.options.width)
        options.options.width = '450px';

    var alertHtml = app.window.alert.contentHtml.replace(/{{Type}}/, options.type).replace(/{{Content}}/, options.text);

    app.window.alert.$this.kendoWindow({
        modal: true,
        resizable: false,
        visible: false,
        actions: ["Close"],
        open: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        },
        deactivate: function (e) {
            app.window.alert.$this.find('.modal-body').html('');
        },
        resize: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        }
    });
    var $kWindow = app.window.alert.$this.closest('.k-widget.k-window');
    app.window.alert.$this.find('.modal-footer button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.alert.$this.data("kendoWindow").close();
    });
    app.window.alert.$this.data("kendoWindow").setOptions(options.options);

    app.window.handlers.resizeForScroll({ $kWindow: $kWindow, center: false });

    app.window.alert.$this.data("kendoWindow")
        .title(options.title)
        .content(alertHtml)
        .center()
        .open();
    app.window.alert.$this.find('.modal-footer button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.alert.$this.data("kendoWindow").close();
    });
    app.window.alert.$this.data("kendoWindow")
        .center()
        .open();
}
app.window.alert.close = function () {
    var winAlert = app.window.alert.$this.data('kendoWindow');
    if (winAlert)
        winAlert.close();
};

//#endregion

//#region app.window.confirm

app.window.confirm.init = function () {
    app.window.confirm.$this = $(app.window.confirm.selector);
    app.window.confirm.contentHtml = app.window.confirm.$this.clone().html();
}
app.window.confirm.open = function (options) {
    if (!options.type)
        options.type = 'default';
    if (!options.title)
        options.title = window.Res['Common.Confirm.Default.Title'];
    if (!options.text)
        options.text = window.Res['Common.Confirm.Default.Content'];
    if (!options.options)
        options.options = {};
    if (!options.options.width)
        options.options.width = '450px';

    var confirmHtml = app.window.confirm.contentHtml.replace(/{{Type}}/, options.type).replace(/{{Content}}/, options.text);

    app.window.confirm.$this.kendoWindow({
        modal: true,
        resizable: false,
        visible: false,
        actions: ["Close"],
        open: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        },
        deactivate: function (e) {
            app.window.confirm.$this.find('.modal-body').html('');
        },
        resize: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        }
    });
    var $kWindow = app.window.confirm.$this.closest('.k-widget.k-window');
    app.window.confirm.$this.find('.modal-footer button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.confirm.$this.data("kendoWindow").close();
    });
    app.window.confirm.$this.find('.modal-footer button[data-action="confirm"]').off('click').on('click', function (e) {
        app.window.confirm.$this.data("kendoWindow").close();
        options.callback();
    });
    app.window.confirm.$this.data("kendoWindow").setOptions(options.options);

    app.window.handlers.resizeForScroll({ $kWindow: $kWindow, center: false });

    app.window.confirm.$this.data("kendoWindow")
        .title(options.title)
        .content(confirmHtml)
        .center()
        .open();
    app.window.confirm.$this.find('.modal-footer button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.confirm.$this.data("kendoWindow").close();
    });
    app.window.confirm.$this.find('.modal-footer button[data-action="confirm"]').off('click').on('click', function (e) {
        app.window.confirm.$this.data("kendoWindow").close();
        options.callback();
    });
    app.window.confirm.$this.data("kendoWindow")
        .center()
        .open();
}
app.window.confirm.close = function () {
    var winConfirm = app.window.confirm.$this.data('kendoWindow');
    if (winConfirm)
        winConfirm.close();
};

//#endregion

//#region app.window.deletes

app.window.deletes.init = function () {
    app.window.deletes.$this = $(app.window.deletes.selector);
    app.window.deletes.contentHtml = app.window.deletes.$this.clone().html();
}
app.window.deletes.initWindow = function () {
    app.window.deletes.$this.find('.form-actions button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.deletes.$this.data("kendoWindow").close();
    });

    // Init ajax
    setTimeout(function () {
        App.initAjax();
        app.initAjax();
    }, 100);
}
app.window.deletes.open = function (options) {
    if (!options.type)
        options.type = 'default';
    if (!options.title)
        options.title = window.Res['Common.Deletes.Confirm.Default.Title'];
    if (!options.text)
        options.text = window.Res['Common.Deletes.Confirm.Default.Content'];
    if (!options.options)
        options.options = {};
    if (!options.options.width)
        options.options.width = '450px';

    var ids = '<div class="deletes-ids">';
    $.each(options.ids, function (i, x) {
        ids += '<input type="hidden" id="ids[' + i + ']" name="ids[' + i + ']" value="' + x + '" />';
    });
    ids += '</div>';
    var deleteText = '<div class="deletes-text">' + options.text + '</div>';
    var deleteHtml = app.window.deletes.contentHtml.replace(/{{Type}}/, options.type).replace(/{{Content}}/, ids + deleteText);

    app.window.deletes.$this.kendoWindow({
        modal: true,
        resizable: false,
        visible: false,
        actions: ["Close"],
        open: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        },
        deactivate: function (e) {
            app.window.deletes.$this.find('.modal-body').html('');
        },
        resize: function (e) {
            app.window.handlers.fixTop(this.wrapper, options.options.top);
        }
    });
    var $kWindow = app.window.deletes.$this.closest('.k-widget.k-window');
    app.window.deletes.$this.find('.modal-footer button[data-dismiss="modal"]').off('click').on('click', function (e) {
        app.window.deletes.$this.data("kendoWindow").close();
    });
    app.window.deletes.$this.data("kendoWindow").setOptions(options.options);

    app.window.handlers.resizeForScroll({ $kWindow: $kWindow, center: false });

    app.window.deletes.$this.data("kendoWindow")
        .title(options.title)
        .content(deleteHtml)
        .center()
        .open();
    app.window.deletes.initWindow();
    app.window.deletes.$this.data("kendoWindow")
        .center()
        .open();

    var token = app.form.getAntiForgeryToken();
    var mergedData = {};
    if (options.extraData !== undefined && options.extraData != null) {
        var objExtraData;
        if (typeof options.extraData === 'function') {
            objExtraData = options.extraData();
        } else if (typeof options.extraData === 'object') {
            objExtraData = options.extraData;
        }
        if (objExtraData)
            mergedData = $.extend(token, objExtraData);
    } else {
        mergedData = token;
    }

    app.window.deletes.$this.initAjaxForm({
        url: options.url,
        extraData: mergedData,
        submitElement: null,
        errorDisplayMode: null,
        initCallback: null,
        validationCallback: null,
        callback: options.callback
    });
}
app.window.deletes.close = function () {
    var winDeletes = app.window.deletes.$this.data('kendoWindow');
    if (winDeletes)
        winDeletes.close();
};

//#endregion

//#region Event Handlers

app.window.handlers.fixTop = function ($wrapper, top) {
    if (top !== undefined && top != null) {
        $wrapper.css({ top: top });
    } else {
        var wrapperTop = parseFloat($wrapper.css('top'));
        // Fix top of Kendo Window after customize Window & it's header (decreasing height) to center the Window exactly
        // fixNewTopPad = (origin height of "k-window-titlebar k-header" - customized height of "k-window-titlebar k-header") / 2
        var fixNewTopPad = 7.125;
        var fixTop = wrapperTop - fixNewTopPad;
        fixTop = fixTop > 0 ? fixTop : 0;
        $wrapper.css({ top: fixTop });
    }
}
app.window.handlers.resize = function () {
    var $kWindows = $('.k-widget.k-window');
    if ($kWindows.length) {
        $.each($kWindows, function (i, x) {
            var $kWindow = $(x);

            app.window.handlers.resizeForScroll({ $kWindow: $kWindow });
        });
    }
}

app.window.handlers.resizeForScroll = function (options) {
    if (!options.$kWindow.length || !options.$kWindow.is('.k-widget.k-window'))
        return;

    var center = options.initAjaxForm === undefined || options.initAjaxForm == null || options.initAjaxForm;

    var $kWindowContent = options.$kWindow.find('.k-window-content'),
        $modalBody = $kWindowContent.find('.form .modal-body'),
        kWindowHeight = options.$kWindow.outerHeight(),
        windowHeight = $(window).height(),
        kWindow = $kWindowContent.data('kendoWindow');

    if (options.formScrollParts) {
        var $scrollParts = $modalBody.find('.form-scroll-part:visible');
        if ($scrollParts.length) {
            var totalPartHeight = 0;
            $.each($scrollParts, function (i, x) {
                var $scrollPart = $(x);
                var scrollPartHeight = $scrollPart.outerHeight();
                totalPartHeight += scrollPartHeight;
            });

            var modalBodyHeightPad = $modalBody.height() - totalPartHeight;
            options.$kWindow.outerHeight(options.$kWindow.outerHeight() - modalBodyHeightPad);
            kWindowHeight = options.$kWindow.outerHeight();
        }
    }

    if (kWindowHeight > windowHeight) {
        options.$kWindow.outerHeight(windowHeight);
    } else if (kWindowHeight < windowHeight) {
        var scrollHeight = $modalBody.prop('scrollHeight'),
            modalBodyPad = 0,
            kWindowHeightWithModalBodyPad = 0;

        if (scrollHeight > 0)
            modalBodyPad = scrollHeight - $modalBody.height();
        if (modalBodyPad > 0)
            kWindowHeightWithModalBodyPad = kWindowHeight + modalBodyPad;

        if (kWindowHeightWithModalBodyPad > 0) {
            if (kWindowHeightWithModalBodyPad > windowHeight)
                options.$kWindow.outerHeight(windowHeight);
            else if (kWindowHeightWithModalBodyPad < windowHeight)
                options.$kWindow.outerHeight(kWindowHeightWithModalBodyPad);
        }
    }

    if (center)
        kWindow.center();

    kWindow.trigger('resize');
}

//#endregion
