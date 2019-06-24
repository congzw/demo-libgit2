## create a new repository on the command line

```sh

echo "# demo-empty-git" >> README.md
git init
git add README.md
git commit -m "first commit"
git remote add origin https://github.com/congzw/demo-empty-git.git
git push -u origin master

```
## or push an existing repository from the command line

```sh

git remote add origin https://github.com/congzw/demo-empty-git.git
git push -u origin master

```