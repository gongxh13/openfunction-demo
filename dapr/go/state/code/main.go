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

	err = client.SaveState(ctx, STATE_STORE_NAME, "order_1", []byte(strconv.Itoa(1)), nil)
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

	err = client.SaveBulkState(ctx, STATE_STORE_NAME, &dapr.SetStateItem{
		Key:   "order_1",
		Value: []byte(strconv.Itoa(1)),
	}, &dapr.SetStateItem{
		Key:   "order_2",
		Value: []byte(strconv.Itoa(2)),
	})

	if err != nil {
		log.Println("Bulk save err:", err.Error())
		panic(err)
	}

	result_bulk, err := client.GetBulkState(ctx, STATE_STORE_NAME, []string{"order_1", "order_2"}, nil, 1)
	for i := 0; i < len(result_bulk); i++ {
		log.Println("Bulk get key: ", result_bulk[i].Key, ", value: ", string(result_bulk[i].Value), " etag: ", result_bulk[i].Etag)
	}

	err = client.DeleteBulkState(ctx, STATE_STORE_NAME, []string{"order_1", "order_2"}, nil)
	if err != nil {
		log.Println("Delete bulk err:", err.Error())
		panic(err)
	}

	result_bulk, err = client.GetBulkState(ctx, STATE_STORE_NAME, []string{"order_1", "order_2"}, nil, 1)
	if err != nil {
		log.Println("Get bulk err:", err.Error())
		panic(err)
	}
	for i := 0; i < len(result_bulk); i++ {
		log.Println("Bulk get key: ", result_bulk[i].Key, ", value: ", result_bulk[i].Value)
	}
}
