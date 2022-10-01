[CmdletBinding(PositionalBinding=$false)]
param (

)

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

# If snapshots are changed, create a pull request.
if (git status --porcelain | ForEach-Object { $_.Trim() } | Where-Object { $_ }) {
    $currentBranch = $(git branch --show-current)

    # Avoid making new PR against snapshot-updating PR.
    if ($currentBranch.StartsWith("update-snapshots")) {
        Write-Error "Ignoring snapshot-updating branch $currentBranch"
        exit 1
    }

    git branch -D update-snapshots/$currentBranch
    git checkout -b update-snapshots/$currentBranch
    try {
        git commit -am "Update snapshots"
        git push -f origin update-snapshots/$currentBranch
        gh pr create --base $currentBranch --title "Update snapshots" `
            --body "Generate automatically by script ``process-test-results.ps1``."
    } finally {
        git checkout $currentBranch
    }
} else {
    Write-Output "No changes in snapshots"
}
