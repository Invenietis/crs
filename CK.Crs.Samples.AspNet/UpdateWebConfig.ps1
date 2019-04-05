$assemblyName = 'CK.Crs.Samples.AspNet'
$binDirectory = 'bin'

$ErrorActionPreference = "Stop"

$dllConfigPath = "$PSScriptRoot/$($binDirectory)/$($assemblyName).dll.config"
$webConfigPath = "$PSScriptRoot/Web.config"
$webConfigBackupPath = "$PSScriptRoot/Web.config.$(get-date -f yyyyMMdd-HHmmss)"

if( Test-Path $webConfigPath) {
    if(Test-Path $dllConfigPath) {
        # Backup Web.config because why the fuck not
        Copy-Item $webConfigPath $webConfigBackupPath -Force

        # Load both XML files
        $dllConfigXml = New-Object Xml
        $dllConfigXml.Load($dllConfigPath)

        $webConfigXml = New-Object Xml
        $webConfigXml.Load($webConfigPath)

        # Remove old <configuration><runtime> element if it's there
        if($webConfigXml.configuration.runtime -and $dllConfigXml.configuration.runtime.InnerXML) {
            $webConfigXml.configuration.RemoveChild($webConfigXml.configuration.runtime) | Out-Null
        }

        # create a new <runtime> node and copy it from DLL config
        $runtimeNode = $webConfigXml.CreateElement("runtime")
        $runtimeNode.InnerXML = $dllConfigXml.configuration.runtime.InnerXML
        
        # Add the <runtime> node to <configuration>
        [void]$webConfigXml.configuration.AppendChild($runtimeNode)

        # Save and overwrite Web.config
        $webConfigXml.Save("$($webConfigPath)")

    } else {
        throw "$($configPath) does not exist"
    }

} else {
    throw "$($webConfigPath) does not exist"
}
