/*
    For each of the types that spawn, the array is set like this:
    [0] -- guaranteed number of spawns
    [1] -- up to this many addition spawns
*/

var locationsMap = {};

locationsMap['Park:  Forest Trail'] = {
    'name': 'Park:  Forest Trail',
    'infamyLimit': 100,
    'btnClass': 'btn-default',
    'Descripton' : 'The park\'s forest trail is a quiet little stretch of mulch-paved paths that wind through the forest.  It\'s pretty rare to bump into groups any larger than a lone hiker or two, perfect for introducing some bystanders into why they should join up with you in your quest to become the greatest fashion witch of all... or make a comfy new dress out of them, if you feel so inclined!',

    'Female Bystander': [1,2],
    'Witchling': [0, 0],
    'Senior Witchling': [0, 0],
    'Fashion Witch': [0, 0],

};

locationsMap['Park:  Parking Lot'] = {
    'name': 'Park:  Parking Lot',
    'infamyLimit': 150,
    'btnClass': 'btn-info',
    'Descripton': 'The park\'s parking lot is just a bit more crowded than the trails and there\'s a chance you\'ll meet a fellow witch or two out here in addition to the usual clueless bystanders.  Still probably nothing you won\'t be able to take care of single-handedly.',

    'Female Bystander': [2,2],
    'Witchling': [0, 1],
    'Senior Witchling': [0, 0],
    'Fashion Witch': [0, 0],

};

locationsMap['Park:  Lodge'] = {
    'name': 'Park:  Lodge',
    'infamyLimit': 200,
    'btnClass': 'btn-success',
    'Descripton': 'The park\'s lodge is a large wooden building near the entrance where all the historical displays and maps are kept.  There\'s more often than not a small crowd here, and some of the employees just might be witches based on rumors of midnight chanting deep in the woods...',

    'Female Bystander': [4, 2],
    'Witchling': [1, 1],
    'Senior Witchling': [0, 0],
    'Fashion Witch': [0, 0],

};

locationsMap['Coffee Shop'] = {
    'name': 'Coffee Shop',
    'infamyLimit': 250,
    'btnClass': 'btn-danger',
    'Descripton': 'A normally peaceful little coffee shop, a favorite gathering place of regular bystanders and mages alike.',

    'Female Bystander': [4, 3],
    'Witchling': [2, 2],
    'Senior Witchling': [0, 1],
    'Fashion Witch': [0, 0],

};
