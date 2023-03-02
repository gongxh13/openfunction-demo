package com.openfunction;

import dev.openfunction.functions.HttpFunction;
import dev.openfunction.functions.HttpRequest;
import dev.openfunction.functions.HttpResponse;
import java.io.BufferedWriter;
import java.io.IOException;

public class HelloWorld implements HttpFunction {
    @Override
    public void service(HttpRequest request, HttpResponse response) throws Exception {

        response.getWriter().write("Hello World");
    }
}