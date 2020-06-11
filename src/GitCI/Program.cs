using System;
using System.IO;
using System.Linq;
using GitCI.Libs;

namespace GitCI
{
    class Program
    {
        static void Main(string[] args)
        {
            var myGitConfig = new MyGitConfig() {Username = "congzw", Email = "congzw@zqnb.com", Password = "DDkk1212"};
            var myGitHelper = new MyGitHelper(myGitConfig);
            var logServerProjectName = "pilot_ci_log_server";
            var logServerUri = "http://192.168.1.176:81/practice/pilot_ci_log_server.git";
            var repoPath = Path.Combine("./", logServerProjectName);

            //clone
            if (!Directory.Exists(repoPath))
            {
                var messageResult = myGitHelper.Clone(logServerUri, repoPath, "master");
                Console.WriteLine(messageResult.Message);
            }

            //get repo
            var repo = myGitHelper.GetRepository(repoPath);

            //set config
            myGitHelper.SetConfig(repo, "user.name", myGitConfig.Username);
            myGitHelper.SetConfig(repo, "user.email", myGitConfig.Email);

            //var createBranch = myGitHelper.CreateBranch(repo, "new_branch");
            //Console.WriteLine(createBranch.Message);

            //get branches
            var branches = myGitHelper.GetBranches(repo, null);
            foreach (var branch in branches)
            {
                Console.WriteLine(branch);
            }

            //var checkoutTrackRemoteBranches = myGitHelper.CheckoutTrackRemoteBranches(repo);
            //Console.WriteLine(checkoutTrackRemoteBranches.Message);

            //check out
            var checkout = myGitHelper.Checkout(repo, "dev");
            Console.WriteLine(checkout.Message);

            ////commit
            //var commit = myGitHelper.Commit(repo, MyCommitMessage.Create("1th C# COMMIT"));
            //Console.WriteLine(commit.Message);

            //var checkout2 = myGitHelper.Checkout(repo, "master");
            //Console.WriteLine(checkout2.Message);
            //var commit2 = myGitHelper.Commit(repo, MyCommitMessage.Create("2th C# COMMIT"));
            //Console.WriteLine(commit.Message);

            //var push = myGitHelper.Push(repo, "dev");
            //Console.WriteLine(push.Message);

            var tags = myGitHelper.GetTags(repo, null);
            var theTag = tags.SingleOrDefault(x => x.FriendlyName == "a-nice-tag-name");
            if (theTag == null)
            {
                myGitHelper.ApplyTag(repo, "a-nice-tag-name");
            }
            foreach (var tag in tags)
            {
                Console.WriteLine(tag.FriendlyName);
            }


            Console.WriteLine("==== DEMO COMPLETE! ====");
            Console.Read();
        }
    }
}
