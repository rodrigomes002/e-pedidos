# E-Pedidos Golang Version

Sistema de processamento de pedidos em Golang com Kafka e PostgreSQL.

## Estrutura

- `src/shared/`: Modelos compartilhados
- `src/producer/`: Worker produtor que gera pedidos e publica no Kafka
- `src/consumer/`: Worker consumidor que lê do Kafka e salva no PostgreSQL
- `database/`: Scripts SQL para criar tabelas e limpar dados

## Como executar

1. Certifique-se de ter Docker e Docker Compose instalados.

2. Execute o banco de dados:
   ```bash
   docker-compose up postgres
   ```

3. Em outro terminal, execute os scripts de criação de tabelas:
   ```bash
   docker-compose exec postgres psql -U postgres -d orders -f /docker-entrypoint-initdb.d/create-tables.sql
   ```

4. Execute toda a stack:
   ```bash
   docker-compose up
   ```

## Limpar dados

Para limpar os dados:
```bash
docker-compose exec postgres psql -U postgres -d orders -f /docker-entrypoint-initdb.d/cleanup.sql
```