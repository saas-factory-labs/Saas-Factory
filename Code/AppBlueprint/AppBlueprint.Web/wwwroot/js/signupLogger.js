/**
 * Signup logger module for safe telemetry logging
 * Prevents XSS by accepting parameters instead of eval() or inline script injection
 */
const signupLogger = {
    /**
     * Logs when a workspace is successfully created
     * @param accountType - The type of account created ('personal' or 'business')
     */
    logWorkspaceCreated(accountType) {
        // Validate accountType to prevent injection
        const validAccountTypes = ['personal', 'business'];
        const sanitizedAccountType = validAccountTypes.includes(accountType.toLowerCase())
            ? accountType.toLowerCase()
            : 'unknown';
        console.log('[Signup] Workspace created', {
            accountType: sanitizedAccountType,
            timestamp: new Date().toISOString()
        });
        // Send to analytics service (example - replace with your actual analytics)
        // window.gtag?.('event', 'workspace_created', { account_type: sanitizedAccountType });
        // window.plausible?.('Workspace Created', { props: { accountType: sanitizedAccountType } });
    },
    /**
     * Logs a signup flow step
     * @param step - The step name (e.g., 'account_type_selected', 'form_submitted')
     * @param accountType - The type of account ('personal' or 'business')
     * @param metadata - Optional additional metadata
     */
    logSignupStep(step, accountType, metadata) {
        const validSteps = [
            'account_type_selected',
            'form_submitted',
            'auth_callback_received',
            'tenant_created',
            'user_created',
            'signup_complete'
        ];
        const validAccountTypes = ['personal', 'business', 'unknown'];
        const sanitizedStep = validSteps.includes(step.toLowerCase())
            ? step.toLowerCase()
            : 'custom_step';
        const sanitizedAccountType = validAccountTypes.includes(accountType.toLowerCase())
            ? accountType.toLowerCase()
            : 'unknown';
        console.log('[Signup]', sanitizedStep, {
            accountType: sanitizedAccountType,
            timestamp: new Date().toISOString(),
            metadata: metadata || {}
        });
        // Send to analytics service
        // window.gtag?.('event', 'signup_step', { 
        //     step: sanitizedStep, 
        //     account_type: sanitizedAccountType 
        // });
    }
};
globalThis.signupLogger = signupLogger;
export default signupLogger;
//# sourceMappingURL=signupLogger.js.map