Param
(
    [Parameter(Mandatory = $true)] [string] $SubscriptionId,
    [Parameter(Mandatory = $true)] [string] $Location,
    [Parameter(Mandatory = $true)] [string] $DeletedResourceName
)

Import-Module "Az.Accounts"

try
{
    $token = Get-AzAccessToken

    $request = @{
        Method = 'DELETE'
        Uri = 'https://management.azure.com/subscriptions/' + $SubscriptionId + '/providers/Microsoft.ApiManagement/locations/' + $Location + '/deletedservices/' + $Name + '?api-version=2020-06-01-preview'
        Headers = @{ Authorization = "Bearer $( $token.Token )" }
    }

    $response = Invoke-RestMethod $request

    if ($response.StatusCode -eq 202)
    {
        <# Action to perform if the condition is true #>
        Write-Host "Purging $Name completed successfully" -ForegroundColor Green
        return $true
    }
    else
    {
        return $false
    }
}
catch
{
    Write-Host "An error occured while purging $Name. Error: $_" -ForegroundColor Red
    return $false
}


# function Purge-ApiManagementInstance()