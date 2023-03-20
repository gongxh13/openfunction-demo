package com.example;

import io.dapr.client.DaprClient;
import io.dapr.client.DaprClientBuilder;
import io.dapr.client.domain.State;
import io.dapr.client.domain.TransactionalStateOperation;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class StateApp {
    private static final String STATE_STORE_NAME = "lockstore";

    public static void main(String[] args) {
        try (DaprClient client = new DaprClientBuilder().build()) {
            System.out.println("Waiting for Dapr sidecar ...");
            client.waitForSidecar(10000).block();
            System.out.println("Dapr sidecar is ready.");

            // save value to key
            client.saveState(STATE_STORE_NAME, "order_1", Integer.toString(1)).block();

            // get value from key
            Mono<io.dapr.client.domain.State<String>> result = client.getState(STATE_STORE_NAME, "order_1", String.class);
            System.out.println("Get order_1 value is " + result.block());

            String storedEtag = client.getState(STATE_STORE_NAME, "order_1", String.class).block().getEtag();
            client.deleteState(STATE_STORE_NAME, "order_1", storedEtag, null).block();
            System.out.println("Delete order_1 value");

            // get value from key
            result = client.getState(STATE_STORE_NAME, "order_1", String.class);
            System.out.println("Get order_1 value after delete is " + result.block());

            // save value to key
            client.saveState(STATE_STORE_NAME, "order_1", Integer.toString(1)).block();
            // save value to key
            client.saveState(STATE_STORE_NAME, "order_2", Integer.toString(2)).block();
            List<io.dapr.client.domain.State<String>> resultBulk = client.getBulkState(STATE_STORE_NAME,
                    Arrays.asList("order_1", "order_2"), String.class).block();
            System.out.println("Bulk get state result is " + resultBulk.toString());

            List<TransactionalStateOperation<?>> operationList = new ArrayList<>();
            operationList.add(new TransactionalStateOperation<>(TransactionalStateOperation.OperationType.UPSERT,
                    new State<>("order_3", Integer.toString(1), "")));
            operationList.add(new TransactionalStateOperation<>(TransactionalStateOperation.OperationType.DELETE,
                    new State<>("order_2")));
            //Using Dapr SDK to perform the state transactions
            client.executeStateTransaction(STATE_STORE_NAME, operationList).block();
            System.out.println("Bulk transaction succeed");
        } catch (Exception e) {
            System.out.printf("Exception %s%n", e.getMessage());
        }
    }
}
