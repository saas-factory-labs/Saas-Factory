# How to run dockerfile target build: docker buildx bake appblueprint-web

target "_base" {
  # secret = [
  #   "type=env,id=GITHUB_TOKEN",
  # ]
}

group "all" {
  targets = ["appblueprint-web", "appblueprint-api", "appblueprint-appgw"]
}

target "appblueprint-web" {
  inherits = ["_base"]
  context = "."  
  dockerfile = "AppBlueprint.Web/Dockerfile"
  tags = ["appblueprint-web:dev"]
  args = {
    foo = "bar"
  }
  no-cache = false
  platforms = ["linux/amd64"]
}

target "appblueprint-api" {
  inherits = ["_base"]
  context = "."
  dockerfile = "AppBlueprint.ApiService/Dockerfile"
  tags = ["appblueprint-api:dev"]
  args = {
    foo = "bar"
  }
  no-cache = false
  platforms = ["linux/amd64"]
}

target "appblueprint-appgw" {
  inherits = ["_base"]
  context = "."
  dockerfile = "AppBlueprint.AppGateway/Dockerfile"
  tags = ["appblueprint-gw:dev"]
  args = {
    foo = "bar"
  }
  no-cache = false
  platforms = ["linux/amd64"]
}


target "appblueprint-tests" {
  inherits = ["_base"]
  context = "."
  dockerfile = "AppBlueprint.Tests/Dockerfile"
  tags = ["appblueprint-tests:dev"]
  args = {
    foo = "bar"
  }
  no-cache = false
  platforms = ["linux/amd64"]
}