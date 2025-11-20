# AuraPlus API

## 👥 Integrantes

- **Arthur Spedine**
- **Matheus Esteves**
- **Gabriel Falanga**

## 📋 Descrição do Projeto

O **AuraPlus** é uma API RESTful desenvolvida em .NET 9 que implementa um sistema de gestão de equipes e reconhecimento de colaboradores. A API segue as melhores práticas REST e inclui recursos avançados como **autenticação JWT**, **versionamento de API**, **paginação**, **HATEOAS**, **observabilidade com OpenTelemetry** e **documentação OpenAPI completa**.

## ✨ Recursos Principais

### ✅ Autenticação e Autorização
- **JWT Tokens**: Autenticação baseada em tokens
- **BCrypt**: Hashing seguro de senhas (work factor 12)
- **Roles**: Sistema de permissões (NOVO_USUARIO, EMPREGADO, GESTOR)
- **Soft Delete**: Desativação lógica de usuários (Ativo 1→0)

### ✅ Gestão de Equipes
- **Criação**: Usuário cria equipe e se torna GESTOR
- **Entrada**: Usuário entra em equipe e vira EMPREGADO
- **Saída**: Usuário sai e volta a NOVO_USUARIO
- **Auto-Delete**: Equipes vazias são automaticamente removidas
- **Gestão de Membros**: Gestor pode adicionar/remover membros

### ✅ Integração Oracle
- **Procedures**: Chamada de stored procedures (prc_inserir_usuario)
- **Validação**: Validação de email via pkg_utils.fn_validar_email
- **Compatibilidade**: Adaptações para particularidades do Oracle

### ✅ Observabilidade
- **OpenTelemetry**: Tracing distribuído com OTLP
- **Jaeger**: Visualização de traces e spans
- **HealthChecks**: Monitoramento de saúde da API
- **Logging**: Logs estruturados em todas as operações

### ✅ Boas Práticas REST
- **Status Codes HTTP** apropriados (200, 201, 204, 400, 401, 403, 404, 500)
- **Verbos HTTP** semânticos (GET, POST, PUT, DELETE)
- **HATEOAS**: Links de navegação em respostas
- **Paginação**: Suporte a paginação com metadados

## 🏗️ Arquitetura do Domínio

### Entidades Principais

1. **Users** - Usuários/colaboradores do sistema
   - Gerencia autenticação e perfil
   - Controla vínculo com equipe e role
   - Suporta soft delete (Ativo 1/0)

2. **Equipe** - Times/departamentos da organização
   - Agrupa colaboradores
   - Possui um gestor responsável
   - Auto-gerenciamento (delete quando vazia)

3. **Sentimentos** - Registro de estado emocional dos colaboradores
   - Acompanha bem-estar da equipe
   - Histórico de sentimentos ao longo do tempo
   - **Regra**: 1 sentimento por dia por pessoa
   - Validação: Usuário deve estar em uma equipe

4. **Reconhecimento** - Sistema de reconhecimento entre pares
   - Reconhecimento de colaborador para colaborador
   - Fortalece cultura organizacional
   - **Regras**: 
     - 1 reconhecimento por dia
     - Mesma pessoa 1x por mês
     - Apenas membros da mesma equipe
     - Não pode reconhecer a si mesmo

5. **RelatorioPessoa** - Relatórios individuais de desempenho
   - Métricas por colaborador (últimos 30 dias)
   - Análise de produtividade
   - ML.NET: Predição de engajamento pessoal
   - Recomendações automáticas baseadas em IA

6. **RelatorioEquipe** - Relatórios consolidados por equipe
   - Visão macro do time (últimos 30 dias)
   - Análise de sentimento médio
   - ML.NET: Predição de engajamento da equipe
   - Sugestões de melhorias via Machine Learning

## 🛠️ Tecnologias Utilizadas

- **.NET 9**: Framework web moderno
- **ASP.NET Core Web API**: API RESTful
- **Entity Framework Core**: ORM para persistência
- **Oracle Database**: Banco de dados relacional (oracle.fiap.com.br:1521/orcl)
- **ML.NET 5.0**: Machine Learning para predições de engajamento
- **Swagger/OpenAPI**: Documentação da API
- **JWT Bearer**: Autenticação por tokens
- **BCrypt.Net**: Hashing de senhas
- **OpenTelemetry**: Observabilidade e tracing
- **Jaeger**: Visualização de traces
- **HealthChecks**: Monitoramento de saúde
- **API Versioning**: Versionamento de endpoints
- **xUnit + Moq**: Framework de testes unitários

## 📁 Estrutura do Projeto

```
dotnet/
├── AuraPlus.Web/              # API Web principal
│   ├── Controllers/           # Controladores da API
│   │   ├── AuthController.cs
│   │   ├── EquipeController.cs
│   │   ├── ReconhecimentoController.cs
│   │   ├── SentimentosController.cs
│   │   └── RelatorioController.cs
│   ├── Services/              # Lógica de negócio
│   │   ├── AuthService.cs
│   │   ├── EquipeService.cs
│   │   ├── ReconhecimentoService.cs
│   │   ├── SentimentosService.cs
│   │   ├── RelatorioService.cs
│   │   └── MLPredictionService.cs
│   ├── Repositories/          # Acesso a dados
│   │   ├── UserRepository.cs
│   │   ├── EquipeRepository.cs
│   │   ├── ReconhecimentoRepository.cs
│   │   ├── SentimentosRepository.cs
│   │   └── RelatorioRepository.cs
│   ├── Models/                # Entidades e DTOs
│   │   ├── Users.cs
│   │   ├── Equipe.cs
│   │   ├── Reconhecimento.cs
│   │   ├── Sentimento.cs
│   │   ├── RelatorioPessoa.cs
│   │   ├── RelatorioEquipe.cs
│   │   └── DTOs/
│   ├── ML/                    # Machine Learning
│   │   └── EngajamentoData.cs
│   ├── Data/                  # Contexto e Mappings
│   │   ├── OracleDbContext.cs
│   │   └── Mappings/
│   ├── Migrations/            # Migrações EF Core
│   ├── Infrastructure/        # Tracing, HealthChecks
│   ├── Program.cs             # Ponto de entrada
│   ├── appsettings.json       # Configurações
│   └── auraplus-ml-model.zip  # Modelo ML treinado
│
├── AuraPlus.Test/             # Testes unitários (xUnit)
│   └── MLPredictionServiceTests.cs
│
├── AuraPlus.Trainer/          # Treinamento ML.NET
│   ├── Program.cs             # Treina modelo com FastTree
│   └── EngajamentoData.cs     # Classes de dados ML
│
└── AuraPlus.sln               # Solution principal
```

## 📊 Estrutura de Endpoints

### Autenticação
```
POST   /api/v1/Auth/register                # Registro de usuário (anônimo)
POST   /api/v1/Auth/login                   # Login (anônimo)
GET    /api/v1/Auth/me                      # Obter perfil (autenticado)
PUT    /api/v1/Auth/me                      # Atualizar perfil (autenticado)
DELETE /api/v1/Auth/me                      # Soft delete (autenticado)
```

### Equipes
```
POST   /api/v1/Equipe                       # Criar equipe (vira GESTOR)
GET    /api/v1/Equipe                       # Listar todas as equipes
GET    /api/v1/Equipe/{id}                  # Obter equipe específica
PUT    /api/v1/Equipe/{id}                  # Atualizar equipe (apenas GESTOR)
DELETE /api/v1/Equipe/{id}                  # Deletar equipe (apenas GESTOR)
POST   /api/v1/Equipe/{id}/entrar           # Entrar em equipe (vira EMPREGADO)
POST   /api/v1/Equipe/sair                  # Sair da equipe (vira NOVO_USUARIO)
```

### Gestão de Membros (GESTOR)
```
POST   /api/v1/Equipe/membros               # Adicionar membro à sua equipe
DELETE /api/v1/Equipe/membros/{membroId}    # Remover membro da sua equipe
```

### Reconhecimentos
```
POST   /api/v1/Reconhecimento               # Criar reconhecimento (1 por dia)
GET    /api/v1/Reconhecimento/{id}          # Obter reconhecimento específico
GET    /api/v1/Reconhecimento/enviados      # Listar reconhecimentos enviados
GET    /api/v1/Reconhecimento/recebidos     # Listar reconhecimentos recebidos
DELETE /api/v1/Reconhecimento/{id}          # Deletar reconhecimento próprio
```

### Sentimentos
```
POST   /api/v1/Sentimentos                  # Registrar sentimento (1 por dia)
GET    /api/v1/Sentimentos/{id}             # Obter sentimento específico
GET    /api/v1/Sentimentos/meus             # Listar meus sentimentos
DELETE /api/v1/Sentimentos/{id}             # Deletar sentimento próprio
```

### Relatórios (com ML.NET)
```
POST   /api/v1/Relatorio/pessoa             # Gerar relatório pessoal (30 dias)
POST   /api/v1/Relatorio/equipe/{id}        # Gerar relatório de equipe (30 dias)
GET    /api/v1/Relatorio/pessoa/{id}        # Obter relatório pessoa por ID
GET    /api/v1/Relatorio/equipe/{id}        # Obter relatório equipe por ID
GET    /api/v1/Relatorio/pessoa/historico   # Histórico de relatórios pessoais
GET    /api/v1/Relatorio/equipe/historico/{equipeId}  # Histórico equipe
```

## 🤖 Machine Learning - Predição de Engajamento

### Visão Geral

O AuraPlus utiliza **ML.NET 5.0** com algoritmo **FastTree** (regressão) para prever o nível de engajamento de equipes e pessoas com base em métricas coletadas.

### Métricas Utilizadas

O modelo analisa 5 variáveis principais:
1. **Número de Membros**: Tamanho da equipe
2. **Reconhecimentos/Mês**: Total de reconhecimentos no período
3. **Sentimento Médio**: Pontuação média (0-10) dos sentimentos
4. **Taxa de Participação**: % de membros ativos
5. **Dias Ativos**: Período de análise (30 dias)

### Classificação de Engajamento

O modelo retorna uma pontuação de 0-100% classificada em:

| Pontuação | Classificação | Descrição |
|-----------|---------------|-----------|
| ≥ 90% | **Excelente** | Equipe altamente engajada! |
| ≥ 75% | **Bom** | Equipe com engajamento saudável |
| ≥ 60% | **Moderado** | Requer atenção para melhorias |
| ≥ 45% | **Baixo** | Necessita intervenção urgente |
| < 45% | **Crítico** | Situação requer ação imediata |

### Treinamento do Modelo

O modelo foi treinado com 30 amostras sintéticas representando diferentes cenários:

```bash
cd AuraPlus.Trainer
dotnet run

# Saída:
=== AuraPlus ML Model Trainer ===
Total de amostras de treinamento: 30
✓ Modelo treinado com sucesso!
✓ Modelo salvo como 'auraplus-ml-model.zip'

📊 Equipe de Alto Desempenho:
   ➜ Nível de Engajamento Previsto: 91,57%

📊 Equipe Moderada:
   ➜ Nível de Engajamento Previsto: 67,27%

📊 Equipe Crítica:
   ➜ Nível de Engajamento Previsto: 26,75%
```

### Integração na API

O `MLPredictionService` carrega o modelo treinado e fornece predições:

```csharp
// Injetado como Singleton para reutilizar modelo
builder.Services.AddSingleton<MLPredictionService>();

// Uso no RelatorioService
var engajamentoPrevisto = _mlPredictionService.PreverEngajamentoEquipe(
    numeroMembros: 15,
    reconhecimentosMes: 42,
    sentimentoMedio: 8.5f,
    taxaParticipacao: 90.0f,
    diasAtivos: 30
);
// Retorna: 91.57%
```

### Recomendações Automáticas

O serviço também gera sugestões baseadas nas métricas:

- ⚠️ "Considere realizar atividades de team building" (engajamento < 60%)
- 😔 "Sentimento da equipe está baixo - agende conversas individuais" (sentimento < 6.0)
- 👏 "Incentive mais reconhecimentos entre os membros" (reconhecimentos < 10)
- 🎉 "Excelente trabalho! Continue mantendo este nível" (engajamento ≥ 90%)

### Exemplo de Relatório com ML

```json
POST /api/v1/Relatorio/equipe/1

{
  "id": 15,
  "data": "2025-11-20T10:30:00",
  "sentimentoMedio": "Excelente",
  "descritivo": "📊 Análise ML: Excelente - Equipe altamente engajada!\n\nEquipe com 15 membros. Reconhecimentos no mês: 42. Taxa de participação: 90.0%.\nSentimento médio: 8.50/10 (Excelente).\n\n🎯 Engajamento previsto: 91.57%\n\n💡 Recomendações:\n🎉 Excelente trabalho! Continue mantendo este nível",
  "idEquipe": 1
}
```

## 📝 Exemplos de Uso

### Registrar Usuário
```json
POST /api/v1/Auth/register
{
  "nome": "João Silva",
  "email": "joao@empresa.com",
  "senha": "SenhaSegura123!"
}
```

### Login
```json
POST /api/v1/Auth/login
{
  "email": "joao@empresa.com",
  "senha": "SenhaSegura123!"
}
```

### Resposta com Token JWT
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@empresa.com",
    "role": "NOVO_USUARIO"
  }
}
```

### Criar Equipe (Torna-se GESTOR)
```json
POST /api/v1/Equipe
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "nmTime": "Desenvolvimento Backend",
  "descricao": "Equipe responsável pela API",
  "cargo": "Tech Lead",
  "dataAdmissao": "2025-11-20"
}
```

### Gestor Adiciona Membro
```json
POST /api/v1/Equipe/membros
Authorization: Bearer <token_do_gestor>

{
  "membroId": 5,
  "cargo": "Desenvolvedor Pleno",
  "dataAdmissao": "2025-11-20"
}
```

## 🔧 Como Executar

### Pré-requisitos

- .NET 9.0 SDK
- Docker (para Jaeger/tracing)
- Acesso ao Oracle Database (oracle.fiap.com.br:1521/orcl)
- Git (opcional)

### Passos

1. **Clone o repositório**
```bash
git clone https://github.com/arthurspedine/gs-auraplus.git
cd gs-auraplus/dotnet
```

2. **Configure o banco de dados**

O arquivo `appsettings.json` já está configurado com as credenciais:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=rm555061;Password=201005;Data Source=oracle.fiap.com.br:1521/orcl"
  }
}
```

3. **Execute as migrações**
```bash
cd AuraPlus.Web
dotnet ef database update
```

4. **Execute a aplicação**
```bash
dotnet run
```

5. **(Opcional) Treinar modelo ML**

Se desejar re-treinar o modelo de Machine Learning:

```bash
cd ../AuraPlus.Trainer
dotnet run
# Copia automaticamente auraplus-ml-model.zip para AuraPlus.Web/
```

6. **Acesse a documentação**
- Swagger UI: `http://localhost:5186/swagger/index.html`
- API Base v1.0: `http://localhost:5186/api/v1`
- HealthCheck: `http://localhost:5186/health`

## 🧪 Executar Testes Unitários

O projeto inclui 5 testes unitários focados na lógica de classificação de engajamento:

```bash
cd AuraPlus.Test
dotnet test

# Saída esperada:
Test summary: total: 5, failed: 0, succeeded: 5, skipped: 0
```

### Testes Implementados

1. ✅ `ClassificarEngajamento_QuandoNivelExcelente_DeveRetornarMensagemCorreta`
2. ✅ `ClassificarEngajamento_QuandoNivelBom_DeveRetornarMensagemCorreta`
3. ✅ `ClassificarEngajamento_QuandoNivelModerado_DeveRetornarMensagemCorreta`
4. ✅ `ClassificarEngajamento_QuandoNivelBaixo_DeveRetornarMensagemCorreta`
5. ✅ `ClassificarEngajamento_QuandoNivelCritico_DeveRetornarMensagemCorreta`

Todos os testes utilizam **xUnit** e validam a lógica de classificação do `MLPredictionService`.

## 📊 Como Visualizar o Tracing

A API está configurada com **OpenTelemetry** para exportar traces para o Jaeger.

### 1. Inicie o Jaeger com Docker

```bash
docker run -d --name jaeger \
  -e COLLECTOR_OTLP_ENABLED=true \
  -p 16686:16686 \
  -p 4318:4318 \
  jaegertracing/all-in-one:latest
```

**Portas:**
- `16686`: Interface web do Jaeger
- `4318`: Endpoint OTLP/HTTP para receber traces

### 2. Execute a API

```bash
cd AuraPlus.Web
dotnet run
```

A API automaticamente começa a enviar traces para `http://localhost:4318/v1/traces`.

### 3. Faça algumas requisições

```bash
# Registrar usuário
curl -X POST http://localhost:5186/api/v1/Auth/register \
  -H "Content-Type: application/json" \
  -d '{"nome":"Teste","email":"teste@email.com","senha":"Senha123!"}'

# Login
curl -X POST http://localhost:5186/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"teste@email.com","senha":"Senha123!"}'
```

### 4. Visualize os Traces no Jaeger

1. Abra o navegador em: **http://localhost:16686**
2. No dropdown **"Service"**, selecione **"AuraPlus.Api"**
3. Clique em **"Find Traces"**

**O que você verá:**
- ✅ Todas as requisições HTTP
- ✅ Queries SQL executadas pelo Entity Framework
- ✅ Chamadas a procedures Oracle
- ✅ Tempo de execução de cada operação
- ✅ Stack trace completo da requisição

### 5. Explorar um Trace

Clique em qualquer trace para ver:
- **Spans**: Cada operação (HTTP, SQL, lógica de negócio)
- **Duration**: Tempo gasto em cada span
- **Tags**: Metadados (status code, query SQL, etc)
- **Logs**: Eventos importantes durante a execução

### 6. Parar e Remover o Jaeger

```bash
docker stop jaeger && docker rm jaeger
```

## 🔐 Fluxo de Autenticação e Roles

### Ciclo de Vida do Usuário

```
1. REGISTRO
   ↓
NOVO_USUARIO (sem equipe)
   ↓
2a. CRIAR EQUIPE          2b. ENTRAR EM EQUIPE
    ↓                          ↓
GESTOR (líder)            EMPREGADO (membro)
    ↓                          ↓
3. SAIR DA EQUIPE         3. SAIR DA EQUIPE
    ↓                          ↓
NOVO_USUARIO              NOVO_USUARIO
```

### Permissões por Role

| Ação                     | NOVO_USUARIO | EMPREGADO | GESTOR |
|--------------------------|--------------|-----------|--------|
| Criar equipe             | ✅           | ❌        | ❌     |
| Entrar em equipe         | ✅           | ❌        | ❌     |
| Sair da equipe           | ❌           | ✅        | ✅*    |
| Atualizar equipe         | ❌           | ❌        | ✅     |
| Deletar equipe           | ❌           | ❌        | ✅**   |
| Adicionar membros        | ❌           | ❌        | ✅     |
| Remover membros          | ❌           | ❌        | ✅     |

\* Gestor só pode sair se não houver outros membros  
\** Equipe deve estar vazia ou ter apenas o gestor

### Stored Procedure

A API utiliza a procedure `prc_inserir_usuario` para registro:

```sql
CREATE OR REPLACE PROCEDURE prc_inserir_usuario (
    p_nome IN VARCHAR2,
    p_senha IN VARCHAR2,
    p_email IN VARCHAR2,
    p_role IN VARCHAR2,
    p_cargo IN VARCHAR2,
    p_id_equipe IN NUMBER,
    p_data_admissao IN TIMESTAMP
) AS
    v_email_valido NUMBER;
BEGIN
    -- Valida email
    v_email_valido := pkg_utils.fn_validar_email(p_email);
    
    IF v_email_valido = 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'Email inválido');
    END IF;
    
    -- Verifica duplicidade
    IF fn_email_existe(p_email) = 1 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Email já cadastrado');
    END IF;
    
    -- Insere usuário
    INSERT INTO t_arp_users (nome, senha, email, role, cargo, id_equipe, data_admissao, ativo)
    VALUES (p_nome, p_senha, p_email, p_role, p_cargo, p_id_equipe, p_data_admissao, '1');
    
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20003, 'Erro ao inserir usuário: ' || SQLERRM);
END;
```
