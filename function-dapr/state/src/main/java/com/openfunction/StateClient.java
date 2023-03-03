package com.openfunction;

import dev.openfunction.functions.HttpFunction;
import dev.openfunction.functions.HttpRequest;
import dev.openfunction.functions.HttpResponse;
import io.dapr.client.DaprClient;
import io.dapr.client.DaprClientBuilder;
import io.dapr.client.domain.State;
import io.dapr.exceptions.DaprException;
import io.grpc.Status;
import reactor.core.publisher.Mono;

public class StateClient implements HttpFunction {
    private static final String STATE_STORE_NAME = "statestore";
    private static final String FIRST_KEY_NAME = "myKey";
    @Override
    public void service(HttpRequest request, HttpResponse response) throws Exception {
        try (DaprClient client = new DaprClientBuilder().build()) {
            System.out.println("Waiting for Dapr sidecar ...");
            client.waitForSidecar(10000).block();
            System.out.println("Dapr sidecar is ready.");
            MyClass myClass = new MyClass();
            myClass.message = "test";

            client.saveState(STATE_STORE_NAME, FIRST_KEY_NAME, myClass).block();
            System.out.println("Saving class with message: " + "test");

            Mono<State<MyClass>> retrievedMessageMono = client.getState(STATE_STORE_NAME, FIRST_KEY_NAME, MyClass.class);
            System.out.println("Retrieved class message from state: " + (retrievedMessageMono.block().getValue()).message);

            // delete state API
            try {
                System.out.println("Trying to delete again with correct etag.");
                String storedEtag = client.getState(STATE_STORE_NAME, FIRST_KEY_NAME, MyClass.class).block().getEtag();
                client.deleteState(STATE_STORE_NAME, FIRST_KEY_NAME, storedEtag, null).block();
            } catch (DaprException ex) {
                if (ex.getErrorCode().equals(Status.Code.ABORTED.toString())) {
                    // Expected error due to etag mismatch.
                    System.out.println(String.format("Expected failure. %s", ex.getErrorCode()));
                } else {
                    System.out.println("Unexpected exception.");
                    throw ex;
                }
            }
            System.out.println("Done");
        }
        response.getWriter().write("Hello World");
    }

    public static class MyClass {
        public String message;

        @Override
        public String toString() {
        return message;
        }
    }
}