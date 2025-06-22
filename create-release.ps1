# Script para criar releases automáticos
# Uso: .\create-release.ps1 [major|minor|patch]

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch")]
    [string]$VersionType
)

# Função para obter a versão atual do arquivo AssemblyInfo.cs
function Get-CurrentVersion {
    $assemblyInfoPath = "ChecaDados\Properties\AssemblyInfo.cs"
    $content = Get-Content $assemblyInfoPath -Raw
    
    if ($content -match 'AssemblyVersion\("([^"]+)"\)') {
        return $matches[1]
    }
    return "1.0.0.0"
}

# Função para atualizar a versão no AssemblyInfo.cs
function Update-Version {
    param([string]$NewVersion)
    
    $assemblyInfoPath = "ChecaDados\Properties\AssemblyInfo.cs"
    $content = Get-Content $assemblyInfoPath -Raw
    
    # Atualizar AssemblyVersion
    $content = $content -replace 'AssemblyVersion\("[^"]+"\)', "AssemblyVersion(`"$NewVersion`")"
    
    # Atualizar AssemblyFileVersion
    $content = $content -replace 'AssemblyFileVersion\("[^"]+"\)', "AssemblyFileVersion(`"$NewVersion`")"
    
    Set-Content $assemblyInfoPath $content -Encoding UTF8
    Write-Host "Versão atualizada para $NewVersion" -ForegroundColor Green
}

# Função para incrementar versão
function Add-Version {
    param([string]$CurrentVersion, [string]$Type)
    
    $parts = $CurrentVersion.Split('.')
    $major = [int]$parts[0]
    $minor = [int]$parts[1]
    $patch = [int]$parts[2]
    $build = [int]$parts[3]
    
    switch ($Type) {
        "major" { 
            $major++
            $minor = 0
            $patch = 0
            $build = 0
        }
        "minor" { 
            $minor++
            $patch = 0
            $build = 0
        }
        "patch" { 
            $patch++
            $build = 0
        }
    }
    
    return "$major.$minor.$patch.$build"
}

# Função para atualizar o arquivo version.xml
function Update-VersionXml {
    param([string]$NewVersion)
    
    $versionXml = @"
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>$NewVersion</version>
    <url>https://github.com/LynxDevIO/ChecaDados/releases/download/v$NewVersion/ChecaDados-v$NewVersion.zip</url>
    <changelog>https://github.com/LynxDevIO/ChecaDados/releases</changelog>
    <mandatory>false</mandatory>
</item>
"@
    
    Set-Content "version.xml" $versionXml -Encoding UTF8
    Write-Host "version.xml atualizado" -ForegroundColor Green
}

# Função para criar tag e push
function New-GitTag {
    param([string]$Version)
    
    $tagName = "v$Version"
    
    # Verificar se já existe um repositório Git
    if (-not (Test-Path ".git")) {
        Write-Host "Repositório Git não encontrado. Execute 'git init' primeiro." -ForegroundColor Red
        exit 1
    }
    
    # Adicionar todas as mudanças
    git add .
    
    # Commit das mudanças
    git commit -m "Release version $Version"
    
    # Criar tag
    git tag $tagName
    
    # Push das mudanças e tag
    git push origin main
    git push origin $tagName
    
    Write-Host "Tag $tagName criada e enviada para o GitHub" -ForegroundColor Green
}

# Execução principal
Write-Host "Iniciando processo de release..." -ForegroundColor Cyan

# Obter versão atual
$currentVersion = Get-CurrentVersion
Write-Host "Versão atual: $currentVersion" -ForegroundColor Yellow

# Calcular nova versão
$newVersion = Add-Version $currentVersion $VersionType
Write-Host "Nova versão: $newVersion" -ForegroundColor Yellow

# Atualizar arquivos
Update-Version $newVersion
Update-VersionXml $newVersion

# Criar tag e push
New-GitTag $newVersion

Write-Host "Release $newVersion criado com sucesso!" -ForegroundColor Green
Write-Host "O GitHub Actions irá compilar e criar a release automaticamente." -ForegroundColor Cyan
Write-Host "Acesse: https://github.com/LynxDevIO/ChecaDados/releases" -ForegroundColor Blue 