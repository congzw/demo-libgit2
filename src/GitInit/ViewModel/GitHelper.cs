using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitInit.ViewModel
{
    public interface IGitHelper
    {

    }

    public class GitHelper
    {

        public void CreateBranch(string repoName, string username, string remoteName, string branch)
        {
            //remoteName: origin
            //branch: master

            //this will set to config:
            //[remote "origin"]
            //url = https://github.com/{username}/{repoName}.git
            //fetch = +refs/heads/*:refs/remotes/origin/*

            using (var repo = new Repository(repoName))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == remoteName);
                if (remote == null)
                {
                    var repoSource = $"https://github.com/{username}/{repoName}.git";
                    remote = repo.Network.Remotes.Add(remoteName, repoSource);
                }
            }
        }

    }
}
