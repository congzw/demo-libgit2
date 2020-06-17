using System;
using System.IO;
using GitCIWeb.Libs;
using Microsoft.AspNetCore.Mvc;

namespace GitCIWeb.Controllers
{
    [Route("api/git")]
    [ApiController]
    public class GitController : ControllerBase
    {
        [Route("Checkout")]
        public MessageResult Checkout()
        {
            var myGitConfig = new MyGitConfig() { Username = "congzw", Email = "congzw@zqnb.com", Password = "DDkk1212" };
            var myGitHelper = new MyGitHelper(myGitConfig);
            var logServerProjectName = "pilot_ci_log_server";
            var logServerUri = "http://192.168.1.176:81/practice/pilot_ci_log_server.git";
            var repoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "repos", logServerProjectName);
            if (Directory.Exists(repoPath))
            {
                return MessageResult.CreateFail("Already Exist: " + repoPath); 
            }
            var messageResult = myGitHelper.Clone(logServerUri, repoPath, "master");
            return messageResult;
        }
    }
}