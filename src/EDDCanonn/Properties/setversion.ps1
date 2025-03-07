param (
    [string]$projectDir
)

$envVarName = "BUILD_ALLOWED"
$envVarValue = [System.Environment]::GetEnvironmentVariable($envVarName)

if ($null -eq $envVarValue -or $envVarValue.Trim().ToLower() -eq "false") {
    Write-Host "Skipping script execution due to environment variable condition."
    exit 0
}

if (-not $projectDir) {
    Write-Host "Error: Project directory argument missing!"
    exit 1
}

$projectDir = $projectDir -replace '"', ''
$assemblyFile = [System.IO.Path]::Combine($projectDir, "Properties", "AssemblyInfo.cs")
$versionFile = [System.IO.Path]::Combine($projectDir, "Properties", "version.txt")
$versionPattern = '\[assembly: AssemblyVersion\("(\d+)\.(\d+)\.(\d+)\.(\d+)"\)\]'

if (-Not (Test-Path $assemblyFile)) {
    Write-Host "Error: AssemblyInfo.cs not found at $assemblyFile"
    exit 1
}

$content = Get-Content $assemblyFile
$match = $content | Select-String -Pattern $versionPattern

if ($match) {
    $major, $minor, $build, $revision = $match.Matches[0].Groups[1..4].Value
} else {
    Write-Host "Error: AssemblyVersion pattern not found in AssemblyInfo.cs!"
    exit 1
}

$prevBuild = 0
$prevMajor, $prevMinor = $major, $minor

if (Test-Path $versionFile) {
    $lastVersion = Get-Content $versionFile
    if ($lastVersion -match '(\d+)\.(\d+)\.(\d+)\.(\d+)') {
        $prevMajor, $prevMinor, $prevBuild, $prevRevision = $matches[1..4]
    }
}

if ($prevMajor -ne $major -or $prevMinor -ne $minor) {
    $build = 0
} else {
    $build = [int]$prevBuild + 1
}

$newVersion = "$major.$minor.$build.0"
$newVersion | Set-Content $versionFile

$content = $content -replace $versionPattern, "[assembly: AssemblyVersion(`"$newVersion`")]"
$content | Set-Content $assemblyFile

Write-Host "Updated AssemblyVersion to: $newVersion"
Write-Host "AssemblyFileVersion remains unchanged."
