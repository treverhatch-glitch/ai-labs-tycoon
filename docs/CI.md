# GitHub Actions on this repo

## Why pull request checks may show no workflow runs

GitHub only registers workflows that exist on the repository **default branch** (`main` today). Until
`rules.yml` and `unity.yml` are on `main`, the API lists zero workflows and `pull_request` events on open PRs
(including PR #2) do not start runs.

This is not a merge conflict issue: PR #2 is mergeable against `foundation-eras-world-map`. Path filters on
`unity.yml` are correct once Actions runs.

## What Hatch does (no secrets or repo settings from agents)

1. **Settings → Actions → General** — ensure Actions are enabled for this repository (not disabled).
2. **Land workflows on `main`** — cherry-pick or copy from PR #2:
   - `.github/workflows/rules.yml` (dotnet SimTests + 2-year SimCli on every PR)
   - `.github/workflows/unity.yml` (GameCI Edit Mode when `unity/**` changes)
3. **First run approval** — if the Actions tab shows “Workflows aren’t being run”, approve new workflows for this
   repo (or org policy).
4. **Unity secrets** (only for the Unity job): `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` in
   Settings → Secrets → Actions. The rules job needs no extra secrets.

After step 2, reopen or push PR #2; `rules` should run on all PRs and `unity` when the diff touches `unity/**` or
the workflow file.