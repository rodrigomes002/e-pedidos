module github.com/e-pedidos/consumer

go 1.21

require (
	github.com/Shopify/sarama v1.38.1
	github.com/e-pedidos/shared v0.0.0
	github.com/lib/pq v1.10.9
)

replace github.com/e-pedidos/shared => ../shared