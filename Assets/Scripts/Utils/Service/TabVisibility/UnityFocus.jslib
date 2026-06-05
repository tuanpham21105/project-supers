mergeInto(LibraryManager.library, {
    RegisterVisibilityChange: function (gameObjectName, methodName) {
        var goName  = UTF8ToString(gameObjectName);
        var method  = UTF8ToString(methodName);

        document.addEventListener('visibilitychange', function () {
            var state = document.hidden ? 'hidden' : 'visible';
            SendMessage(goName, method, state);
        });

        window.addEventListener('blur', function () {
            SendMessage(goName, method, 'hidden');
        });

        window.addEventListener('focus', function () {
            SendMessage(goName, method, 'visible');
        });
    }
});