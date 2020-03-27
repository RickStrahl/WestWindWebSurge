# param([string]$uid = "uid", [string]$pwd = "")

Set-ExecutionPolicy Bypass -Scope CurrentUser

# Credential set with:
#  Get-Credential | Export-CliXml  -Path .\FtpCredential.xml
$credential = Import-Clixml -Path .\FtpCredential.xml
if( $null -ne $credential)
{
    $uid = $credential.Username;
    $pwd = $credential.Password;    
}
else {
    # otherwise prompt for password
    $uid= Read-Host -Prompt 'Username' 
    $pwd=Read-Host -Prompt 'Password' -AsSecureString
}

$pwd = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
       [Runtime.InteropServices.Marshal]::SecureStringToBSTR($pwd))

if(!$pwd) {Exit;}

# $uid= Read-Host -Prompt 'Username' 
# $pwd=Read-Host -Prompt 'Password' -AsSecureString
# $pwd = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
#        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($pwd))

# if(!$pwd) {Exit;}

\utl\curl.exe -T ".\Builds\CurrentRelease\WebSurgeSetup.exe"  "ftps://west-wind.com/Westwind_sysroot/Ftp/Files/" -u ${uid}:${pwd} -k
\utl\curl.exe -T ".\Builds\CurrentRelease\WebSurgeSetup.zip"  "ftps://west-wind.com/Westwind_sysroot/Ftp/Files/" -u ${uid}:${pwd} -k
\utl\curl.exe -T ".\Builds\CurrentRelease\WebSurge_Version.xml"  "ftps://west-wind.com/Westwind_sysroot/Ftp/Files/" -u ${uid}:${pwd} -k
