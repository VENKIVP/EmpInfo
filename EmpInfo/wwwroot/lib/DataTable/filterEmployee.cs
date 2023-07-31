(function($) {
    jQuery.fn.dataTableWithFilter = function(settings) {
        // alias the original jQuery object passed in since there is a possibility of multiple dataTables and search containers on a single page.
        // If we don't do this then we run the risk of having the wrong jQuery object before forcing a dataTable.fnDraw() call
        var $dataTable = this,
            searchCriteria = [],
            filterOptions = settings.filterOptions;

        // remove the filterOptions object from the object literal (json) that will be passed to dataTables
        delete settings.filterOptions;
        searchCriteria = fnGetSearchCriteria();

        // SEACRH & RESET are the same. In the case of Reset the data is cleared in the component before calling Reset.
        document.addEventListener('click', function(e) {
            var resetButton = document.getElementById(filterOptions.clearSearchButton);
            var searchButton = document.getElementById(filterOptions.searchButton);

            if (e.target == resetButton || e.target == searchButton)
            {
                searchCriteria = fnGetSearchCriteria();
                if (searchCriteria)
                {
                    addDatatableShimmer($dataTable.attr("id"));
                    var tables = $("#" + $dataTable.attr("id")).DataTable();
                    tables.draw();
                }
            }
        });

        var pageSize = fnGetPageDisplayedValue(filterOptions.gridName);
        var DisplayStarts = fnGetPageStartRecord(filterOptions.gridName);

        settings.iDisplayStart = DisplayStarts;
        settings.iDisplayLength = pageSize != 0 ? pageSize : filterOptions.defaultPageSize;
        settings.fnServerParams = function(aoData) {
            $('.datatable-loading').fadeIn();
            var i;
            for (i = 0; i < searchCriteria.length; i++)
            {
                // pushing each name/value pair that was found from the searchButton click event in to the aoData array
                // which will be sent to the server in the request
                aoData.push(searchCriteria[i]);
            }
        };
        return $dataTable.DataTable(settings);
    };
} (jQuery));

function fnGetSearchCriteria()
{
    var searchCriteria = [];

    // created date ranges (dd/MMM/yyyy)
    var fromDate = $('#txtCreatedFrom_dateRange').datepicker("getDate");
    if (fromDate)
        searchCriteria.push({ "name": "CreatedDate_Fr", "value": $.datepicker.formatDate("yy-mm-dd", fromDate) });
    else
    searchCriteria.push({ "name": "CreatedDate_Fr", "value": '1/1/1900' });

var toDate = $('#txtCreatedTo_dateRange').datepicker("getDate");
if (toDate)
    searchCriteria.push({ "name": "CreatedDate_To", "value": $.datepicker.formatDate("yy-mm-dd", toDate) });
    else
    searchCriteria.push({ "name": "CreatedDate_To", "value": '1/1/2050' });

if (!fromDate && toDate)
{
    ebillity.alert('Please enter Created From Date', 'Ok');
    searchCriteria = [];
    return false;
}
else if (fromDate && !toDate)
{
    ebillity.alert('Please enter Created To Date', 'Ok');
    searchCriteria = [];
    return false;
}
else if (fromDate > toDate)
{
    ebillity.alert('Please enter valid Created To Date', 'Ok');
    searchCriteria = [];
    return false;
}

// closed date ranges (dd/MMM/yyyy)
var closeDate = $('#txtClosedFrom_dateRangeClosed').datepicker("getDate");
if (closeDate)
    searchCriteria.push({ "name": "CloseDate_Fr", "value": $.datepicker.formatDate("yy-mm-dd", closeDate) });
    else
    searchCriteria.push({ "name": "CloseDate_Fr", "value": '1/1/1900' });

var closeToDate = $('#txtClosedTo_dateRangeClosed').datepicker("getDate");
if (closeToDate)
    searchCriteria.push({ "name": "CloseDate_To", "value": $.datepicker.formatDate("yy-mm-dd", closeToDate) });
    else
    searchCriteria.push({ "name": "CloseDate_To", "value": '1/1/2050' });

if (!closeDate && closeToDate)
{
    ebillity.alert('Please enter closed from Date', 'Ok');
    searchCriteria = [];
    return false;
}
else if (closeDate && !closeToDate)
{
    ebillity.alert('Please enter closed To Date', 'Ok');
    searchCriteria = [];
    return false;
}
else if (closeDate > closeToDate)
{
    ebillity.alert('Please enter valid closed To Date', 'Ok');
    searchCriteria = [];
    return false;
}

var status = $('#projectListStatus').val();

if (status)
    searchCriteria.push({ "name": "Status", "value": status });

var projectName = $('#projectName').val();
if (projectName)
    searchCriteria.push({ "name": "ProjectName", "value": projectName });

var clientList = $('#ddlClient_prjt').val();
if (clientList)
    searchCriteria.push({ "name": "ClientList", "value": clientList.toString() });

var projectId = $('#projectId').val();
if (projectId)
    searchCriteria.push({ "name": "ProjectId", "value": projectId });

var projectType = $('#ddlProjectType_prjt').val();
if (projectType)
    searchCriteria.push({ "name": "ProjectType", "value": projectType });

var teamLeader = $('#ddlTeamLead_prjt').val();
if (teamLeader)
    searchCriteria.push({ "name": "TeamLeader", "value": teamLeader });

var accountManager = $('#ddlAccountManager_prjt').val();
if (accountManager)
    searchCriteria.push({ "name": "AccountManagerId", "value": accountManager });

var teamMembers = $('#ddlTeamMember_prjt').val();
if (teamMembers)
    searchCriteria.push({ "name": "TeamMember", "value": teamMembers });

var billableTypes = $('#ddlBillableType_prjt').val();
if (billableTypes)
    searchCriteria.push({ "name": "BillableType_Id", "value": billableTypes });

if (status === '1')
{
    searchCriteria.push({ "name": "ProjectStatus", "value": $('#projectStatusOpen').val() });
}
else if (status === '2')
{
    searchCriteria.push({ "name": "ProjectStatus", "value": $('#projectStatusClosed').val() });
}
else if (status === '')
{
    var projectStatus = $('#ddlProjectStatus_prjt').val();
    if (projectStatus)
        searchCriteria.push({ "name": "ProjectStatus", "value": projectStatus });
}

var hasEstimate = $('#ddlEstimate_prjt').val();
if (hasEstimate)
    searchCriteria.push({ "name": "HasAnEstimate", "value": hasEstimate });
    else
    searchCriteria.push({ "name": "HasAnEstimate", "value": '-1' });

searchCriteria.push({ "name": "appSortBy", "value": "" });
searchCriteria.push({ "name": "ColReorder", "value": true });
return searchCriteria;
}
