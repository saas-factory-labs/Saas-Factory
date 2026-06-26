import { Container, getContainer } from "@cloudflare/containers";

type Env = {
  WEB_CONTAINER: DurableObjectNamespace<Web>;
  API_CONTAINER: DurableObjectNamespace<Api>;
  ASPNETCORE_ENVIRONMENT?: string;
  DOTNET_ENVIRONMENT?: string;
  AUTHENTICATION_PROVIDER?: string;
  LOGTO_ENDPOINT?: string;
  LOGTO_APPID?: string;
  LOGTO_APPSECRET?: string;
  LOGTO_APIRESOURCE?: string;
  LOGTO_RESOURCE?: string;
  DATABASE_URL?: string;
  CONNECTIONSTRINGS__DEFAULTCONNECTION?: string;
  AdminPortal__Security__RequireMfaClaim?: string;
  AdminPortal__Modules__boligportal__ConnectionString?: string;
  OTEL_EXPORTER_OTLP_ENDPOINT?: string;
};

function pickDefinedEnvVars(values: Record<string, string | undefined>): Record<string, string> {
  return Object.fromEntries(Object.entries(values).filter((entry): entry is [string, string] => {
    const value = entry[1];
    return typeof value === "string" && value.length > 0;
  }));
}

function getSharedDotNetEnvVars(env: Env): Record<string, string> {
  return pickDefinedEnvVars({
    ASPNETCORE_ENVIRONMENT: env.ASPNETCORE_ENVIRONMENT,
    DOTNET_ENVIRONMENT: env.DOTNET_ENVIRONMENT,
    OTEL_EXPORTER_OTLP_ENDPOINT: env.OTEL_EXPORTER_OTLP_ENDPOINT,
  });
}

function getWebContainerEnvVars(env: Env): Record<string, string> {
  return pickDefinedEnvVars({
    ...getSharedDotNetEnvVars(env),
    AUTHENTICATION_PROVIDER: env.AUTHENTICATION_PROVIDER,
    LOGTO_ENDPOINT: env.LOGTO_ENDPOINT,
    LOGTO_APPID: env.LOGTO_APPID,
    LOGTO_APPSECRET: env.LOGTO_APPSECRET,
    LOGTO_APIRESOURCE: env.LOGTO_APIRESOURCE ?? env.LOGTO_RESOURCE,
    LOGTO_RESOURCE: env.LOGTO_RESOURCE,
    DATABASE_URL: env.DATABASE_URL,
    CONNECTIONSTRINGS__DEFAULTCONNECTION: env.CONNECTIONSTRINGS__DEFAULTCONNECTION,
    AdminPortal__Security__RequireMfaClaim: env.AdminPortal__Security__RequireMfaClaim,
    AdminPortal__Modules__boligportal__ConnectionString: env.AdminPortal__Modules__boligportal__ConnectionString,
  });
}

function getApiContainerEnvVars(env: Env): Record<string, string> {
  return pickDefinedEnvVars({
    ...getSharedDotNetEnvVars(env),
    AUTHENTICATION_PROVIDER: env.AUTHENTICATION_PROVIDER,
    LOGTO_ENDPOINT: env.LOGTO_ENDPOINT,
    LOGTO_APPID: env.LOGTO_APPID,
    LOGTO_APPSECRET: env.LOGTO_APPSECRET,
    LOGTO_APIRESOURCE: env.LOGTO_APIRESOURCE ?? env.LOGTO_RESOURCE,
    LOGTO_RESOURCE: env.LOGTO_RESOURCE,
    DATABASE_URL: env.DATABASE_URL,
    CONNECTIONSTRINGS__DEFAULTCONNECTION: env.CONNECTIONSTRINGS__DEFAULTCONNECTION,
    AdminPortal__Security__RequireMfaClaim: env.AdminPortal__Security__RequireMfaClaim,
  });
}

export class Web extends Container<Env> {
  defaultPort = 8080;
  pingEndpoint = "localhost/ping";
  constructor(ctx: DurableObjectState, env: Env) {
    super(ctx, env);
    this.envVars = getWebContainerEnvVars(env);
  }

  // override async onActivityExpired(): Promise<void> {
  //   // Keep the web container always running — do not stop on inactivity.
  // }
}

export class Api extends Container<Env> {
  defaultPort = 8080;
  pingEndpoint = "localhost/ping";

  constructor(ctx: DurableObjectState, env: Env) {
    super(ctx, env);
    this.envVars = getApiContainerEnvVars(env);
  }
}

export default {
  async fetch(request: Request, env: Env): Promise<Response> {
    const url = new URL(request.url);

    if (url.pathname.startsWith("/api")) {
      return getContainer(env.API_CONTAINER, "production").fetch(request);
    }

    return getContainer(env.WEB_CONTAINER, "production").fetch(request);
  },
};
