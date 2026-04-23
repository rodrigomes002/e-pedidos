package shared

import "time"

type Order struct {
	Id         int       `json:"id"`
	CustomerId int       `json:"customerId"`
	ProductId  int       `json:"productId"`
	Quantity   int       `json:"quantity"`
	OrderDate  time.Time `json:"orderDate"`
}