package main

import (
	"context"
	"log"
	"math/rand"
	"strconv"
	"time"

	dapr "github.com/dapr/go-sdk/client"
)

func main() {
	const STATE_STORE_NAME = "statestore"
	rand.Seed(time.Now().UnixMicro())
	ctx := context.Background()
    client, err := dapr.NewClient()
    if err != nil {
        panic(err)
    }
    defer client.Close()

	err = client.SaveState(ctx, STATE_STORE_NAME, "order_1", []byte(strconv.Itoa(orderId)), nil)
	if err != nil {
		panic(err)
	}
	result, err := client.GetState(ctx, STATE_STORE_NAME, "order_1", nil)
	if err != nil {
		panic(err)
	}
	log.Println("Result after get:", string(result.Value))

	if err = client.DeleteState(ctx, STATE_STORE_NAME, "order_1", nil); err != nil {
        panic(err)
    }
}
