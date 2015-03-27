// ----------------- FORM TYPE MAP ------------------

/*
 baseHP:  Base amount of hit points
 graphic:  filename of this form
 description:  text description of this form
 gender:  male or female
 class:  civilian (these can't cast spells), witch (no specialization), seduction, strength, cuteness, science, trickery
 promotesTo:  All possible forms that this form can be promoted into when XP reaches xpNeededForLevelup

 strengthAttack:  Base attack damage in Strength type
 strengthDefense:  Base defense in Strength type

 seductionAttack:  Base attack damage in Seduction type
 seductionDefense:  Base defense in Seduction type

 energyNeededToRecruit:  The amount of Energy it takes for the covenant leader to instantly turn a person of this form to their own side
 xpNeededForLevelup:  The amount of XP it takes before this form is eligible to be promoted to a superior form
 brainwashCost:  The amount of Energy it takes to make this form wander off after a battle, keeping infamy gain from it to a minimum

*/

var formStatsMap = {};

/* HERO CLASS */
formStatsMap['Fashion Witch'] = {
    'name':'Fashion Witch',
    'baseHP': 50,
    'graphic': 'Fashion_Witch.jpg',
    'description': 'A powerful woman knowledgeable of many types of magic... mainly improving fashion by turning people into new clothes!',
    'gender': 'female',
    'promotesTo': [],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 3,
    'seductionDefense': 0,
    'energyNeededToRecruit': 10000,

    'levelRequiredForPromotion': 99999,
    'xpNeededForLevelup': 9999,
    'brainwashCost': 9999,
};

/* FEMALE FORMS - NOOB CLASS */
formStatsMap['Female Bystander'] = {
    'name': 'Female Bystander',
    'baseHP': 2,
    'graphic': 'Female_Bystander.jpg',
    'description': 'A regular, non-magical woman like the kind who make of the majority of the female population.',
    'gender': 'female',
    'class': 'civilian',
    'promotesTo': ['Witchling'],

    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,

    'levelRequiredForPromotion': 3,
    'xpNeededForLevelup': 2,


    'brainwashCost': 15,
};

formStatsMap['Witchling'] = {
    'name': 'Witchling',
    'baseHP': 4,
    'graphic': 'Witchling.jpg',
    'description': 'A young witch, still learning her newfound powers!',
    'description': '',
    'gender': 'female',
    'promotesTo': ['Senior Witchling'],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
    'energyNeededToRecruit': 200,

    'levelRequiredForPromotion': 5,
    'xpNeededForLevelup': 5,
    'brainwashCost': 35,
};

formStatsMap['Senior Witchling'] = {
    'name': 'Senior Witchling',
    'baseHP': 8,
    'graphic': 'Senior_Witchling.jpg',
    'description': 'A more accomplished witch, ready to move on to her specialization.',
    'gender': 'female',
    'promotesTo': [],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 1,
    'seductionAttack': 3,
    'seductionDefense': 3,
    'energyNeededToRecruit': 300,

    'levelRequiredForPromotion': 99,
    'xpNeededForLevelup': 8,
    'brainwashCost': 50,
};

/* MALE FORMS - NOOB CLASS */
formStatsMap['Male Bystander'] = {
    'name': 'Male Bystander',
    'baseHP': 2,
    'graphic': 'Male_Bystander.jpg',
    'description': 'A regular male human, the kind that most males in the area are like.',
    'gender': 'male',
    'class': 'civilian',
    'promotesTo': ['Apprentice'],

    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,

    'levelRequiredForPromotion': 3,
    'xpNeededForLevelup': 2,
    'brainwashCost': 15,
};

formStatsMap['Apprentice'] = {
    'name': 'Apprentice',
    'baseHP': 4,
    'graphic': 'Apprentice.jpg',
    'description': 'A male possessing some basic magic.',
    'gender': 'male',
    'promotesTo': ['Mage'],
    'class': 'witch',

    'strengthAttack': 0,
    'strengthDefense': 1,
    'seductionAttack': 0,
    'seductionDefense': 2,
    'energyNeededToRecruit': 200,

    'levelRequiredForPromotion': 5,
    'xpNeededForLevelup': 5,
    'brainwashCost': 35,
};

formStatsMap['Mage'] = {
    'name': 'Mage',
    'baseHP': 8,
    'graphic': 'Mage.jpg',
    'description': 'A male human posessing some slightly more powerful magic.',
    'gender': 'male',
    'promotesTo': [],
    'class': 'witch',

    'strengthAttack': 3,
    'strengthDefense': 3,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToRecruit': 300,

    'levelRequiredForPromotion': 99,
    'xpNeededForLevelup': 8,
    'brainwashCost': 50,
};

