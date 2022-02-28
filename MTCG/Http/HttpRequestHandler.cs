using System.Linq;
using System.Net.Sockets;

namespace MTCG.Http
{
    static class HttpRequestHandler
    {
        //GET
        public static string GetCardsRequest(string request)
        {
            if (!CheckIfTokenExists(request))
            {
                return "Task Failed: User-Token don't match!\n\r";
            }
            if (DBCard.GetAllUserCards(request).Count() == 0)
            {
                return "Task Failed: There are no cards in your inventory!\n\r";
            }
            return HttpResponse.CardsResponseMsg(
                request.Contains("format=plain"), DBCard.GetAllUserCards(request));
        }

        public static string GetDeckRequest(string request)
        {
            if(DBCard.GetDeck(request) == null)
            {
                return "Task Failed: There are no cards in your deck!\n\r";
            }
            return HttpResponse.CardsResponseMsg(
                request.Contains("format=plain"), DBCard.GetDeck(request));
        }

        public static string GetUsersRequest(string request)
        {
            if (!VerifyUserWithToken(request))
            {
                return "Task Failed: User and User-Token don't match!\n\r";
            }
            return HttpResponse.UserResponseMsg(DBUser.GetUserByUsername(Helper.ExtractUsername(request)));
        }

        public static string GetStatsRequest(string request)
        {
            return HttpResponse.StatsResponseMsg(DBScore.GetStats(request));
        }

        public static string GetScoreRequest()
        {
            return HttpResponse.ScoreboardResponseMsg(DBScore.GetScoreBoard());
        }

        public static string GetTradingsRequest()
        {
            if(DBTrade.GetAllDeals().Count() == 0)
            {
                return "Task Failed: There are currently no deals available!\n\r";
            }
            return HttpResponse.DealsResponseMsg(DBTrade.GetAllDeals());
        }

        //POST
        public static string PostUsersRequest(string request)
        {
            if (DBUser.AddUser(request))
            {
                DBScore.SetDefaultStats(request);
                return "User was successfully added!\n\r";
            }
            return "Task Failed: User can't be added, this User might already exist!\n\r";
        }

        public static string PostSessionsRequest(string request)
        {
            if (DBUser.LoginUser(Helper.ExtractUser(request).Username, Helper.ExtractUser(request).Password))
            {
                return "You logged in successfully!\n\r";
            }
            return "Task failed: Login didn't work!";
        }

        public static string PostTransactionsRequest(string request)
        {
            if (DBPackage.AcquirePackage(request))
            {
                return "Package was successfully acquired!\n\r";
            }
            return "Task Failed: You don't have enough coins to buy a package, or there are no packages available!\n\r";
        }

        public static string PostPackagesRequest(string request)
        {
            if (!VerifyAdmin(request))
            {
                return "Task failed: You don't have the permissions to add new packages!\n\r";
            }
            DBPackage.AddPackage(request);
            return "Package was successfully added!\n\r";
        }

        //works differently, the functions in BattleRequest are sending the response to the clients.
        public static void PostBattlesRequest(NetworkStream stream, string request)
        {
            BattleRequests.AddRequestToPool(stream, request);
            BattleRequests.startMatch(request);
        }

        public static string PostAcceptTradeRequest(string request)
        {
            if (DBTrade.AcceptTradingDeal(request))
            {
                return "Trade was successful!\n\r";
            }
            return "Task Failed: Possible reasons\n\r - You might try to trade a card that doesn't belong to you.\n\r" +
                "- You can't trade cards on your own deal.\n\r";
        }

        public static string PostTradingsRequest(string request)
        {
            if (DBTrade.AddTradingDeal(request))
            {
                return "Trading Deal was successfully added!\n\r";
            }
            return "Task Failed: You either don't own this card or this card is currently used in your deck!\n\r";
        }

        //PUT
        public static string PutDeckRequest(string request)
        {
            if(DBCard.GetAllUserCards(request).Count() == 0)
            {
                return "Task failed: You don't have any cards!\n\r";
            }
            if (DBCard.ConfigureDeck(request))
            {
                return "Deck was successfully built!\n\r";
            }
            return "Task Failed: The number of cards you want to set as a new deck are either lower than 4 " +
                "or one/multipe/all cards are not in your inventory (id was wrong)!\n\r";
        }

        public static string PutUsersRequest(string request)
        {
            if (!VerifyUserWithToken(request))
            {
                return "Task Failed: User and User-Token don't match!\n\r";
            }
            DBUser.UpdateUserData(request);
            return "You successfully edited your profile!\n\r";
        }

        //DELETE
        public static string DeleteTradingsRequest(string request)
        {
            if (DBTrade.DeleteTradingDeal(Helper.ExtractDealId(request)))
            {
                return "Deal was successfully deleted!\n\r";
            }
            return "Task Failed: Deal couldn't be deleted!\n\r";
        }

        //Verification
        private static bool CheckIfTokenExists(string request)
        {
            if (Helper.ExtractUsernameToken(request).Equals(""))
            {
                return false;
            }
            return true;
        }
        private static bool VerifyUserWithToken(string request)
        {
            return Helper.ExtractUsername(request).Equals(Helper.ExtractUsernameToken(request));
        }

        private static bool VerifyAdmin(string request)
        {
            return Helper.ExtractUsernameToken(request).Equals("admin");
        }

        public static bool UserLoggedInCheck(string request)
        {
            return DBUser.LoggedInCheck(Helper.ExtractUsernameToken(request));
        }
    }
}
