using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using A2.Models;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DBContext _dbContext;

        public A2Repo(A2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Register(User user)
        {
            User existingUser = GetUserByUsername(user.UserName);
            if (existingUser != null)
            {
                return false;
            }
            else
            {
                EntityEntry<User> e = _dbContext.Users.Add(user);
                User u = e.Entity;
                _dbContext.SaveChanges();
                return true;
            }
        }

        public User GetUserByUsername(string userName)
        {
            User user = _dbContext.Users.FirstOrDefault(e => e.UserName.Trim() == userName.Trim());
            return user;
        }

        public GameRecord GetGameByUsername(string userName)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(e => e.Player1 == userName || e.Player2 == userName);
            return gameRecord;
        }

        public GameRecord GetGameByGameId(string gameId)
        {
            GameRecord gameRecord = _dbContext.GameRecords.FirstOrDefault(e => e.GameID == gameId);
            return gameRecord;
        }

        public bool ValidLogin(string userName, string password)
        {
            User u = _dbContext.Users.FirstOrDefault(e => e.UserName == userName && e.Password == password);
            if (u == null)
            {
                return false;
            } 
            else
            {
                return true;
            }
        }

        public GameRecord PairMe(User user)
        {
            GameRecord game = _dbContext.GameRecords.FirstOrDefault(e => e.State == "wait");
            if (game != null)
            {
                if (game.Player1 != user.UserName)
                {
                    game.State = "progress";
                    game.Player2 = user.UserName;
                }
            }
            else
            {
                GameRecord newGame = new GameRecord { GameID = System.Guid.NewGuid().ToString(), 
                    State = "wait", Player1 = user.UserName, Player2 = null, LastMovePlayer1 = null, LastMovePlayer2 = null };
                _dbContext.GameRecords.Add(newGame);
                game = newGame;
            }
            _dbContext.SaveChanges();
            return game;
        }

        public string TheirMove(User user, string gameId)
        {
            if (GetGameByUsername(user.UserName) == null)
            {
                return "You are not playing a game now.";
            }

            GameRecord foundGame = GetGameByGameId(gameId);
            if (foundGame == null)
            {
                return "no such gameId";
            }

            if (foundGame.Player1 != user.UserName && foundGame.Player2 != user.UserName)
            {
                return "not your game id";
            }

            else if (foundGame.State != "progress")
            {
                return "You do not have an opponent yet.";
            } 
            else if (foundGame.Player1 == user.UserName)
            {
                if (foundGame.LastMovePlayer2 == null)
                {
                    return "Your opponent has not moved yet.";
                } 
                else
                {
                    return foundGame.LastMovePlayer2;
                }
            }
            else
            {
                if (foundGame.LastMovePlayer1 == null)
                {
                    return "Your opponent has not moved yet.";
                }
                else
                {
                    return foundGame.LastMovePlayer1;
                }
            }
        }

        public string MyMove(User user, string gameId, string move)
        {
            if (GetGameByUsername(user.UserName) == null)
            {
                return "You are not playing a game now.";
            }

            GameRecord foundGame = GetGameByGameId(gameId);
            if (foundGame == null)
            {
                return "no such gameId";
            }

            if (foundGame.Player1 != user.UserName && foundGame.Player2 != user.UserName)
            {
                return "not your game id";
            }
            else if (foundGame.State != "progress")
            {
                return "You do not have an opponent yet.";
            }
            else if (foundGame.Player1 == user.UserName)
            {
                if (foundGame.LastMovePlayer1 != null)
                {
                    return "It is not your turn.";
                }
                else
                {
                    foundGame.LastMovePlayer1 = move;
                    foundGame.LastMovePlayer2 = null;
                    _dbContext.SaveChanges();
                    return "move registered";
                }
            }
            else
            {
                if (foundGame.LastMovePlayer2 != null)
                {
                    return "It is not your turn.";
                }
                else
                {
                    foundGame.LastMovePlayer2 = move;
                    foundGame.LastMovePlayer1 = null;
                    _dbContext.SaveChanges();
                    return "move registered";
                }
            }
        }

        public string QuitGame(User user, string gameId)
        {
            if (GetGameByUsername(user.UserName) == null)
            {
                return "You have not started a game.";
            }

            GameRecord foundGame = GetGameByGameId(gameId);
            if (foundGame == null)
            {
                return "no such gameId";
            }

            if (foundGame.Player1 != user.UserName && foundGame.Player2 != user.UserName)
            {
                return "not your game id";
            }
            else
            {
                _dbContext.GameRecords.Remove(foundGame);
                _dbContext.SaveChanges();
                return "game over";
            }
        }
    }
}