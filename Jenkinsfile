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
     GITCOMMITSHA = sh(returnStdout: true, script: "git log -n 1 --pretty=format:'%h'").trim()
     SERVICE_NAME = "shop-api"
     
     REGISTRY = "registry.cn-hangzhou.aliyuncs.com/hyper/${SERVICE_NAME}"

     // credencials database connectionstring
     StoreConnection = credentials('StoreConnection')
     IdentityConnection = credentials('IdentityConnection')
   //    withCredentials([string(credentialsId: 'StoreConnection', variable: 'StoreConnection')]) { //set SECRET with the credential content
        
   //  }
   //     withCredentials([string(credentialsId: 'IdentityConnection', variable: 'IdentityConnection')]) { //set SECRET with the credential content
        
   //  }
   
   }

   stages {
      // stage('Preparation') {
      //    steps {
      //       cleanWs()
      //       git credentialsId: 'GitHub', url: "https://github.com/${ORGANIZATION_NAME}/${SERVICE_NAME}"
      //    }
      // }

         stage('clean up') {
         steps {
            cleanWs()
         }
      }

      stage('Build') {
         // steps {
         //     sh 'echo current git commit is ${GITCOMMITSHA}'
         //     sh 'docker image build  -t ${SERVICE_NAME}:latest -t ${SERVICE_NAME}:${GITCOMMITSHA} .'
           
         // }

               steps {
        container("kaniko") {
           // pwd means find current working directory Dockerfile and build it 
          sh "/kaniko/executor --context `pwd` --build-arg StoreConnection=${StoreConnection} --build-arg IdentityConnection=${IdentityConnection} --destination ${REGISTRY}:latest --destination ${REGISTRY}:${env.BRANCH_NAME.toLowerCase()}-${GITCOMMITSHA}"
        }
      }
      }

      stage('Deploy') {
          when { branch "master" }
          steps {
                  sh 'kubectl apply -f deploy.yaml'
                  sh 'kubectl set image deployments/shop-api shop-api=${REGISTRY}:${env.BRANCH_NAME.toLowerCase()}-${GITCOMMITSHA}'
                }
      }
   }
}
