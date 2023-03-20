package main

import (
    "fmt"
	"context"
    dapr "github.com/dapr/go-sdk/client"
)

func main() {
    client, err := dapr.NewClient()
    if err != nil {
        panic(err)
    }
    defer client.Close()
    
    resp, err := client.TryLockAlpha1(context.Background(), "lockstore", &dapr.LockRequest{
			LockOwner:         "random_id_abc123",
			ResourceID:      "my_file_name",
			ExpiryInSeconds: 60,
		})

    fmt.Println("First try lock result:%s", resp.Success)

    resp, err := client.TryLockAlpha1(context.Background(), "lockstore", &dapr.LockRequest{
		LockOwner:         "random_id_abc123",
		ResourceID:      "my_file_name",
		ExpiryInSeconds: 60,
	})

    fmt.Println("Second try lock result:%s", esp.Success)

    resp, err := client.UnlockAlpha1(context.Background(), "lockstore", &UnlockRequest{
		LockOwner:    "random_id_abc123",
		ResourceID: "my_file_name",
	})

    fmt.Println("Release lock result:%s", resp.Success)

    resp, err := client.TryLockAlpha1(context.Background(), "lockstore", &dapr.LockRequest{
		LockOwner:         "random_id_abc123",
		ResourceID:      "my_file_name",
		ExpiryInSeconds: 60,
	})

    fmt.Println("After release lock then try lock result:%s", resp.Success)
}
