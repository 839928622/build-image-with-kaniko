pipeline {
   agent {
      kubernetes {
      defaultContainer "shell"
      yamlFile "jenkins-pod.yaml"
    }
   }

   environment {
    
     // YOUR_DOCKERHUB_USERNAME (it doesn't matter if you don't have one)
     // get curretn commit sha, command 'git rev-parse HEAD' return full sha
     // if you wanna push image to dockerhub, image name must be unique
     // GITCOMMITSHA = sh(returnStdout: true, script: "git log -n 1 --pretty=format:'%h'").trim()
     SERVICE_NAME = "shop-api"
     
     REGISTRY = "registry.cn-hangzhou.aliyuncs.com/hyper/${SERVICE_NAME}"

     // credencials database connectionstring
     StoreConnection = credentials('StoreConnection')
     IdentityConnection = credentials('IdentityConnection')
     GitHub = credentials('GitHub')
   //    withCredentials([string(credentialsId: 'StoreConnection', variable: 'StoreConnection')]) { //set SECRET with the credential content
        
   //  }
   //     withCredentials([string(credentialsId: 'IdentityConnection', variable: 'IdentityConnection')]) { //set SECRET with the credential content
        
   //  }
   
   }

   stages {
      // stage('Preparation') {
      //    steps {
      //       cleanWs()
      //       git credentialsId: 'GitHub', url: "https://github.com/839928622/build-image-with-kaniko.git"
      //       script {    
      //                GITCOMMITSHA = sh(returnStdout: true, script: "git log -n 1 --pretty=format:'%h'").trim()
      //       }

      //    }
      // }



      stage('Build') {

               steps {
        container("kaniko") {
           // pwd means find current working directory Dockerfile and build it 
           // Using single-quotes instead of double-quotes when referencing these sensitive environment variables prevents this type of leaking.
           sh('/kaniko/executor --context git://$GitHub_USR:$GitHub_PSW@github.com/839928622/build-image-with-kaniko.git --build-arg StoreConnection=$StoreConnection --build-arg IdentityConnection=$IdentityConnection --destination ${REGISTRY}:latest --destination ${REGISTRY}:${env.BRANCH_NAME.toLowerCase()}-${BUILD_NUMBER}')
        }
      }
      }

      stage('Deploy') {
          when { branch "master" }
          steps {
                  sh 'kubectl apply -f deploy.yaml'
                  sh 'kubectl set image deployments/shop-api shop-api=${REGISTRY}:${env.BRANCH_NAME.toLowerCase()}-${BUILD_NUMBER}'
                }
      }
   }
}
