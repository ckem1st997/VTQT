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