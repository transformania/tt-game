var attackTextMap = {};

// ------------------------ FASHION WITCH --------------------

attackTextMap['Fashion Witch'] = {};
attackTextMap['Fashion Witch']['Generic'] = {
    'texts': [
    '"You look nice.  But I could make you oh so much prettier..." [ATTACKER] teases, zapping [VICTIM] on the nose with a quick little enchantment.',
    ],
};
attackTextMap['Fashion Witch']['Female Bystander'] = {
    'texts': [

    '"What have we here?  Some boring old nine-to-five white collar dweeb who needs a little... change in their lives?" [ATTACKER] asks, running a finger along [VICTIM]\'s chest seductively, leaving a streak of magic seeping into their body with them knowing.',

     '"I bet you haven\t taken a nice vacation in a very long time know.  You stuff, stuff always comes up... long hours at work... nagging boyfriend or girlfriend... all those bills to pay...  Wouldn\t you like to have some time off?" [ATTACKER] coaxes.  [VICTIM] nods reluctantly, looking disheartened and both scared and excited at what [ATTACKER] might have in store for them.',

    ],
};

// ------------------------ WITCHLING --------------------

attackTextMap['Witchling'] = {};
attackTextMap['Witchling']['Generic'] = {
    'texts': [
    '"If you\'re not with us, you\'re against us," [ATTACKER] threatens, laughing as she casts some spells toward [VICTIM]\'s torso, watching as they twist to avoid the crackling bolts of energy.',
    ],
};
attackTextMap['Witchling']['Female Bystander'] = {
    'texts': [
    '"I was like you once," [ATTACKER] taunts.  "Then I learned a thing... or two or three..." [ATTACKER] says, shooting small bolts of crackling magic at [VICTIM]\'s feet.',
    ],
};

// ------------------------ FEMALE BYSTANDER --------------------

attackTextMap['Female Bystander'] = {};
attackTextMap['Female Bystander']['Generic'] = {
    'texts': [
    '"I don\'t know any magic, but I can slap like a queen bitch..." [ATTACKER] squeaks timidly.  It\s just enough to make [VICTIM] recoil a little in fright.',
    ],
};
attackTextMap['Female Bystander']['Female Bystander'] = {
    'texts': [
   '"So it looks like neither of us know any fun little spells," [ATTACKER] sighs, tossing a tube of lipstick from out of her purse at [VICTIM], envious of the witches whose attacks are a little less... lame.',
    ],
};

// ------------------------ MALE BYSTANDER --------------------

attackTextMap['Male Bystander'] = {};
attackTextMap['Male Bystander']['Generic'] = {
    'texts': [
    '"I don\'t know any magic, but I do throw some mean punches..." [ATTACKER] squeaks timidly.  It\s just enough to make [VICTIM] recoil a little in fright.',
    ],
};
attackTextMap['Male Bystander']['Male Bystander'] = {
    'texts': [
   '"So it looks like neither of us know any fun little spells," [ATTACKER] sighs, tossing a clod of dirt from the ground at [VICTIM], envious of the warlocks whose attacks are a little less... lame.',
    ],
};


function getRandomAttackText(attackerForm, victimForm) {
    try {
        var texts = attackTextMap[attackerForm][victimForm].texts;
        var randIndex = Math.floor(Math.random() * texts.length);
        var result = texts[randIndex];
        return result;
    } catch (e) {
        var texts = attackTextMap[attackerForm]['Generic'].texts;
        var randIndex = Math.floor(Math.random() * texts.length);
        var result = texts[randIndex];
        return result;
    }
}

function insertNames(text, attacker, victim) {
    return text.replace(/\[ATTACKER\]/gi, attacker.firstName).replace(/\[VICTIM\]/gi, victim.firstName);
}