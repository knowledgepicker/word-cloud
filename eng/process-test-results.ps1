[CmdletBinding(PositionalBinding=$false)]
param (
    $branch = $null,
    [switch]$dry = $false
)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

# If snapshots are changed, create a pull request.
if (git status --porcelain) {
    $currentBranch = $branch ?? $(git branch --show-current)
    Write-Output "Branch: '$currentBranch'"

    # Avoid making new PR against snapshot-updating PR.
    if ($currentBranch.StartsWith("update-snapshots")) {
        Write-Error "Ignoring snapshot-updating branch $currentBranch"
        exit 1
    }

    if ($dry) {
        Write-Output "Dry run, exiting"
        exit
    }

    git branch -D update-snapshots/$currentBranch
    git checkout -b update-snapshots/$currentBranch
    git commit -am "Update snapshots"
    git push -f origin update-snapshots/$currentBranch
    gh pr create --base $currentBranch --title "Update snapshots" `
        --body "Generated automatically by script ``process-test-results.ps1``."
} else {
    Write-Output "No changes in snapshots"
}
