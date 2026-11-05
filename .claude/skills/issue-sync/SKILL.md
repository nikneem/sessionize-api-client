---
name: issue-sync
description: Keep OpenSpec changes and their GitHub source issues in step through the whole lifecycle (propose, apply, pull request, archive), and close the issue when its change is archived. Use whenever you propose a change from an issue, open the pull request for a change, archive a change, abandon a change, or when someone asks whether issues and changes are in sync.
---

An OpenSpec change that came from a GitHub issue keeps that issue up to date. The issue is what maintainers watch;
the change is where the work happens.

Repository: `nikneem/sessionize-api-client`. Default branch: `main`.

## Why this needs a step of its own

GitHub only closes an issue through `Closes #n` or `Fixes #n` when the pull request merges into the default branch
(`main`). When pull requests target another branch (for example `development`), merging never closes the
issue, so it is closed when the change is archived: that is the point where the work is merged and finished.

## The link

The issue and the change point at each other with a fixed structure:

- The change's `proposal.md` ends with a `## Source issue` section carrying exactly one
  `<!-- source-issue: nikneem/sessionize-api-client#<n> -->`.
- The issue body has a `## Triage` block with exactly one `<!-- openspec-change: <change-name> -->`.

A change without a `source-issue` marker has no issue to keep in sync; skip the steps below for it.

## Lifecycle

| Moment | What to do on the issue |
|---|---|
| Propose from an issue | Write both markers. |
| Pull request opened | Put `Refs #<n>` in the pull request body (not `Closes #<n>` when the pull request does not target the default branch: it would not fire). Comment on the issue with the pull request link. |
| Archive | After the archive commit and once the pull request is merged, close the issue as completed with a comment naming the change and its archive folder. |
| Abandoned change | Ask a maintainer before touching the issue. If they agree, close it as not planned with a comment that says why. |

Other cases:

- The issue is already closed when you archive: leave it closed; add a comment linking the archived change only if it
  has none.
- Never reopen, relabel or edit an issue's description beyond the marker block without asking.

## Closing the issue on archive

1. Confirm the change's pull request is merged (`gh pr list --state merged --head <branch>`). If it is not, say so and
   ask before closing.
2. Close the issue:

   ```bash
   gh issue close <n> --repo nikneem/sessionize-api-client --reason completed \
     --comment "Implemented by OpenSpec change \`<change-name>\` (PR #<pr>), archived at \`openspec/changes/archive/<dated-name>/\`."
   ```

3. Check with `gh issue view <n> --repo nikneem/sessionize-api-client --json state` that it reads `CLOSED`.

## Checking for drift

Compare every `proposal.md` under `openspec/changes/` (active and archived) with its issue and report:

- an archived change whose issue is still open;
- an active change whose issue is closed, or whose issue lacks the `openspec-change` marker;
- an open issue whose `openspec-change` marker points to a change that does not exist;
- more than one marker on either side.

Report every finding; only closing the issue of an archived, merged change needs no decision from a person.
