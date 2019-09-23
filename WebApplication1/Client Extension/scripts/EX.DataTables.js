//datatables
function GetTableDefaults() {
	return {
		"bProcessing": false,
		"bDestroy": false,
		"searching": true,
		"autoWidth": true,
		"sScrollX": "100%",
		"scrollCollapse": true,
		"bSort": false,
		"ordering": false,
		"order": [],
		"bInfo": true,
		"bFilter": false,
		"paging": false,
		"lengthMenu": [[-1, 50, 25], ["All", 50, 25]],
		"deferRender": true,
		"sPaginationType": "full_numbers",
		"dom": 'rt'
	}
}

//returns the table obejct 
function ShowTable(values, table) {
	var _opt = $.extend(true, {}, GetTableDefaults(), {
		"columns": values.columns,
		"data": values.data
	});
	return $(table).DataTable(_opt);
};

function SimpleSearch (tableObj, value) {
    tableObj.search(value).draw(true);
}