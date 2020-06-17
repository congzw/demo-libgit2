using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace GitCIWeb.Libs
{
    public class MyGitHelper
    {
        public MyGitConfig Config { get; }

        public MyGitHelper(MyGitConfig config)
        {
            Config = config;
        }

        public void Init(string repoPath, bool isBare = false)
        {
            Repository.Init(repoPath, isBare);
        }

        public MessageResult Clone(string sourceUrl, string repoPath, string branch = "master")
        {
            var fullPath = Path.GetFullPath(repoPath);
            var cloneOptions = new CloneOptions();
            cloneOptions.Checkout = true;
            cloneOptions.BranchName = branch;
            cloneOptions.IsBare = false;
            cloneOptions.CredentialsProvider = (url, fromUrl, types) => new UsernamePasswordCredentials()
            {
                Username = Config.Username,
                Password = Config.Password
            };

            //var discover = Repository.Discover(repoPath);
            //if (!string.IsNullOrWhiteSpace(discover))
            //{
            //    return MessageResult.CreateFail(".git already exist: " + discover);
            //}
            var gitRepoPath = Repository.Clone(sourceUrl, fullPath, cloneOptions);
            return MessageResult.CreateSuccess("Clone Success: " + sourceUrl).WithData(gitRepoPath);
        }

        public Repository GetRepository(string repoPath)
        {
            var repo = new Repository(repoPath);
            return repo;
        }

        public MessageResult SetConfig(Repository repo, string key, string value)
        {
            repo.Config.Set(key, value);
            return MessageResult.CreateSuccess(string.Format("set config: {0}={1}", key, value));
        }

        public MessageResult CheckoutTrackRemoteBranches(Repository repo, string remote = "origin")
        {
            var remoteBranchNames = GetBranches(repo, true).Where(x => x.StartsWith(remote)).ToList();
            foreach (var remoteBranchName in remoteBranchNames)
            {
                var localBranchName = remoteBranchName.Replace(remote + "/", string.Empty);

                if (repo.Branches[localBranchName] == null)
                {
                    //local branch not exist yet
                    //Let's get a reference on the remote tracking branch...
                    var trackedBranch = repo.Branches[remoteBranchName];
                    // ...and create a local branch pointing at the same Commit
                    var branch = repo.CreateBranch(localBranchName, trackedBranch.Tip);

                    // The local branch is not configured to track anything
                    if (!branch.IsTracking)
                    {
                        // So, let's configure the local branch to track the remote one.
                        var updatedBranch = repo.Branches.Update(branch, b => b.TrackedBranch = trackedBranch.CanonicalName);
                        //updatedBranch.IsTracking => true
                        //trackedBranchName == updatedBranch.TrackedBranch.Name
                    }
                }
            }
            return MessageResult.CreateSuccess("CheckTracRemoteBranchesOK");
        }

        public IList<string> GetBranches(Repository repo, bool? isRemote)
        {
            var branches = new List<string>();
            var query = repo.Branches.AsQueryable();
            if (isRemote.HasValue)
            {
                query = query.Where(b => b.IsRemote == isRemote);
            }

            var list = query.ToList();
            foreach (var b in list)
            {
                branches.Add(b.FriendlyName);
            }
            return branches;
        }

        public MessageResult CreateBranch(Repository repo, string branchName)
        {
            //$ git branch branch
            if (repo.Branches[branchName] != null)
            {
                return MessageResult.CreateFail("branch already exist: " + branchName);
            }
            repo.CreateBranch(branchName);
            // Or repo.Branches.Add(branch, "HEAD");
            return MessageResult.CreateSuccess(string.Format("create branch: {0}", branchName));
        }

        public MessageResult RemoveBranch(Repository repo, string branchName)
        {
            if (repo.Branches[branchName] == null)
            {
                return MessageResult.CreateFail("branch not exist: " + branchName);
            }
            repo.Branches.Remove(branchName);
            return MessageResult.CreateSuccess(string.Format("remove branch: {0}", branchName));
        }

        public MessageResult Checkout(Repository repo, string branchName, string remoteName = "origin")
        {
            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                if (string.IsNullOrWhiteSpace(remoteName))
                {
                    return MessageResult.CreateFail("branch not exist: " + branchName);
                }

                //auto find remote and track it
                var remoteBranchName = remoteName + "/" + branchName;
                var remoteBranch = repo.Branches[remoteBranchName];
                if (remoteBranch == null)
                {
                    return MessageResult.CreateFail("remote branch not exist: " + remoteBranchName);
                }
                // ...and create a local branch pointing at the same Commit
                branch = repo.CreateBranch(branchName, remoteBranch.Tip);

                // The local branch is not configured to track anything
                if (!branch.IsTracking)
                {
                    repo.Branches.Update(branch, b => b.TrackedBranch = remoteBranch.CanonicalName);
                }
            }

            Branch currentBranch = Commands.Checkout(repo, branch);
            return MessageResult.CreateSuccess(string.Format("checkout branch: {0}", branchName)).WithData(currentBranch);
        }

        public MessageResult Commit(Repository repo, MyCommitMessage commitMessage)
        {
            var message = string.IsNullOrWhiteSpace(commitMessage.Message) ? "EMPTY COMMIT" : commitMessage.Message;
            var username = string.IsNullOrWhiteSpace(commitMessage.Username) ? Config.Username : commitMessage.Username;
            var email = string.IsNullOrWhiteSpace(commitMessage.Email) ? Config.Email : commitMessage.Email;
            var signature = new Signature(username, email, DateTimeOffset.UtcNow);
            repo.Commit(message, signature, signature, new CommitOptions() { AllowEmptyCommit = true });
            return MessageResult.CreateSuccess("Commit Success");
        }

        public MessageResult Push(Repository repo, string branchName, string remoteName = "origin")
        {
            var remote = repo.Network.Remotes[remoteName];
            var options = new PushOptions();
            options.CredentialsProvider = (url, fromUrl, types) => new UsernamePasswordCredentials()
            {
                Username = Config.Username,
                Password = Config.Password
            };

            Checkout(repo, branchName, remoteName);
            repo.Network.Push(remote, @"refs/heads/" + branchName, options);
            return MessageResult.CreateSuccess("push success");
        }

        public IList<Tag> GetTags(Repository repo, bool? isAnnotated)
        {
            var query = repo.Tags.AsQueryable();
            if (isAnnotated.HasValue)
            {
                query = query.Where(x => x.IsAnnotated == isAnnotated);
            }

            return query.ToList();
        }

        public MessageResult ApplyTag(Repository repo, string tagName)
        {
            //Lightweight tag pointing at the current HEAD
            repo.ApplyTag(tagName);

            //Lightweight tag pointing at a specific target
            //Tag t = repo.ApplyTag("a-better-tag-name", "refs/heads/cool-branch");
            //Tag t2 = repo.ApplyTag("the-best-tag-name", "5df3e2b3ca5ebe8123927a81d682993ad597a584");
            return MessageResult.CreateSuccess("tag success: " + tagName);
        }

    }
}