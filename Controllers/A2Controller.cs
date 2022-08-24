using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using A2.Models;
using A2.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace A2.Controllers
{
    [Route("api")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repository;

        public A2Controller(IA2Repo repository)
        {
            _repository = repository;
        }

        [HttpPost("Register")]
        public ActionResult<string> Register(User user)
        {
            bool registerSuccess = _repository.Register(user);
            if (registerSuccess)
            {
                return Ok("User successfully registered.");
            }
            else
            {
                return Ok("Username not available.");
            }
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("GetVersionA")]
        public ActionResult<string> GetVersionA()
        {
            return Ok("1.0.0 (auth)");
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public ActionResult<Order> PurchaseItem(int id)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            User user = _repository.GetUserByUsername(userName);
            Order order = new Order { UserName = user.UserName, ProductId = id };
            return Ok(order);
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PairMe")]
        public ActionResult<GameRecordOut> PairMe()
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            User user = _repository.GetUserByUsername(userName);
            GameRecord game = _repository.PairMe(user);
            GameRecordOut gameOut = new GameRecordOut
            {
                GameId = game.GameID,
                State = game.State,
                Player1 = game.Player1,
                Player2 = game.Player2,
                LastMovePlayer1 = game.LastMovePlayer1,
                LastMovePlayer2 = game.LastMovePlayer2
            };
            return Ok(gameOut);
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("TheirMove/{gameId}")]
        public ActionResult<string> TheirMove(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            User user = _repository.GetUserByUsername(userName);
            string res = _repository.TheirMove(user, gameId);
            return Ok(res);
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("MyMove")]
        public ActionResult<string> MyMove(string gameId, string move)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            User user = _repository.GetUserByUsername(userName);
            string res = _repository.MyMove(user, gameId, move);
            return Ok(res);
        }

        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("QuitGame")]
        public ActionResult<string> QuitGame(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            User user = _repository.GetUserByUsername(userName);
            string res = _repository.QuitGame(user, gameId);
            return Ok(res);
        }
    }
}