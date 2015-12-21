
var scenedbName = "a";
var personalLog = "";
var waitingForPersonalLog = false;
var dontShowSceneImgNow = false;


//$(document).ready(function () {

    // first thing to do is get information on the scene we are currently in
    RefreshScene();

    function RefreshScene() {
        $.ajax({
            url: 'TFWorld/GetSceneInfo/',
            //  data: { sceneDbName: scenedbName },
            type: 'POST',
            beforeSend: loadWaiting,
            success: updateScene,
            dataType: 'text'
        });
    }

    

    function AjaxUpdateSceneLog() {
        $.ajax({
            url: 'TFWorld/GetLog/',
            //  data: { sceneDbName: scenedbName },
            type: 'POST',
            //beforeSend: loadWaiting,
            success: loadSuccess,
            dataType: 'text'
        });
    }

    function AjaxUpdateCharactersItems() {
        $.ajax({
            url: 'TFWorld/RefreshCharactersItems/',
            //  data: { sceneDbName: scenedbName },
            type: 'POST',
            //beforeSend: loadWaiting,
            success: updateCharactersItems,
            dataType: 'text'
        });
    }

    function AjaxSendChat() {
        $.ajax({
            url: 'TFWorld/AjaxTestChat/',
            data: { statement: $("input").val() },
            type: 'POST',
            // beforeSend: loadWaiting,
            success: loadSuccessClearChat,
            dataType: 'text'
        });
    }

    function AjaxSaveMeToDatabase() {
        $.ajax({
            url: 'TFWorld/SaveUserToDatabase/',
            // data: { statement: $("input").val() },
            type: 'POST',
            success: loadSuccessClearChat,
            dataType: 'text'
        });
    }

    var int = self.setInterval(function () { AjaxUpdateSceneLog() }, 1000);
    var int = self.setInterval(function () { AjaxUpdateCharactersItems() }, 5000);

    var borderflicker = self.setInterval(function () { FlickerPersonalLogBorder() }, 100);
    var borderflickerState = "off";

    function loadSuccess(data, newWidth, newHeight) {
        $("#ActionWindow").html(data);
        $("#ActionWindow").scrollTop($("#ActionWindow").scrollTop() + 100);
    }

    function loadSuccessClearChat(data, newWidth, newHeight) {
        $("input").val("");
        $("#ActionWindow").html(data);
        $("#ActionWindow").scrollTop($("#ActionWindow").scrollTop() + 100);

    }

    function updateScene(data) {


        var scene = jQuery.parseJSON(data);
        var connections = scene.Connections;
        var characters = scene.Characters;


        $("#SceneName").html(scene.SceneName);

        $("#SceneImg").css("background-image", "url(MMO_Images/Scenes/" + scene.Img + ")");
            
        var dat = jQuery.parseJSON(data);
        var me = dat.Me;

        
        setSelfInfo(me, false);

        clearConnections();
        clearPeople();

        for (var i = 0; i < connections.length; i++) {

            if (connections[i].Position == "N") {
                $("#Connection_N").html(connections[i].DestinationName);
                $("#Connection_N").show();
                $("#Connection_N_Hidden").html(connections[i].ChildSceneDbName);
            } else if (connections[i].Position == "NE") {
                $("#Connection_NE").html(connections[i].DestinationName);
                $("#Connection_NE").show();
                $("#Connection_NE_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "E") {
                $("#Connection_E").html(connections[i].DestinationName);
                $("#Connection_E").show();
                $("#Connection_E_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "SE") {
                $("#Connection_SE").html(connections[i].DestinationName);
                $("#Connection_SE").show();
                $("#Connection_SE_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "S") {
                $("#Connection_S").html(connections[i].DestinationName);
                $("#Connection_S").show();
                $("#Connection_S_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "SW") {
                $("#Connection_SW").html(connections[i].DestinationName);
                $("#Connection_SW").show();
                $("#Connection_SW_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "W") {
                $("#Connection_W").html(connections[i].DestinationName);
                $("#Connection_W").show();
                $("#Connection_W_Hidden").html(connections[i].ChildSceneDbName);
            } if (connections[i].Position == "NW") {
                $("#Connection_NW").html(connections[i].DestinationName);
                $("#Connection_NW").show();
                $("#Connection_NW_Hidden").html(connections[i].ChildSceneDbName);
            }

        }

        //for (var i = 0; i < characters.length; i++) {
        //    $("#People").append("<div class = 'PersonListing'>" + characters[i].Name + "</div>");
        //    // var oldHtml = $("#People").html();
        //    // $("#People").html(oldHtml + characters[i].Name + "<br>");
        //}

        updatePeopleListing(data);

    }

    function updatePeopleListing(data) {
        
        var dat = jQuery.parseJSON(data);
        var characters = dat.Characters;
        clearPeople();

        for (var i = 0; i < characters.length; i++) {
            $("#People").append("<div class = 'PersonListing'><div class = 'PersonListingPortrait' style='background-image: url(../MMO_Images/Portraits/" + characters[i].Form.Img + ");'> </div><div class= 'PersonListingName'>" + characters[i].Name + "</div></div>");
        }
    }

        

    function clearPeople() {
        $("#People").html("");
    }

    function loadWaiting() {
        $("#ActionWindow").html("....loading....");
    }

    function moveSuccess(data) {

        var dat = jQuery.parseJSON(data);
        var plog = dat.pLog;

        

        if (plog.length > 0) {
            dontShowSceneImgNow = true;
            personalLog = plog;
            updatePersonalLog();
        }

        
        RefreshScene();
    }

    function LookAtSceneSuccess(data) {
        var scene = jQuery.parseJSON(data);
        $("#SceneImg").css("background-image", "url(MMO_Images/Scenes/" + scene.Img + ")");
        var plog = $("#PersonalLog").html();
        $("#PersonalLog").html(plog  + scene.Description);

    }

    function clearLogWhileWaiting() {

    }

    function updateCharactersItems(data) {
        var characters = jQuery.parseJSON(data);
        clearPeople();


        for (var i = 0; i < characters.length; i++) {

            //  $("#People").append("<div class = 'PersonListing'>" + characters[i].Name + "</div>");  //background-image: url("../MMO_Images/Portraits/man01.jpg");

            $("#People").append("<div class = 'PersonListing'><div class = 'PersonListingPortrait' style='background-image: url(../MMO_Images/Portraits/" + characters[i].Form.Img + ");'> </div><div class= 'PersonListingName'>" + characters[i].Name + "</div></div>");

            //  var oldHtml = $("#People").html();
            // $("#People").html(oldHtml + characters[i].Name + "<br>");
        }
    }

    function characterQuerySuccess(data) {
        var person = jQuery.parseJSON(data);

        $("#SceneImg").css("background-image", "url(MMO_Images/People/" + person.Form.Img + ")");

        var plogtext = $("#PersonalLog").html();
        $("#PersonalLog").html(plogtext + "<br><br>" + person.Name + " " + person.Form.Description3rd);
    }

    function selfQuerySuccess(data) {
        var me = jQuery.parseJSON(data);

        personalLog = me.pLog;
            
        updatePersonalLog();

        setSelfInfo(me.character, true);
    }


    function updatePersonalLog() {

        var plogtext = $("#PersonalLog").html();

        // nothing new.  No updates.
        if (personalLog.length == 0) {

        }

            // one new post so automatically publish it
        else if (personalLog.length >= 1) {

            grayOutConnections(true);

            $("#PersonalLog").html(plogtext + "<br><br>" + personalLog[0].Message);

            // update the image if it is not null
            if (personalLog[0].Img != null) {
                $("#SceneImg").css("background-image", "url(MMO_Images/People/" + personalLog[0].Img + ")");
            }

            // update the PORTRAIT image if it is not null
            if (personalLog[0].PortraitImg != null) {
                $("#Portrait").css("background-image", "url(MMO_Images/Portraits/" + personalLog[0].PortraitImg + ")");
            }

            // remove the front element of our message log
            personalLog.shift();
        }

        // if we still have more new elements in our list, flash a bit:
        if (personalLog.length == 0) {
            grayOutConnections(false);
            waitingForPersonalLog = false;
            borderflickerState = "off";
            $("#PersonalLog").css("border-color", "black");
        } else {
            waitingForPersonalLog = true;
        }

    }

//});
