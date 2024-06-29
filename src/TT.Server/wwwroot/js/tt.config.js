var ConfigModule = (function () {
    var configStore = 'tt.config';
    var config;
    var defaultConfig = {
        chat: {
            imagesEnabled: localStorage['chat_ImagesOn'] !== undefined ? localStorage['chat_ImagesOn'] : true,
            colorsDisabled: localStorage['chat_ColorsOff'] !== undefined ? localStorage['chat_ColorsOff'] : false,
            nyanEnabled: localStorage['chat_nyanOn'] !== undefined ? localStorage['chat_nyanOn'] : false,
            autoScrollEnabled: true,
            ignoreList: localStorage['chat_IgnoreList'] !== undefined ? JSON.parse(localStorage['chat_IgnoreList']) : [],
            roomConfig: {}
        }
    }

    var pub = {};

    function update(newConfig) {
        config = newConfig;

        if (config.chat.ignoreList === undefined)
            config.chat.ignoreList = [];

        pub.chat = config.chat;
    }

    function initialize() {
        if (localStorage[configStore] === undefined) {
            update(defaultConfig);
            localStorage.removeItem('chat_ImagesOn');
            localStorage.removeItem('chat_IgnoreList');
        } else
            update(JSON.parse(localStorage[configStore]));
    }

    pub.save = function () {
        config.chat = pub.chat;
        localStorage[configStore] = JSON.stringify(config);
    }

    window.addEventListener('storage', function (event) {
        if (event.key === configStore) {
            update(JSON.parse(event.newValue));
        }
    });

    initialize();

    return pub;
})();