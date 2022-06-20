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
