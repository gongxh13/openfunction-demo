package dev.openfunction.samples;

import dev.openfunction.functions.OpenFunction;
import dev.openfunction.functions.Context;
import dev.openfunction.functions.Out;

public class OpenFunctionImpl implements OpenFunction {

    @Override
    public Out accept(Context context, String payload) throws Exception {
        System.out.printf("receive event: %s", payload).println();

        if (context.getOutputs() != null) {
            for (String key : context.getOutputs().keySet()) {
                Error error = context.send(key, payload);
                if (error != null) {
                    System.out.printf("send to output %s error, %s", key, error.getMessage()).println();
                } else {
                    System.out.printf("send to output %s", key).println();
                }
            }
        }
        return new Out();
    }
}