"use strict";
/**
 * Navigation helper functions for Blazor interop
 */
const navigationHelper = {
    navigateTo(url) {
        window.location.href = url;
    }
};
// Attach to window for Blazor interop
window.navigationHelper = navigationHelper;
//# sourceMappingURL=navigationHelper.js.map