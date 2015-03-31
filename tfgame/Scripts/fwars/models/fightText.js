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

    '"What have we here?  Some boring old nine-to-five white collar chick who needs a little... change in her life?" [ATTACKER] asks, running a finger along [VICTIM]\'s nipples, leaving a streak of magic seeping into her breasts without [VICTIM] suspecting a thing.',

     '"I bet you haven\'t taken a nice vacation in a very long time know.  You stuff, stuff always comes up... long hours at work... nagging boyfriend.. all those bills to pay... all that time putting on makeup to try and look good for just a few short hours... Wouldn\'t you like to have some time off?" [ATTACKER] coaxes.  [VICTIM] nods reluctantly, looking disheartened and both scared and excited at what [ATTACKER] might have in store for her.',

    ],
};

attackTextMap['Fashion Witch']['Male Bystander'] = {
    'texts': [

    '"What have we here?  Some boring old nine-to-five white collar dweeb who needs a little... change in his life?" [ATTACKER] asks, running a finger along [VICTIM]\'s stiffening manhood seductively, leaving a streak of magic seeping into his cock without him suspecting a thing.',

     '"I bet you haven\'t taken a nice vacation in a very long time know.  You stuff, stuff always comes up... long hours at work... girlfriend... all those bills to pay... all that time spent at the gym trying to buff up for girls who never notice...  Wouldn\'t you like to have some time off?" [ATTACKER] coaxes.  [VICTIM] nods reluctantly, looking disheartened and both scared and excited at what [ATTACKER] might have in store for him...',

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

// ------------------------ SENIOR WITCHLING --------------------

attackTextMap['Senior Witchling'] = {};
attackTextMap['Senior Witchling']['Generic'] = {
    'texts': [
    '"TODO',
    ],
};


// ------------------------ FEMALE BYSTANDER --------------------

attackTextMap['Female Bystander'] = {};
attackTextMap['Female Bystander']['Generic'] = {
    'texts': [
    '"I don\'t know any magic, but I can sure slap like a bitch..." [ATTACKER] squeaks timidly.  It\s just enough to make [VICTIM] recoil a little in fright.',
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
   '"So it looks like neither of us know any fun little spells," [ATTACKER] sighs, tossing some coins out of his wallet at [VICTIM], envious of the witches and warlock whose attacks are a little less... lame.',
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