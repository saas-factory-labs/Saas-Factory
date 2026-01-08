"use strict";
/**
 * Navigation helper functions for Blazor interop
 */
Object.defineProperty(exports, "__esModule", { value: true });
const navigationHelper = {
    navigateTo(url) {
        window.location.href = url;
    }
};
window.navigationHelper = navigationHelper;
exports.default = navigationHelper;
//# sourceMappingURL=navigationHelper.js.map