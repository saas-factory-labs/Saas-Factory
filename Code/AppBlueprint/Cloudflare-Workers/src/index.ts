import { Container, getContainer } from "@cloudflare/containers";

type Env = {
  WEB_CONTAINER: DurableObjectNamespace<Web>;
  API_CONTAINER: DurableObjectNamespace<Api>;

  // .NET runtime
  ASPNETCORE_ENVIRONMENT?: string;
  DOTNET_ENVIRONMENT?: string;
  OTEL_EXPORTER_OTLP_ENDPOINT?: string;

  // Auth
  AUTHENTICATION_PROVIDER?: string;
  LOGTO_ENDPOINT?: string;
  LOGTO_APPID?: string;
  LOGTO_APPSECRET?: string;
  LOGTO_APIRESOURCE?: string;

  // Database
  DATABASE_CONNECTIONSTRING?: string;
  DATABASECONTEXT_ENABLEHYBRIDMODE?: string;

  // Cloudflare R2 storage
  CLOUDFLARE_R2_ACCESSKEYID?: string;
  CLOUDFLARE_R2_SECRETACCESSKEY?: string;
  CLOUDFLARE_R2_ENDPOINTURL?: string;
  CLOUDFLARE_R2_PRIVATEBUCKETNAME?: string;
  CLOUDFLARE_R2_PUBLICBUCKETNAME?: string;
  CLOUDFLARE_R2_PUBLICDOMAIN?: string;
  CLOUDFLARE_R2_MAXIMAGESIZEMB?: string;
  CLOUDFLARE_R2_MAXDOCUMENTSIZEMB?: string;
  CLOUDFLARE_R2_MAXVIDEOSIZEMB?: string;

  // Web → API routing
  API_BASE_URL?: string;
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
    LOGTO_APIRESOURCE: env.LOGTO_APIRESOURCE,
    API_BASE_URL: env.API_BASE_URL,
    CLOUDFLARE_R2_PUBLICDOMAIN: env.CLOUDFLARE_R2_PUBLICDOMAIN,
  });
}

function getApiContainerEnvVars(env: Env): Record<string, string> {
  return pickDefinedEnvVars({
    ...getSharedDotNetEnvVars(env),
    AUTHENTICATION_PROVIDER: env.AUTHENTICATION_PROVIDER,
    LOGTO_ENDPOINT: env.LOGTO_ENDPOINT,
    LOGTO_APPID: env.LOGTO_APPID,
    LOGTO_APPSECRET: env.LOGTO_APPSECRET,
    LOGTO_APIRESOURCE: env.LOGTO_APIRESOURCE,
    DATABASE_CONNECTIONSTRING: env.DATABASE_CONNECTIONSTRING,
    DATABASECONTEXT_ENABLEHYBRIDMODE: env.DATABASECONTEXT_ENABLEHYBRIDMODE,
    CLOUDFLARE_R2_ACCESSKEYID: env.CLOUDFLARE_R2_ACCESSKEYID,
    CLOUDFLARE_R2_SECRETACCESSKEY: env.CLOUDFLARE_R2_SECRETACCESSKEY,
    CLOUDFLARE_R2_ENDPOINTURL: env.CLOUDFLARE_R2_ENDPOINTURL,
    CLOUDFLARE_R2_PRIVATEBUCKETNAME: env.CLOUDFLARE_R2_PRIVATEBUCKETNAME,
    CLOUDFLARE_R2_PUBLICBUCKETNAME: env.CLOUDFLARE_R2_PUBLICBUCKETNAME,
    CLOUDFLARE_R2_PUBLICDOMAIN: env.CLOUDFLARE_R2_PUBLICDOMAIN,
    CLOUDFLARE_R2_MAXIMAGESIZEMB: env.CLOUDFLARE_R2_MAXIMAGESIZEMB,
    CLOUDFLARE_R2_MAXDOCUMENTSIZEMB: env.CLOUDFLARE_R2_MAXDOCUMENTSIZEMB,
    CLOUDFLARE_R2_MAXVIDEOSIZEMB: env.CLOUDFLARE_R2_MAXVIDEOSIZEMB,
  });
}

export class Web extends Container<Env> {
  defaultPort = 8080;
  pingEndpoint = "localhost/ping";
  constructor(ctx: DurableObjectState, env: Env) {
    super(ctx, env);
    this.envVars = getWebContainerEnvVars(env);
  }
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
