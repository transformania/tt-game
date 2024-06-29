function detail_success() {
    //$('#itemDetailModal').modal('show');
}

function detail_failure() {
    $("#fullDetails").html("Failed to load details");
}

function detail_wait() {
    $("#fullDetails").html("Loading details...");
}