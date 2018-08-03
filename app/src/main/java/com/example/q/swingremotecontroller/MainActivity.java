package com.example.q.swingremotecontroller;



import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.Button;

import org.json.JSONException;
import org.json.JSONObject;

import java.net.URISyntaxException;
import io.socket.client.IO;
import io.socket.client.Socket;
import io.socket.emitter.Emitter;

public class MainActivity extends AppCompatActivity {



    //Using the Accelometer & Gyroscoper
    private SensorManager mSensorManager = null;

    //Using the Accelometer
    private SensorEventListener mAccLis;
    private Sensor mAccelometerSensor = null;
    private View.OnTouchListener accListener = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            switch (event.getAction()){

                case MotionEvent.ACTION_DOWN:
                    mSensorManager.registerListener(mAccLis, mAccelometerSensor, SensorManager.SENSOR_DELAY_UI);
                    break;

                case MotionEvent.ACTION_UP:
                    mSensorManager.unregisterListener(mAccLis);
                    break;

            }
            return false;
        }
    };

    //Using the Gyroscope
    private SensorEventListener mGyroLis;
    private Sensor mGgyroSensor = null;
    private View.OnTouchListener gyroListener = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            switch (event.getAction()){

                case MotionEvent.ACTION_DOWN:
                    mSensorManager.registerListener(mGyroLis, mGgyroSensor, SensorManager.SENSOR_DELAY_UI);
                    break;

                case MotionEvent.ACTION_UP:
                    mSensorManager.unregisterListener(mGyroLis);
                    break;

            }
            return false;
        }
    };

    //Roll and Pitch
    private double pitch;
    private double roll;
    private double yaw;

    //timestamp and dt
    private double timestamp;
    private double dt;

    // for radian -> dgree
    private double RAD2DGR = 180 / Math.PI;
    private static final float NS2S = 1.0f/1000000000.0f;

    public NetworkManager nManager = null;
    public Socket mSocket = null;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);





        //Using the Gyroscope & Accelometer
        mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);

        //Using the Accelometer
        mGgyroSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);
        mGyroLis = new GyroscopeListener();

        mAccelometerSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        mAccLis = new AccelometerListener();


        final Button connect = findViewById(R.id.a_start);
        final Button touch = findViewById(R.id.touch);

        connect.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                v.setVisibility(View.GONE);
                nManager = new NetworkManager();
                nManager.init();
                mSocket = nManager.mSocket;
                touch.setVisibility(View.VISIBLE);
            }});

        //Touch Listener for Accelometer
        touch.setOnClickListener(new View.OnClickListener(){
           @Override
           public void onClick(View v){
               connect.setVisibility(View.GONE);
               v.setVisibility(View.VISIBLE);
           }
        });


        touch.setOnTouchListener(accListener);



    }

    @Override
    public void onPause(){
        super.onPause();
        Log.e("LOG", "onPause()");
        mSensorManager.unregisterListener(mGyroLis);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
        Log.e("LOG", "onDestroy()");
        mSensorManager.unregisterListener(mGyroLis);
    }
    public Emitter.Listener onResponse = new Emitter.Listener() {
        @Override
        public void call(final Object... args) {
            runOnUiThread(new Runnable() {
                @Override
                public void run() {

                    JSONObject obj = (JSONObject) args[0];
                    String result = null;
                    try{
                        result = obj.getString("result");
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                    if (result == "SUCCESS")
                    {

                    }
                    else if (result == "FAIL")
                    {

                    }

                }

            });
        }
    };

    private class AccelometerListener implements SensorEventListener {

        private boolean isFirst = true;
        private double prevX;
        private double thrust = 12;
        @Override
        public void onSensorChanged(SensorEvent event) {

            double accX = event.values[0];
            double accY = event.values[1];
            double accZ = event.values[2];

            double angleXZ = Math.atan2(accX,  accZ) * 180/Math.PI;
            double angleYZ = Math.atan2(accY,  accZ) * 180/Math.PI;

            if (isFirst){
                isFirst = false;
            }
            else {
                Log.e("LOG", "ACCELOMETER           [X]:" + String.format("%.4f", event.values[0])
                        + "           [Y]:" + String.format("%.4f", event.values[1])
                        + "           [Z]:" + String.format("%.4f", event.values[2])
                        + "           [angleXZ]: " + String.format("%.4f", angleXZ)
                        + "           [angleYZ]: " + String.format("%.4f", angleYZ));

                double weight = accX - prevX;
                if (weight < 0){
                    weight = -1 * weight;
                }
                if (weight > 7) {
                    //json.put("upper", 1);
                    int level = (int) ((weight - 7) / thrust);
                    if (level > 4)
                        level = 4;
                    System.out.println("level " + String.valueOf(level));
                    mSocket.emit("Shake", level);
                }
            }
            prevX = accX;

        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int accuracy) {

        }
    }

    private class GyroscopeListener implements SensorEventListener {

        private double prev_gyroZ;
        private double thrust = 2.5;
        @Override
        public void onSensorChanged(SensorEvent event) {

            /* 각 축의 각속도 성분을 받는다. */
            double gyroX = event.values[0];
            double gyroY = event.values[1];
            double gyroZ = event.values[2];

            /* 각속도를 적분하여 회전각을 추출하기 위해 적분 간격(dt)을 구한다.
             * dt : 센서가 현재 상태를 감지하는 시간 간격
             * NS2S : nano second -> second */
            dt = (event.timestamp - timestamp) * NS2S;
            timestamp = event.timestamp;

            /* 맨 센서 인식을 활성화 하여 처음 timestamp가 0일때는 dt값이 올바르지 않으므로 넘어간다. */
            if (dt - timestamp*NS2S != 0) {

                /* 각속도 성분을 적분 -> 회전각(pitch, roll)으로 변환.
                 * 여기까지의 pitch, roll의 단위는 '라디안'이다.
                 * SO 아래 로그 출력부분에서 멤버변수 'RAD2DGR'를 곱해주어 degree로 변환해줌.  */
                pitch = pitch + gyroY*dt;
                roll = roll + gyroX*dt;
                yaw = yaw + gyroZ*dt;

                Log.e("LOG", "GYROSCOPE           [X]:" + String.format("%.4f", event.values[0])
                        + "           [Y]:" + String.format("%.4f", event.values[1])
                        + "           [Z]:" + String.format("%.4f", event.values[2])
                        + "           [Pitch]: " + String.format("%.1f", pitch*RAD2DGR)
                        + "           [Roll]: " + String.format("%.1f", roll*RAD2DGR)
                        + "           [Yaw]: " + String.format("%.1f", yaw*RAD2DGR)
                        + "           [dt]: " + String.format("%.4f", dt));
                /*
                JSONObject json = new JSONObject();
                try{

                    json.put("x", event.values[0]);
                    json.put("y", event.values[1]);
                    json.put("z", event.values[2]);
                    json.put("pitch", pitch*RAD2DGR);
                    json.put("roll", roll*RAD2DGR);
                    json.put("yaw", yaw*RAD2DGR);
                    json.put("dt", dt);

                } catch (JSONException e){
                    e.printStackTrace();
                }
                */
                /*
                if (event.values[0] > 15 || event.values[2] > 15) {
                    //json.put("upper", 1);
                    System.out.println("asdf");
                    mSocket.emit("Swing", 1);
                }
                else if (event.values[0] < -12 || event.values[2] < -12) {
                    //json.put("upper", 0);
                    System.out.println("asdf2");
                    mSocket.emit("Swing", 0);
                }
                */
                double weight = event.values[2] - prev_gyroZ;
                if (weight < 0){
                    weight = -1 * weight;
                }
                if (weight > 10) {
                    //json.put("upper", 1);
                    int level = (int) ((weight - 10) / thrust);
                    if (level > 4)
                        level = 4;
                    System.out.println("asdf");
                    mSocket.emit("Shake", level);
                }
            }
            prev_gyroZ = gyroZ;
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int accuracy) {

        }
    }




}
