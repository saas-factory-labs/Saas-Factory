# Changelog

All notable changes to the AppBlueprint NuGet packages will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.5](https://github.com/saas-factory-labs/Saas-Factory/compare/v1.0.4...v1.0.5) (2026-03-13)


### Bug Fixes

* refine Snyk analysis workflow by removing Node.js setup and enhancing project-specific scans ([b29a035](https://github.com/saas-factory-labs/Saas-Factory/commit/b29a035c28136626a67d052fd731ad8127c696eb))
* update Snyk analysis workflow to specify working directories and correct project file paths ([4e06fa5](https://github.com/saas-factory-labs/Saas-Factory/commit/4e06fa52b2ba9ae9209d7a2c3fc34941f1ca541d))

## [1.0.4](https://github.com/saas-factory-labs/Saas-Factory/compare/v1.0.3...v1.0.4) (2026-03-13)


### Bug Fixes

* update Firebase configuration to include refresh response model and adjust Profile component namespace ([ffb2c34](https://github.com/saas-factory-labs/Saas-Factory/commit/ffb2c343145003656c92665e0acc660f4ac6c206))

## [1.0.3](https://github.com/saas-factory-labs/Saas-Factory/compare/v1.0.2...v1.0.3) (2026-03-13)


### Bug Fixes

* update Dockerfile paths to remove redundant directory references ([1c93a4f](https://github.com/saas-factory-labs/Saas-Factory/commit/1c93a4f190b08771b0757f0525cc813b5396ffa4))

## [1.0.2](https://github.com/saas-factory-labs/Saas-Factory/compare/v1.0.1...v1.0.2) (2026-03-13)


### Bug Fixes

* add context for Dockerfile paths in Snyk analysis workflow ([78c789c](https://github.com/saas-factory-labs/Saas-Factory/commit/78c789ca0c186a152269cc11d9edd40ab626beb4))

## [1.0.1](https://github.com/saas-factory-labs/Saas-Factory/compare/v1.0.0...v1.0.1) (2026-03-13)


### Bug Fixes

* remove .dockerignore content reference from project file ([d40e26b](https://github.com/saas-factory-labs/Saas-Factory/commit/d40e26b3e01c8d229287fa6dda0f833dcb3e1f00))

# 1.0.0 (2026-03-13)


### Bug Fixes

* **mudblazor:** add .NET 10 compatibility polyfill and update script loading order ([669f597](https://github.com/saas-factory-labs/Saas-Factory/commit/669f59778a2fd7379bbfa0a203d04c6659a528e6))
* **docs:** Add base href tag to RazorPress layout for GitHub Pages deployment ([f79eb11](https://github.com/saas-factory-labs/Saas-Factory/commit/f79eb11ef0876b83d0452dfa79a79ee88246b9a4))
* add clean build and verification for CSS persistence ([01b15d7](https://github.com/saas-factory-labs/Saas-Factory/commit/01b15d7a95e0a2c9813dd82e64737014d04603e5))
* add diagnostics and verification for Blazor WASM landing page deployment ([d5e6e11](https://github.com/saas-factory-labs/Saas-Factory/commit/d5e6e117820a6a1707faccdaef064433af2e15ac))
* add missing HTTP verb attributes and improve API controller consistency ([221da91](https://github.com/saas-factory-labs/Saas-Factory/commit/221da91fd5bdeb4c206970d328ae0647b16110c5))
* **signalr:** Add missing using directive for ILogger in NotificationHub ([295cb5e](https://github.com/saas-factory-labs/Saas-Factory/commit/295cb5e233c09d2de93edb1d8497997849807401))
* **docker:** Add Node.js to Web Dockerfile for TypeScript compilation ([3e4d8e7](https://github.com/saas-factory-labs/Saas-Factory/commit/3e4d8e7a8ffafc676d374af766962caf546e1334))
* **EnvironmentVariableCommand:** add platform support attributes for Windows in environment variable commands ([52cc0a5](https://github.com/saas-factory-labs/Saas-Factory/commit/52cc0a59ef17db6d18ca42804c95604fc5dd9d85))
* add railway.toml to explicitly define Dockerfile path ([de125c7](https://github.com/saas-factory-labs/Saas-Factory/commit/de125c7147eec59316ff894a347c19248c45f37b))
* **signalr:** Add WithCredentials = true to HubConnectionBuilder in NotificationBell.razor ([5096752](https://github.com/saas-factory-labs/Saas-Factory/commit/5096752c3dae39da6c20b755b623a98775d6b263))
* align Owner and Admin role assignment with comment ranges ([a3fb321](https://github.com/saas-factory-labs/Saas-Factory/commit/a3fb321999e957b9c2a64fa6b48c066ccba5e700))
* allow anonymous access for Blazor components to prevent CORS errors ([dd3a939](https://github.com/saas-factory-labs/Saas-Factory/commit/dd3a9397c95f8f51b9dad803290c06930698f643))
* Build Tailwind CSS AFTER prerender to scan generated HTML ([76403e5](https://github.com/saas-factory-labs/Saas-Factory/commit/76403e5341e020446795e3e80857a26fa3b54ee2))
* build Tailwind CSS before .NET build in deployment workflow ([fe27a3a](https://github.com/saas-factory-labs/Saas-Factory/commit/fe27a3a335f4260f26b5ad6f5c7ef4a49a0f6c3e))
* Chart.js rendering and dashboard layout improvements ([6db2a97](https://github.com/saas-factory-labs/Saas-Factory/commit/6db2a97697969d53c4d49a32d784a2e5768e54a0))
* clean up sonar-project.properties exclusions for consistency ([ed51927](https://github.com/saas-factory-labs/Saas-Factory/commit/ed519277f18a8be31e3139aac1b67a451fd88088))
* **ui:** close unclosed tags and improve placeholder formatting in various components ([8cfc896](https://github.com/saas-factory-labs/Saas-Factory/commit/8cfc89628274251bbb4c4cd7a4f5ef6cbbbe6523))
* comment out documentation link in Home.razor ([a32ac41](https://github.com/saas-factory-labs/Saas-Factory/commit/a32ac4121bfe5a2a3392913a1a36bc60d128db3f))
* comment out unused CI/CD and quality analysis badges in README ([2667f88](https://github.com/saas-factory-labs/Saas-Factory/commit/2667f885733d0dc0e04791470b94aab06453b8f7))
* conditionally configure HTTPS based on environment for Railway deployment ([e274dd3](https://github.com/saas-factory-labs/Saas-Factory/commit/e274dd35d467429b6949ab77946f450a1eda3c5a))
* configure forwarded headers for Railway HTTPS redirect URIs ([eecb2b2](https://github.com/saas-factory-labs/Saas-Factory/commit/eecb2b2576f59dc7eabf0b90f5f2bdab7412b846))
* **signalr:** Configure SignalR hubs with SignalRAnonymous policy to prevent auth redirects ([68c7fc9](https://github.com/saas-factory-labs/Saas-Factory/commit/68c7fc9e2dfdc0963f9fd68b2f5e67fc7f8a72f8))
* **teams:** correct API endpoint from /team to /teams (plural) ([6d0a2f4](https://github.com/saas-factory-labs/Saas-Factory/commit/6d0a2f4cd00b80142f1915e476e2fcd346c5032c))
* correct EF Core configurations ([34371af](https://github.com/saas-factory-labs/Saas-Factory/commit/34371af8ab004b00c2e39a2bc6d46dc76b67a7e3))
* **workflows:** correct formatting of workflow name in scorecard.yml ([28674a6](https://github.com/saas-factory-labs/Saas-Factory/commit/28674a62ad940581656564638c485c368545a7c2))
* correct workflow name in Qodana analysis configuration ([a59c1ab](https://github.com/saas-factory-labs/Saas-Factory/commit/a59c1ab57af5cffff6870e37f908ab24ce93fbad))
* deploy SaaS-Factory instance to GitHub Pages instead of API-Reference ([b6d30cb](https://github.com/saas-factory-labs/Saas-Factory/commit/b6d30cbb58fc25f9bdb07fe3df41aa66eee370cd))
* **migration:** drop identity before changing PaymentProviders.Id column type ([31a442e](https://github.com/saas-factory-labs/Saas-Factory/commit/31a442edb6152c926c598fc74a5fd79fcc48459d))
* Enhance base href handling in Error page for better deployment compatibility ([1825324](https://github.com/saas-factory-labs/Saas-Factory/commit/182532451f43134b40c3bbd9aa283913524075ea))
* enhance connection string retrieval with fallback for DATABASE_URL ([8cd7f46](https://github.com/saas-factory-labs/Saas-Factory/commit/8cd7f46a8a8ab9f4c25f21e05abdb3fb992a72d5))
* enhance CSS generation verification with file size checks ([3552d5f](https://github.com/saas-factory-labs/Saas-Factory/commit/3552d5f4a402d215813218ed1bdb5cb0a2c06924))
* **auth:** enhance error handling and user feedback for authentication issues ([731055c](https://github.com/saas-factory-labs/Saas-Factory/commit/731055c69e4a6a128430be9bee4d28d76c6062ab))
* enhance Kestrel configuration for development and production environments ([64ffbee](https://github.com/saas-factory-labs/Saas-Factory/commit/64ffbee2444887705a37820f4c54d2a25c129443))
* **auth:** enhance token saving and retrieval for API authentication ([9a8cb16](https://github.com/saas-factory-labs/Saas-Factory/commit/9a8cb16ddb15f76ccefc9fea130b1639cae4cdfe))
* ensure correct <base href> tag in generated HTML content ([24acebc](https://github.com/saas-factory-labs/Saas-Factory/commit/24acebccb255b9362a7e1c103773d80e24ea1e46))
* **arch:** fix naming convention violations to pass architecture tests ([36139b6](https://github.com/saas-factory-labs/Saas-Factory/commit/36139b64deb81baf41b9d7b809e5c29425b7bf69))
* **docs:** Fix RazorPress prerendering for GitHub Pages subdirectory deployment ([9c3a378](https://github.com/saas-factory-labs/Saas-Factory/commit/9c3a378521d23be454c010b5fbc3ee38172af9db))
* **docs:** Fix Tailwind CSS build in GitHub Actions using standalone binary ([a0bda2b](https://github.com/saas-factory-labs/Saas-Factory/commit/a0bda2b63a4c1a8264f84c97d21fc90ad32121e1))
* **ci:** Fix YAML syntax errors in workflow ([34826ca](https://github.com/saas-factory-labs/Saas-Factory/commit/34826ca9f0bcf8294ab451959685342fbc068bc6))
* Hide blazor-error-ui banner by default in Cruip layouts ([39206c8](https://github.com/saas-factory-labs/Saas-Factory/commit/39206c82499e1b748ec0787abbcf965eceed2488))
* **auth:** implement full Logto sign-out flow and update sign-out logic ([885d1ac](https://github.com/saas-factory-labs/Saas-Factory/commit/885d1ac4dc502b2411a9026ea57afd673591692e))
* implement proper tenant context in ApiKeyController.CreateApiKey ([e84c958](https://github.com/saas-factory-labs/Saas-Factory/commit/e84c958aa0264b7437a2e3e0d6f0ef76d546673f))
* improve BaseHref handling in HTML post-processing ([47f973f](https://github.com/saas-factory-labs/Saas-Factory/commit/47f973f81880445cf31c1abacdb66ea539ed158f))
* **auth:** increase OIDC backchannel timeout and enhance error handling for Railway ([161bfc9](https://github.com/saas-factory-labs/Saas-Factory/commit/161bfc95c69287648c02724d684ab2128dec2719))
* **docs:** Load dynamic sidebar in Index.cshtml to show all navigation sections ([c07ea6d](https://github.com/saas-factory-labs/Saas-Factory/commit/c07ea6d686b9f53fe47005dad965b20b87dab59e))
* make AuthDebugController and AuthTestController internal ([ebf1145](https://github.com/saas-factory-labs/Saas-Factory/commit/ebf1145cf879a5af26e4f67c7cb1da46bffb3fc2))
* **auth:** make IUserService optional in ForgotPassword page ([391d404](https://github.com/saas-factory-labs/Saas-Factory/commit/391d404d7bd77e9e0f05f88f9209d5755a5ee2e8))
* **auth:** make Logto authentication optional for Railway deployment ([08951c9](https://github.com/saas-factory-labs/Saas-Factory/commit/08951c90c2d451d3c3e413ad99faa787ea0ddb75))
* Migrate test project to TUnit/Moq and remove MudBlazor dependencies ([27fbfa3](https://github.com/saas-factory-labs/Saas-Factory/commit/27fbfa390d45b764164b1a4a19ff2271f915b0f7))
* **signalr:** Pass access token to SignalR HubConnectionBuilder in NotificationBell.razor ([3131c82](https://github.com/saas-factory-labs/Saas-Factory/commit/3131c82df7c8a81363209841b696be9d8ef8016c))
* **logging:** pass exception objects to logging methods in catch blocks ([28358ae](https://github.com/saas-factory-labs/Saas-Factory/commit/28358aee880d21fe891e30f41d485ddb7173ca81))
* Post-process HTML files to convert absolute paths to relative based on BaseHref ([5372489](https://github.com/saas-factory-labs/Saas-Factory/commit/537248978e01633948142bda4c5dea18899daa9e))
* reference owner foreign key in team invite ([6fe37a5](https://github.com/saas-factory-labs/Saas-Factory/commit/6fe37a5ae48c6267df1cde2b3f0a21f00bc3b18c))
* Remove broken chart components with Blazorise dependencies ([2524aad](https://github.com/saas-factory-labs/Saas-Factory/commit/2524aad0b5794d458e065d15172880e3d81a1a38))
* Remove invalid [@theme](https://github.com/theme) directive from Tailwind CSS input ([ec91b07](https://github.com/saas-factory-labs/Saas-Factory/commit/ec91b07d91ce884481ac2a2b4c981bd82bf4376c))
* remove Kestrel port configuration conflicts for Railway deployment ([af59b11](https://github.com/saas-factory-labs/Saas-Factory/commit/af59b11833f862b37cb5b61e9eb072596f024912))
* **tests:** Remove MudBlazor/NSubstitute/Xunit, migrate to Moq/TUnit ([fdaafe8](https://github.com/saas-factory-labs/Saas-Factory/commit/fdaafe8b8bc8447b6dec9623c9ae9d740d365039))
* **nuget:** Remove package icons to resolve NuGet.org publishing error ([eebae00](https://github.com/saas-factory-labs/Saas-Factory/commit/eebae00e1e76103b5f1e87efd446ed5c905c70f1))
* remove redundant condition from release job in auto-release workflow ([36a7824](https://github.com/saas-factory-labs/Saas-Factory/commit/36a782454223601eb9128e64d83a8f25ad3de1ca))
* remove redundant default value initializations in UiKit components ([8246b91](https://github.com/saas-factory-labs/Saas-Factory/commit/8246b91d42379c2327cf89ba1b0b15260c6376a9))
* remove redundant qodana.projectId from settings.json and add missing using directive in Profile.razor ([a62e373](https://github.com/saas-factory-labs/Saas-Factory/commit/a62e37382d4483eb6689285862ed684639685349))
* remove redundant token setup in schema documentation workflow ([e90b3c9](https://github.com/saas-factory-labs/Saas-Factory/commit/e90b3c965141e735fd03ed8ddb1cf2fbe330d248))
* **workflows:** remove unnecessary SONAR_TOKEN checks in SonarCloud analysis steps ([2ec270f](https://github.com/saas-factory-labs/Saas-Factory/commit/2ec270fd0363c85be7356353bcd5564dcef1cfe6))
* remove unused lite-yt-embed.js script from DocsPage ([4926a06](https://github.com/saas-factory-labs/Saas-Factory/commit/4926a06f9566edc0171236b6f582b59eed393a35))
* remove unused logger and configuration fields from UserController ([3e6f870](https://github.com/saas-factory-labs/Saas-Factory/commit/3e6f870dce14ee5077f3b7ce8cacf906ecfe0bcf))
* rename GetUser to GetUserEndpoint in AuthTestController ([2ad3979](https://github.com/saas-factory-labs/Saas-Factory/commit/2ad39793b3f2bd6ec9679cdc8e7a137176b4b098))
* replace new List<T>() and array initializers with collection expressions ([73f808a](https://github.com/saas-factory-labs/Saas-Factory/commit/73f808a28f1a183b9bcb489f4a965d27439deb50))
* Replace string URLs with Uri objects in HttpClient methods (CA2234) ([a25afea](https://github.com/saas-factory-labs/Saas-Factory/commit/a25afeaa03864045e9cf637519a2da734da8a1a9))
* **JwtTokenCommand:** replace string.IsNullOrEmpty checks with string.IsNullOrWhiteSpace for token validation ([2bd08f7](https://github.com/saas-factory-labs/Saas-Factory/commit/2bd08f7a5fa46cb4206bb4bf7d57356def790489))
* resolve API service dependency injection and routing configuration ([c2927e7](https://github.com/saas-factory-labs/Saas-Factory/commit/c2927e7aa0aeb5f7be7fe22be4e5f2bc333813b4))
* resolve Aspire dashboard memory exhaustion causing crashes ([d926531](https://github.com/saas-factory-labs/Saas-Factory/commit/d92653100c93965ee70e9a25e49ec5f67a582b65))
* **code-quality:** resolve CA1054 warnings - change string URL parameters to Uri types ([d62b2b4](https://github.com/saas-factory-labs/Saas-Factory/commit/d62b2b41280e7a0cda1f4ad26c76fc8f154b5bdc))
* **notifications:** Resolve compilation errors after Firebase refactor ([0c2aeb0](https://github.com/saas-factory-labs/Saas-Factory/commit/0c2aeb0fd9484832f4626fb811f032344a7e8bf4))
* resolve compilation warnings in AppHost and ApiService ([6de943f](https://github.com/saas-factory-labs/Saas-Factory/commit/6de943f7625993902e5c46e7a69a40f769f943d0))
* **auth:** resolve conflicts with Login component and enhance backchannel timeout for Railway ([0166879](https://github.com/saas-factory-labs/Saas-Factory/commit/0166879ae1c01ee54d31f42a2812a8ce1f0a10e5))
* resolve database migration and seeding issues ([032c8c1](https://github.com/saas-factory-labs/Saas-Factory/commit/032c8c117ce50a5a36020402fad001e398dfac74))
* resolve EF Core entity tracking conflict during database seeding ([84e9921](https://github.com/saas-factory-labs/Saas-Factory/commit/84e99217c480dee9ba2b507f25c03438e74bf6e5))
* **auth:** resolve Logto sign-out and API authentication issues ([dab0c3c](https://github.com/saas-factory-labs/Saas-Factory/commit/dab0c3c97dfe94199f45024bb6fdf48fb1578b51))
* restore AppHost build by removing Partials refs and repairing SeedTest/AppHost helpers ([909709f](https://github.com/saas-factory-labs/Saas-Factory/commit/909709fbdf3b19021d3876cc5e195e13d2bbf796))
* **signalr:** Revert IAccessTokenProvider usage and remove Authorize attribute from NotificationHub ([55a5ae3](https://github.com/saas-factory-labs/Saas-Factory/commit/55a5ae35db01b9bd7d2fe673f12c3f3176eeee9f))
* **signalr:** Revert WithCredentials = true from HubConnectionBuilder in NotificationBell.razor ([db34767](https://github.com/saas-factory-labs/Saas-Factory/commit/db34767f4c94670c11f680d98c567361e1098ab4))
* **config:** set API_BASE_URL to correct port 8091 in launchSettings ([a0757d1](https://github.com/saas-factory-labs/Saas-Factory/commit/a0757d1e498d2e11448b9cb484497cbca9ee8bf7))
* streamline git configuration in schema push step and remove redundant token setup ([bb2b726](https://github.com/saas-factory-labs/Saas-Factory/commit/bb2b72666138c79d5dae8d4faff4d3a1b924ba3b))
* suppress LocalizableElement warnings for non-localizable diagnostic strings ([23de04d](https://github.com/saas-factory-labs/Saas-Factory/commit/23de04d0a384414bf43eeafa6380faf538faeb33))
* **docker:** Switch to standard ASP.NET images for ICU support ([2d30639](https://github.com/saas-factory-labs/Saas-Factory/commit/2d30639753195fbdeeca79dccd8bb15bbed2e276))
* **ci:** update actions/cache to non-deprecated v4.2.0 in SonarCloud workflow ([a8f51f1](https://github.com/saas-factory-labs/Saas-Factory/commit/a8f51f1ecd11e748a3e489eea7dd57205b57595c))
* update Aspire SDK and package versions to 13.1.2 ([f720f0c](https://github.com/saas-factory-labs/Saas-Factory/commit/f720f0ca5d8bd037cdf9cd5b18db9f296f3295c1))
* Update BaseHref handling in RazorPress configuration and layout ([460a3a1](https://github.com/saas-factory-labs/Saas-Factory/commit/460a3a1a7a7cdd83d536af34b26385cb38c54e6f))
* **workflow:** Update branches for GitHub Pages deployment trigger ([51c6e4b](https://github.com/saas-factory-labs/Saas-Factory/commit/51c6e4bb525dae666590af7d9fb0f15a6ef31025))
* update cookie SameSite and SecurePolicy settings for production OAuth support ([994ea8c](https://github.com/saas-factory-labs/Saas-Factory/commit/994ea8cb6fc76f2490b61b05104b9b754365c176))
* update copyright year in LICENSE and disable publish_results in scorecard.yml ([8123cfc](https://github.com/saas-factory-labs/Saas-Factory/commit/8123cfc25a134da001eb7044976031f8b14ce652))
* update deployment workflow to build RazorPress documentation and remove Writerside references ([22b63eb](https://github.com/saas-factory-labs/Saas-Factory/commit/22b63ebdafd62848aed6d6829405b0a7f1aa30a0))
* update Docker Scout action to run only on pull requests ([3ce70d7](https://github.com/saas-factory-labs/Saas-Factory/commit/3ce70d70aae29025635f838e5430a795b63990bb))
* **workflows:** update Docker Scout and SonarCloud analysis conditions for better security checks ([4d95262](https://github.com/saas-factory-labs/Saas-Factory/commit/4d95262c98755a85a60ec57a7b6581f0ae1d0e4c))
* update Dockerfile for runtime port handling and improve SQL function parameter naming ([f9575fb](https://github.com/saas-factory-labs/Saas-Factory/commit/f9575fbb56f2595fa76c83a351604c00be9b2c5c))
* **authentication:** update error logging to use asynchronous methods in UserAuthenticationProviderAdapter ([d89b36c](https://github.com/saas-factory-labs/Saas-Factory/commit/d89b36c48e1d170010a94ce8e369409fc7475400))
* update GITHUB_TOKEN to use SEMANTIC_RELEASE_PAT for semantic-release ([d7a996f](https://github.com/saas-factory-labs/Saas-Factory/commit/d7a996fc9c4c803bd22bcd7b78f0989f0b89be7a))
* update permissions and add token to PR creation step in SQL schema documentation workflow ([32ddc38](https://github.com/saas-factory-labs/Saas-Factory/commit/32ddc389a6f6c647f9dbf78bb311c5f5894e02d3))
* update project structure and module names in README ([dc2a5e9](https://github.com/saas-factory-labs/Saas-Factory/commit/dc2a5e9e1bdfb68b54ac5953a8f67e1d4fe44345))
* **workflows:** update SonarCloud analysis conditions to ensure SONAR_TOKEN is checked correctly ([f31a2fb](https://github.com/saas-factory-labs/Saas-Factory/commit/f31a2fbcdba91f829aa63d9ba0c110e47249c960))
* **workflow:** Update symlink removal command to handle directories ([6ed9062](https://github.com/saas-factory-labs/Saas-Factory/commit/6ed9062de2c343023043186c7a89a6b73232a255))
* update Tailwind content paths for reliable CSS generation in CI ([bf48ca6](https://github.com/saas-factory-labs/Saas-Factory/commit/bf48ca6f4ea078464d258c1e8beb836d9cc978ca))
* Update Tailwind content paths to include prerendered dist files ([1dcadfe](https://github.com/saas-factory-labs/Saas-Factory/commit/1dcadfe2f042b37e94ef98ead38a5a6ad1c72086))
* **diagnostics:** update TodoPage diagnostics for cookie-based auth ([3071887](https://github.com/saas-factory-labs/Saas-Factory/commit/3071887e8f536aa5b1702136553c2ed57d63ecbe))
* upgrade CommunityToolkit.Aspire.Hosting.PostgreSQL.Extensions to stable version ([2ca745e](https://github.com/saas-factory-labs/Saas-Factory/commit/2ca745e51978592c16f35655a330acfdd041a258))
* **signalr:** Use AllowAnonymous() directly on SignalR hub mappings and remove SignalRAnonymous policy ([65dd70b](https://github.com/saas-factory-labs/Saas-Factory/commit/65dd70bf13ea8fae82e2827ed381258cb8df6a1a))
* **docs:** Use custom Index.cshtml with direct HTML rendering instead of markdown ([f2304dc](https://github.com/saas-factory-labs/Saas-Factory/commit/f2304dcf1bcb97a4513f1f8408f46817551044e0))


### Code Refactoring

* **tenancy:** Move TenantEntity to Baseline with B2C/B2B discriminator ([a1f9815](https://github.com/saas-factory-labs/Saas-Factory/commit/a1f9815df2d412e04368f4b98577e68f52528fc9))


### Features

* **Infrastructure:** add authentication setup to AddAppBlueprintInfrastructure ([dabfc64](https://github.com/saas-factory-labs/Saas-Factory/commit/dabfc642443bfdd2fe17887f0d73fc36c664a117))
* add Azimutt database analysis workflow ([2523929](https://github.com/saas-factory-labs/Saas-Factory/commit/2523929bc6bdc05ce1291daebf31a92260bb549e))
* add CodeQL advanced analysis workflow for enhanced code quality ([55c018e](https://github.com/saas-factory-labs/Saas-Factory/commit/55c018ed888239954821e881de257101b344121b))
* add CodeQL analysis workflow for enhanced code quality and security scanning ([32b74f7](https://github.com/saas-factory-labs/Saas-Factory/commit/32b74f7d5c884dee7b5c9c32119da22236cd53cf))
* add CodeQL and Snyk analysis workflows for enhanced security scanning ([f0779a9](https://github.com/saas-factory-labs/Saas-Factory/commit/f0779a95e2ba91527545e9d6bb0ababa8f7d9a45))
* Add comprehensive authentication provider configuration and security best practices documentation ([b375a0c](https://github.com/saas-factory-labs/Saas-Factory/commit/b375a0c070fe2bde33a866bf7fba01b1a31acb00))
* **nuget:** add comprehensive customization patterns and documentation for package consumption ([f41a26f](https://github.com/saas-factory-labs/Saas-Factory/commit/f41a26f5951a0bf40ccc345e366410994baa33f1))
* add comprehensive documentation for Railway Cloud deployment ([b97fcbd](https://github.com/saas-factory-labs/Saas-Factory/commit/b97fcbdf2b4bf91ab47afaf1ef203fa9d5a8a83d))
* Add comprehensive Railway deployment documentation and configuration ([bd0c27c](https://github.com/saas-factory-labs/Saas-Factory/commit/bd0c27cb7d38e8f0037f4c3f7a7d51538d63d24f))
* add comprehensive Railway deployment guide and environment variable reference ([9b86226](https://github.com/saas-factory-labs/Saas-Factory/commit/9b86226b83caf6c9a1f71dbc3710f2dd2d955370))
* **infrastructure:** add configuration validation with helpful error messages ([c3bb651](https://github.com/saas-factory-labs/Saas-Factory/commit/c3bb651589b7efb819da92d97a49cf0562049471))
* add contributor section for hornvieh3u in README.md ([be6f5c5](https://github.com/saas-factory-labs/Saas-Factory/commit/be6f5c5553d0bd872915e141724fc684f9839130))
* Add documentation for RazorPress content directory setup and local GitHub Actions execution ([890a96e](https://github.com/saas-factory-labs/Saas-Factory/commit/890a96e6a6b542edf31238ba6be52f94c93f4ee6))
* add documentation links and disable old workflows ([cc38784](https://github.com/saas-factory-labs/Saas-Factory/commit/cc38784bd0a5b105c2bbf05486e2d1db8536afc0))
* Add easy authentication middleware setup for rapid integration ([14658db](https://github.com/saas-factory-labs/Saas-Factory/commit/14658dbceeef3d3d2093e474c5f162fa70cb65d6))
* add environment variable support for API base URL in Program.cs ([b8623aa](https://github.com/saas-factory-labs/Saas-Factory/commit/b8623aad4d2b429b40c230eed8acb82c8ff59557))
* **infrastructure:** add external service registrations and package changelogs ([4ee9b97](https://github.com/saas-factory-labs/Saas-Factory/commit/4ee9b97300570af291565ac5b80408f2e76ebb66))
* Add ExternalAuthId to User and implement CurrentTenantService ([f43247b](https://github.com/saas-factory-labs/Saas-Factory/commit/f43247bd4a0519fe18bbac3ae2d9e718e7f22f2a))
* **validation:** add FluentValidation rules for Team and Role request DTOs ([83f3442](https://github.com/saas-factory-labs/Saas-Factory/commit/83f34427ee634eccd6ace76e37ef12f7e0bff51f))
* Add foundational infrastructure components including PII detection and auditing, database context configuration, and a Luhn validator. ([3668c2b](https://github.com/saas-factory-labs/Saas-Factory/commit/3668c2bf149bdc34ceac4a0df6777ba40b4eca62))
* **tests:** add framework dependency and design rule architecture tests ([62ab532](https://github.com/saas-factory-labs/Saas-Factory/commit/62ab532d74ce349b5281ebc8c334e97c2f13db92))
* add integration tests and DeploymentManager documentation for admin tenant access ([e61a5d3](https://github.com/saas-factory-labs/Saas-Factory/commit/e61a5d391f25613766e88c35bc973cf50e98467a))
* Add NuGet package icons for all 8 modules ([20b2bad](https://github.com/saas-factory-labs/Saas-Factory/commit/20b2bad9a0a5856dede25d8aa563590d060d5bd4))
* **ui:** add Onboarding section with 4-step wizard ([1101883](https://github.com/saas-factory-labs/Saas-Factory/commit/1101883235b39ef1a99f36742cc82a3b0c0ae222))
* add Qodana analysis workflow for code quality checks ([d5fe940](https://github.com/saas-factory-labs/Saas-Factory/commit/d5fe940b293d8a1acb00b1cf7605af4954962546))
* add README files and update project metadata for NuGet packages ([6795e17](https://github.com/saas-factory-labs/Saas-Factory/commit/6795e17047d9951b45d7b3664d4775d73894ab4e))
* Add real-time chat feature with tenant isolation and conversation mode ([8782d26](https://github.com/saas-factory-labs/Saas-Factory/commit/8782d2678a0edf99ccf8e0d27b5555cd1cb6a08b))
* **roles:** add Roles Management CRUD page with full functionality ([4da19f6](https://github.com/saas-factory-labs/Saas-Factory/commit/4da19f6bc553006c13fd739ecc8b6cafd9ce86a5))
* add Row-Level Security setup script and Visual Studio solution files for AppBlueprint projects; update README with key features and improvements ([2c24f8b](https://github.com/saas-factory-labs/Saas-Factory/commit/2c24f8b8feab12a37815a6aaa3c54609abd95383))
* Add service registration extensions for AppBlueprint packages ([b349a0c](https://github.com/saas-factory-labs/Saas-Factory/commit/b349a0c537d26e34cf3ccd32ae4bcd1980b7b6ab))
* **teams:** add Teams Management CRUD page with full functionality ([7b40bfb](https://github.com/saas-factory-labs/Saas-Factory/commit/7b40bfb39f47b8dff0bb0f1f77c1f3d246fd6a82))
* add Typesense search component and configuration ([f90e9aa](https://github.com/saas-factory-labs/Saas-Factory/commit/f90e9aa6b5c916f28909606f447f945a827cb84a))
* Add unified ProductCard component with customizable features, badges, and actions ([5ff64a9](https://github.com/saas-factory-labs/Saas-Factory/commit/5ff64a9f8553ffeff254e69a22b87551941514ec))
* Apply GDPR data classification attributes directly on entity properties ([26944a6](https://github.com/saas-factory-labs/Saas-Factory/commit/26944a6b6ba5edfacd683c55b55f615aeaec5cc7))
* auth improvements - Firebase provider, identity mapping, bug fixes ([13707ba](https://github.com/saas-factory-labs/Saas-Factory/commit/13707ba416c82b618552a44fb5980522751e2072))
* Complete EF Core entity configurations cleanup and standardization ([5b6785b](https://github.com/saas-factory-labs/Saas-Factory/commit/5b6785b44aec58384f6ef92980fa109cf8561c70))
* Docker local dev, EF migration, Stripe webhook security ([9ac2706](https://github.com/saas-factory-labs/Saas-Factory/commit/9ac2706b63f551f0770ad14bf3949caf39db655d))
* **ef-core:** eliminate redundant TenantUser entities and optimize schema ([5d303bb](https://github.com/saas-factory-labs/Saas-Factory/commit/5d303bb0967c179f313947d668b522deafb298eb))
* **ef-core:** eliminate redundant TenantUser entities and optimize schema ([f244db8](https://github.com/saas-factory-labs/Saas-Factory/commit/f244db88db2d0cf43e329d224d59f91fbd660410))
* enhance authentication providers and improve Firebase integration ([bd91a51](https://github.com/saas-factory-labs/Saas-Factory/commit/bd91a517ca316c99565652960188587bc3fb6b39))
* Enhance AuthenticationController with email validation and null checks ([77f934e](https://github.com/saas-factory-labs/Saas-Factory/commit/77f934ea55d7870fcc86193e2ec01155484b6997))
* enhance Azimutt database analysis workflow with automated PR creation and report updates ([d38a774](https://github.com/saas-factory-labs/Saas-Factory/commit/d38a7740ea30eba3284e56493498ddf44ebbe518))
* enhance dark mode support in Typesense component ([5881ce9](https://github.com/saas-factory-labs/Saas-Factory/commit/5881ce97af9bc4d3082f3d105b03daab45aca480))
* **home:** enhance Getting Started section with carousel navigation and improved layout ([69e3501](https://github.com/saas-factory-labs/Saas-Factory/commit/69e3501f673e4518c8f48bab8cb5e08652258165))
* Enhance GitHub workflows with improved naming conventions and security features ([6f5d7ec](https://github.com/saas-factory-labs/Saas-Factory/commit/6f5d7ec1df4a6e0f2c21dac420cedf6384d2cb98))
* enhance health checks with PostgreSQL connection string normalization and RLS validation; add Row-Level Security setup migration script ([c1f5aed](https://github.com/saas-factory-labs/Saas-Factory/commit/c1f5aed5ffe15f00146c6b3805bfda32423abd86))
* **home:** enhance homepage with update notification and improved layout ([a3121ed](https://github.com/saas-factory-labs/Saas-Factory/commit/a3121ed3231f7b478222e69f575cd74647b24ca7))
* enhance multi-tenancy security by enforcing JWT claims for tenant resolution and implementing Row-Level Security health checks ([0cf496a](https://github.com/saas-factory-labs/Saas-Factory/commit/0cf496a8c9bd2a745c86ef56e9e9b27cc3603641))
* Enhance sidebar functionality with expand/collapse management ([743d4f1](https://github.com/saas-factory-labs/Saas-Factory/commit/743d4f1694d2a69d539169568d43482dc475d585))
* enhance Snyk analysis workflows for comprehensive vulnerability scanning ([82a8eae](https://github.com/saas-factory-labs/Saas-Factory/commit/82a8eaed55ae6be49473f8c6bd45a79e3fd032bc))
* enhance SQL schema documentation workflow to create PR with automated updates and improved commit message ([e36ddd3](https://github.com/saas-factory-labs/Saas-Factory/commit/e36ddd36d02c096a01e35bcc36e6f09806468572))
* ensure release job only runs on successful workflow completion ([f3bfebf](https://github.com/saas-factory-labs/Saas-Factory/commit/f3bfebf5976e95a1e9d3615aedccfb6de14a4975))
* expose Swagger UI endpoint in Aspire dashboard and enhance API documentation access ([13e276b](https://github.com/saas-factory-labs/Saas-Factory/commit/13e276b9dfc7a59c7b73562bf6fb6bb3c2a39dcb))
* Fix Clean Architecture violations and compilation errors ([51b362a](https://github.com/saas-factory-labs/Saas-Factory/commit/51b362a7a5a73738c07dfcd3550eac3c05ee3a71))
* **auth:** implement API key authentication scheme ([864477d](https://github.com/saas-factory-labs/Saas-Factory/commit/864477d9238a19bbf4087a7c51171760e2393383))
* implement Azimutt database analysis report generation and update workflow ([6421b06](https://github.com/saas-factory-labs/Saas-Factory/commit/6421b06c63ce379e3532b39de878cb391fffd7ce))
* implement combined deployment for landing page and documentation ([2563776](https://github.com/saas-factory-labs/Saas-Factory/commit/25637769355729c42d010f77f16ee52692a4dc81))
* Implement comprehensive notification services, PII scanning, and full-text search with associated database migrations and UI components. ([a033022](https://github.com/saas-factory-labs/Saas-Factory/commit/a033022e11166a3279d3d55f4121282b8af8604b))
* implement Configuration Externalization with IOptions<T> pattern ([88c2c57](https://github.com/saas-factory-labs/Saas-Factory/commit/88c2c574854b832858ff3451f7a979541c8c448c)), closes [#5](https://github.com/saas-factory-labs/Saas-Factory/issues/5)
* implement connection string password validation and normalization across multiple components ([e1b3c60](https://github.com/saas-factory-labs/Saas-Factory/commit/e1b3c60a06158d2f7f19954e019b451ab64e0dff))
* implement defense-in-depth tenant isolation with Named Query Filters + RLS ([085882f](https://github.com/saas-factory-labs/Saas-Factory/commit/085882f275c974697aff41ae0618ce10b177a2d9))
* **cli:** implement environment variable management ([48106ac](https://github.com/saas-factory-labs/Saas-Factory/commit/48106aca0bf607b16a7ab19b593f9803296bcb2c))
* Implement flexible database context configuration and provide comprehensive examples ([42ff661](https://github.com/saas-factory-labs/Saas-Factory/commit/42ff661cfea5fda422573c270bdf85734b15c57c))
* Implement onboarding and onboarding complete pages with account setup functionality ([b8ba88a](https://github.com/saas-factory-labs/Saas-Factory/commit/b8ba88a8780c736d6beb9435c03f2af2b65b69db))
* **webhooks:** implement outbound webhook delivery ([ff6bcfe](https://github.com/saas-factory-labs/Saas-Factory/commit/ff6bcfed399d226696a1c639c7cfbe3897d9a4ad))
* **infrastructure:** implement PaymentProvider seeding with BaseEntity ([c0fec0f](https://github.com/saas-factory-labs/Saas-Factory/commit/c0fec0fb6d0015a371eaa740a6063459424b02b6))
* implement Railway deployment configuration and Dockerfile updates ([0d326e6](https://github.com/saas-factory-labs/Saas-Factory/commit/0d326e60922ee31f3db3cacd23b439af03f1e3bd))
* implement read-only admin tenant access with defense-in-depth ([9f61f8e](https://github.com/saas-factory-labs/Saas-Factory/commit/9f61f8ef86629c43173e94fbbc1c7f711aed84e2))
* **infrastructure:** implement SeedUserRolesAsync for database seeding ([1874a73](https://github.com/saas-factory-labs/Saas-Factory/commit/1874a73a26978a723fa7fc442b656f06684ec7df))
* **infrastructure:** implement SeedUserRolesAsync for user-role assignments ([d5ebc2e](https://github.com/saas-factory-labs/Saas-Factory/commit/d5ebc2ea76d07d0470711abb49114db582553297))
* implement shared database multi-tenancy with PostgreSQL RLS ([29640cd](https://github.com/saas-factory-labs/Saas-Factory/commit/29640cd38b7be1b05156415bc1ca97243df43dd2))
* **webhooks:** Implement Stripe webhook processing with signature verification, event storage, and retry logic ([92e835a](https://github.com/saas-factory-labs/Saas-Factory/commit/92e835a7d8acf14e105421e54f7d3df1c5353229))
* Initialize Landing Page with Tailwind CSS and custom styles ([6bce716](https://github.com/saas-factory-labs/Saas-Factory/commit/6bce71638a7cd3bd84dfb5916247169ce3a71863))
* Integrate FluentRegex for improved regex handling and add PII type registry for GDPR compliance ([3181e6f](https://github.com/saas-factory-labs/Saas-Factory/commit/3181e6f39245b689d1b3b68b74d27bd4801d3169))
* Migrate Analytics dashboard from CruipBlazor template to active components ([89c3e35](https://github.com/saas-factory-labs/Saas-Factory/commit/89c3e3519432f2fbf7491dba936c72594fd88b5f))
* **UiKit:** Migrate Analytics dashboard from CruipBlazor template ([1dc0bc6](https://github.com/saas-factory-labs/Saas-Factory/commit/1dc0bc66c72b534ffa403cec8536d208d43f4976))
* **UiKit:** Migrate Fintech dashboard from CruipBlazor template ([fa448d3](https://github.com/saas-factory-labs/Saas-Factory/commit/fa448d34b643c09b019c04eb3cc45ad32502aaf8))
* **job-board:** migrate job board section from CruipBlazor template ([e4e9f27](https://github.com/saas-factory-labs/Saas-Factory/commit/e4e9f27a59191cfef82c0bca608c6463d0e2a5a9))
* **notifications:** Refactor Firebase and clean up SignalR ([0e16fb0](https://github.com/saas-factory-labs/Saas-Factory/commit/0e16fb050691a995d34502b37cc8fbeea1f5cc5d))
* refactor validation logic in user service and Firebase config controller for clarity ([7a9cc7f](https://github.com/saas-factory-labs/Saas-Factory/commit/7a9cc7f704f4cf32b43266034e06fb016aa731a5))
* **ui:** restore Auth section with Blazor form component support ([6f56184](https://github.com/saas-factory-labs/Saas-Factory/commit/6f56184ff73555af531ff6e0f24826931b8418a7))
* Standardize all entities to use BaseEntity with ULID string IDs and tenant scoping ([e739100](https://github.com/saas-factory-labs/Saas-Factory/commit/e739100b849e38b78fd6dc5cafd5d45631f47405))
* **payment,subscription:** uncomment and finalize Payment and Subscription controllers ([b3b5b3f](https://github.com/saas-factory-labs/Saas-Factory/commit/b3b5b3f4020236e1ebb9099158275d4432e8547a))
* update auto-release workflow to trigger on successful CodeQL analysis completion ([2a93d3b](https://github.com/saas-factory-labs/Saas-Factory/commit/2a93d3bec2878706e4e2db27b100ad72b94c7a8c))
* update DB_URL in Docker Compose for correct database connection ([ef81f3d](https://github.com/saas-factory-labs/Saas-Factory/commit/ef81f3d417e14ab1071ef41ced5097c0dd8e95d3))
* Update Developer CLI with new RouteCommand and RouteScanner functionality ([bb56efa](https://github.com/saas-factory-labs/Saas-Factory/commit/bb56efa1a03bfd18e085d26347b04ca925d47141))
* **home:** update feature highlights and add new feature cards ([49f390b](https://github.com/saas-factory-labs/Saas-Factory/commit/49f390bf76d0de9b401c0e53249132d0a552e303))
* update Firebase response property names for consistency and clarity ([6591185](https://github.com/saas-factory-labs/Saas-Factory/commit/6591185b291f7eec1007e4788288a9f085190f30))
* update Qodana workflow name and runner environment ([93e5064](https://github.com/saas-factory-labs/Saas-Factory/commit/93e50648875a41d409746b1c62e466b7bfd074c2))
* Update role-based access control to use DeploymentManagerAdmin role and enhance error messages across controllers and fix security bugs ([1e1fa5d](https://github.com/saas-factory-labs/Saas-Factory/commit/1e1fa5d327e09970f194fd2ef83c88ed4b1d197c))
* update workflow name to reflect Qodana usage ([b7f791d](https://github.com/saas-factory-labs/Saas-Factory/commit/b7f791db323660fcdf15c0ccd740ca40fcae510d))
* upgrade Qodana workflow to use a more powerful runner ([4c45ff6](https://github.com/saas-factory-labs/Saas-Factory/commit/4c45ff62c5ab125e18619ef3dae131fa16009252))


### Performance Improvements

* Use AsSpan instead of Substring for string operations (CA1845) ([ae1dabf](https://github.com/saas-factory-labs/Saas-Factory/commit/ae1dabf117260b9ee89c71af299bb5396a29b977))


### BREAKING CHANGES

* **code-quality:** Method signatures changed in IMultiChannelNotificationService, IFileStorageService, and related records
* SetupRowLevelSecurity.sql must be applied to existing databases
* **tenancy:** Unified tenant model following Microsoft's multi-tenancy patterns

## Architecture Changes
- Moved TenantEntity from B2B namespace to Baseline namespace
- Added TenantType enum discriminator (Personal=0, Organization=1)
- Made B2B-specific fields nullable (VatNumber, Country, ContactPersons)
- Created TenantFactory service for creating tenants with clear factory methods
- Removed TenantEntity.Teams navigation property (unidirectional relationship)

## Files Changed

### Created
- `SharedKernel/Enums/TenantType.cs` - Discriminator enum for tenant types
- `Infrastructure/.../Baseline/Entities/Tenant/TenantEntity.cs` - Unified tenant entity
- `Infrastructure/.../Baseline/Entities/Tenant/TenantEntityConfiguration.cs` - EF Core config
- `Infrastructure/.../Baseline/Entities/Tenant/TenantFactory.cs` - Factory service
- `Infrastructure/.../Baseline/Partials/BaselineDBContext.Tenants.cs` - DbContext partial
- `Migrations/20251223025112_AddTenantTypeDiscriminator.cs` - Database migration
- `docs/architecture/B2C-B2B-TENANT-REFACTORING.md` - 11,000+ line ADR
- `docs/architecture/SAAS-APP-TYPES-GUIDE.md` - SaaS app types guide

### Modified
- `Infrastructure/DatabaseContexts/Baseline/BaselineDBContext.cs` - Added Tenants DbSet
- `Infrastructure/DatabaseContexts/Baseline/Entities/User/UserEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/B2B/Entities/Team/Team/TeamEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/B2B/Entities/Team/Team/TeamEntityConfiguration.cs` - Removed WithMany
- `Infrastructure/DatabaseContexts/Baseline/Entities/ContactPerson/ContactPersonEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Customer/CustomerEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Addressing/Address/AddressEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Auditing/AuditLog/AuditLogEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Billing/Subscription/SubscriptionEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Email/EmailAddress/EmailAddressEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/Email/EmailEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/Baseline/Entities/PhoneNumberEntity.cs` - Updated namespace
- `Infrastructure/DatabaseContexts/TenantCatalog/CatalogDBContext.cs` - Updated namespace
- `Infrastructure/Repositories/Interfaces/ITenantRepository.cs` - Updated namespace
- `Infrastructure/Repositories/TenantRepository.cs` - Updated namespace
- `.github/copilot-instructions.md` - Added mandatory documentation research requirement

### Deleted
- `Infrastructure/DatabaseContexts/B2B/Entities/Tenant/` - Old B2B-specific tenant entity (replaced by Baseline)
- `Infrastructure/Domain/Baseline/Tenants/TenantFactory.cs` - Removed from Domain layer (layering violation)

## Migration Details

### Database Changes
```sql
-- Add discriminator column
ALTER TABLE "Tenants" ADD COLUMN "TenantType" integer NOT NULL DEFAULT 0
COMMENT '0 = Personal (B2C), 1 = Organization (B2B)';

-- Make B2B fields nullable
ALTER TABLE "Tenants" ALTER COLUMN "VatNumber" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Country" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Email" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "CustomerId" DROP NOT NULL;

-- Add performance indexes
CREATE INDEX "IX_Tenants_TenantType" ON "Tenants" ("TenantType");
CREATE INDEX "IX_Tenants_Email_TenantType" ON "Tenants" ("Email", "TenantType");
CREATE INDEX "IX_Tenants_Type_Active_NotDeleted" ON "Tenants" ("TenantType", "IsActive", "IsSoftDeleted");
* Configuration must now be set via IOptions pattern.
Legacy environment variables (STRIPE_API_KEY, RESEND_API_KEY) still
supported for backward compatibility, but APPBLUEPRINT_ prefix is preferred.

Added:
- StripeOptions, CloudflareR2Options, ResendEmailOptions, FeatureFlagsOptions classes
- IConfigurationService interface and ConfigurationService implementation
- ConfigurationServiceCollectionExtensions for centralized options registration
- appsettings.configuration-example.json with complete configuration examples
- Validation at startup for all Options classes (fail fast with clear errors)

Modified:
- StripeSubscriptionService: now injects IOptions<StripeOptions>
- ObjectStorageService: now injects IOptions<CloudflareR2Options>
- TransactionEmailService: now uses IOptions<ResendEmailOptions>
- ServiceCollectionExtensions: refactored to use Options pattern instead of direct env vars
- Program.cs (ApiService, Web): added AddAppBlueprintConfiguration() registration

Benefits:
- Type safety with IntelliSense support
- Compile-time checking of configuration access
- Validation at startup (no runtime surprises)
- Easy unit testing with Options.Create()
- No null checks needed after startup validation
- Clear separation of public config vs secrets
- Support for multiple configuration sources (appsettings, env vars, Key Vault, User Secrets)

Configuration hierarchy (highest to lowest priority):
1. Command-line arguments
2. Environment variables (APPBLUEPRINT_ prefix)
3. Azure Key Vault
4. User Secrets (development)
5. appsettings.{Environment}.json
6. appsettings.json

All projects build successfully. No breaking changes to existing applications
that don't use the new Options pattern.
* **infrastructure:** None
