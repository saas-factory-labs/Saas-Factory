type App  {
	id: String! 
	name: String 
	deployments: [Deployment!] 
}

type Deployment  {
	id: String! 
	environment: String! 
	startedAt: String! 
	finishedAt: String! 
	status: String! 
	dockerImage: [DockerImage!] 
	sqlSchemaMigration: [SQLSchemaMigration!] 
}

type DeploymentConfiguration  {
	key: String 
	value: String 
}

type SQLSchemaMigration  {
	id: String 
	name: String 
}

type DockerImage  {
	name: String 
	tag: String 
	digest: String 
	size: Int 
	architecture: String 
	os: String 
	created: String 
	build: Build 
	dockerImages: [DockerImageLayer!] 
}

type DockerImageLayer  {
	id: String 
	digest: String 
	size: Int 
	architecture: String 
	os: String 
	created: String 
}

type Build  {
	id: String 
	version: Int 
	startedAt: String 
	finishedAt: String 
	status: String 
	sourceCommit: String 
}

type GitCommit  {
	id: String 
	message: String 
}

type FeatureFlag  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type NugetPackage  {
	id: String 
	name: String 
	version: String 
	description: String 
	authors: [String] 
	totalDownloads: Int 
}

type ContainerAppEnvironment  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type PulumiProject  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type ContainerApp  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type SQLDatabase  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type SQLTable  {
	id: String 
	name: String 
}

type InfrastructureProvider  {
	id: String 
	name: String 
	description: String 
	deployments: [Deployment] 
}

type JiraIssue  {
	id: String 
	key: String 
	summary: String 
	description: String 
	deployments: [Deployment] 
}

type CVEVulnerability  {
	id: String 
	name: String 
	description: String 
	nugetPackages: [NugetPackage] 
}

type ApplicationBlueprint  {
	id: String 
	name: String 
	description: String 
	builds: [Build] 
	deployments: [Deployment] 
}

type ExternalProvider  {
	id: String 
	name: String 
	description: String 
	builds: [Build] 
	deployments: [Deployment] 
}

type SourceCodeLicense  {
	id: String 
	name: String 
}

type UseCase  {
	id: String 
	name: String 
}

type Method {
  MethodId: Int
  MethodName: String
  ClassId: Int
}

type Property {
  PropertyId: Int
  PropertyName: String
  ClassId: Int
}

type ClassData {
  ClassId: Int
  ClassName: String
  Methods: [Method]
  Properties: [Property]
  DependsOn: [ClassData]
}

input ClassDataInput {
  ClassId: Int!
  ClassName: String!
  Methods: [MethodInput!]
  Properties: [PropertyInput!]
  DependsOn: [ClassDataInput!]
}

input MethodInput {
  MethodId: Int
  MethodName: String
  ClassId: Int
}

input PropertyInput {
  PropertyId: Int
  PropertyName: String
  ClassId: Int
}

type ClassDataPayload {
  classData: [ClassData]
}


