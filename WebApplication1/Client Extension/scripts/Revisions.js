var vaultGUID;
var m_vault;
var m_dashboard;
var _customData;
var m_startEventHandle;
var m_stopEventHandle;
var _datatable;
var _pageIsReady = false;


$(function () {
    initSearch();
    refreshRevisionTab();
})

//dash event handlers
function OnNewDashboard(dashboard) {
    /// <summary>Dashboard code entry point.</summary>
    /// <param name="dashboard" type="MFiles.Dashboard">The new dashboard object.</param>

    // Register a handler to listen the started event.
    m_startEventHandle = dashboard.Events.Register(Event_Started, getDashboardStartedHandler(dashboard));
    m_stopEventHandle = dashboard.Events.Register(Event_Stop, getDashboardStopHandler());

    // register a handler to listen for tab selection changes
    var shellPaneContainer = dashboard.parent;
    m_tabSelectedEventHandler = shellPaneContainer.Events.Register(Event_TabSelected, tabSelected());
}

function getDashboardStartedHandler(dashboard) {
    /// <summary>Gets the event handler function for dashboard starting event.</summary>
    /// <param name="dashboard" type="MFiles.Dashboard">The current dashboard object.</param>
    /// <returns type="MFiles.Events.OnStarted">The event handler.</returns>

    // initialise member variables
    m_dashboard = dashboard;
    m_vault = m_dashboard.Vault;
    _customData = dashboard.CustomData;
    _pageIsReady = true;
    // Return the handler function.
    return function () {

        //only update if tab selected
        if (m_dashboard.Parent.ShellFrame.rightPane.GetSelectedTab().TabId != "_documentrevision") {
            return;
        }
        //initialise Vault GUID and Project ID globals
        vaultGUID = m_vault.getGUID();

        //update Project Header
        updateHeader(_customData);

        //populate Log Table
        //getDocumentLogData();
        //refreshRevisionTab();
    };
}

function getDashboardStopHandler() {
    return function () {
        m_dashboard.Events.Unregister(m_startEventHandle);
        m_dashboard.Events.Unregister(m_stopEventHandle);
    };
}

function tabSelected() {
    return function (selectedTab) {
        if (selectedTab.TabId == "_documentRevision")
            refreshRevisionTab();
    }
}
//end

//methods
function initSearch() {
    $("#_tableSearch").on("input propertychange paste", function (ev) {
        SimpleSearch(_datatable, $(this).val());
    })
}

function updateHeader(objectData) {
    $('#objectHeading').text(objectData.Title);
    $('#versionInfo').text("ID " + objectData.ObjectID + " Version " + objectData.Version);

    var currentSelection = objectData.selectedItem
    // set object details
    if (currentSelection.length == 1) {

        $('#documentHeading').text(currentSelection[0].VersionData.Title);
        $('#versionInfo').text("ID " + currentSelection[0].ObjVer.ID + " Version " + currentSelection[0].ObjVer.Version);
    } else {
        $('#documentHeading').text("(varies)");
        $('#versionInfo').text("ID (varies) Version (varies)");
    }
}

function refreshRevisionTab() {
    var jsonObj;
    jsonObj = JSON.parse(_customData.jsonStr)
    //the json data should suit us to use the datatables object type where by we set the columns and then the data is objec and object name
    var values = {}
    values.columns = createRevisionColumns();
    values.data = jsonObj;
    _datatable = ShowTable(values, $("#_dataTable"))

    $("#_tableSearch").prop("disabled", jsonObj.length <= 0)
}

function createRevisionColumns() {
    var _columns = [];
    _columns.push(
        {
            name: "RevisionID",
            title: "RevisionID",
            data: "RevisionID"
        },
        {
            name:"Ammendment",
            title: "Ammendment",
            data: "Ammendment"
        },
        {
            name: "CheckedBy",
            title: "CheckedBy",
            data: "CheckedBy"
        },
        {
            name: "CheckedDate",
            title: "CheckedDate",
            data: "CheckedDate",
            render: function (data, type, row) {
                if (data == "") { return ""; }
                if (data == null) { return "";}
                var _date = new Date(data);
                return moment(_date).format("DD/MM/YYYY");
            }
        },
        {
            name: "ApproveBy",
            title: "Approved By",
            data: "ApproveBy"
        },
        {
            name: "ApproveDate",
            title: "Approved Date",
            data: "ApproveDate",
            render: function (data, type, row) {
                if (data == "") { return ""; }
                if (data == null) { return ""; }
                var _date = new Date(data);
                return moment(_date).format("DD/MM/YYYY");
            }
        }
    )

    return _columns
}
//end