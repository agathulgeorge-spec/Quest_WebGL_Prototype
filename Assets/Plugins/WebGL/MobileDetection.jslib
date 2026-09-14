mergeInto(LibraryManager.library, {

    IsMobileBrowser: function () {

        var userAgent =
            navigator.userAgent ||
            navigator.vendor ||
            window.opera;

        if (/android/i.test(userAgent)) {
            return 1;
        }

        if (/iPhone|iPad|iPod/i.test(userAgent)) {
            return 1;
        }

        if (
            navigator.platform === 'MacIntel' &&
            navigator.maxTouchPoints &&
            navigator.maxTouchPoints > 1
        ) {
            return 1;
        }

        return 0;
    }
});