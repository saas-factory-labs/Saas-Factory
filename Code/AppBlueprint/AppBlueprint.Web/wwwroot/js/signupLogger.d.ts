/**
 * Signup logger module for safe telemetry logging
 * Prevents XSS by accepting parameters instead of eval() or inline script injection
 */
interface SignupLoggerModule {
    logWorkspaceCreated(accountType: string): void;
    logSignupStep(step: string, accountType: string, metadata?: Record<string, string>): void;
}
declare const signupLogger: SignupLoggerModule;
declare global {
    interface Window {
        signupLogger: SignupLoggerModule;
    }
}
export default signupLogger;
//# sourceMappingURL=signupLogger.d.ts.map