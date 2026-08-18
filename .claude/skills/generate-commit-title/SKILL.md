---
name: generate-commit-title
description: >-
  Generate a single-line git commit title from the current working tree changes.
  Use when the user asks for a commit message, commit title, or invokes this skill.
disable-model-invocation: true
---

# Generate Commit Title

Produce **only** a commit title (subject line). Do **not** write a body/description. Do **not** create a commit unless the user explicitly asks.

## Steps

1. Inspect current changes (run in parallel):
   - `git status --short`
   - `git diff` and `git diff --cached`
   - `git log -8 --format='%s'` (match this repo's title style)
2. Summarize the **why** of the change set, not a file laundry list.
3. Reply with **only** the title — no preamble, no bullets, no body, no code fences.

## Title rules

- One line, imperative mood, ~72 characters max
- Prefer Conventional Commits prefixes used in this repo: `feat:`, `fix:`, `refactor:`, `chore:`, `docs:`, `style:`, `test:`
- Focus on intent (e.g. `feat: add checkout booking flow`), not every touched file
- If there are no changes, say so in one short sentence instead of inventing a title

## Examples

```
feat: add invoice summary cards to invoices page
fix: correct booking calendar timezone handling
refactor: simplify pet picker selection state
```
