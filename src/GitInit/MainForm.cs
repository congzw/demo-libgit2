using System;
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
                MessageBox.Show("仓库名不合法" + repo);
                return;
            }

            Main.Delete(repo);
            Main.DirectoryCopy(@".\_Template", repo, true);
            var slnPath = String.Format(@".\{0}\src\foo.sln", repo);
            var newSlnPath = String.Format(@".\{0}\src\{0}.sln", repo);
            Main.ChangeRepositoryAsSln(slnPath, newSlnPath);
            Main.Init(repo);
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.txtRepo.Text = "MyRepository";
        }
    }
}
