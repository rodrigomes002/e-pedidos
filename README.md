# e-pedidos

Sistema de processamento de pedidos em .NET com arquitetura event-driven.

## Arquitetura

- **Producer**: Worker que gera eventos de pedidos e envia para Kafka
- **Consumer**: Worker que consome eventos de pedidos do Kafka e salva no PostgreSQL
- **Kafka**: Message broker para comunicação entre producer e consumer
- **PostgreSQL**: Banco de dados para persistência de pedidos

## Como executar

1. Certifique-se de que Docker está instalado e em execução
2. Na raiz do projeto, execute:

   ```bash
   docker compose up --build
   ```

3. O Producer começará a gerar pedidos de exemplo a cada 5 segundos
4. O Consumer receberá esses pedidos e os salvará no PostgreSQL

## Scripts do Banco de Dados

Na pasta `database/` você encontrará scripts SQL para:

- **`create-tables.sql`**: Cria as tabelas `Orders` e `OrderItems` com índices
- **`seed-data.sql`**: Insere dados de exemplo para teste
- **`cleanup.sql`**: Limpa todas as tabelas (reset do banco)

Para executar os scripts manualmente:

```bash
# Conectar ao PostgreSQL
psql -h localhost -p 5432 -U epedidos -d epedidos

# Executar scripts
\i database/create-tables.sql
\i database/seed-data.sql
```

## Serviços

- **Zookeeper**: Coordenador do Kafka (porta interna)
- **Kafka**: Message broker (porta 9092)
- **PostgreSQL**: Banco de dados (porta 5432)
  - User: `epedidos`
  - Password: `epedidos123`
  - Database: `epedidos`

## Stack

- .NET 8
- EF Core
- PostgreSQL 16
- Kafka + Zookeeper
- Docker & Docker Compose
