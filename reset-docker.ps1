Write-Output 'Shutting down any remaining running docker containers....'
docker rm --force $(docker ps -aq)

Write-Output 'Pruning docker build cache...'
docker builder prune --all --force

Write-Output 'Pruning docker images cache...'
docker system prune --all --force --volumes

Write-Output 'Removing all docker volumes...'
docker volume rm --force $(docker volume ls -q)

if ($null -eq $env:RL_DOCKER_VOLS_ROOT) {
    Write-Output 'RL_DOCKER_VOLS_ROOT environment variable not set; not deleting files...'
}
else
{
    $RL_DOCKER_VOLS_OUT = "$env:RL_DOCKER_VOLS_ROOT/out"

    Write-Output "Deleting docker-volumes output files ($RL_DOCKER_VOLS_OUT)"

    foreach ($file in Get-ChildItem -Path "$RL_DOCKER_VOLS_OUT" -Include *.txt, *.trx, *.json -Recurse) {
        Write-Output "Removing $($file.fullname)"
        Remove-Item $file.fullname -Force -Recurse -ErrorAction silentlycontinue
    }
}