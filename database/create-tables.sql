-- Scripts para criação das tabelas do sistema e-pedidos
-- Execute estes comandos no PostgreSQL (banco: epedidos)

-- Criar tabela Orders
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "CustomerName" text NOT NULL,
    "TotalAmount" numeric NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);

-- Criar tabela OrderItems
CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Sku" text NOT NULL,
    "Description" text NOT NULL,
    "UnitPrice" numeric NOT NULL,
    "Quantity" integer NOT NULL,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

-- Criar índices para melhor performance
CREATE INDEX IF NOT EXISTS "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");
CREATE INDEX IF NOT EXISTS "IX_Orders_CustomerId" ON "Orders" ("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Orders_CreatedAt" ON "Orders" ("CreatedAt");

-- Inserir alguns dados de exemplo (opcional)
-- INSERT INTO "Orders" ("Id", "CustomerId", "CustomerName", "TotalAmount", "CreatedAt") VALUES
-- ('550e8400-e29b-41d4-a716-446655440000', '550e8400-e29b-41d4-a716-446655440001', 'João Silva', 150.00, NOW()),
-- ('550e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440003', 'Maria Santos', 299.99, NOW());

-- INSERT INTO "OrderItems" ("Id", "OrderId", "Sku", "Description", "UnitPrice", "Quantity") VALUES
-- ('550e8400-e29b-41d4-a716-446655440010', '550e8400-e29b-41d4-a716-446655440000', 'PROD-001', 'Produto 1', 50.00, 2),
-- ('550e8400-e29b-41d4-a716-446655440011', '550e8400-e29b-41d4-a716-446655440000', 'PROD-002', 'Produto 2', 25.00, 1),
-- ('550e8400-e29b-41d4-a716-446655440012', '550e8400-e29b-41d4-a716-446655440002', 'PROD-003', 'Produto 3', 299.99, 1);