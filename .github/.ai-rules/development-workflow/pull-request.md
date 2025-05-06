# Pull Request Conventions

This guide defines clear and actionable rules for writing commit descriptions (commonly used in pull requests), optimized for readability, precision, and downstream integration in the PlatformPlatform project. This guide is intended for use by AI coding assistants or automation tools generating descriptions.

## Inspect the pull request

When generating a commit description, always follow these steps to get more context:

1. Run this command to get the full list of commits in the branch:

    `git --no-pager log --format=%s --reverse $(git merge-base HEAD main)..HEAD`

2. Run this command to get a full diff of the code changes:

    `git --no-pager diff main`

3. If the diff is not sufficient to understand what changed, the assistant should look up the full file using file context from the repository.

## Create a good pull request title

Create a pull request title following these conventions:

- Use imperative form ("Fix", "Add", "Upgrade", "Refactor", etc.)
- Write the title as a sentence (not in Title Case)
- Do not end the title with a period
- Keep it short, but descriptive

## Create a good pull request description

### General conventions when writing pull request descriptions
- Use imperative form in sentences
- Use backticks (`) around code concepts like filenames, classnames, env vars, cli commands, etc.
- Do not use the term "Pull Request", "PR", or any abbreviation of it
- Do not use personal pronouns such as "we", "our", "he", "she"
- Avoid generic phrases like "to improve code quality" or "to make the codebase cleaner"
- Prefer describing functional changes and their motivation (but don't try to oversell the changes)

The pull request description only has these two sections:

### Summary & motivation

This section should:

- Start with the most important change
- Consider using bullet points when listing multiple related changes
- Mention additional minor fixes or cleanup last, so the reader doesn't get distracted up front

### Downstream projects

Only include this section if changes require manual updates in downstream projects of PlatformPlatform. All downstream projects have a self-contained system that is following the same structure as AccountManagement and BackOffice. So if the git diff contains the exact same changes as in AccountManagement or BackOffice, then this section should describe the changes that must be made in the downstream projects.

This section should:

- Start with a brief introduction for maintainers of downstream projects
- If more than one change is required, create a numbered list with each change as a separate item
- When showing code that must be updated, include:
    - The exact filename (e.g. `your-self-contained-system/...`) where the change must be made
    - For new code that must be added, the exact code that must be updated
    - For code that must be changed, a Git diff (if appropriate) using ```diff formatting
- If the change is big, instruct maintainers to look at the diff and apply the same changes to their self-contained system as it has been done in AccountManagement and BackOffice
- Use the placeholder term `your-self-contained-system` for downstream implementations
- When changes must be made in `PlatformPlatform.slnx` file, instruct maintainers that these must be manually ported to their `YourSolutionFile.slnx`

## Template
This is the template for a pull request description:

```markdown
### Summary & motivation

Description goes here following the conventions above.

- xxx
- xxx

Additional notes about small changes or additional details go here.

### Downstream projects

Short intro to what maintainers of downstream projects must do.

1. Step 1 - Example of a diff

Make this change in `your-self-contained-system/Api/Foo.cs`

'''diff
- Delete this
+ Add this
'''

2. Step 2 - Example of a code change

Add this change in `your-self-contained-system/WebApp/bar.tsx`

'''tsx
// Some new code
'''

3. Step 3 - Example of a manual change

Make these manual changes.
```

## How to save the result

The description must be written in full markdown. If the description contains nested ``` ...``` code blocks, offer the user to save the result in a markdown file called `###-branch-name.md` in the root of the repository.