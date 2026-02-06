"use strict";
/**
 * Navigation helper functions for Blazor interop
 */
const navigationHelper = {
    navigateTo(url) {
        globalThis.location.href = url;
    }
};
// Attach to window for Blazor interop
globalThis.navigationHelper = navigationHelper;
//# sourceMappingURL=navigationHelper.js.map