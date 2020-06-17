using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitCIWeb.Libs;
using Microsoft.AspNetCore.Mvc;

namespace GitCIWeb.Controllers
{
    [Route("api/git")]
    [ApiController]
    public class GitController : ControllerBase
    {
        private readonly MyGitHelper _myGitHelper;

        public GitController(MyGitHelper myGitHelper)
        {
            _myGitHelper = myGitHelper;
        }

        [Route("GetDate")]
        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        [Route("Checkout")]
        public MessageResult Checkout()
        {
            var logServerProjectName = "pilot_ci_log_server";
            var logServerUri = "http://192.168.1.176:81/practice/pilot_ci_log_server.git";
            var repoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "repos", logServerProjectName);
            if (Directory.Exists(repoPath))
            {
                return MessageResult.CreateFail("Already Exist: " + repoPath);
            }
            var messageResult = _myGitHelper.Clone(logServerUri, repoPath, "master");
            return messageResult;
        }

        [Route("GetTags")]
        public IList<string> GetTags()
        {
            var logServerProjectName = "pilot_ci_log_server";
            var repoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "repos", logServerProjectName);
            var repo = _myGitHelper.GetRepository(repoPath);
            var tags = _myGitHelper.GetTags(repo, null).Select(x => x.FriendlyName).ToList();
            return tags;
        }

        [Route("Pull")]
        public MessageResult Pull()
        {
            return MessageResult.CreateSuccess("TODO Pull");
        }
    }
}