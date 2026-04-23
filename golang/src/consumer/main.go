package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"

	"github.com/Shopify/sarama"
	"github.com/e-pedidos/shared"
	_ "github.com/lib/pq"
)

func main() {
	db, err := sql.Open("postgres", "host=postgres port=5432 user=postgres password=postgres dbname=postgres sslmode=disable")
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	consumer, err := sarama.NewConsumer([]string{"kafka:9092"}, nil)
	if err != nil {
		log.Fatal(err)
	}
	defer consumer.Close()

	partitionConsumer, err := consumer.ConsumePartition("orders", 0, sarama.OffsetNewest)
	if err != nil {
		log.Fatal(err)
	}
	defer partitionConsumer.Close()

	for msg := range partitionConsumer.Messages() {
		var order shared.Order
		err := json.Unmarshal(msg.Value, &order)
		if err != nil {
			log.Printf("Failed to unmarshal: %v", err)
			continue
		}

		_, err = db.Exec("INSERT INTO orders (id, customer_id, product_id, quantity, order_date) VALUES ($1, $2, $3, $4, $5)",
			order.Id, order.CustomerId, order.ProductId, order.Quantity, order.OrderDate)
		if err != nil {
			log.Printf("Failed to insert: %v", err)
		} else {
			fmt.Printf("Consumed and inserted order: %v\n", order)
		}
	}
}