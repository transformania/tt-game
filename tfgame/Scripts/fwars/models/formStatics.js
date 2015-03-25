// ----------------- FORM TYPE MAP ------------------
var formStatsMap = {};

formStatsMap['Female Bystander'] = {
    'baseHP': 2,
    'graphic': 'blurg.png',
    'gender': 'female',
    'class': 'civilian',
    'promotesTo': ['Witchling'],

    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,
    'xpNeededForLevelup': 2,
    'brainwashCost': 15,
};

formStatsMap['Male Bystander'] = {
    'baseHP': 2,
    'graphic': 'blurg.png',
    'gender': 'male',
    'class': 'civilian',
    'promotesTo': ['Witchling'],

    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,
    'xpNeededForLevelup': 2,
    'brainwashCost': 15,
};

formStatsMap['Fashion Witch'] = {
    'baseHP': 50,
    'graphic': 'blurg.png',
    'gender': 'female',
    'promotesTo': [],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 3,
    'seductionDefense': 0,
    'energyNeededToRecruit': 10000,
    'xpNeededForLevelup': 9999,
    'brainwashCost': 9999,
};

formStatsMap['Witchling'] = {
    'baseHP': 4,
    'graphic': 'blurg.png',
    'gender': 'female',
    'promotesTo': ['Senior Witchling'],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
    'energyNeededToRecruit': 200,
    'xpNeededForLevelup': 5,
    'brainwashCost': 35,
};

formStatsMap['Senior Witchling'] = {
    'baseHP': 8,
    'graphic': 'blurg.png',
    'gender': 'female',
    'promotesTo': [''],
    'class': 'witch',

    'strengthAttack': 1,
    'strengthDefense': 1,
    'seductionAttack': 3,
    'seductionDefense': 3,
    'energyNeededToRecruit': 300,
    'xpNeededForLevelup': 8,
    'brainwashCost': 50,
};