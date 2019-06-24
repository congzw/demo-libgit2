using System;
using System.IO;
using System.Linq;
using GitInit.Libs;
using LibGit2Sharp;

namespace GitInit.ViewModel
{
    public class MainFormVo
    {
        public void Init(string repoPath)
        {
            Repository.Init(repoPath);
        }

        public void SetConfig(string repoPath, string key, string value)
        {
            using (var repo = new Repository(repoPath))
            {
                repo.Config.Set(key, value);
            }
        }

        public void SetGithubRemote(string repoName, string username, string remoteName, string branch)
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
        
        public void SetUpstreamBranch(string repoName, string remoteName, string branch, string username = null, string email = null, string message = null)
        {
            using (var repo = new Repository(repoName))
            {
                var localBranch = repo.Branches.SingleOrDefault(x => x.FriendlyName == branch);
                if (localBranch == null)
                {
                    message = string.IsNullOrWhiteSpace(message) ? "first init" : message;
                    username = string.IsNullOrWhiteSpace(username) ? "creator" : username;
                    email = string.IsNullOrWhiteSpace(email) ? "creator@no-email.com" : email;

                    var signature = new Signature(username, email, DateTimeOffset.UtcNow);
                    repo.Commit(message, signature, signature, new CommitOptions() { AllowEmptyCommit = true });
                    //should not null!
                    localBranch = repo.Branches.Single(x => x.FriendlyName == branch);
                }
                repo.Branches.Update(localBranch,
                    b => b.Remote = remoteName,
                    b => b.UpstreamBranch = localBranch.CanonicalName);
            }
        }
        
        public void Delete(string repoPath)
        {
            if (Directory.Exists(repoPath))
            {
                Directory.Delete(repoPath, true);
            }
        }

        public void ChangeRepositoryAsSln(string sln, string repo)
        {
            File.Move(sln, repo);
        }

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
