param(
  [Parameter(Mandatory=$true)][string]$SolutionDir,
  [Parameter(Mandatory=$true)][string]$OutputDir,
  [Parameter(Mandatory=$true)][string]$ZipName
)

# 1) Arbo
Set-Location $SolutionDir
tree /F /A > (Join-Path $SolutionDir "project_tree.txt")

# 2) ZIP (hors solution)
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$zipPath = Join-Path $OutputDir $ZipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

# Exclusions (dossiers racine à ignorer)
$exclude = @("bin","obj",".vs","Library","Temp","Logs","Build","Builds",".git",".idea",".vscode")

# Prendre les éléments à zipper à la racine de la solution, sauf exclusions
$items = Get-ChildItem -LiteralPath $SolutionDir -Force | Where-Object { $exclude -notcontains $_.Name }

Compress-Archive -Path $items.FullName -DestinationPath $zipPath -Force
Write-Host "Snapshot created: $zipPath"
