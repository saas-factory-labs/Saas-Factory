 # AI Rules

This directory contains the rules for AI assistants to follow when working with this codebase from https://github.com/Trubador/SaaS-Factory
This directory contains specific guidance for AI assistants to follow when working with this codebase.

## AI Assistant Instructions

* When formulating a plan start by reading the relevant instructions from the following:

- AppBlueprint Directory.Packages.props file at `/Code/AppBlueprint/Directory.Packages.props`
- Directory.Build.props file at `/Code/AppBlueprint/Directory.Build.props`
- Writerside documentation at `/Writerside/topic/README.md`
- Assess folder structure and project files for example to build and run each project
- Assess the tech stack from the writerside documentation
- Implement null checks and error handling where appropriate, but do not over-engineer the code. Null checks should be achieved with `is null` or `is not null` checks, and error handling should be done using exceptions. Do not use `??` or `??=` operators. In addition use `ThrowIfNull` and `ThrowIfEmpty` methods to throw exceptions when null or empty values are encountered.
- Use `dotnet format` to format the code according to the `.editorconfig` file. If you are unable to do so, please inform the user and ask for assistance.
- Read the `.editorconfig` file to understand the coding style that need to be followed
- Do not remove commented out code unless explicitly asked to do so. If you are unsure, please ask the user for clarification.
- Use Tunit for unit tests and integration tests, use bUnit for blazor ui tests, and FluentAssertions for assertions. Do not use other testing frameworks unless explicitly asked to do so.
- Use `Nuget MCP server` to query correct versions of release nuget packages that do not contain known vulnerabilities - if no release version is available use the latest pre-release version instead. Ask the user before upgrading or downgrading a package.
- Assess the Entity Framework code and use `PostgreSQL MCP server` to query the database for schema verification and data
- Do not implement password hashing or encryption unless explicitly asked to do so. The system will use an external authentication provider for this purpose.
- Set namespace in `.cs (c#)` files according to the folder structure that the file is placed at
- Remove unnecessary using statements when adding or editing a file
- Always provide clear warning to the user when deleting a file or folder such that they can assess the change
- Focus on resolving errors - not warnings. If a warning is not relevant to the task at hand, ignore it.
- If you are asked to check which issues are open in the repository, then use the `Github MCP server` to query the issues in the MVP Github project by using this query `{ "q": "repo:Trubador/SaaS-Factory is:issue state:open" }`
- If a bug occurs that can't be resolved then create a bug issue in the MVP Github project for the repo with the exact details of the bug by using a `Github MCP server`
- Create unit and integration tests when making additions and modifications
- When running in Agent Mode in Copilot edit at most 3 files in total before asking me to confirm the changes so far so you can proceed with the changes to achieve the task you were given
- Copilot is running on a windows machine, so use Powershell commands in the command line when running commands. Do not use bash commands or any other command line interface.



It is **EXTREMELY** important that you follow the instructions very carefully.
Only do work on the AppBlueprint directory and related projects.

**Read the following additional instructions:**

[Backend Rules](/.ai-rules/backend/README.md) 
[Baseline Rules](/.ai-rules/baseline/README.md) 
[Frontend Rules](/.ai-rules/frontend/README.md)
[Infrastructure Rules](/.ai-rules/infrastructure/README.md)
[Developer CLI Rules](/.ai-rules/developer-cli/README.md)
[Development Workflow](/.ai-rules/development-workflow/README.md)

When we learn new things that deviate from the existing rules, suggest making changes to the rules files or creating new rules files. When creating new rules files, always make sure to add them to the relevant README.md file.

## Project Structure

** The AppHost project is the entry point. Do NOT RUN THIS as it's likely already running in watch mode. 
The project structure is in the Writerside documentation at `/Writerside/topic/README.md`

## Final instructions

- ONLY MAKE CHANGES ACCORDING TO THE TASK YOU ARE GIVEN - NO OVERENGINEERING IS ALLOWED
- MAKE SURE EVERYTHING COMPILES CORRECTLY BY BUILDING THE AFFECTED PROJECTS THAT AND RUNTIME ALSO FUNCTIONS CORRECTLY BEFORE ASSESING IF THE TASK WAS COMPLETED - YOU DONT NEED TO ASK FOR PERMISSION TO BUILD THE PROJECTS - JUST DO IT, UNLESS THE SPECIFIC PROJECT OR APPHOST IS ALREADY RUNNING
- Always ask the user for clarification if you didn't understand the task, or if you need more information about the rules.
- Based on the changes you made - formulate a git commit message that describes the changes you made.
