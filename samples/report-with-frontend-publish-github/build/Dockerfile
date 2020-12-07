FROM samlangten/jugsatac:latest

ARG PUBLISH_REPO_URL
ARG PUBLISH_BRANCH
ARG PUBLISH_CNAME
ARG GIT_EMAIL
ARG GIT_NAME
ENV PUBLISH_REPO_URL=${PUBLISH_REPO_URL}
ENV PUBLISH_BRANCH=${PUBLISH_BRANCH}
ENV PUBLISH_CNAME=${PUBLISH_CNAME}

COPY . /app/frontend

RUN apt update && \
    apt install git -y && \
    mkdir ~/.ssh/ && \
    cp /app/frontend/config/github_ssh_key_pub.pem ~/.ssh/id_rsa.pub && \
    cp /app/frontend/config/github_ssh_key.pem ~/.ssh/id_rsa && \
    ssh-keyscan -t rsa github.com >> ~/.ssh/known_hosts && \
    chmod 600 ~/.ssh/id_rsa && \
    chmod +x /app/frontend/entrypoint.sh && \
    git config --global user.email "${GIT_EMAIL}" && \
    git config --global user.username "${GIT_NAME}"

ENTRYPOINT ["sh","-c","/app/frontend/entrypoint.sh"]