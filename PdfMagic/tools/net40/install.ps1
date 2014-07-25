param($installPath, $toolsPath, $package, $project)

Write-Host "Executing install script for " + $project.FullName

$libFolder = $project.ProjectItems.Item("lib")
$libFolder.ProjectItems.Item("wkhtmltox0.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$libFolder.ProjectItems.Item("libeay32.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$libFolder.ProjectItems.Item("libgcc_s_dw2-1.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$libFolder.ProjectItems.Item("mingwm10.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$libFolder.ProjectItems.Item("ssleay32.dll").Properties.Item("CopyToOutputDirectory").Value = 1

Write-Host "Finished execution of the install script for " + $project.FullName