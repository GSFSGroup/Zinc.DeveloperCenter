$ErrorActionPreference = "Continue"

Write-Output "Searching for folders to clean"
 foreach($_file in Get-ChildItem  -include bin,obj  -Exclude node_modules -Recurse ){
 	Write-Output "Removing " $_file.fullname
 	remove-item $_file.fullname -Force -Recurse -ErrorAction silentlycontinue
 }  