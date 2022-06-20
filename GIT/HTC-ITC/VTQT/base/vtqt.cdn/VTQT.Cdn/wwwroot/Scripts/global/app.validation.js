app.validator.init = function () {
    app.validator.setDefaults();
    app.validator.initComponents();
}
app.validator.initComponents = function () {
    //#region Select2

    $(document).on('change', 'select', function (e) {
        var $form = $(this).closest('form');
        if ($form.length && $.validator) {
            var validator = $form.validate();
            validator.element($(this));
        }
    });

    //#endregion
}
app.validator.setDefaults = function () {
    /* data-val-control="true" là để đánh dấu validate các control bị hidden đi khi dùng plugin (mặc định jquery.validate sẽ bỏ qua các control bị hidden), VD: select bị ẩn đi khi dùng plugin Select2 */
    $.validator.setDefaults({
        ignore: ':hidden:not([data-val="true"], [data-val="True"]), [data-val="false"], [data-val="False"]'
    });
}
