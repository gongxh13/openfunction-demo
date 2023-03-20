package main

import (
    "fmt"
	"context"
    dapr "github.com/dapr/go-sdk/client"
)

func main() {
	ctx := context.Background()
    client, err := dapr.NewClient()
    if err != nil {
        panic(err)
    }
    defer client.Close()
    
    resp, err := client.TryLockAlpha1(ctx, "lockstore", &dapr.LockRequest{
			LockOwner:         "random_id_abc123",
			ResourceID:      "my_file_name",
			ExpiryInSeconds: 60,
		})

    fmt.Println("First try lock result:%s", resp.Success)

    resp, err = client.TryLockAlpha1(ctx, "lockstore", &dapr.LockRequest{
		LockOwner:         "random_id_abc123",
		ResourceID:      "my_file_name",
		ExpiryInSeconds: 60,
	})

    fmt.Println("Second try lock result:%s", resp.Success)

    resp1, err1 := client.UnlockAlpha1(ctx, "lockstore", &dapr.UnlockRequest{
		LockOwner:    "random_id_abc123",
		ResourceID: "my_file_name",
	})

    fmt.Println("Release lock result:%s", resp1.Status)

    resp, err = client.TryLockAlpha1(ctx, "lockstore", &dapr.LockRequest{
		LockOwner:         "random_id_abc123",
		ResourceID:      "my_file_name",
		ExpiryInSeconds: 60,
	})

    fmt.Println("After release lock then try lock result:%s", resp.Success)
}
