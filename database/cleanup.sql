-- Script para limpar todas as tabelas (reset do banco)
-- Execute com cuidado - irá remover todos os dados!

-- Desabilitar temporariamente as constraints de FK
ALTER TABLE "OrderItems" DROP CONSTRAINT IF EXISTS "FK_OrderItems_Orders_OrderId";

-- Limpar tabelas (ordem importa devido às FKs)
DELETE FROM "OrderItems";
DELETE FROM "Orders";

-- Recriar a constraint de FK
ALTER TABLE "OrderItems"
ADD CONSTRAINT "FK_OrderItems_Orders_OrderId"
FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE;

-- Reset das sequences se houver (não aplicável para UUIDs)

-- Verificar se as tabelas estão vazias
SELECT 'Orders count:' as table_name, COUNT(*) as record_count FROM "Orders"
UNION ALL
SELECT 'OrderItems count:', COUNT(*) FROM "OrderItems";