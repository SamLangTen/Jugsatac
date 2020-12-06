#!/bin/sh

mail_sync_path="/app/"
mail_sync="Jugsatac"
mail_config_path="/app/frontend/config/config.json"
mail_cache_path="/app/frontend/config/cache/cache.json"
frontend_path="/app/frontend/resource/"
repo_clone_url=${PUBLISH_REPO_URL}
repo_publish_branch=${PUBLISH_BRANCH}
repo_cname=${PUBLISH_CNAME}

trap 'rm -rf "$TMP"' EXIT
TMP=$(mktemp -d) || exit 1

cd ${mail_sync_path}
./${mail_sync} -s ${mail_config_path} -c ${mail_cache_path} -o $TMP/info.json
if [ $? -ne 0 ]; then
    rm ${mail_cache_path}
    ./${mail_sync} -s ${mail_config_path} -c ${mail_cache_path} -o $TMP/info.json
fi

cd $TMP
mkdir publish
cp $TMP/info.json $TMP/publish/
#cat $TMP/info.json
cp -r ${frontend_path}/* $TMP/publish/

cd ./publish
echo ${repo_cname} >> CNAME

git init .
git add .
git commit -m "init"
git remote add origin ${repo_clone_url}
git branch ${repo_publish_branch}
git checkout ${repo_publish_branch}
git push origin ${repo_publish_branch} --force
