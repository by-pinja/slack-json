library 'jenkins-ptcs-library@0.6.0'

podTemplate(label: pod.label,
  containers: pod.templates + [
    containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    def project = "slack-json"

    node(pod.label) {
        stage('Checkout') {
            checkout scm
        }
        container('dotnet') {
            stage('Build') {
                sh """
                    dotnet build
                """
            }
            stage('Test') {
                sh """
                    dotnet test
                """
            }
        }
        stage('Package') {
            publishContainerToGcr(project);
            publishTagToDockerhub(project);
        }
    }
  }
