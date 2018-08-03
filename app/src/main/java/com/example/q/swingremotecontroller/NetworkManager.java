package com.example.q.swingremotecontroller;

import org.json.JSONException;
import org.json.JSONObject;

import java.net.URISyntaxException;

import io.socket.client.IO;
import io.socket.client.Socket;
import com.google.gson.JsonObject;
import io.socket.emitter.Emitter;

public class NetworkManager {
    public Socket mSocket;
    private String url = "http://52.231.66.226:8080";

    public void init(){
        initSocket();
        //mSocket.on("Response", .onResponse);
        mSocket.connect();
        System.out.println("Connected, now sending role");

        /*JSONObject json = new JSONObject();
        try {
            json.put("role", "CONTROL");
            json.put("side", 0);
        } catch (JSONException e) {
            e.printStackTrace();
        }*/

        JsonObject json = new JsonObject();
        json.addProperty("role", "CONTROL");
        json.addProperty("side", 0); // 0 = left

        System.out.println(json.toString());
        mSocket.emit("SetRole", json.toString());

        System.out.println("Hi");
        //mSocket.emit("connection", "wow");
    }


    protected void initSocket() {
        // 설명의 편의를 위해 onCreate()메서드에 추가하였으나,
        // 꼭 onCreate() 메서드에 위치할 필요는 없을 것 같습니다.
        try {
            mSocket = IO.socket(url);
            System.out.println("Socket made");
        } catch(URISyntaxException e) {
            e.printStackTrace();
        }
    }




}
