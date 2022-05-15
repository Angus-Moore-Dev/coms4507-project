package BotHandling;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.logging.Logger;

/**
 * The main bothandler that provides lookup details for active bots.
 */
public class BotHandler {
    Logger logger = Logger.getLogger(String.valueOf(BotHandler.class));
    private Map<String, Bot> activeBotsMap; // Key is the botId.
    private Map<String, Bot> allBots;

    // This is tracking active bots
    private Map<String, Long> botIdLastRequest;
    public BotHandler() {
        activeBotsMap = new HashMap<>();
        botIdLastRequest = new HashMap<>();
        allBots = new HashMap<>();

        // This runs, removing bots from the list when they've been inactive for more than 10 seconds.
        new Thread(() -> {
            while(true) {
                try {
                    for (Map.Entry<String, Long> entry: botIdLastRequest.entrySet()) {
                        long currentTime = TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());
                        if (currentTime - entry.getValue() > 30) {
                            // This bot has been inactive for more than 10 seconds, remove it from the list.
                            activeBotsMap.remove(entry.getKey());
                            botIdLastRequest.remove(entry.getKey());
                            // We don't remove from all bots because the C2 needs to know it's offline.
                            logger.info("REMOVED BOT: " + entry.getKey() + " AT TIME: " + currentTime);
                        }
                    }
                    Thread.sleep(250);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }).start();
    }

    public Bot getActiveBot(String botId) {
        return activeBotsMap.get(botId);
    }

    public boolean botInSystem(String botId) {
        return allBots.containsKey(botId);
    }

    public List<Bot> getAllActiveBots() {
        return activeBotsMap.values().stream().toList();
    }

    public List<Bot> getAllBots() {
        return allBots.values().stream().toList();
    }

    /**
     * When a bot pings, it will be updated here.
     * @param botId the id of the bot.
     */
    public void botPing(String botId, String ip, String port) {
        if (!allBots.containsKey(botId))
            allBots.put(botId, new Bot(botId, ip, port));
        if (!activeBotsMap.containsKey(botId))
            activeBotsMap.put(botId, new Bot(botId, ip, port));
        botIdLastRequest.put(botId, TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis()));
    }
}
