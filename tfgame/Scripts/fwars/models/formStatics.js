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

formStatsMap['Female Bystander'] = {
    'baseHP': 2,
    'graphic': 'Female_Bystander.jpg',
    'description': '',
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

formStatsMap['Male Bystander'] = {
    'baseHP': 2,
    'graphic': 'Male_Bystander.jpg',
    'description': '',
    'gender': 'male',
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

formStatsMap['Fashion Witch'] = {
    'baseHP': 50,
    'graphic': 'Fashion_Witch.jpg',
    'description': '',
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

formStatsMap['Witchling'] = {
    'baseHP': 4,
    'graphic': 'Witchling.jpg',
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
    'baseHP': 8,
    'graphic': 'Senior_Witchling.jpg',
    'description': '',
    'gender': 'female',
    'promotesTo': [],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 1,
    'seductionAttack': 3,
    'seductionDefense': 3,
    'energyNeededToRecruit': 300,

    'levelRequiredForPromotion': 83,
    'xpNeededForLevelup': 8,
    'brainwashCost': 50,
};