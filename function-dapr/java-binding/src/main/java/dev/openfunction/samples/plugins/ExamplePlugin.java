package dev.openfunction.samples.plugins;

import dev.openfunction.functions.Context;
import dev.openfunction.functions.Plugin;

import java.text.SimpleDateFormat;
import java.util.Date;

public class ExamplePlugin implements Plugin {
    @Override
    public String name() {
        return "plugin-example";
    }

    @Override
    public String version() {
        return "v1.0.0";
    }

    @Override
    public Plugin init() {
        return this;
    }

    @Override
    public Error execPreHook(Context ctx) {
        execHook(ctx, "pre");
        return null;
    }

    @Override
    public Error execPostHook(Context ctx) {
        execHook(ctx, "post");
        return null;
    }

    private void execHook(Context ctx, String type) {
        String ts = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.XXX").format(new Date());
        if (ctx.getBindingEvent() != null) {
            System.out.printf("plugin %s:%s exec %s hook for binding %s at %s", name(), version(), type, ctx.getBindingEvent().getName(), ts).println();
        } else if (ctx.getTopicEvent() != null) {
            System.out.printf("plugin %s:%s exec %s hook for pubsub %s at %s", name(), version(), type, ctx.getTopicEvent().getName(), ts).println();
        } else if (ctx.getHttpRequest() != null) {
            if (ctx.getCloudEvent() != null) {
                System.out.printf("plugin %s:%s exec %s hook for cloudevent function at %s", name(), version(), type, ts).println();
            } else {
                System.out.printf("plugin %s:%s exec %s hook for http function at %s", name(), version(), type, ts).println();
            }
        } else  {
           System.out.println("unknown function type");
        }
    }

    @Override
    public Object getField(String fieldName) {
        return null;
    }
}