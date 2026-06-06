mergeInto(LibraryManager.library, {

    JS_SetCookie: function (name, value, days) {
        var nameStr  = UTF8ToString(name);
        var valueStr = UTF8ToString(value);

        var expires = "";
        if (days > 0) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }

        document.cookie = nameStr + "=" + encodeURIComponent(valueStr) + expires + "; path=/";
    },

    JS_GetCookie: function (name) {
        var nameStr = UTF8ToString(name) + "=";
        var cookies = document.cookie.split(';');

        for (var i = 0; i < cookies.length; i++) {
            var c = cookies[i].trim();
            if (c.indexOf(nameStr) === 0) {
                var val = decodeURIComponent(c.substring(nameStr.length));
                var bufferSize = lengthBytesUTF8(val) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(val, buffer, bufferSize);
                return buffer;
            }
        }

        return 0; // null — không tìm thấy
    },

    JS_RemoveCookie: function (name) {
        var nameStr = UTF8ToString(name);
        document.cookie = nameStr + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/";
    },
});