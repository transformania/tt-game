var ConfigModule = (function () {
    var configStore = 'tt.config';
    var config;
    var defaultConfig = {
        chat: {
            imagesEnabled: localStorage['chat_ImagesOn'] !== undefined || localStorage['chat_ImagesOn'] === 'false' ? false : true,
            autoScrollEnabled: true,
            ignoreList: localStorage['chat_IgnoreList'] !== undefined ? JSON.parse(localStorage['chat_IgnoreList']) : [],
            roomConfig: {}
        }
    }

    var pub = {};

    function update(newConfig) {
        config = newConfig;
        pub.chat = config.chat;
    }

    function initialize() {
        if (localStorage[configStore] === undefined) {
            update(defaultConfig);
            localStorage.removeItem('chat_ImagesOn');
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