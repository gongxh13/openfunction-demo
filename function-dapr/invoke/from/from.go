package from

import (
	"context"
	"log"

	ofctx "github.com/OpenFunction/functions-framework-go/context"

	dapr "github.com/dapr/go-sdk/client"
)

func FromFunc(ctx ofctx.Context, in []byte) (ofctx.Out, error) {
	log.Printf("event - Data: %s", in)
	ctxb := context.Background()
	client, err := dapr.NewClientWithAddress("function-dapr-invoke-from-default-dapr.default.svc.cluster.local:50001")
	if err != nil {
		panic(err)
	}
	//Using Dapr SDK
	res, err := client.InvokeMethod(ctxb, "function-dapr-invoke-to-default", "test", "get")
	if err != nil {
		log.Printf("err - invoke method: %s", err)
		panic(err)
	}
	log.Printf("call invoke method result is %s", string(res))
	return ctx.ReturnOnSuccess(), nil
}
