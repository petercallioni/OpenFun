param (
    [string]$AssetsFile,
    [string]$OutputFile
)

if (-not (Test-Path $AssetsFile)) {
    Write-Warning "Assets file '$AssetsFile' not found. Skipping license generation."
    exit 0
}

# Continue processing once we know the file exists.
$json = Get-Content $AssetsFile | ConvertFrom-Json

# Ensure the output directory exists (create it if needed).
$outputDir = Split-Path $OutputFile -Parent
if (!(Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

# Define the default global NuGet packages folder.
$globalPackagesPath = Join-Path $env:USERPROFILE ".nuget\packages"

$licensesContent = ""

foreach ($lib in $json.libraries.PSObject.Properties.Name) {
    # The key is typically in the format "PackageName/Version".
    $parts = $lib -split '/'
    if ($parts.Length -lt 2) { continue }
    
    $packageName = $parts[0]
    $packageVersion = $parts[1]

    # Exclude packages from Microsoft, Xamarin, or MAUI.
    $lowerPackageName = $packageName.ToLower()
    if ($lowerPackageName -like "microsoft*" -or $lowerPackageName -like "xamarin*" -or $lowerPackageName -like "maui*") {
        continue
    }
    
    # Construct the path to the package folder.
    $packagePath = Join-Path $globalPackagesPath ($lowerPackageName)
    $packageVersionPath = Join-Path $packagePath $packageVersion

    $licenseText = ""
    $foundLicense = $false

    # Attempt to get a license file from the package folder.
    $licenseFiles = Get-ChildItem -Path $packageVersionPath -Filter "*license*" -File -ErrorAction SilentlyContinue
    if ($licenseFiles -and $licenseFiles.Count -gt 0) {
        # Prefer commonly named files.
        $preferredNames = @("license", "license.txt", "license.md", "LICENSE", "LICENSE.txt", "LICENSE.md")
        $selectedFile = $null
        foreach ($name in $preferredNames) {
            $selectedFile = $licenseFiles | Where-Object { $_.Name -ieq $name } | Select-Object -First 1
            if ($selectedFile) { break }
        }
        if (-not $selectedFile) {
            $selectedFile = $licenseFiles[0]
        }
        $licenseText = Get-Content $selectedFile.FullName -Raw
        $foundLicense = $true
    }
    
    if (-not $foundLicense) {
        # As a fallback, look for the nuspec file and extract license data.
        $nuspecFiles = Get-ChildItem -Path $packageVersionPath -Filter "*.nuspec" -File -ErrorAction SilentlyContinue
        if ($nuspecFiles -and $nuspecFiles.Count -gt 0) {
            $nuspecPath = $nuspecFiles[0].FullName
            try {
                [xml]$nuspec = Get-Content $nuspecPath
                # Look for the <license> node first (this is common in newer packages).
                $licenseNode = $nuspec.package.metadata.license
                if ($licenseNode) {
                    $licenseText = $licenseNode.InnerText
                    $foundLicense = $true
                }
                else {
                    # If no <license> element, fallback to the <licenseUrl> element.
                    $licenseUrlNode = $nuspec.package.metadata.licenseUrl
                    if ($licenseUrlNode) {
                        $licenseText = "License available at: " + $licenseUrlNode.InnerText
                        $foundLicense = $true
                    }
                }
            }
            catch {
                Write-Warning "Failed to parse nuspec for package: $packageName"
            }
        }
    }
    
    # If no license info was found, output a message.
    if (-not $foundLicense) {
        $licenseText = "License information is not available for package $packageName version $packageVersion."
    }

    # Append formatted information for the package.
    $licensesContent += "=========================================`n"
    $licensesContent += "Package: $packageName`n"
    $licensesContent += "Version: $packageVersion`n"
    $licensesContent += "----------------------------------------`n"
    $licensesContent += "$licenseText`n"
    $licensesContent += "`n"
}

# Write the collected license text to the designated file.
$licensesContent | Out-File -FilePath $OutputFile -Encoding UTF8

Write-Host "License file generated at: $OutputFile"