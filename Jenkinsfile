library 'jenkins-ptcs-library@0.3.0'

podTemplate(label: pod.label,
  containers: pod.templates + [
    containerTemplate(name: 'dotnet', image: 'microsoft/dotnet:2.2-sdk', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
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
            publishContainerToGcr(project);
            publishTagToDockerhub(project);
        }
    }
  }