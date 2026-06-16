Aplicação Faturamento .NET 

- Aplicação de cadastro de usuários, clientes, faturamentos e parcelamentos correspondes e suas respectivas atualizações de dados.

### Inicialização do projeto 
Após download, rodar 'dotnet run' para inicializar. O projeto divide-se em dois tipos de rotas, as publicas e as privadas. O gerenciamento de rotas pode ser feito no arquivo: 
http://localhost:5076/swagger/index.html

### Rotas Públicas 
 - Login 


 ### Rotas Privadas 
  - Cliente 
  - Faturamento 


 *** Após fazer cadastro na rota de login e logar com usuário e senha, vai receber token de acesso que deve ser usado como Bearer Token para acesso as demais rotas. 

### Configuração JWT e Banco de Dados
Substituir no arquivo appstettings.json o Database, usuario e senha e criar a key jwt.


/////
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=${seu_database};Uid=${seu_usuario};Pwd=${sua_senha};"
  },
  "Jwt": {
    "Key": "${sua_key}"
  } 
/////

- Key JWT: Sugestão, colocar no terminal o comando "openssl rand -base64 32" e usar a chave gerada como key.  


### Schema Banco de Dados
CREATE TABLE `usuario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `senha` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci


CREATE TABLE `cliente` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `nome` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `documento` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci

CREATE TABLE `faturamento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `cliente_id` int DEFAULT NULL,
  `titulo` varchar(255) DEFAULT NULL,
  `descricao` mediumtext,
  `valor` decimal(10,2) DEFAULT NULL,
  `parcelas` int DEFAULT NULL,
  `status` tinyint DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci

CREATE TABLE `parcelamento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `faturamento_id` int DEFAULT NULL,
  `valor` decimal(10,2) DEFAULT NULL,
  `status` tinyint DEFAULT NULL,
  `comprovante_pagamento` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci


