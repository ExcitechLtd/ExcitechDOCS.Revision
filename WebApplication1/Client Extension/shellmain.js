"use strict"
var _shellUI;

function OnNewShellUI(shellUI) {
    /// <summary>The entry point of ShellUI module.</summary>
    /// <param name="shellUI" type="MFiles.ShellUI">The new shell UI object.</param> 

    // Register to listen new shell frame creation event.
    shellUI.Events.Register(Event_NewNormalShellFrame, newShellFrameHandler);
    _shellUI = shellUI;
}

function newShellFrameHandler(shellFrame) {
    /// <summary>Handles the OnNewShellFrame event.</summary>
    /// <param name="shellFrame" type="MFiles.ShellFrame">The new shell frame object.</param> 

    // Register to listen the started event.
    shellFrame.Events.Register(Event_Started, getShellFrameStartedHandler(shellFrame));
    shellFrame.Events.Register(Event_ActiveListingChanged, activeListingChangedHandler);
    shellFrame.Events.Register(Event_NewShellListing, newShellListingHandler);
}

function getShellFrameStartedHandler(shellFrame) {
    /// <summary>Gets a function to handle the Started event for shell frame.</summary>
    /// <param name="shellFrame" type="MFiles.ShellFrame">The current shell frame object.</param> 
    /// <returns type="MFiles.Events.OnStarted">The event handler.</returns>

    // Return the handler function for Started event.
    return function () {
        //add Workflow Tab
        m_workflowTab = shellFrame.RightPane.AddTab("_documentRevision", "Revision", "_last");
    };
}

function activeListingChangedHandler(oldListing, newListing) {

    newListing.Events.Register(Event_SelectionChanged, selectionChangedHandler);
}

function newShellListingHandler(shellListing) {

    shellListing.Events.Register(Event_Started, function () {
        shellListing.Events.Register(Event_SelectionChanged, selectionChangedHandler);
        shellListing.Events.Register(Event_SelectedItemsChanged, selectedItemsChangedHandler);
    });
}

//region " Event Handlers "
function selectedItemsChangedHandler(selectedItems) {
    selectionChangedHandler(selectedItems);
}

function selectionChangedHandler(selectedItems) {

    // update Extranet Queue tab visibility
    WorkflowTabAvailability(selectedItems);
}

function WorkflowTabAvailability(selectedItems) {

    //check at least one object selected
    if (selectedItems.ObjectVersions.count == 0) { m_workflowTab.visible = false; return; }

    //check first object is a document object type
    var objVer = selectedItems.ObjectVersions[0].ObjVer;
    if (MFBuiltInObjectTypeDocument !== objVer.Type) { m_workflowTab.visible = false; return; }

    //get Object Version and Properties
    var objVerProps = selectedItems.ObjectVersionsAndProperties[0];

    //ensure Workflow and State set
    var jsonProp = objVerProps.properties.SearchForPropertyByAlias(_shellUI.Vault, "HESIMM.Property.Revision.JSON", true)
    if (jsonProp == null) { m_workflowTab.visible = false; return; }

    var objectTitle = selectedItems.ObjectVersionsAndProperties[0].VersionData.Title;

    var customData = {
        jsonStr: jsonProp.GetValueAsLocalizedText(),
        selectedItem: selectedItems.ObjectVersionsAndProperties,
        Title: objectTitle,
        Version: objVer.Version
    }

    m_workflowTab.ShowDashboard("DocumentRevision", customData);
    m_workflowTab.visible = true;
    return;

}
//end region

