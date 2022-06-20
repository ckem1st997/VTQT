/*
*  Project: SmartStore select wrapper 
*  Author: Murat Cakir, SmartStore AG
*/
;
(function ($, window, document, undefined) {

	var lists = [];

	$.fn.selectWrapper = function (options) {

        if (options && !_.isEmpty(options.resetDataUrl) && lists[options.resetDataUrl]) {
			lists[options.resetDataUrl] = null;
			return this.each(function () { });
		}

		return this.each(function () {

			var sel = $(this);

			if (sel.data("select2")) {
				// skip process, if select is skinned already
				return;
			}

			var autoWidth = sel.hasClass("autowidth"),
                minResultsForSearch = sel.data("select-min-results-for-search"),
                minInputLength = sel.data("select-min-input-length"),
                url = sel.data("select-url"),
                noCache = sel.data("select-nocache"), // future use
                loaded = sel.data("select-loaded"),
                lazy = sel.data("select-lazy"),
                initText = sel.data("select-init-text"),
                selectedId = sel.data("select-selected-id"),
                containerCssClass = sel.attr('class'), // Custom: Add classes to select2
                multiple = sel.attr('multiple'); // Custom: For config multiple select in Select2 >= 4.0

			var placeholder = getPlaceholder();

		    // Custom
            var closeOnSelect = true, attrCloseOnSelect = sel.data('select-close-on-select');
            if (attrCloseOnSelect !== undefined && attrCloseOnSelect != null)
                closeOnSelect = attrCloseOnSelect.toString().toLowerCase() === 'true' ? true : false;

			if (sel.is("select")) {
				// following code only applicable to select boxes (not input:hidden)
				var firstOption = sel.children("option").first();
				var hasOptionLabel = firstOption.length &&
                                     (firstOption[0].attributes['value'] === undefined || _.isEmpty(firstOption.val()));

				if (placeholder && hasOptionLabel) {
					// clear first option text in nullable dropdowns.
					// "allowClear" doesn't work otherwise.
					firstOption.text("");
				}

				if (placeholder && !hasOptionLabel) {
					// create empty first option
					// "allowClear" doesn't work otherwise.
					firstOption = $('<option></option>').prependTo(sel);
				}

				if (!placeholder && hasOptionLabel && firstOption.text()) {
					// use first option text as placeholder
					placeholder = firstOption.text();
					firstOption.text("");
				}
			}
			else {
				// sel is input:hidden
				if (placeholder && sel.val() == 0) {
					// we assume that a "0" value indicates nullability
					sel.removeAttr("value");
				}
			}

			function renderSelectItem(item) {
				try {
					var option = $(item.element),
						imageUrl = option.data('imageurl');

					if (imageUrl) {
						return '<img class="attribute-value-image" src="' + imageUrl + '" />' + item.text;
					}
				}
				catch (e) { }

				return item.text;
			}

			var opts = {
			    width: 'resolve',
			    // Custom: For config multiple select in Select2 >= 4.0
			    //allowClear: !!(placeholder), // assuming that a placeholder indicates nullability
			    placeholder: placeholder,
                minimumResultsForSearch: _.isNumber(minResultsForSearch) ? minResultsForSearch : 0,
			    minimumInputLength: _.isNumber(minInputLength) ? minInputLength : 0,
			    formatResult: renderSelectItem,
			    formatSelection: renderSelectItem,
			    containerCssClass: containerCssClass, // Custom: Add classes to select2
			    closeOnSelect: closeOnSelect // Custom
			};
		    // Custom: For config multiple select in Select2 >= 4.0
			if (!multiple)
			    opts.allowClear = !!(placeholder);
			$.extend(opts, options); // Custom

			if (url) {
				// url specified: load data remotely...
				if (sel.is("input:hidden") || lazy) {
					// ...but lazy (on first open)
					prepareLazyLoad(opts);
				}
				else {
					// ...immediately
					buildOptions();
				}
			}

		    // Custom: Change autowidth to dropdownAutoWidth in Select2 >= 4.0
		    //sel.select2(opts);

			if (autoWidth) {
			    // move special "autowidth" class to plugin container,
			    // so we are able to omit min-width per css
			    //sel.data("select2").container.addClass("autowidth"); // Custom: Change autowidth to dropdownAutoWidth in Select2 >= 4.0

			    // Custom: Change autowidth to dropdownAutoWidth in Select2 >= 4.0
			    opts.dropdownAutoWidth = true;
			}

		    // Custom: Change autowidth to dropdownAutoWidth in Select2 >= 4.0
			sel.select2(opts);

			function load() {
				$.ajax({
					url: url,
					dataType: 'json',
					async: false,
					data: { selectedId: selectedId || 0 },
					success: function (data, status, jqXHR) {
						lists[url] = data;
					}
				});
			};

			function prepareLazyLoad(o) {
				o.query = function (q) {
					if (!lists[url]) {
						load();
					}
					var list;
					if (!q.term) {
						list = lists[url];
					}
					else {
						list = _.filter(lists[url], function (val) {
							return new RegExp(q.term, "i").test(val.text);
						});
					}
					var data = { results: list };
					q.callback(data);
				}
				if (initText) {
					o.initSelection = function (element, callback) {
						callback({ id: element.val(), text: initText });
					}
				}
			}

			function buildOptions() {
				if (!lists[url]) {
					load();
				}

				// create options
				if (!loaded) {
					$.each(lists[url], function () {
						var o = $(document.createElement('option'))
                                    .attr('value', this.id)
                                    .text(this.text || this.name)
                                    .appendTo(sel);
						if (this.selected) {
							o.attr("selected", "selected");
						}
					})

					// mark select as 'filled'
					sel.data("loaded", true);
				}
			}

			function getPlaceholder() {
				return sel.attr("placeholder") ||
                       sel.data("placeholder") ||
                       sel.data("select-placeholder");
			}

		});

	}

})(jQuery, window, document);
