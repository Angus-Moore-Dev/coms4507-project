package athena.nameserver.handler.lookup;

import org.springframework.web.bind.annotation.*;

import java.util.logging.Logger;

@RestController
@RequestMapping("/api/lookup")
public class LookupController {
    Logger logger = Logger.getLogger(String.valueOf(LookupController.class));

    public LookupController() {
        logger.info("LOGGER STARTED");
    }

    /**
     * Find the IP address of the bot.
     * @param bot_id the id name of the bot
     * @return JSONObject containing the whereabouts of the bot.
     */
    @CrossOrigin
    @GetMapping("{bot_id}")
    public String getBotIPAddress(@PathVariable String bot_id) {
        return bot_id;
    }

    @CrossOrigin
    @GetMapping("")
    public String getAllBots() {
        return "ALL_BOTS";
    }
}
