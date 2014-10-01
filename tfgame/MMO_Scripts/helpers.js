function FlickerPersonalLogBorder() {
    if (personalLog.length > 0) {
        if (borderflickerState == "off") {
            borderflickerState = "on";
            $("#PersonalLog").css("border-color", "red");
        } else {
            borderflickerState = "off";
            $("#PersonalLog").css("border-color", "black");
        }
    }
}

function setSelfInfo(character, setMainImg) {

    // don't set this unless we clicked on our portrait
    if (setMainImg == true) {
        $("#SceneImg").css("background-image", "url(MMO_Images/People/" + character.Form.Img + ")");
    }

    if (dontShowSceneImgNow == false) {
        $("#Portrait").css("background-image", "url(MMO_Images/Portraits/" + character.Form.Img + ")");
    } else {
        dontShowSceneImgNow = false;
    }
    

    $("#MyName").html(character.Name);
    $("#MyForm").html(character.Form.FormNameCasual);
    $("#MyHealth").html(character.Health + " / " + character.HealthMax + " Resistance");
    $("#MyMana").html(character.Mana + " / " + character.ManaMax + " Energy");
}

function clearConnections() {
    $("#Connection_N").html("");
    $("#Connection_N").hide();
    $("#Connection_N_Hidden").html("");

    $("#Connection_NE").html("");
    $("#Connection_NE").hide();
    $("#Connection_NE_Hidden").html("");

    $("#Connection_E").html("");
    $("#Connection_E").hide();
    $("#Connection_E_Hidden").html("");

    $("#Connection_SE").html("");
    $("#Connection_SE").hide();
    $("#Connection_SE_Hidden").html("");

    $("#Connection_S").html("");
    $("#Connection_S").hide();
    $("#Connection_S_Hidden").html("");

    $("#Connection_SW").html("");
    $("#Connection_SW").hide();
    $("#Connection_SW_Hidden").html("");

    $("#Connection_W").html("");
    $("#Connection_W").hide();
    $("#Connection_W_Hidden").html("");

    $("#Connection_NW").html("");
    $("#Connection_NW").hide();
    $("#Connection_NW_Hidden").html("");
}

function grayOutConnections(bval) {
    if (bval == true) {
     
        $("#Connection_N").css("color","gray");
        $("#Connection_NE").css("color","gray");
        $("#Connection_E").css("color","gray");
        $("#Connection_SE").css("color","gray");
        $("#Connection_S").css("color","gray");
        $("#Connection_SW").css("color","gray");
        $("#Connection_W").css("color","gray");
        $("#Connection_NW").css("color", "gray");
        $("#Connection_N").css("border-color", "gray");
        $("#Connection_NE").css("border-color", "gray");
        $("#Connection_E").css("border-color", "gray");
        $("#Connection_SE").css("border-color", "gray");
        $("#Connection_S").css("border-color", "gray");
        $("#Connection_SW").css("border-color", "gray");
        $("#Connection_W").css("border-color", "gray");
        $("#Connection_NW").css("border-color", "gray");
    } else {
        $("#Connection_N").css("color","black");
        $("#Connection_NE").css("color","black");
        $("#Connection_E").css("color","black");
        $("#Connection_SE").css("color","black");
        $("#Connection_S").css("color","black");
        $("#Connection_SW").css("color","black");
        $("#Connection_W").css("color","black");
        $("#Connection_NW").css("color", "black");
        $("#Connection_N").css("border-color", "blue");
        $("#Connection_NE").css("border-color", "blue");
        $("#Connection_E").css("border-color", "blue");
        $("#Connection_SE").css("border-color", "blue");
        $("#Connection_S").css("border-color", "blue");
        $("#Connection_SW").css("border-color", "blue");
        $("#Connection_W").css("border-color", "blue");
        $("#Connection_NW").css("border-color", "blue");
    }
}

