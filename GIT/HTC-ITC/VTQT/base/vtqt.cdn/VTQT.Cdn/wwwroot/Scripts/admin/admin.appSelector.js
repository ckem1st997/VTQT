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
