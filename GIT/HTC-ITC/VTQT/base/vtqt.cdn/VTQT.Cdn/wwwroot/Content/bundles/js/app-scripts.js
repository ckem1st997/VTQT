/* application.system.js
-------------------------------------------------------------- */
;

(function ($) {

    var formatRe = /\{(\d+)\}/g;

    String.prototype.format = function () {
        var s = this, args = arguments;
        return s.replace(formatRe, function (m, i) {
            return args[i];
        });
    };

    // define noop funcs for window.console in order
    // to prevent scripting errors
    var c = window.console = window.console || {};
    function noop() { };
    var funcs = ['log', 'debug', 'info', 'warn', 'error', 'assert', 'dir', 'dirxml', 'group', 'groupEnd',
					'time', 'timeEnd', 'count', 'trace', 'profile', 'profileEnd'],
		flen = funcs.length,
		noop = function () { };
    while (flen) {
        if (!c[funcs[--flen]]) {
            c[funcs[flen]] = noop;
        }
    }

    // define default secure-casts
    jQuery.extend(window, {

        toBool: function (val) {
            var defVal = typeof arguments[1] === "boolean" ? arguments[1] : false;
            var t = typeof val;
            if (t === "boolean") {
                return val;
            }
            else if (t === "string") {
                switch (val.toLowerCase()) {
                    case "1": case "true": case "yes": case "on": case "checked":
                        return true;
                    case "0": case "false": case "no": case "off":
                        return false;
                    default:
                        return defVal;
                }
            }
            else if (t === "number") {
                return Boolean(val);
            }
            else if (t === "null" || t === "undefined") {
                return defVal;
            }
            return defVal;
        },

        toStr: function (val) {
            var defVal = typeof arguments[1] === "string" ? arguments[1] : "";
            if (!val || val === "[NULL]") {
                return defVal;
            }
            return String(val) || defVal;
        },

        toInt: function (val) {
            var defVal = typeof arguments[1] === "number" ? arguments[1] : 0;
            var x = parseInt(val);
            if (isNaN(x)) {
                return defVal;
            }
            return x;
        },

        toFloat: function (val) {
            var defVal = typeof arguments[1] === "number" ? arguments[1] : 0;
            var x = parseFloat(val);
            if (isNaN(x)) {
                return defVal;
            }
            return x;
        }


    });


})(jQuery);


/* xbase.jquery.utils.js
-------------------------------------------------------------- */
;
(function ($) {

    $.extend({

        topZIndex: function (selector) {
            /*
            /// summary
            /// 	Returns the highest (top-most) zIndex in the document
            /// 	(minimum value returned: 0).
            /// param "selector"
            /// 	(optional, default = "body *") jQuery selector specifying
            /// 	the elements to use for calculating the highest zIndex.
            /// returns
            /// 	The minimum number returned is 0 (zero).
            */
            return Math.max(0, Math.max.apply(null, $.map($(selector || "body *"),
				function (v) {
				    return parseInt($(v).css("z-index")) || null;
				}
			)));
        }

    }); // $.extend

    $.fn.extend({

        topZIndex: function (opt) {
            /*
            /// summary:
            /// 	Increments the CSS z-index of each element in the matched set
            /// 	to a value larger than the highest current zIndex in the document.
            /// 	(i.e., brings all elements in the matched set to the top of the
            /// 	z-index order.)
            /// param "opt"
            /// 	(optional) Options, with the following possible values:
            /// 	increment: (Number, default = 1) increment value added to the
            /// 		highest z-index number to bring an element to the top.
            /// 	selector: (String, default = "body *") jQuery selector specifying
            /// 		the elements to use for calculating the highest zIndex.
            /// returns type="jQuery"
            */

            // Do nothing if matched set is empty
            if (this.length === 0) {
                return this;
            }

            opt = $.extend({ increment: 1, selector: "body *" }, opt);

            // Get the highest current z-index value
            var zmax = $.topZIndex(opt.selector), inc = opt.increment;

            // Increment the z-index of each element in the matched set to the next highest number
            return this.each(function () {
                $(this).css("z-index", zmax += inc);
            });
        },

        cushioning: function (withMargins) {
            var el = $(this[0]);
            // returns the differences between outer and inner
            // width, as well as outer and inner height
            withMargins = _.isBoolean(withMargins) ? withMargins : true;
            return {
                horizontal: el.outerWidth(withMargins) - el.width(),
                vertical: el.outerHeight(withMargins) - el.height()
            }
        },

        horizontalCushioning: function (withMargins) {
            var el = $(this[0]);
            // returns the difference between outer and inner width
            return el.outerWidth(_.isBoolean(withMargins) ? withMargins : true) - el.width();
        },

        verticalCushioning: function (withMargins) {
            var el = $(this[0]);
            // returns the difference between outer and inner height
            return el.outerHeight(_.isBoolean(withMargins) ? withMargins : true) - el.height();
        },

        outerHtml: function () {
            // returns the (outer)html of a new DOM element that contains
            // a clone of the first match
            return $(document.createElement("div"))
				.append($(this[0]).clone())
				.html();
        },

        evenIfHidden: function (callback) {
            return this.each(function () {
                var self = $(this);
                var styleBackups = [];

                var hiddenElements = self.parents().addBack().filter(':hidden');

                if (!hiddenElements.length) {
                    callback(self);
                    return true; //continue the loop
                }

                hiddenElements.each(function () {
                    var style = $(this).attr('style');
                    style = typeof style == 'undefined' ? '' : style;
                    styleBackups.push(style);
                    $(this).attr('style', style + ' display: block !important;');
                });

                hiddenElements.eq(0).css('left', -10000);

                callback(self);

                hiddenElements.each(function () {
                    $(this).attr('style', styleBackups.shift());
                });
            });
        },

        /*
            Binds a simple JSON object (no collection) to a set of html elements
            defining the 'data-bind-to' attribute
        */
        bindData: function (data, options) {
            var defaults = {
                childrenOnly: false,
                includeSelf: false,
                showFalsy: false,
                fade: false
            };
            var opts = $.extend(defaults, options);

            return this.each(function () {
                var el = $(this);

                var elems = el.find(opts.childrenOnly ? '>[data-bind-to]' : '[data-bind-to]');
                if (opts.includeSelf)
                    elems = elems.addBack();

                elems.each(function () {
                    var elem = $(this);
                    var val = data[elem.data("bind-to")];
                    if (val !== undefined) {

                        if (opts.fade) {
                            elem.fadeOut(400, function () {
                                elem.html(val);
                                elem.fadeIn(400);
                            });
                        }
                        else {
                            elem.html(val);
                        }

                        if (!opts.showFalsy && !val) {
                            // it's falsy, so hide it
                            elem.hide();
                        }
                        else {
                            elem.show();
                        }
                    }
                });
            });
        },

        /**
		 * Copyright 2012, Digital Fusion
		 * Licensed under the MIT license.
		 * http://teamdf.com/jquery-plugins/license/
		 *
		 * @author Sam Sehnert
		 * @desc A small plugin that checks whether elements are within
		 *       the user visible viewport of a web browser.
		 *       only accounts for vertical position, not horizontal.
		*/
        visible: function (partial, hidden, direction) {
            if (this.length < 1)
                return;

            var $w = $(window);
            var $t = this.length > 1 ? this.eq(0) : this,
				t = $t.get(0),
				vpWidth = $w.width(),
				vpHeight = $w.height(),
				direction = (direction) ? direction : 'both',
				clientSize = hidden === true ? t.offsetWidth * t.offsetHeight : true;

            if (typeof t.getBoundingClientRect === 'function') {

                // Use this native browser method, if available.
                var rec = t.getBoundingClientRect(),
					tViz = rec.top >= 0 && rec.top < vpHeight,
					bViz = rec.bottom > 0 && rec.bottom <= vpHeight,
					lViz = rec.left >= 0 && rec.left < vpWidth,
					rViz = rec.right > 0 && rec.right <= vpWidth,
					vVisible = partial ? tViz || bViz : tViz && bViz,
					hVisible = partial ? lViz || rViz : lViz && rViz;

                if (direction === 'both')
                    return clientSize && vVisible && hVisible;
                else if (direction === 'vertical')
                    return clientSize && vVisible;
                else if (direction === 'horizontal')
                    return clientSize && hVisible;
            } else {

                var viewTop = $w.scrollTop(),
					viewBottom = viewTop + vpHeight,
					viewLeft = $w.scrollLeft(),
					viewRight = viewLeft + vpWidth,
					offset = $t.offset(),
					_top = offset.top,
					_bottom = _top + $t.height(),
					_left = offset.left,
					_right = _left + $t.width(),
					compareTop = partial === true ? _bottom : _top,
					compareBottom = partial === true ? _top : _bottom,
					compareLeft = partial === true ? _right : _left,
					compareRight = partial === true ? _left : _right;

                if (direction === 'both')
                    return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop)) && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
                else if (direction === 'vertical')
                    return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop));
                else if (direction === 'horizontal')
                    return !!clientSize && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
            }
        },

        moreLess: function () {
            var moreText = '<button class="btn btn-mini"><i class="fa fa-plus mini-button-icon"></i>' + Res['Common.ShowMore'] + '</button>';
            var lessText = '<button class="btn btn-mini"><i class="fa fa-minus mini-button-icon"></i>' + Res['Common.ShowLess'] + '</button>';

            return this.each(function () {
                var el = $(this),
          			opt = $.extend({ adjustheight: 260 }, el.data('options'));

                if (el.height() > opt.adjustheight) {
                    el.find(".more-block").css('height', opt.adjustheight).css('overflow', 'hidden');

                    el.append('<p class="continued">[&hellip;]</p><a href="#" class="adjust"></a>');

                    el.find(".adjust").html(moreText).toggle(function () {
                        $(this).parents("div:first").find(".more-block").css('height', 'auto').css('overflow', 'visible');
                        $(this).parents("div:first").find("p.continued").css('display', 'none');
                        $(this).html(lessText);
                    }, function () {
                        $(this).parents("div:first").find(".more-block").css('height', opt.adjustheight).css('overflow', 'hidden');
                        $(this).parents("div:first").find("p.continued").css('display', 'block');
                        $(this).html(moreText);
                    });
                }
            });
        }



    }); // $.fn.extend


})(jQuery);
(function ($, window, document, undefined) {

    window.setLocation = function (url) {
        window.location.href = url;
    }

    window.OpenWindow = function (query, w, h, scroll) {
        var l = (screen.width - w) / 2;
        var t = (screen.height - h) / 2;

        // TODO: (MC) temp only. Global viewport is larger now.
        // But add this value to the callers later.
        h += 100;

        winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
        if (scroll) winprops += ',scrollbars=1';
        var f = window.open(query, "_blank", winprops);
    }

    window.htmlEncode = function (value) {
        return $('<div/>').text(value).html();
    }

    window.htmlDecode = function (value) {
        return $('<div/>').html(value).text();
    }

    window.displayNotification = function (message, type, sticky, delay) {
        if (window.PubSub === undefined || window._ === undefined)
            return;

        var notify = function (msg) {

            if (!msg)
                return;

            PubSub.publish("message", {
                text: msg,
                type: type,
                delay: delay || (type == "success" ? 2000 : 5000),
                hide: !sticky
            });
        };

        if (_.isArray(message)) {
            $.each(message, function (i, val) {
                notify(val);
            });
        }
        else {
            notify(message);
        }
    }

    window.createCircularSpinner = function (size, active, strokeWidth, boxed, white) {
        var spinner = $('<div class="spinner"></div>');
        if (active) spinner.addClass('active');
        if (boxed) spinner.addClass('spinner-boxed').css('font-size', size + 'px');
        if (white) spinner.addClass('white');

        if (!_.isNumber(strokeWidth)) {
            strokeWidth = 6;
        }

        var svg = '<svg style="width:{0}px; height:{0}px" viewBox="0 0 64 64"><circle cx="32" cy="32" r="{1}" fill="none" stroke-width="{2}" stroke-miterlimit="10"></circle></svg>'.format(size, 32 - strokeWidth, strokeWidth);
        spinner.append($(svg));

        return spinner;
    }

    window.loadHtml = function ($container, url, extraData, callback) {
        $.ajax({
            type: 'GET',
            dataType: 'html',
            data: extraData,
            url: url,
            beforeSend: function (jqXHR, settings) {
                kendo.ui.progress($container, true);
            },
            success: function (data, textStatus, jqXHR) {
                $container.html(data);
                if (typeof callback == 'function')
                    callback(data);

                kendo.ui.progress($container, false);
                return;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                kendo.ui.progress($container, false);
            }
        });
    }
    $.fn.loadHtml = function (url, extraData, callback) {
        window.loadHtml(this, url, extraData, callback);
    }

    // on document ready
    $(function () {

        function getFunction(code, argNames) {
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

        function decode(str) {
            try {
                if (str)
                    return decodeURIComponent(escape(str));
            }
            catch (e) {
                return str;
            }
            return str;
        }

        if (!Modernizr.csstransitions) {
            $.fn.transition = $.fn.animate;
        }

        // adjust pnotify global defaults
        if (PNotify) {
            $.extend(PNotify.prototype.options, {
                history: false,
                animate_speed: "normal",
                shadow: true,
                width: "400px",
                icon: true
            });
        }

        // global notification subscriber
        if (PubSub && window._ && PNotify) {
            PubSub.subscribe("message", function (message, data) {
                var opts = _.isString(data) ? { text: data } : data;
                var notice = new PNotify(opts);
            });
        }

        // Handle ajax notifications
        $(document)
            .ajaxSuccess(function (ev, xhr) {
                if (!app.ajax.handleUnauthorizedRequest(xhr))
                    return;

                var enMsg = xhr.getResponseHeader('X-XBase-Message');
                if (!enMsg)
                    return;

                var msg = atob(enMsg);
                if (msg) {
                    displayNotification(decode(msg), xhr.getResponseHeader('X-XBase-Message-Type'));
                }
            });
            //.ajaxError(function (ev, xhr) {
            //    var enMsg = xhr.getResponseHeader('X-XBase-Message');
            //    if (!enMsg)
            //        return;

            //    var msg = atob(enMsg);
            //    if (msg) {
            //        displayNotification(decode(msg), xhr.getResponseHeader('X-XBase-Message-Type'));
            //    }
            //    else {
            //        try {
            //            var data = JSON.parse(xhr.responseText);
            //            if (data.error && data.message) {
            //                displayNotification(decode(data.message), "error");
            //            }
            //        }
            //        catch (ex) {
            //            displayNotification(xhr.responseText, "error");
            //        }
            //    }
            //});

        // html text collapser
        if ($({}).moreLess) {
            $('.more-less').moreLess();
        }

        // fixes bootstrap 2 bug: non functional links on mobile devices
        // https://github.com/twbs/bootstrap/issues/4550
        $('body').on('touchstart.dropdown', '.dropdown-menu a', function (e) { e.stopPropagation(); });
    });

})(jQuery, this, document);

//#region Utilities

//#region Data

$.fn.loadOptions = function (data, valueField = 'Value', textField = 'Text', displayTemplate) {
    var $this = $(this);
    if ($this.is('select')) {
        $.each(data, function (i, x) {
            var txt;
            if (displayTemplate) {
                txt = displayTemplate(x);
            }
            var $opt = $('<option></option>', {
                value: x[valueField],
                text: txt || x[textField]
            });
            $.each(x, function (k, v) {
                $opt.data(k, v);
            });
            $this.append($opt);
        });
    }
    return $this;
};

//#endregion

//#region Convert

if (!String.prototype.toBool) {
    String.prototype.toBool = function () {
        var val = this.valueOf();
        return val === 'true' || val === 'True' || val === '1' || val === 1;
    };
}

//#endregion

//#region Format

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}
if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

//#endregion

//#region GUID

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}

//#endregion

//#region JSON

function isJSON(data) {
    var isJson = false;
    try {
        // this works with JSON string and JSON object, not sure about others
        var json = JSON.parse(data);
        isJson = typeof json === 'object';
    } catch (ex) {
        //console.error('Data is not JSON.');
    }
    return isJson;
}

//#endregion

//#region Layout

$.fn.isOverflow = function () {
    var element = $(this)[0];
    return (element.offsetHeight < element.scrollHeight) || (element.offsetWidth < element.scrollWidth);
};

$.fn.enable = function () {
    var $this = this.find('a,button,input,select,textarea').prop('disabled', false);
    app.plugins.uniform.update();
    return $this;
};
$.fn.disable = function () {
    var $this = this.find('a,button,input,select,textarea').prop('disabled', true);
    app.plugins.uniform.update();
    return $this;
};

//#endregion

//#region Serialize

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

//#endregion

//#region Timer

var delay = (function () {
    var timers = {};
    return function (label, callback, ms) {
        label = label || 'defaultTimer';
        clearTimeout(timers[label] || 0);
        timers[label] = setTimeout(callback, ms);
    };
})();

//#endregion

//#endregion
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

/*
*  Project: SmartStore Throbber
*  Author: Murat Cakir, SmartStore AG
*/

; (function ($, window, document, undefined) {

    var pluginName = 'throbber';

    // element: the DOM element
    // options: the instance specific options
    function Throbber(element, options) {
        var self = this;

        this.element = element;
        var el = this.el = $(element),
            opts = this.options = options,
            throbber = this.throbber = null,
            throbberContent = this.throbberContent = null;

        this.visible = false;

        this.init = function () {
            throbbers.push(self);
            if (opts.show) {
                self.show();
            }
        };

        this.initialized = false;
        this.init();

    }

    Throbber.prototype = {

        show: function (o) {
            if (this.visible)
                return;

            var self = this,
                opts = $.extend({}, this.options, o);

            // create throbber if not avail
            //if (!self.throbber) {
            // Custom: For show throbber with new option
            if (opts._global) {
                $('body').find('> .throbber').remove();
            } else {
                self.el.find('> .throbber').remove();
            }
            self.throbber = $('<div class="throbber"><div class="throbber-flex"><div><div class="throbber-content"></div></div></div></div>')
                             .addClass(opts.cssClass)
                             .addClass(opts.small ? "small" : "large")
                             .appendTo(opts._global ? 'body' : self.el);
            if (opts.white) {
                self.throbber.addClass("white");
            }
            if (opts._global) {
                self.throbber.addClass("global");
            }
            else {
                if (/static/.test(self.el.css("position"))) {
                    self.el.css("position", "relative");
                }
            }

            self.throbberContent = self.throbber.find(".throbber-content");
            //var spinner = window.createCircularSpinner(opts.small ? 50 : 100, true, 3);
            var spinner = window.createCircularSpinner(opts.size > 0 ? opts.size : (opts.small ? 50 : 100), true, 3); // Custom: size option
            spinner.insertAfter(self.throbberContent);

            self.initialized = true;
            //}

            // set text
            self.throbber.css({ visibility: 'hidden', display: 'block' });
            self.throbberContent.html(opts.message);
            self.throbberContent.toggleClass('hide', !(_.isString(opts.message) && opts.message.trim().length > 0));
            self.throbber.css({ visibility: 'visible', opacity: 0 });
            // Custom-XBase-DucNV: Fix loader's position in scrollable container
            if (self.el.isOverflow() && self.el.scrollTop() > 0) {
                var top = self.el.scrollTop();
                self.throbber.css('top', top);
            }

            var show = function () {
                if (_.isFunction(opts.callback)) {
                    opts.callback.apply(this);
                }
                if (!self.visible) {
                    // could have been set to false in 'hide'.
                    // this can happen in very short running processes.
                    self.hide();
                }
            }

            self.visible = true;
            //self.throbber.delay(opts.delay).transition({ opacity: 1 }, opts.speed || 0, "linear", show);
            self.throbber.delay(opts.delay).transition({ opacity: 1 }, opts.speed || 0, opts.timingFunction ? opts.timingFunction : "linear", show); // Custom: timing function for showing in many cases, such as: Kendo Grid Loading

            if (opts.timeout) {
                var hideFn = _.bind(self.hide, this);
                window.setTimeout(hideFn, opts.timeout + opts.delay);
            }

        },

        hide: function (immediately) {
            var self = this, opts = this.options;
            if (self.throbber && self.visible) {
                var hide = function () {
                    self.throbber.css('display', 'none');
                }
                self.visible = false;

                !defaults.speed || immediately == true
            		? self.throbber.stop(true).hide(0, hide)
                    : self.throbber.stop(true).transition({ opacity: 0 }, opts.speed || 0, "linear", hide);
            }

        }

    }

    // A really lightweight plugin wrapper around the constructor, 
    // preventing against multiple instantiations
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, pluginName)) {
                options = $.extend({}, $[pluginName].defaults, options);
                $.data(this, pluginName, new Throbber(this, options));
            }
        });

    }

    // global (window)-throbber
    var globalThrobber = null,
        throbbers = [],
        defaults = {
            delay: 0,
            speed: 150,
            timeout: 0,
            white: false,
            small: false,
            message: "Please wait...",
            cssClass: null,
            callback: null,
            show: true,
            // internal
            _global: false
        };

    $[pluginName] = {

        // the global, default plugin options
        defaults: defaults,

        // options: a message string || options object
        show: function (options) {
            var opts = $.extend(defaults, _.isString(options) ? { message: options } : options, { show: false, _global: true });

            if (!globalThrobber) {
                globalThrobber = $(window).throbber(opts).data("throbber");
            }

            globalThrobber.show(opts);
        },

        hide: function (immediately) {
            if (globalThrobber) {
                globalThrobber.hide(immediately);
            }
        }

    } // $.throbber

})(jQuery, window, document);


app.ajax.init = function () {
    app.ajax.setup();
}
app.ajax.setup = function () {
    $.ajaxSetup({
        cache: false,
        statusCode: app.ajax.statusCodeHandler
    });
}
app.ajax.handleUnauthorizedRequest = function (jqXhr) {
    var xRespondedJson = jqXhr.getResponseHeader('X-Responded-JSON');
    if (xRespondedJson !== null) {
        var obj = JSON.parse(xRespondedJson);
        if (!obj)
            return true;

        if (obj.status === 401) {
            window.location.href = app.urls.sessionExpired + '?ReturnUrl=' + encodeURIComponent(window.location.pathname + window.location.search);
            return false;
        }
        if (obj.status === 403) {
            app.ajax.statusCodeHandler[403]();
            return false;
        }
    }

    return true;
}

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

/*
*  Project: SmartStore ajax wrapper
*  Author: Marcus Gesing, SmartStore AG
*/

; (function ($, window, document, undefined) {

    $.fn.doAjax = function (options) {
        normalizeOptions(this, options);

        if (_.isEmpty(options.url)) {
            console.log('doAjax can\'t find the url!');
        }
        else if (!_.isFalse(options.valid)) {
            doRequestSwitch(options);
        }

        return this.each(function () { });
    };

    $.fn.doAjax.defaults = {
        /* [...] */
    };

    $.fn.doPostData = function (options) {
        function createAndSubmitForm() {
            var id = 'DynamicForm_' + Math.random().toString().substring(2),
				form = '<form id="' + id + '" action="' + options.url + '" method="' + options.type + '">';

            if (!_.isUndefined(options.data)) {
                $.each(options.data, function (key, val) {
                    form += '<input type="hidden" name="' + key + '" value="' + $('<div/>').text(val).html() + '" />';
                });
            }

            form += '</form>';

            $('body').append(form);
            $('#' + id).submit();
        }

        normalizeOptions(this, options);

        if (_.isEmpty(options.url)) {
            console.log('doPostData can\'t find the url!');
        }
        else if (_.isEmpty(options.ask)) {
            createAndSubmitForm();
        }
        else if (confirm(options.ask)) {
            createAndSubmitForm();
        }

        return this.each(function () { });
    }


    function normalizeOptions(element, opt) {
        opt.ask = (_.isUndefined(opt.ask) ? $(element).attr('data-ask') : opt.ask);

        if (_.isUndefined(opt.type)) {
            opt.type = (_.isUndefined(opt.autoContainer) ? 'POST' : 'GET');
        }

        if (!_.isUndefined(opt.autoContainer)) {
            opt.smallIcon = opt.autoContainer;
        }

        if ($(element).is('form')) {
            if (_.isUndefined(opt.data))
                opt.data = $(element).serialize();
            if (_.isUndefined(opt.url))
                opt.url = $(element).attr('action');
        }

        opt.url = (_.isUndefined(opt.url) ? findUrl(element) : opt.url);
    }

    function findUrl(element) {
        var url;
        if (_.isObject(element)) {
            url = $(element).attr('href');
            if (typeof url === 'string' && url.substr(0, 11) === 'javascript:')
                url = '';

            if (_.isUndefined(url) || url.length <= 0)
                url = $(element).attr('data-url');

            if (_.isUndefined(url) || url.length <= 0)
                url = $(element).attr('data-button');
        }
        return url;
    }

    function showAnimation(opt) {
        if (opt.curtainTitle) {
            $.throbber.show(opt.curtainTitle);
        }
        else if (opt.throbber) {
            $(opt.throbber).removeData('throbber').throbber({ white: true, small: true, message: '' });
        }
        else if (opt.smallIcon) {
            $(opt.smallIcon).append(window.createCircularSpinner(16, true));
        }
    }

    function hideAnimation(opt) {
        if (opt.curtainTitle)
            $.throbber.hide(true);
        if (opt.throbber)
            $(opt.throbber).data('throbber').hide(true);
        if (opt.smallIcon) {
            $(opt.smallIcon).find('.spinner').remove();
        }
    }

    function doRequest(opt) {
        $.ajax({
            cache: false,
            type: opt.type,
            data: opt.data,
            url: opt.url + (_.isEmpty(opt.appendToUrl) ? '' : opt.appendToUrl),
            async: opt.async,
            beforeSend: function () {
                if (!_.isUndefined(opt.autoContainer))
                    $(opt.autoContainer).empty();

                _.call(opt.callbackBeforeSend);
            },
            success: function (response) {
                if (!_.isUndefined(opt.autoContainer))
                    $(opt.autoContainer).html(response);

                _.call(opt.callbackSuccess, response);
            },
            error: function (objXml) {
                try {
                    if (objXml != null && objXml.responseText != null && objXml.responseText !== '') {
                        if (_.isTrue(opt.consoleError))
                            console.error(objXml.responseText);
                        else
                            PubSub.publish("message", { title: objXml.responseText, type: "error" });
                    }
                }
                catch (e) { }
            },
            complete: function () {
                hideAnimation(opt);
                _.call(opt.callbackComplete);
            }
        });

        showAnimation(opt);
    }

    function doRequestSwitch(opt) {
        if (_.isEmpty(opt.ask)) {
            doRequest(opt);
        }
        else if (confirm(opt.ask)) {
            doRequest(opt);
        }
    }

})(jQuery, window, document);
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
//#region @typedef

//#region Extends

/**
 * Options for get grid selected item models.
 * @typedef {Object} app.grid.selectedItemModels.options
 * @property {String=} idField - [Id] field name of model.
 * @property {String=} nameField - [Name] field name of model.
 */

//#endregion

//#endregion

app.grid.editingColumnField = function (e) {
    var idx = app.grid.editingColumnIndex(e);
    return e.sender.columns[idx].field;
};
app.grid.editingColumnIndex = function (e) {
    var $table = $(e.container).closest('table');
    var $div = $table.parent('div[class*="k-grid-content"]');

    var isLockedTable = $div.hasClass('k-grid-content-locked');
    var nLocked = 0;
    if (e.sender.lockedTable)
        nLocked = e.sender.lockedTable.find('tr:eq(0)').find('td').length;
    var idx = e.container.index();
    if (!isLockedTable)
        idx += nLocked;

    return idx;
};

//#region Event Handlers

app.grid.handlers.columnMenuInit = function (e) {
    // Initialize Filter Menu
    var fields = e.sender.dataSource.options.schema.model.fields;
    var $filterGroup = e.container.find(".k-filter-item .k-menu-group");
    var firstValueDropDown, logicDropDown, secondValueDropDown;

    if (fields[e.field].type === "string") {
        firstValueDropDown = $filterGroup.find("select:eq(0)").data("kendoDropDownList");
        if (firstValueDropDown) {
            firstValueDropDown.value("contains");
            firstValueDropDown.trigger('change');
        }
        logicDropDown = $filterGroup.find("select:eq(1)").data("kendoDropDownList");
        if (logicDropDown) {
            logicDropDown.value("or");
            logicDropDown.trigger('change');
        }
        secondValueDropDown = $filterGroup.find("select:eq(2)").data("kendoDropDownList");
        if (secondValueDropDown) {
            secondValueDropDown.value("contains");
            secondValueDropDown.trigger('change');
        }
    }
    else if (fields[e.field].type === "date") {
        firstValueDropDown = $filterGroup.find("select:eq(0)").data("kendoDropDownList");
        if (firstValueDropDown) {
            firstValueDropDown.value("gte");
            firstValueDropDown.trigger('change');
        }
        logicDropDown = $filterGroup.find("select:eq(1)").data("kendoDropDownList");
        if (logicDropDown) {
            logicDropDown.value("and");
            logicDropDown.trigger('change');
        }
        secondValueDropDown = $filterGroup.find("select:eq(2)").data("kendoDropDownList");
        if (secondValueDropDown) {
            secondValueDropDown.value("lte");
            secondValueDropDown.trigger('change');
        }
    }
    else if (fields[e.field].type === "number") {
        firstValueDropDown = $filterGroup.find("select:eq(0)").data("kendoDropDownList");
        if (firstValueDropDown) {
            firstValueDropDown.value("gte");
            firstValueDropDown.trigger('change');
        }
        logicDropDown = $filterGroup.find("select:eq(1)").data("kendoDropDownList");
        if (logicDropDown) {
            logicDropDown.value("and");
            logicDropDown.trigger('change');
        }
        secondValueDropDown = $filterGroup.find("select:eq(2)").data("kendoDropDownList");
        if (secondValueDropDown) {
            secondValueDropDown.value("lte");
            secondValueDropDown.trigger('change');
        }
    }
};
app.grid.handlers.resize = function () {
    var $grids = $('.k-widget.k-grid');
    if ($grids.length) {
        $.each($grids, function (i, x) {
            var $grid = $(x);
            var grid = $grid.data("kendoGrid");
            if (grid) {
                grid.resize();
            }
        });
    }
};

//#endregion

//#region Filters

//app.grid.filters.templates.bool = function (args, trueText, falseText, labelText) {
//    args.element.kendoDropDownList({
//        autoBind: false,
//        dataTextField: "text",
//        dataValueField: "value",
//        dataSource: new kendo.data.DataSource({
//            data: [
//                { text: trueText || "Tạm ngưng", value: "true" },
//                { text: falseText || "Áp dụng", value: "false" }
//            ]
//        }),
//        index: 0,
//        optionLabel: {
//            text: labelText || "Tất cả",
//            value: ""
//        },
//        valuePrimitive: true
//    });
//};

//#endregion

//#region Extends

kendo.ui.Grid.prototype.getById = function (id) {
    return this.dataSource.get(id);
};
kendo.ui.Grid.prototype.getByUid = function (uid) {
    return this.dataSource.getByUid(uid);
};
kendo.ui.Grid.prototype.selectedIds = function () {
    var ids = [], select = this.selectedRows();
    for (var i = 0; i < select.length; i++) {
        ids.push(this.dataItem(select[i]).Id);
    }
    return ids;
};
kendo.ui.Grid.prototype.selectedItems = function () {
    var items = [], select = this.selectedRows();
    for (var i = 0; i < select.length; i++) {
        items.push(this.dataItem(select[i]));
    }
    return items;
};
kendo.ui.Grid.prototype.selectedCount = function () {
    return this.selectedRows().length;
};
/**
 * Get grid selected item models.
 * @param {app.grid.selectedItemModels.options=} options - [Id] & [Name] field name (optional).
 * @returns {Object[]} - Grid selected item models. 
 */
kendo.ui.Grid.prototype.selectedItemModels = function (options) {
    var idField = 'Id', nameField = 'Name';

    if (options) {
        if (options.idField)
            idField = options.idField;
        if (options.nameField)
            nameField = options.nameField;
    }

    var items = this.selectedItems();
    var models = [];
    $.each(items, function (i, x) {
        models.push({ id: x[idField], name: x[nameField] });
    });

    return models;
};
kendo.ui.Grid.prototype.selectedRows = function () {
    var select;
    if (!this.lockedTable) {
        select = this.select();
        if (!select.length)
            select = this.tbody.find('tr[aria-selected="true"]').has('td[id*="active_cell"]');
    } else {
        select = this.tbody.find('tr[class*="k-state-selected"]');
        if (!select.length)
            select = this.lockedTable.find('tr[class*="k-state-selected"]');
        if (!select.length)
            select = this.tbody.find('tr[aria-selected="true"]').has('td[id*="active_cell"]');
        if (!select.length)
            select = this.lockedTable.find('tr[aria-selected="true"]').has('td[id*="active_cell"]');
    }
    return select;
};
kendo.ui.Grid.prototype.selectRow = function ($element) {
    var grid = this;
    var $tr = $element.is('tr') ? $element : $element.closest('tr');

    grid.clearSelection();
    grid.select($tr);

    return $tr;
};
kendo.ui.Grid.prototype.selectId = function (id) {
    if (!id) return;

    var grid = this;
    var item = grid.dataSource.get(id);

    if (item) {
        var uid = item.uid;
        var $rows = grid.tbody.find(`tr[data-uid="${uid}"]`);
        if ($rows.length) {
            var $r = $($rows[0]);
            grid.select($r);
        }
    }
};
kendo.ui.Grid.prototype.selectIds = function (ids) {
    if (!id || !id.length) return;

    var grid = this;
    // Không dùng filter Id vì field Id có thể là Name khác,
    // code thủ công qua dataSource.get kết hợp với config Grid Model .Id(...) để generic Id field name qua config
    //var ds = grid.DataSource.data();
    //var uids = ds.filter(function (w) { return ids.includes(w.Id); }).map(function (x) { return x.uid; });
    var items = [];
    ids.forEach(function (id) {
        var item = grid.dataSource.get(id);
        if (item)
            items.push(item);
    });

    if (items.length) {
        var lstUid = items.map(function (x) { return x.uid; });
        var rows = [];
        lstUid.forEach(function (uid) {
            var $rows = grid.tbody.find(`tr[data-uid="${uid}"]`);
            if ($rows.length) {
                var $r = $($rows[0]);
                rows.push($r);
            }
        });

        rows.forEach(function ($r) {
            grid.select($r);
        });
    }
};
kendo.ui.Grid.prototype.reload = function (data) {
    if (typeof data === 'undefined' || data == null)
        return this.dataSource.read();
    else
        return this.dataSource.read(data);
};
kendo.ui.Grid.prototype.initDblClick = function (callback) {
    this.tbody.dblclick(dblclick);

    // For Grid has locked columns on left side
    if (this.lockedTable) {
        this.lockedTable.dblclick(dblclick);
    }

    function dblclick(e) {
        var $elem = $(e.target);
        if ($elem.is('td')) {
            if (typeof callback == 'function')
                callback();
        }
    }
};
kendo.ui.Grid.prototype.initDetails = function (callback) {
    var grid = this;
    grid.tbody.on('click', 'tr td a[data-action="details"]', details);

    if (grid.lockedTable) {
        grid.lockedTable.on('click', 'tr td a[data-action="details"]', details);
    }

    function details(e) {
        var $this = $(this);
        var $tr = grid.selectRow($this);

        var item = grid.dataItem($tr);
        var id = item.Id;

        callback(id);
    }
};
kendo.ui.Grid.prototype.initEdit = function (callback) {
    var grid = this;
    grid.tbody.on('click', 'tr td a[data-action="edit"]', edit);

    if (grid.lockedTable) {
        grid.lockedTable.on('click', 'tr td a[data-action="edit"]', edit);
    }

    function edit(e) {
        var $this = $(this);
        var $tr = grid.selectRow($this);

        var item = grid.dataItem($tr);
        var id = item.Id;

        callback(id);
    }
}
kendo.ui.Grid.prototype.initActivate = function (callback) {
    var grid = this;
    grid.tbody.on('click', 'tr td i[data-action="activate"], tr td i[data-action="deactivate"]', activate);

    if (grid.lockedTable) {
        grid.lockedTable.on('click', 'tr td i[data-action="activate"], tr td i[data-action="deactivate"]', activate);
    }

    function activate(e) {
        var $this = $(this);
        var $tr = grid.selectRow($this);

        var item = grid.dataItem($tr);
        var id = item.Id;
        var ids = [id];
        var action = $this.attr('data-action');
        var active = action === 'activate' ? true : false;

        callback(active, ids);
    }
}
kendo.ui.Grid.prototype.initPublish = function (callback) {
    var grid = this;
    grid.tbody.on('click', 'tr td i[data-action="publish"], tr td i[data-action="unpublish"]', publish);

    if (grid.lockedTable) {
        grid.lockedTable.on('click', 'tr td i[data-action="publish"], tr td i[data-action="unpublish"]', publish);
    }

    function publish(e) {
        var $this = $(this);
        var $tr = grid.selectRow($this);

        var item = grid.dataItem($tr);
        var id = item.Id;
        var ids = [id];
        var action = $this.attr('data-action');
        var publish = action === 'publish' ? true : false;

        callback(publish, ids);
    }
}
kendo.ui.Grid.prototype.initSelect = function (selectionMode) {
    const grid = this;
    grid.bind('dataBound', function (e) {
        grid.initCheckBoxes(selectionMode);
    });
};
/**
 * 
 * @param {enums.grid.SelectionMode} selectionMode - Grid selection mode.
 */
kendo.ui.Grid.prototype.initCheckBoxes = function (selectionMode) {
    if (selectionMode === enums.grid.SelectionMode.Single || selectionMode === enums.grid.SelectionMode.Multiple) {
        var grid = this;
        var $checkAll, $checkboxes;

        if (!grid.lockedTable) {
            // Check all click
            $checkAll = grid.thead.find('input[type="checkbox"][class*="check-all"]');
            if ($checkAll) {
                $checkAll.prop('checked', false);
                $checkAll.click(function (e) {
                    var checked = this.checked;
                    if (checked) {
                        grid.items().addClass("k-state-selected");
                    } else {
                        grid.items().removeClass("k-state-selected");
                    }
                    var $checkboxes = grid.tbody.find('tr td input[type="checkbox"][class*="row-checkbox"]');
                    if ($checkboxes.length) {
                        $checkboxes.prop('checked', checked);
                    }
                });
            }

            // Row checkbox click
            grid.tbody.find('tr td input[type="checkbox"][class*="row-checkbox"]').click(function (e) {
                var checked = this.checked;
                var $row = $(this).closest('tr');
                if (checked)
                    $row.addClass("k-state-selected");
                else
                    $row.removeClass("k-state-selected");

                if ($checkAll) {
                    var total = grid.dataSource.total();
                    var checkCount = grid.tbody.find('tr td input[type="checkbox"][class*="row-checkbox"]:checked').length;
                    $checkAll.prop('checked', total === checkCount);
                }
            });

            // Set row selected by checkbox checked
            $checkboxes = grid.tbody.find('tr td input[type="checkbox"][class*="row-checkbox"]');
            $.each($checkboxes, function (i, x) {
                var $checkbox = $(x);
                var checked = $checkbox.prop('checked');
                var $row = $checkbox.closest('tr');
                if (checked)
                    $row.addClass("k-state-selected");
                else
                    $row.removeClass("k-state-selected");
            });

            // Select event
            if (selectionMode === enums.grid.SelectionMode.Single) {
                // Row click
                grid.tbody.find('tr').click(function (e) {
                    var $this = $(e.target);
                    if (!$this.is('input[type="checkbox"][class*="row-checkbox"]')) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });
            } else if (selectionMode === enums.grid.SelectionMode.Multiple) {
                grid.bind('change', function (e) {
                    // Set checkbox checked by row selected
                    var $rows = grid.items();
                    $.each($rows, function (i, x) {
                        var $row = $(x);
                        var selected = $row.hasClass('k-state-selected');
                        var $checkbox = $row.find('td input[type="checkbox"][class*="row-checkbox"]');
                        if ($checkbox.length) {
                            $checkbox.prop('checked', selected);
                        }
                    });

                    // Set Check all
                    if ($checkAll) {
                        var total = grid.dataSource.total();
                        var checkCount = grid.tbody.find('tr td input[type="checkbox"][class*="row-checkbox"]:checked').length;
                        $checkAll.prop('checked', total === checkCount);
                    }
                });
            }
        } else {
            // Check all click
            $checkAll = grid.lockedHeader.find('input[type="checkbox"][class*="check-all"]');
            if ($checkAll) {
                $checkAll.prop('checked', false);
                $checkAll.click(function (e) {
                    var checked = this.checked;
                    if (checked) {
                        grid.items().addClass("k-state-selected");
                    } else {
                        grid.items().removeClass("k-state-selected");
                    }
                    var $checkboxes = grid.lockedTable.find('tr td input[type="checkbox"][class*="row-checkbox"]');
                    if ($checkboxes.length) {
                        $checkboxes.prop('checked', checked);
                    }
                });
            }

            // Row checkbox click
            grid.lockedTable.find('tr td input[type="checkbox"][class*="row-checkbox"]').click(function (e) {
                var checked = this.checked;
                var $lockedRow = $(this).closest('tr');
                var uid = $lockedRow.attr('data-uid');
                var $row = grid.table.find('tr[data-uid="' + uid + '"]');
                if (checked) {
                    $row.addClass("k-state-selected");
                    $lockedRow.addClass("k-state-selected");
                } else {
                    $row.removeClass("k-state-selected");
                    $lockedRow.removeClass("k-state-selected");
                }

                if ($checkAll) {
                    var total = grid.dataSource.total();
                    var checkCount = grid.lockedTable.find('tr td input[type="checkbox"][class*="row-checkbox"]:checked').length;
                    $checkAll.prop('checked', total === checkCount);
                }
            });

            // Set row selected by checkbox checked
            $checkboxes = grid.lockedTable.find('tr td input[type="checkbox"][class*="row-checkbox"]');
            $.each($checkboxes, function (i, x) {
                var $checkbox = $(x);
                var checked = $checkbox.prop('checked');
                var $lockedRow = $checkbox.closest('tr');
                var uid = $lockedRow.attr('data-uid');
                var $row = grid.table.find('tr[data-uid="' + uid + '"]');
                if (checked) {
                    $row.addClass("k-state-selected");
                    $lockedRow.addClass("k-state-selected");
                } else {
                    $row.removeClass("k-state-selected");
                    $lockedRow.removeClass("k-state-selected");
                }
            });

            // Select event
            if (selectionMode === enums.grid.SelectionMode.Single) {
                // Locked Row click
                grid.lockedTable.find('tr').click(function (e) {
                    var $this = $(e.target);
                    if (!$this.is('input[type="checkbox"][class*="row-checkbox"]')) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });
                // Row click
                grid.tbody.find('tr').click(function (e) {
                    var $this = $(e.target);
                    if (!$this.is('input[type="checkbox"][class*="row-checkbox"]')) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });
            } else if (selectionMode === enums.grid.SelectionMode.Multiple) {
                grid.bind('change', function (e) {
                    // Set checkbox checked by row selected
                    var $rows = grid.lockedTable.find('tbody tr');
                    $.each($rows, function (i, x) {
                        var $row = $(x);
                        var selected = $row.hasClass('k-state-selected');
                        var $checkbox = $row.find('td input[type="checkbox"][class*="row-checkbox"]');
                        if ($checkbox.length) {
                            $checkbox.prop('checked', selected);
                        }
                    });

                    // Set Check all
                    if ($checkAll) {
                        var total = grid.dataSource.total();
                        var checkCount = grid.lockedTable.find('tr td input[type="checkbox"][class*="row-checkbox"]:checked').length;
                        $checkAll.prop('checked', total === checkCount);
                    }
                });
            }
        }
    }
};

kendo.ui.Grid.prototype.clear = function () {
    this.dataSource.data([]);
};
kendo.ui.Grid.prototype.resetPage = function (page) {
    if (typeof page === 'undefined' || page == null)
        this.dataSource.page(1);
    else
        this.dataSource.page(page);
};

/**
 * Grid Editing
 * Trigger validate khi gặp trường hợp cell không validate.
 */
kendo.ui.Grid.prototype.validateEditing = function () {
    var lockedRows = [];
    if (this.lockedTable)
        lockedRows = this.lockedTable.find("tbody tr:not(.k-no-data)");
    var rows = this.tbody.find("tr:not(.k-no-data)").add(lockedRows); //get rows
    for (var i = 0; i < rows.length; i++) {
        var rowModel = this.dataItem(rows[i]); //get row data
        if (rowModel && rowModel.isNew()) {
            var colCells = $(rows[i]).find("td"); //get cells
            for (var j = 0; j < colCells.length; j++) {
                if ($(colCells[j]).hasClass("k-group-cell"))
                    continue; //grouping enabled will add extra td columns that aren't actual columns
                this.editCell($(colCells[j])); //open for edit
                if (this.editable && !this.editable.end()) { //trigger validation
                    return false; //if fail, return false
                } else {
                    this.closeCell(); //if success, keep checking
                }
            }
        }
    }
    return true; //all cells are valid
};
/**
 * Grid Editing
 * Khởi tạo thao tác Tab bỏ qua các cell non-editable.
 */
kendo.ui.Grid.prototype.initEditingNavigation = function () {
    this.table.on('keydown', function (e) {
        if (e.keyCode === kendo.keys.TAB) {
            var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
            var current = grid.current();
            if (!current.hasClass("editable-cell")) {
                var nextCell;
                if (e.shiftKey) {
                    nextCell = current.prevAll(".editable-cell");
                    if (!nextCell[0]) {
                        //search the next row
                        var prevRow = current.parent().prev();
                        nextCell = prevRow.children(".editable-cell:last");
                    }
                } else {
                    nextCell = current.nextAll(".editable-cell");
                    if (!nextCell[0]) {
                        //search the next row
                        var nextRow = current.parent().next();
                        if (nextRow.length)
                            nextCell = nextRow.children(".editable-cell:first");
                        else {
                            if (grid.validateEditing()) {
                                grid.addRow();
                                var ds = grid.dataSource.data();
                                var r = grid.options.editable.createAt === 'top'
                                    ? ds[0]
                                    : ds[ds.length - 1];
                                var $r = grid.tbody.find('tr[data-uid=' + r.uid + ']');
                                var $c = $r.find('td.editable-cell:first');
                                grid.editCell($c);
                            }
                            return;
                        }
                    }
                }
                grid.current(nextCell);
                grid.editCell(nextCell[0]);
            }

        }
    });
};

//#endregion

/**
 * Customize for cell refocusing after refresh in grid editing mode.
 */
kendo.ui.Grid.fn.refresh = (function (refresh) {
    return function (e) {
        this._refreshing = true;

        refresh.call(this, e);

        this._refreshing = false;
    }
})(kendo.ui.Grid.fn.refresh);

kendo.ui.Grid.fn.current = (function (current) {
    return function (element) {
        // assuming element is td element, i.e. cell selection
        if (!this._refreshing && element) {
            this._lastFocusedCellIndex = $(element).index(); // note this might break with grouping cells etc, see grid.cellIndex() method
            this._lastFocusedUid = $(element).closest("tr").data("uid");
        }

        return current.call(this, element);
    }
})(kendo.ui.Grid.fn.current);

kendo.ui.Grid.fn.refocusLastEditedCell = function () {
    if (this._lastFocusedUid) {
        var row = $(this.tbody).find("tr[data-uid='" + this._lastFocusedUid + "']");
        var cell = $(row).children().eq(this._lastFocusedCellIndex);
        this.editCell(cell);
    }
};

//#endregion

//#region Formats

app.grid.formats.time = function (time, format) {
    if (time) {
        var d = kendo.parseDate(time);
        return kendo.toString(d, format);
    }

    return '';
};

//#endregion

//#region @typedef

/**
 * Options for notification.
 * @typedef {Object} notify.options
 * @property {String=} title - The notice's title.
 * @property {String} text - The notice's text.
 * @property {String} type - Type of the notice. "notice", "info", "success", or "error".
 * @property {Boolean=} hide - After a delay, remove the notice.
 * @property {Number=} delay - Delay in milliseconds before the notice is removed.
 */

//#endregion

app.notification.init = function () {
    var stackBottomCenter = { "dir1": "up", "dir2": "right", "firstpos1": 100, "firstpos2": 10 };
    PNotify.prototype.options.styling = "fontawesome";
    PNotify.prototype.options.addclass = "stack-bottomcenter";
    PNotify.prototype.options.stack = stackBottomCenter;
    PNotify.prototype.options.delay = 5000;
    $.extend(PNotify.styling.fontawesome, {
        pin_up: "fa fa-thumb-tack fa-rotate-90",
        pin_down: "fa fa-thumb-tack"
    });
    $.extend(PNotify.prototype.options.buttons, {
        labels: { close: "", stick: "" }
    });
}

/**
 * Display notification.
 * @param {notify.options} options - Options for notification.
 * @returns {Object} - Notification object.
 */
function notify(options) {
    var notice = new PNotify({
        title: options.title,
        text: options.text,
        type: options.type,
        hide: options.hide !== undefined && options.hide != null ? options.hide : !(options.type !== "success" && options.type !== "info"),
        delay: options.delay !== undefined && options.delay != null ? options.delay : (options.type === "success" ? 2000 : PNotify.prototype.options.delay)
    });

    return notice;
}

app.pjax.init = function () {
    $.pjax.defaults.timeout = app.pjax.defaults.timeout;
    $.pjax.defaults.container = app.pjax.defaults.container;
    $(document).pjax('a:not(.no-pjax)', app.pjax.defaults.container, { timeout: app.pjax.defaults.timeout }); // Pjax
    $(document).on('pjax:click', function (options) {
        app.qtip.destroyAll();
    });

    $(document).on('pjax:success', function (data, status, xhr, options) {
        $('.dropdown-toggle').dropdown();
        $('.dropdown-toggle[data-hover="dropdown"]').dropdownHover();
    });
    $(function () {
        $(app.pjax.defaults.container)
            .bind('pjax:start', function () {
                $('#ajax-busy').addClass('busy');
            })
            .bind('pjax:end', function () {
                window.setTimeout(function () {
                    $('#ajax-busy').removeClass('busy');
                }, 350);
            });
    });
    /* Pjax: customize for prevent Popstate cache for Back/Forward browser button works correctly */
    $(document).on('pjax:popstate', function (e) {
        e.preventDefault();

        app.window.alert.close();
        app.window.confirm.close();
        app.window.deletes.close();
        app.window.form.destroyAll();
    });
}

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

app.qtip.hideAll = function () {
    var $qtips = $('div.qtip');
    if ($qtips.length) {
        $.each($qtips, function (i, x) {
            var $x = $(x);
            $x.qtip('hide');
        });
    }
}
app.qtip.destroyAll = function () {
    var $qtips = $('div.qtip');
    if ($qtips.length) {
        $.each($qtips, function (i, x) {
            var $x = $(x);
            $x.qtip('destroy', true);
        });
    }
}
app.ui.loader = function ($container, toggle, options) {
    var defaultOptions = {
        delay: 0,
        speed: 150,
        timeout: 0,
        white: true,
        small: true,
        message: "",
        cssClass: null,
        callback: null,
        show: false,
        // internal
        _global: false
    };
    var opts = $.extend(defaultOptions, options);
    var throbber = $container.data("throbber");

    if (throbber) {
        if (toggle)
            throbber.show(options);
        else
            throbber.hide();
    } else {
        throbber = $container.throbber(opts).data("throbber");
        if (toggle)
            throbber.show();
        else
            throbber.hide();
    }

    return throbber;
}

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

/**
 * Các bước thực hiện
 * - Lấy các giá trị:
 * + tab-pane có scroll height max -> A
 * + Height max trong các tab-pane -> A0
 * + Scroll height max trong các tab-pane -> A1
 * + Chênh lệch height của tab-pane khi hiển thị -> A2 = A1 - A0
 * + Kendo Window height -> B
 * - Nếu B < C && A.isOverflow()
 * + B += A2
 * + Call Resize Kendo Window
 * Note:
 * - Không xử lý được hết các trường hợp, vẫn tồn tại khi chuyển qua các tab thì height bị thay đổi
 * vì khi show tab nào height tab đó sẽ là height của các tab khác (đang hidden), khi resize window thì cũng như vậy.
 * Nên vẫn xuất hiện trường hợp Kendo Window co vào giãn ra khi vừa chọn Tab vừa resize liên tục.
 * Tuy nhiên cũng hiếm khi xảy ra trừ khi user vừa chọn Tab vừa resize window liên tục nên cũng không gây ảnh hưởng đáng kể.
 * - Tồn tại khoảng trắng ở các tab có height nhỏ hơn.
 */
app.tab.handlers.resizeWindowsForEachSingleMainTabScroll = function () {
    var $kWindows = $('.k-widget.k-window');
    if ($kWindows.length) {
        $.each($kWindows, function (i, x) {
            var $kWindow = $(x);

            app.tab.handlers.resizeWindowForEachSingleMainTabScroll($kWindow);
        });
    }
}
app.tab.handlers.resizeWindowForEachSingleMainTabScroll = function ($kWindow) {
    var $tabs = $kWindow.find('div.tabbable');
    if (!$tabs.length)
        return;
    if ($tabs.length > 1) {
        console.log('Can only handles for single main tab in window');
        return;
    }

    var $tab = $tabs.first();
    var $tabPanes = $tab.find('.tab-content .tab-pane'),
        kWindowHeight = $kWindow.outerHeight(),
        windowHeight = $(window).height();
    var $tabPaneActive = $tabPanes.filter('.active');

    var $tabPaneMaxScrollHeight = _.max($tabPanes, function (x) {
        var $tabPane = $(x);
        return $tabPane.prop('scrollHeight');
    });
    $tabPaneMaxScrollHeight = $($tabPaneMaxScrollHeight);

    if (kWindowHeight < windowHeight && $tabPaneMaxScrollHeight.isOverflow()
        || $tabPaneActive.is($tabPaneMaxScrollHeight)) {
        var maxHeightTabPane = $tabPaneMaxScrollHeight.outerHeight();
        var maxScrollHeightTabPane = $tabPaneMaxScrollHeight.prop('scrollHeight');
        var tabPaneHeightPad = maxScrollHeightTabPane - maxHeightTabPane;
        if (tabPaneHeightPad > 4)
            $kWindow.outerHeight($kWindow.outerHeight() + tabPaneHeightPad);

        var $kWindowContent = $kWindow.find('.k-window-content');
        var kWindow = $kWindowContent.data('kendoWindow');
        kWindow.trigger('resize');

        if ($kWindow.length)
            app.window.handlers.resizeForScroll({ $kWindow: $kWindow });
    }
}

//#region @typedef

//#region app.ajaxForm extends

/**
 * Options for initialize ajax form.
 * @typedef {Object} $.fn.initAjaxForm.options
 * @property {String} url - Action url of ajax form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {Object=} submitElement - Submit element selector of form.
 * @property {Object=} continueAddingElement - "Continue adding" element selector of form.
 * @property {Object=} continueEditingElement - "Continue editing" element selector of form.
 * @property {enums.form.ErrorDisplayMode} [errorDisplayMode=enums.ajaxForm.ErrorDisplayMode.ShowFirstError] - Error display mode.
 * @property {Function=} initCallback - Initial callback for ajax form.
 * @property {Function=} validationCallback - Validation callback for ajax form.
 * @property {app.ajaxForm.init.options.callback=} callback - Callback for ajax form.
 * @property {app.ajaxForm.init.options.continueEditingCallback=} continueEditingCallback - "Continue editing" callback for ajax form.
 */

/**
 * Options for reload ajax form.
 * @typedef {Object} app.ajaxForm.reload.options
 * @property {String} url - Url for get ajax form.
 * @property {Number=} [delay=0] - Delay of windown form.
 * @property {(Object=|Function=)} extraData - Extra data for ajax form/window form.
 * @property {Boolean=} [initAjaxForm=true] - Specify whether initializes ajax form or not. true: for single form; false: for the View has many forms, in this case, each form should be initialize ajax form separately.
 * @property {String=} actionUrl - Action url of ajax form.
 * @property {Object=} submitElement - Submit element selector of form.
 * @property {Object=} continueAddingElement - "Continue adding" element selector of form.
 * @property {Object=} continueEditingElement - "Continue editing" element selector of form.
 * @property {enums.form.ErrorDisplayMode} [errorDisplayMode=enums.ajaxForm.ErrorDisplayMode.ShowFirstError] - Error display mode.
 * @property {Function=} initCallback - Initial callback for ajax form.
 * @property {Function=} validationCallback - Validation callback for ajax form.
 * @property {app.ajaxForm.init.options.callback=} callback - Callback for ajax form.
 * @property {app.ajaxForm.init.options.continueEditingCallback=} continueEditingCallback - "Continue editing" callback for ajax form.
 */

//#endregion

//#endregion

//#region app.ajaxForm

// app.ajaxForm.init
/**
 * Initialize ajax for single form.
 * @param {$.fn.initAjaxForm.options} options - Options for initialize ajax form.
 */
$.fn.initAjaxForm = function (options) {
    app.ajaxForm.init($.extend(options, { $container: this }));
}

// app.ajaxForm.reload
/**
 * Reload ajax form.
 * @param {$.fn.reloadAjaxForm.options} options - Options for reload ajax form.
 */
$.fn.reloadAjaxForm = function (options) {
    app.ajaxForm.reload($.extend(options, { $container: this }));
}

//#endregion

//#region app.ui

// app.ui.loader
/**
 * Show loading animation on specify container.
 * @param {Boolean} toggle - If true: show the loader, false: hide the loader.
 * @param {app.ui.loader.options=} options - Options for loader.
 * @returns {Object} - Loader instance.
 */
$.fn.loader = function (toggle, options) {
    return app.ui.loader(this, toggle, options);
}

//#endregion

//#region app.plugins

// select2
/**
* Clear select for specify element.
* @param {(Array=|Object=)} eventParams - Event parameters.
*/
$.fn.clearSelect = function (eventParams) {
    app.plugins.select2.clearSelect(this, eventParams);
}

// Fancytree
$.ui.fancytree._FancytreeClass.prototype.selectedIds = function () {
    var selNodes = this.getSelectedNodes();
    var selKeys = $.map(selNodes, function (node) {
        return node.key;
    });

    return selKeys;
};

//#endregion

//#region Form Helper

$.fn.focusFirst = function () {
    app.form.focusFirst(this);
}
$.fn.initFormScroll = function () {
    app.form.initFormScroll(this);
}
/**
 * Initialize form validation.
 * @param {app.form.initFormValidation.options=} options - Options for form validation.
 * @returns {Object} - Form validator. 
 */
$.fn.initFormValidation = function (options) {
    return app.form.initFormValidation($.extend(options, { $form: this }));
}
$.fn.initLabelHint = function () {
    app.form.initLabelHint(this);
}

//#endregion

//#region Kendo UI

$.extend(kendo.ui, {
    progress: function (container, toggle) {
        window.app.ui.loader(container, toggle, { timingFunction: "initial" });
    }
});

//#endregion

app.handlers.init = function () {
    $(function () {
        app.handlers.window.resize();
    });
}
app.handlers.window.resize = function () {
    $(window).on('resize', function (e) {
        // Fix validation tooltip (qTip2) hiện ở vị trí không đúng trong window size mới sau khi resize window
        delay('app.handlers.window.resize__app.qtip.hideAll', function () {
            app.qtip.hideAll();
        }, 100);

        // Resize Kendo Windows for each single main Tab scroll
        delay('app.handlers.window.resize__app.tab.handlers.resizeWindowsForEachSingleMainTabScroll', function () {
            app.tab.handlers.resizeWindowsForEachSingleMainTabScroll();
        }, 100);

        // Resize Kendo Windows
        delay('app.handlers.window.resize__app.window.handlers.resize', function () {
            app.window.handlers.resize();
        }, 100);

        // Resize Kendo Grids
        delay('app.handlers.window.resize__app.grid.handlers.resize', function () {
            app.grid.handlers.resize();
        }, 100);

        delay('app.handlers.window.resize__app.plugins.split.handlers.resize', function () {
            app.plugins.split.handlers.resize();
        }, 0);
    });
}
app.init = function () {
    app.initComponents();
    app.handlers.init();
}
app.initComponents = function () {
    app.ajax.init();
    app.form.init();
    app.notification.init();
    app.validator.init();

    app.window.alert.init();
    app.window.confirm.init();
    app.window.deletes.init();

    app.plugins.applyCommonPlugins($('body'));
}
app.initAjax = function () {
    app.plugins.applyCommonPlugins($('body'));
}