using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        public bool Register(User user);
        public User GetUserByUsername(string userName);
        public bool ValidLogin(string userName, string password);
        public GameRecord PairMe(User user);
        public string TheirMove(User user, string gameId);
        public string MyMove(User user, string gameId, string move);
        public string QuitGame(User user, string gameId);
    }
}