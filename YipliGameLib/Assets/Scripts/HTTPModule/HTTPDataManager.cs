using Yipli.HttpMpdule.Classes;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Yipli.HttpMpdule
{
    public class HTTPDataManager : MonoBehaviour
    {
        string userJsonData = "{\n  \"userID\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n  \"contact_no\": \"7874579889\",\n  \"country_code\": \"+91\",\n  \"created_on\": 1630136077054,\n  \"current_mat_id\": \"-N0FBaBdH_MiOgPkWIjz\",\n  \"current_player_id\": \"-MSX--0uyqI7KgKmNOIY\",\n  \"display_name\": \"Vismay Patel\",\n  \"email\": \"vismay@playyipli.com\",\n  \"has_subcribed\": false,\n  \"profile_pic_url\": \"f6bdea10-bf6f-4fd1-b8f1-fc6b91bcff12\",\n  \"total_players\": 2\n}";

        string playerJsonData = "[\n    {\n        \"playerID\" : \"-MSX--0uyqI7KgKmNOIY\",\n        \"added_on\": 1612256903290,\n        \"dob\": \"07-01-1990\",\n        \"gender\": \"Female\",\n        \"height\": \"172\",\n        \"name\": \"Elden ring\",\n        \"profile_pic_url\": \"-MSX--0uyqI7KgKmNOIY.jpg\",\n        \"user_id\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n        \"weight\": \"54\"\n    },\n    {\n        \"playerID\" : \"-MzpCjARFpSBzkPalPrr\",\n        \"added_on\": 1649086493404,\n        \"dob\": \"01-01-1987\",\n        \"gender\": \"Male\",\n        \"height\": \"161\",\n        \"mat_tut_done\": 1,\n        \"name\": \"Hogwargs Legacy\",\n        \"profile_pic_url\": \"-MzpCjARFpSBzkPalPrr.jpg\",\n        \"user_id\": \"lC4qqZCFEaMogYswKjd0ObE6nD43\",\n        \"weight\": \"39\"\n    }\n]";

        string matJsonData = "[\n    {\n        \"mat_fb_id\" : \"-N0FBaBdH_MiOgPkWIjz\",\n        \"display_name\": \"Black yipli F4\",\n        \"mac_address\": \"A4:DA:32:4F:BF:F4\",\n        \"mac_name\": \"YIPLI-C8\",\n        \"registered_on\": \"2022-04-22T13:05:21.129734\",\n        \"status\": \"Active\"\n    },\n    {\n        \"mat_fb_id\" : \"-N0FBaBdH_MiOgPiWIjz\",\n        \"display_name\": \"Black yipli F8\",\n        \"mac_address\": \"A4:DA:32:4F:BF:F5\",\n        \"mac_name\": \"YIPLI-C9\",\n        \"registered_on\": \"2022-04-22T13:05:21.129734\",\n        \"status\": \"Active\"\n    }\n]";

        string currentMatJson = "{\n    \"mat_fb_id\" : \"-N0FBaBdH_MiOgPkWIjz\",\n    \"display_name\": \"Black yipli F4\",\n    \"mac_address\": \"A4:DA:32:4F:BF:F4\",\n    \"mac_name\": \"YIPLI-C8\",\n    \"registered_on\": \"2022-04-22T13:05:21.129734\",\n    \"status\": \"Active\"\n}";

        [SerializeField] HTTPYipliConfig yipliConfig = null;

        private void Start() {
            CreateUserData();
        }

        private void CreateUserData() {
            yipliConfig.CurrentUserInfo = JsonConvert.DeserializeObject<UserData>(userJsonData);

            yipliConfig.AllPlayersOfThisUser = JsonConvert.DeserializeObject<List<PlayerData>>(playerJsonData);

            yipliConfig.AllMatsOfThisUser = JsonConvert.DeserializeObject<List<MatData>>(matJsonData);

            yipliConfig.CurrentActiveMatData = JsonConvert.DeserializeObject<MatData>(currentMatJson);
        }
    }
}

// 973391