library 'jenkins-ptcs-library@docker-depencies'

podTemplate(label: pod.label,
  containers: pod.templates + [
    containerTemplate(name: 'dotnet', image: 'microsoft/aspnetcore-build:2.2', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    def branch = (env.BRANCH_NAME)

    node(pod.label) {
        stage('Checkout') {
            checkout_with_tags()
        }
        container('dotnet') {
            stage('Build') {
                sh """
                    dotnet publish -c Release -o out
                """
            }
            stage('Test') {
                sh """
                    dotnet test
                """
            }
        }
        stage('Package') {
            container('docker') {
                sh """
                    docker build -t ptcos/slack-json:latest .
                """

                if(env.GIT_TAG_NAME && env.GIT_TAG_NAME != "null") {
                    docker.withRegistry('https://registry.hub.docker.com', 'docker-hub-credentials') {
                        def image = docker.image("ptcos/slack-json")
                        image.push("latest")
                        image.push(env.GIT_TAG_NAME)
                    }
                }
            }
        }
        stage('apply') {
            if(branch == "master")
            {
                toK8sTestEnv() {
                    sh """
                        kubectl apply -f ./k8s/master.yaml
                    """
                }
            }
        }
    }
  }