@Library("PTCSLibrary@1.0.3") _

// Podtemplate and node must match, dont use generic names like 'node', use more specific like projectname or node + excact version number.
// This is because CI environment reuses templates based on naming, if you create node 7 environment with name 'node', following node 8 environment
// builds may fail because they reuse same environment if label matches existing.
podTemplate(label: 'slack-integration',
  containers: [
    containerTemplate(name: 'dotnet', image: 'microsoft/aspnetcore-build:2', ttyEnabled: true, command: '/bin/sh -c', args: 'cat'),
    containerTemplate(name: 'docker', image: 'ptcos/docker-client:1.1.32', alwaysPullImage: true, ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    def project = 'slack-integration'
    def branch = (env.BRANCH_NAME)
    def namespace = "slack-integration"
    def notifyslackchannel = "#jenkins"

    node('slack-integration') {
        stage('Checkout') {
            checkout_with_tags()
        }
        stage('Build') {
            container('dotnet') {
                sh """
                    dotnet publish -c Release -o out
                """
            }
        }
        stage('Test') {
            container('dotnet') {
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
  }