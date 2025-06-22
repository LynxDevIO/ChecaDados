# ChecaDados

Um aplicativo desktop desenvolvido em C# WPF para consulta e validação de CNPJs através da API pública da CNPJA.

## Funcionalidades

- **Consulta de CNPJs** em tempo real via API
- **Validação automática** de formato de CNPJ
- **Cache local** em arquivo CSV para consultas repetidas
- **Controle de taxa** para evitar exceder limites da API
- **Interface intuitiva** com design moderno
- **Sistema de atualizações automáticas** via GitHub
- **Histórico de consultas** persistente

## Pré-requisitos

- Windows 7 ou superior
- .NET Framework 4.8
- Conexão com a internet para consultas à API

## Tecnologias Utilizadas

- **C#** - Linguagem principal
- **WPF (Windows Presentation Foundation)** - Interface gráfica
- **AutoUpdater.NET** - Sistema de atualizações automáticas
- **CsvHelper** - Manipulação de arquivos CSV
- **Newtonsoft.Json** - Processamento de JSON
- **API CNPJA** - Fonte de dados para consultas

## API Utilizada

### CNPJA - API Pública de CNPJs

Este projeto utiliza a **API CNPJA** para consultas de CNPJs em tempo real.

- **URL da API**: https://open.cnpja.com/office/
- **Tipo**: API pública gratuita
- **Documentação**: https://open.cnpja.com/
- **Limite**: 5 consultas por minuto
- **Dados retornados**:
  - Nome da empresa
  - Endereço e estado
  - Inscrições estaduais
  - Informações básicas do CNPJ

### Limitações da API

- **Rate Limit**: 5 consultas por minuto por IP
- **CNPJs válidos**: Apenas CNPJs ativos na base da Receita Federal
- **Conexão**: Requer internet para funcionamento
- **Disponibilidade**: Sujeita à disponibilidade da API CNPJA

### Créditos

Agradecemos à **CNPJA** por disponibilizar esta API pública gratuita que permite o desenvolvimento de aplicações como esta. Para mais informações sobre a API, visite: https://open.cnpja.com/

## Instalação

### Opção 1: Download da Release
1. Acesse a [página de releases](https://github.com/LynxDevIO/ChecaDados/releases)
2. Baixe a versão mais recente
3. Extraia o arquivo ZIP
4. Execute `ChecaDados.exe`

### Opção 2: Compilação do Código Fonte
```bash
# Clone o repositório
git clone https://github.com/SEU_USUARIO/ChecaDados.git

# Abra o projeto no Visual Studio
# Compile em modo Release
# Execute o aplicativo
```

## Como Usar

1. **Digite um CNPJ** no campo de entrada (com ou sem formatação)
2. **Pressione Enter** ou clique em "Consultar"
3. **Aguarde** a consulta à API
4. **Visualize os resultados**:
   - Nome da empresa
   - Estado
   - Inscrição estadual
   - Data/hora da consulta

**Nota**: O aplicativo controla automaticamente o limite de 5 consultas por minuto da API CNPJA.

## Estrutura do Projeto

```
ChecaDados/
├── ChecaDados/
│   ├── MainWindow.xaml          # Interface principal
│   ├── MainWindow.xaml.cs       # Lógica da aplicação
│   ├── App.xaml                 # Configuração da aplicação
│   ├── Properties/              # Configurações do assembly
│   └── bin/                     # Arquivos compilados
├── packages/                    # Pacotes NuGet
├── version.xml                  # Configuração de atualizações
└── README.md                    # Este arquivo
```

## 🔧 Configuração do Sistema de Atualizações

### Releases Automáticos

Este projeto utiliza **GitHub Actions** para criar releases automáticos. O processo é totalmente automatizado:

#### Como criar uma nova release:

1. **Execute o script de release**:
   ```powershell
   # Para correção de bug (patch)
   .\create-release.ps1 patch
   
   # Para nova funcionalidade (minor)
   .\create-release.ps1 minor
   
   # Para mudança importante (major)
   .\create-release.ps1 major
   ```

2. **O script irá**:
   - ✅ Incrementar a versão automaticamente
   - ✅ Atualizar o `AssemblyInfo.cs`
   - ✅ Atualizar o `version.xml`
   - ✅ Criar uma tag Git
   - ✅ Fazer push para o GitHub

3. **GitHub Actions irá**:
   - 🔄 Compilar o projeto automaticamente
   - 📦 Criar um arquivo ZIP da release
   - 🏷️ Criar a release no GitHub
   - 📝 Gerar notas de release automaticamente

#### Estrutura de Versionamento

- **Major** (1.0.0 → 2.0.0): Mudanças incompatíveis
- **Minor** (1.0.0 → 1.1.0): Novas funcionalidades
- **Patch** (1.0.0 → 1.0.1): Correções de bugs

### Configuração Manual

Se preferir criar releases manualmente:

1. **Atualize a versão** em `ChecaDados/Properties/AssemblyInfo.cs`
2. **Atualize o `version.xml`** com a nova versão
3. **Crie uma tag** no GitHub: `v1.0.0`
4. **Faça upload** do arquivo ZIP da release

### Arquivo version.xml

O arquivo `version.xml` deve ser hospedado em um local acessível via HTTP. Exemplo:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>1.0.0.1</version>
    <url>https://github.com/LynxDevIO/ChecaDados/releases/download/v1.0.0.1/ChecaDados-1.0.0.1.zip</url>
    <changelog>https://github.com/LynxDevIO/ChecaDados/releases</changelog>
    <mandatory>false</mandatory>
</item>
```

**Nota**: Substitua `LynxDevIO` pelo seu nome de usuário do GitHub.

## Armazenamento de Dados

- **Localização**: `Documentos/ChecaDados/ChecaDados.csv`
- **Formato**: CSV com as seguintes colunas:
  - CNPJ
  - Nome da empresa
  - Estado
  - Inscrição estadual
  - Data/hora da consulta

## Solução de Problemas

### Erro de Conexão
- Verifique sua conexão com a internet
- Tente novamente em alguns minutos

### Limite de Consultas
- Aguarde 1 minuto entre consultas
- O aplicativo controla automaticamente o limite

### CNPJ Inválido
- Verifique se o CNPJ tem 14 dígitos
- Certifique-se de que o CNPJ existe na base da Receita Federal

## Contribuindo

1. **Fork** o projeto
2. **Crie uma branch** para sua feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. **Abra um Pull Request**

## Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## Desenvolvedor

**Philipe** - Desenvolvedor do projeto ChecaDados

## Agradecimentos

- **CNPJA** pela API pública de consulta de CNPJs
- **Comunidade .NET** pelos recursos e bibliotecas utilizadas
- **Contribuidores** que ajudaram no desenvolvimento

## Suporte

Para suporte, dúvidas ou sugestões:
- Abra uma [issue](https://github.com/LynxDevIO/ChecaDados/issues) no GitHub
- Entre em contato através do email: [seu-email@exemplo.com]

---

**Se este projeto foi útil para você, considere dar uma estrela no repositório!**