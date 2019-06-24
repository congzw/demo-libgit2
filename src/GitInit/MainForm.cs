using System;
using System.IO;
using System.Windows.Forms;
using GitInit.ViewModel;

namespace GitInit
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Main = new MainFormVo();
        }

        public MainFormVo Main { get; set; }

        private void btnInit_Click(object sender, EventArgs e)
        {
            var repo = this.txtRepo.Text.TrimEnd();
            if (string.IsNullOrWhiteSpace(repo))
            {
                MessageBox.Show(@"仓库名不合法" + repo);
                return;
            }

            if (Directory.Exists(repo))
            {
                MessageBox.Show(@"仓库文件夹已经存在：" + repo);
                return;
            }

            ////没有权限删除?
            //Main.Delete(repo);

            Main.DirectoryCopy(@".\_Template", repo, true);
            var slnPath = String.Format(@".\{0}\src\foo.sln", repo);
            var newSlnPath = String.Format(@".\{0}\src\{0}.sln", repo);
            Main.ChangeRepositoryAsSln(slnPath, newSlnPath);
            Main.Init(repo);

            var username = this.txtUsername.Text.Trim();
            if (!string.IsNullOrWhiteSpace(username))
            {
                Main.SetConfig(repo, "user.name", username);
                TheConfig.Username = username;
            }

            var email = this.txtEmail.Text.Trim();
            if (!string.IsNullOrWhiteSpace(email))
            {
                Main.SetConfig(repo, "user.email", email);
                TheConfig.Email = email;
            }

            if (this.cbxInitFirstCommit.Checked)
            {
                //git push -u origin master
                if (!string.IsNullOrWhiteSpace(username))
                {
                    Main.SetGithubRemote(repo, username, "origin", "master");
                    Main.SetUpstreamBranch(repo, "origin", "master", username, email);
                }
            }

            TheConfig.InitFirstCommit = this.cbxInitFirstCommit.Checked;
            MyConfigHelper.Instance.TrySave(TheConfig);
            MessageBox.Show(@"init completed!");
        }

        public MyConfig TheConfig { get; set; }
        private void MainForm_Load(object sender, EventArgs e)
        {
            TheConfig = MyConfigHelper.Instance.TryLoad();
            this.txtRepo.Text = @"myRepo";
            this.txtUsername.Text = TheConfig.Username;
            this.txtEmail.Text = TheConfig.Email;
            this.cbxInitFirstCommit.Checked = TheConfig.InitFirstCommit;
        }
    }
}
