package main

import (
	"encoding/json"
	"fmt"
	"log"
	"math/rand"
	"time"

	"github.com/Shopify/sarama"
	"github.com/e-pedidos/shared"
)

func main() {
	producer, err := sarama.NewSyncProducer([]string{"kafka:9092"}, nil)
	if err != nil {
		log.Fatal(err)
	}
	defer producer.Close()

	for {
		order := shared.Order{
			Id:         rand.Intn(10000),
			CustomerId: rand.Intn(100),
			ProductId:  rand.Intn(100),
			Quantity:   rand.Intn(10) + 1,
			OrderDate:  time.Now(),
		}

		orderJSON, _ := json.Marshal(order)
		msg := &sarama.ProducerMessage{
			Topic: "orders",
			Value: sarama.StringEncoder(orderJSON),
		}

		_, _, err := producer.SendMessage(msg)
		if err != nil {
			log.Printf("Failed to send message: %v", err)
		} else {
			fmt.Printf("Produced order: %v\n", order)
		}

		time.Sleep(5 * time.Second)
	}
}