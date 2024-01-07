Param
(
    [Parameter(Mandatory=$true, 
        ValueFromPipeline=$true,
        ValueFromPipelineByPropertyName=$true, 
        ValueFromRemainingArguments=$false, 
        Position=0)]
    [ValidateNotNull()]
    [ValidateNotNullOrEmpty()]
    $Param1,

    [Parameter(Mandatory=$false, 
        ValueFromPipeline=$true,
        ValueFromPipelineByPropertyName=$true, 
        ValueFromRemainingArguments=$false, 
        Position=1)]
    [ValidateNotNull()]
    [ValidateNotNullOrEmpty()]
    $Param2
)

Start-Process -FilePath $Param1 -ArgumentList $Param2 -Wait | Out-Null