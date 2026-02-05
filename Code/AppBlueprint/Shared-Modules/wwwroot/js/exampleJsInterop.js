/**
 * Example JavaScript module demonstrating Blazor JS interop
 * This is a JavaScript module that is loaded on demand.
 * It can export any number of functions and may import other modules if required.
 */
/**
 * Shows a prompt dialog with the given message
 * @param message - The message to display in the prompt
 * @returns The user's input or null if cancelled
 */
export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}
//# sourceMappingURL=exampleJsInterop.js.map