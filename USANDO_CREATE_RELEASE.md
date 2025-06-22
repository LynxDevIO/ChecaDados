# Como usar o script `create-release.ps1` para criar releases automáticos

Este script PowerShell automatiza o processo de versionamento, atualização de arquivos e criação de tags/releases para o projeto **ChecaDados**.

## Pré-requisitos

- **Git** instalado e configurado no PATH.
- Permissão para executar scripts PowerShell (pode ser necessário rodar `Set-ExecutionPolicy RemoteSigned` no PowerShell como administrador).
- Ter o repositório clonado e atualizado localmente.
- Ter permissões de push para o repositório remoto.

## Como usar

1. **Abra o PowerShell** na raiz do projeto (onde está o arquivo `create-release.ps1`).
2. Execute o script informando o tipo de incremento de versão desejado:

   ```powershell
   .\create-release.ps1 [major|minor|patch]
   ```
   - `major`: incrementa a versão principal (ex: 1.0.0.0 → 2.0.0.0)
   - `minor`: incrementa a versão secundária (ex: 1.0.0.0 → 1.1.0.0)
   - `patch`: incrementa a versão de correção (ex: 1.0.0.0 → 1.0.1.0)

## O que o script faz

- Lê a versão atual do arquivo `ChecaDados\Properties\AssemblyInfo.cs`.
- Incrementa a versão conforme o parâmetro informado.
- Atualiza a versão no `AssemblyInfo.cs` e no arquivo `version.xml`.
- Cria um commit, uma tag Git correspondente à nova versão e faz push para o repositório remoto.
- O GitHub Actions será acionado automaticamente para compilar e criar a release.

## Exemplo de uso

```powershell
.\create-release.ps1 patch
```

Este comando irá:
- Incrementar apenas o número de patch da versão (ex: 1.0.0.0 → 1.0.1.0)
- Atualizar os arquivos de versão
- Criar commit, tag e fazer push para o GitHub

## Observações importantes

- Certifique-se de que todas as alterações estejam commitadas antes de rodar o script.
- O script faz commit e push automaticamente de todas as mudanças não commitadas.
- O processo de release e publicação é finalizado pelo GitHub Actions.
- O arquivo `version.xml` é atualizado com a nova versão e URLs de release.

## Links úteis
- [Releases do projeto no GitHub](https://github.com/LynxDevIO/ChecaDados/releases)

---

Dúvidas ou problemas? Abra uma issue no repositório ou entre em contato com o mantenedor. 