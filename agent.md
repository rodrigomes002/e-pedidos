## Sistema de pedidos
Sistema para processar milhões de pedidos

## Stack
Docker
Worker .NET LTS
EF Core
Postgresql
Kafka

## Arquitetura
Hexagonal + DDD Em camadas (Worker, Domain, Infra, Application)
Event Driven

## Modelo de Dominio
Customer
   ↓
Order ──── OrderItem
