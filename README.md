# 💰 API de Gerenciamento de Faturamentos | .NET 8

API REST desenvolvida em **C# com ASP.NET Core 8**, destinada ao gerenciamento de clientes, faturamentos e parcelamentos, com autenticação JWT e persistência de dados em MySQL.

O projeto implementa regras de negócio para controle financeiro, geração automática de parcelas, acompanhamento de pagamentos e gerenciamento de comprovantes.

## 🚀 Tecnologias utilizadas

- **C# / .NET 8** – Desenvolvimento da API REST.
- **ASP.NET Core Web API** – Construção dos endpoints e gerenciamento das requisições HTTP.
- **Dapper** – Micro ORM para execução de consultas SQL parametrizadas.
- **MySQL** – Banco de dados relacional.
- **MySqlConnector** – Conectividade com o banco de dados.
- **JWT (JSON Web Token)** – Autenticação e autorização de requisições.
- **Swagger / OpenAPI** – Documentação e testes dos endpoints.
- **Dependency Injection** – Gerenciamento e injeção de dependências.

## 📌 Funcionalidades

### 🔐 Autenticação e usuários

- Cadastro de usuários com validação de dados.
- Verificação de e-mail já cadastrado.
- Armazenamento de senhas utilizando `PasswordHasher`.
- Autenticação por e-mail e senha.
- Geração de tokens JWT com validade de 8 horas.
- Proteção de endpoints privados utilizando `[Authorize]`.

### 👥 Gerenciamento de clientes

- Cadastro de clientes vinculados ao usuário autenticado.
- Atualização e exclusão de clientes.
- Consulta de clientes com filtros opcionais por nome, e-mail e documento.
- Verificação de duplicidade de e-mail por usuário.
- Validação de CPF/CNPJ através de um validador customizado.
- Normalização de documentos, removendo caracteres não numéricos.

### 💵 Gerenciamento de faturamentos

- Criação de faturamentos vinculados a clientes.
- Definição de título, descrição, valor e quantidade de parcelas.
- Validação da existência do cliente antes de gerar o faturamento.
- Inicialização do faturamento com status pendente.
- Consulta dos faturamentos de determinado cliente.
- Consulta detalhada dos parcelamentos.
- Cancelamento de faturamentos e suas respectivas parcelas.

### 📊 Controle de parcelamentos

- Geração automática de parcelas durante a criação do faturamento.
- Distribuição do valor total entre as parcelas.
- Inicialização das parcelas com status pendente.
- Atualização individual do status de cada parcela.
- Atualização automática do status geral do faturamento conforme a situação das parcelas.

**Regras de negócio dos status:**

| Status | Valor | Descrição |
|---|---:|---|
| Pendente | 1 | Pagamento aguardando confirmação |
| Pago | 2 | Pagamento realizado |
| Cancelado | 3 | Parcela ou faturamento cancelado |

O faturamento passa para:
- **Pago:** quando todas as parcelas não canceladas estiverem pagas.
- **Cancelado:** quando todas as parcelas estiverem canceladas.
- **Pendente:** enquanto houver parcelas pendentes ou outras combinações de status.

O valor do faturamento também é recalculado considerando somente as parcelas não canceladas.

### 📎 Comprovantes de pagamento

- Upload de comprovantes através de requisições `multipart/form-data`.
- Geração de nomes únicos utilizando GUID.
- Armazenamento local dos arquivos no diretório `uploads/comprovantes`.
- Vinculação do nome do arquivo ao respectivo parcelamento.

## 🏗️ Arquitetura e organização

O projeto utiliza uma organização em camadas, separando responsabilidades entre Controllers, Services, Models, Data e Helpers.

```text
faturamentoClienteDotnet/
│
├── Controllers/
│   ├── LoginController.cs
│   ├── ClienteController.cs
│   ├── FaturamentoController.cs
│   └── BaseController.cs
│
├── Services/
│   ├── UsuarioService.cs
│   ├── ClienteService.cs
│   ├── FaturamentoService.cs
│   └── JwtService.cs
│
├── Models/
│   ├── UsuarioModel.cs
│   ├── ClienteModel.cs
│   ├── FaturamentoModel.cs
│   ├── ParcelamentoModel.cs
│   ├── LoginModel.cs
│   └── CadastroModel.cs
│
├── Data/
│   └── Database.cs
│
├── DependencyInjection/
│   └── ServiceCollectionExtensions.cs
│
├── Helpers/
│   └── ResponsesHelper.cs
│
├── Enums/
│   └── StatusFaturamento.cs
│
├── Validators/
│   └── DocumentoAttribute.cs
│
├── uploads/
│   └── comprovantes/
│
├── Program.cs
└── appsettings.json
```

### Responsabilidades das camadas

- **Controllers:** recebem as requisições HTTP, validam os dados, aplicam as regras de acesso e coordenam as operações.
- **Services:** concentram as operações de negócio e a comunicação com o banco de dados.
- **Models:** representam as entidades e os modelos utilizados na aplicação.
- **Data:** centraliza a configuração e criação das conexões com o MySQL.
- **Helpers:** disponibilizam métodos auxiliares, incluindo a padronização das respostas da API.
- **Validators:** implementam validações customizadas, como CPF e CNPJ.
- **DependencyInjection:** centraliza o registro das dependências da aplicação.

## 🔄 Regras de negócio e implementação técnica

Algumas implementações importantes do projeto:

- Utilização de consultas SQL parametrizadas com Dapper.
- Construção dinâmica de consultas para permitir filtros opcionais na listagem de clientes.
- Isolamento dos dados por usuário autenticado, utilizando o identificador presente nas claims do JWT.
- Utilização de transações no MySQL para operações de persistência de múltiplos parcelamentos.
- Atualização em lote de parcelas através de consultas SQL.
- Utilização de `DynamicParameters` para construção de operações de atualização.
- Padronização das respostas HTTP através de um helper específico.
- Injeção de dependências para desacoplamento dos componentes.

## 🔌 Endpoints da API

### Autenticação

| Método | Endpoint | Descrição | Acesso |
|---|---|---|---|
| POST | `/api/login` | Autenticar usuário | Público |
| POST | `/api/login/cadastro` | Cadastrar usuário | Público |

### Clientes

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/cliente/cadastro_cliente` | Cadastrar cliente |
| POST | `/api/cliente/atualizar_cliente` | Atualizar cliente |
| POST | `/api/cliente/excluir_cliente/{clienteId}` | Excluir cliente |
| GET | `/api/cliente/lista_clientes` | Listar clientes com filtros |

### Faturamentos e parcelamentos

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/faturamento/salvar_faturamento` | Criar faturamento e parcelas |
| GET | `/api/faturamento/get_faturamentos_cliente/{clienteId}` | Consultar faturamentos do cliente |
| GET | `/api/faturamento/get_parcelamentos/{faturamentoId}` | Consultar parcelas |
| POST | `/api/faturamento/editar_status_parcelamento` | Atualizar status de uma parcela |
| POST | `/api/faturamento/cancelar_faturamento/{faturamentoId}` | Cancelar faturamento |
| POST | `/api/faturamento/salvar_comprovante_pagamento/{parcelamentoId}` | Enviar comprovante de pagamento |

Os endpoints de clientes, faturamentos e parcelamentos exigem autenticação via Bearer Token.

## ⚙️ Configuração e execução

### Pré-requisitos

- .NET SDK 8.0 ou superior.
- MySQL Server.
- Git.

### 1. Clonar o repositório

```bash
git clone https://github.com/douglasanschau/dotnetFaturamento.git

cd dotnetFaturamento
```

### 2. Configurar o banco de dados

Crie um banco de dados MySQL e configure as credenciais no arquivo `appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=seu_banco;Uid=seu_usuario;Pwd=sua_senha;"
  },
  "Jwt": {
    "Key": "sua_chave_secreta"
  }
}
```

A estrutura do banco deve conter as tabelas:

- `usuario`
- `cliente`
- `faturamento`
- `parcelamento`

As tabelas utilizam identificadores próprios e campos de relacionamento para vincular usuários, clientes, faturamentos e parcelas.

### 3. Restaurar dependências

```bash
dotnet restore
```

### 4. Executar a aplicação

```bash
dotnet run
```

### 5. Acessar a documentação Swagger

Após iniciar a aplicação em ambiente de desenvolvimento, acesse:

```text
http://localhost:5076/swagger/index.html
```

A porta pode variar conforme a configuração de execução do projeto.

## 🔑 Fluxo de autenticação

1. Realizar o cadastro através de `POST /api/login/cadastro`.
2. Efetuar login através de `POST /api/login`.
3. Receber o token JWT na resposta.
4. Configurar o token no Swagger utilizando o formato Bearer.
5. Realizar requisições aos endpoints privados.

```http
Authorization: Bearer SEU_TOKEN_JWT
```

## 🎯 Objetivo do projeto

Este projeto foi desenvolvido para praticar e aplicar conceitos de desenvolvimento backend com .NET, incluindo construção de APIs REST, autenticação e autorização, persistência de dados relacionais, consultas SQL, injeção de dependências e implementação de regras de negócio.

Também aborda situações comuns em sistemas financeiros, como controle de pagamentos, parcelamentos, cancelamentos e gerenciamento de comprovantes.

---

**Desenvolvido por Douglas Anschau**

[GitHub - douglasanschau](https://github.com/douglasanschau)
