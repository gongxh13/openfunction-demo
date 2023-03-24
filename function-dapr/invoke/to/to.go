package to

import (
	"context"
	"log"
	"net/http"

	ofctx "github.com/OpenFunction/functions-framework-go/context"
	"github.com/dapr/go-sdk/service/common"
	daprd "github.com/dapr/go-sdk/service/http"
)

var s common.Service

func init() {
	s = daprd.NewService(":8080")
	if err := s.AddServiceInvocationHandler("test", testHandler); err != nil {
		log.Fatalf("add invoke: %v", err)
	}
	if err := s.Start(); err != nil && err != http.ErrServerClosed {
		log.Fatalf("error listenning: %v", err)
	}
}

func Entry(ctx ofctx.Context, in []byte) (ofctx.Out, error) {
	log.Printf("event - Data: %s", in)
	return ctx.ReturnOnSuccess(), nil
}

func testHandler(ctx context.Context, in *common.InvocationEvent) (out *common.Content, err error) {
	out = &common.Content{
		ContentType: in.ContentType,
		Data:        []byte("111"),
	}
	return
}
