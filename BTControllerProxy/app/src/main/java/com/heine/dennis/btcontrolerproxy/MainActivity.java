package com.heine.dennis.btcontrolerproxy;

import static android.view.MotionEvent.ACTION_DOWN;
import static android.view.MotionEvent.ACTION_MOVE;

import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.sql.Timestamp;
import java.util.Date;
import java.util.HashMap;

public class MainActivity extends AppCompatActivity  {


    @Override
    protected void onDestroy()
    {
        running=false;
        super.onDestroy();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        String filename="settings";
        FileInputStream fis = null;
        String line="";
        try {

            fis = getApplicationContext().openFileInput(filename);

        InputStreamReader inputStreamReader =
                new InputStreamReader(fis);
        StringBuilder stringBuilder = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(inputStreamReader)) {
          line = reader.readLine();

        } catch (IOException e) {
            // Error occurred when opening raw file for reading.
        } finally {
            String contents = line;
            Globals.serverIP=contents;
        }
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }

        ((TextView)findViewById(R.id.editTextTextPersonName)).setText(Globals.serverIP);
        Thread.setDefaultUncaughtExceptionHandler(h);
        new MyThread1().start();
        new PingThread().start();

    }


    public static boolean blocked=false;


    static Socket ds = null;
    static Socket ds1 = null;

    boolean button=false;

    public void sendMsgThrdUDP(byte[] msg, int port)
    {
        try {
            if ((!blocked && !button)||(!button)) {



                byte[] ipAddr = new byte[]{(byte) 192, (byte) 168, (byte) 2, (byte) 72};
                InetAddress addr = null;

                try {
                    addr = InetAddress.getByName(Globals.serverIP);
                } catch (UnknownHostException e) {
                    e.printStackTrace();
                }
                DatagramSocket ds = null;
                try {

                    ds = new DatagramSocket(port);
                    ds.setSoTimeout(50);
                } catch (SocketException e) {
                    e.printStackTrace();
                }
                DatagramPacket dp;


                dp = new DatagramPacket(msg, msg.length, addr, port);

                try {
                  //  ds.setBroadcast(true);
                } catch (Exception e) {
                    e.printStackTrace();
                }
                try {
                    ds.send(dp);
                    ds.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }

            }
        }catch (Exception e)
        {

        }
    }



    public void sendMsgThrd(byte[] msg, int port) {

        if(ds1==null)
            return;

        int errorCount = 0;
        boolean error = false;
        boolean begining = true;
        while ((error && errorCount < 2 && port==23002) || (error && errorCount < 20 && port==23002 && msg[1]==0)  || (begining && button==false && port==23000) || ((begining  && port==23002))) {
            try {
                if(port==23002 && msg[1]==1)
                    button=true;


                begining = false;
                if (!blocked || port==23002 || blocked) {

                    byte[] ipAddr = new byte[]{(byte) 192, (byte) 168, (byte) 2, (byte) 72};
                    InetAddress addr = null;

                    try {
                        addr = InetAddress.getByName(Globals.serverIP);
                    } catch (UnknownHostException e) {
                        e.printStackTrace();
                    }




                    try {
                    if(port==23000)
                        ds.getOutputStream().write(msg);
                    else
                        ds1.getOutputStream().write(msg);

                        ds1.setSoTimeout(1000);

                            int ret = ds1.getInputStream().read();

                            if(ret==-1&&msg[0]!=0)
                                throw new Exception();
                            else if(ret==-1)
                            {
                               socketClose=true;
                            }

                        }catch (Exception ste)
                        {
                            if(msg[0]==0)
                            {
                               socketClose=true;
                            }
                            else {
                                blocked = false;
                                sendMsgThrd(msg, port);
                            }
                        }

                    if(keyDown.containsKey((int)msg[0]) && port==23002)
                        keyDown.remove((int)msg[0]);


                    if(port==23002 && msg[1]==0)
                        button=false;
                }
            } catch (Exception e) {
                error = true;
                errorCount++;
                if(errorCount>=20 && button)
                    button=false;
            }
        }
        if((error && errorCount >= 2 && port==23002) || (error && errorCount >= 20 && port==23002 && msg[1]==0))
        {
            socketClose=true;
        }
        if(port==23002)
            blocked=false;
    }

    Thread.UncaughtExceptionHandler h = new Thread.UncaughtExceptionHandler() {
        @Override
        public void uncaughtException(Thread th, Throwable ex) {
            runOnUiThread(() -> Toast.makeText(MainActivity.this, ex.getMessage(), Toast.LENGTH_LONG).show());
        }
    };

    public void clickSetServerIP(View view) throws InterruptedException {


        String filename = "settings";
        String fileContents = ((TextView)findViewById(R.id.editTextTextPersonName)).getText().toString();
        try (FileOutputStream fos = openFileOutput(filename, Context.MODE_PRIVATE)) {
            fos.write(fileContents.getBytes());
        } catch (IOException e) {
            e.printStackTrace();
        }
        Globals.serverIP=fileContents;

        running=false;
        ds = null;
        try {
            if (ds1 != null)
                ds1.close();
        }catch (Exception e){}
        ds1 = null;


            Intent i = getApplicationContext().getPackageManager()
                    .getLaunchIntentForPackage(getApplicationContext().getPackageName() );

            i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK );
            startActivity(i);
            Thread.sleep(3000);
            running=true;
            new MyThread1().start();
            new PingThread().start();



    }

    public static MainActivity self;
    public static void setFocus()
    {
       self.setContentView(R.layout.activity_main);


    }

    class MyThread extends Thread {

        private byte[] to;
        private int port;

        public MyThread(byte[] to, int port) {
            this.to = to;
            this.port=port;
        }

        @Override
        public void run() {

            if(port==23002)
                sendMsgThrd(to,port);
            else
                sendMsgThrdUDP(to,port);

        }
    }

    class PingThread extends Thread {

        private byte[] to;
        private int port;

        public PingThread() {
            ;
        }

        @Override
        public void run() {
            while(running)
            {
                byte[] msg= {0,0,0};

                MyThread t = new MyThread(msg, 23002);
                t.setUncaughtExceptionHandler(h);
                t.start();
                try {
                    Thread.sleep(1000);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
    }

    public static boolean running=true;
    static boolean socketClose=false;
    public class MyThread1 extends Thread {

        private byte[] to;

        public MyThread1() {

        }



        @Override
        public void run() {


            while(running)
            {

              /*  if(ds==null)
                {
                    try {
                        ds = new Socket();
                        ds.connect(new InetSocketAddress("192.168.2.72", 23000), 0);
                    } catch (Exception e) {
                        runOnUiThread(() -> Toast.makeText(MainActivity.this, e.getMessage(), Toast.LENGTH_LONG).show());

                    }
                }*/
                try {
                    if(ds1==null || socketClose)
                    {
                        try {
                            ds1 = new Socket();
                            ds1.connect(new InetSocketAddress(Globals.serverIP, 23002), 0);
                            socketClose=false;
                        } catch (Exception e) {
                          //  runOnUiThread(() -> Toast.makeText(MainActivity.this, e.getMessage(), Toast.LENGTH_LONG).show());
                        }
                    }
                } catch (Exception e) {
                    ds1=null;
                }
                try {


                    byte[] msg = new byte[9];

                    byte[] xByteArr=floatToByteArray(xHist);
                    byte[] yByteArr=floatToByteArray(yHist);

                    for(int i=0;i<4;i++)
                        msg[i]=xByteArr[i];

                    for(int i=4;i<8;i++)
                        msg[i]=yByteArr[i-4];

                    msg[8]=0;

                 /*   if(changed && !blocked) {
                        MyThread t = new MyThread(msg, 23000);
                        t.setUncaughtExceptionHandler(h);
                        t.start();
                        changed=false;
                    }
*/

                    Thread.sleep(50);

                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
    }


    static boolean changed=false;

    public static byte[] floatToByteArray(float value) {
        int intBits =  Float.floatToIntBits(value);
        return new byte[] {
                (byte) (intBits >> 24), (byte) (intBits >> 16), (byte) (intBits >> 8), (byte) (intBits) };
    }

    public float xHist=0;
    public float yHist=0;



    @Override
    public boolean onGenericMotionEvent(MotionEvent event) {
        //super.onGenericMotionEvent(event);
        changed=true;
        if(event.getAction()!=ACTION_MOVE)
            return true;

        float x = event.getAxisValue(0);
        float y = event.getAxisValue(1);



        if(event.getHistorySize()>0)
            if((event.getHistoricalAxisValue(0,0)==x && event.getHistoricalAxisValue(1,0)==y))
                return true;

        xHist=x;
        yHist=y;


        byte[] msg = new byte[9];

        byte[] xByteArr=floatToByteArray(x);
        byte[] yByteArr=floatToByteArray(y);

        for(int i=0;i<4;i++)
            msg[i]=xByteArr[i];

        for(int i=4;i<8;i++)
            msg[i]=yByteArr[i-4];

        msg[8]=0;

        if(!blocked) {
            MyThread t = new MyThread(msg, 23000);
            t.setUncaughtExceptionHandler(h);
            t.start();
        }
        return true;
    }

    HashMap<Integer, Timestamp> keyDown=new HashMap<Integer,Timestamp>();

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {

        if(!keyDown.containsKey(keyCode) || true) {
            keyDown.put(keyCode, new Timestamp(new Date().getTime()));
            blocked=true;
            byte[] msg = new byte[3];

            msg[0] = (byte) keyCode;
            msg[1] = (byte) 1;
            msg[2] = 1;

            MyThread t = new MyThread(msg, 23002);
            t.setUncaughtExceptionHandler(h);
            t.start();
        }
        return true;
    }

    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        byte[] msg = new byte[3];
        blocked=true;
        msg[0] = (byte)  keyCode;
        msg[1] = (byte)  0;
        msg[2]=1;

        MyThread t=new MyThread(msg,23002);
        t.setUncaughtExceptionHandler(h);
        t.start();

        return true;
    }


}