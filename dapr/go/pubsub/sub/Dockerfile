FROM golang:1.17-alpine as builder

ENV GO111MODULE=on
ENV GOPROXY=https://goproxy.cn,direct

WORKDIR /app

ADD . /app
RUN go mod download
RUN go mod tidy -e

RUN GOOS=linux CGO_ENABLED=0 GOARCH=amd64 go build -o /app/main ./main.go

FROM ubuntu:18.04 as runner

WORKDIR /go/app

COPY --from=builder /app/main .

EXPOSE 60034

RUN chmod +x ./main

ENTRYPOINT ["./main"]