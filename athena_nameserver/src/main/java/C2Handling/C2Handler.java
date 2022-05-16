package C2Handling;

import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.logging.Logger;

public class C2Handler {
    Logger logger = Logger.getLogger(String.valueOf(C2Handler.class));
    private C2 activeC2;
    private Long c2LastRequest = TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());

    public C2Handler() {
        activeC2 = null;
        new Thread(() -> {
            while(true) {
                try {
                    long currentTime = TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());
                    if ((activeC2 != null) && currentTime - c2LastRequest > 10) {
                        logger.info("DELETED C2: " + activeC2.ip() + " AT TIME: " + currentTime);
                        activeC2 = null;
                    }
                    Thread.sleep(250);
                } catch (InterruptedException e) {
                    logger.warning("THREAD SLEEP FAILED ON C2 UPDATE WAIT!");
                }
            }
        }).start();
        logger.info("C2 COMPONENT UP AND RUNNING");
    }

    /**
     * Updates the server with the details about the new command and control bot.
     * @param ip the ip of the server.
     * @param port the port of the server.
     */
    public void updateC2(String ip, String port) {
        activeC2 = new C2(ip, port);
        c2LastRequest = TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());
    }

    public C2 getActiveC2() {
        return activeC2;
    }
}
