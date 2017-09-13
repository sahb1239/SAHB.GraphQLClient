pipeline {
  agent {
	node {
	  label 'windows_dotnetcore_1.0'
	}
  }
  environment {
	NUGET_URL = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
  }
  stages {
	stage("Bootstrap") {
	  steps {
		script {
		  if (isUnix()) {
			sh "git submodule update --init"
		  }
		  else {
			bat "git submodule update --init"
		  }
		}
	  }
	}
	stage("Build") {
	  steps {
		script {
	      if (isUnix()) {
			dir('build') {
			  sh "./build.sh --target Build"
			}
		  }
		  else {
			dir('build') {
			  bat "powershell -ExecutionPolicy Bypass -File Build.ps1 -target Build"
			}
		  }
		
		  archive "src/**/bin/**/*.nupkg"
		  archive "releasenotes.md"
		}
	  }
	}
	stage("Tests") {
	  steps {
		script {
	    if (isUnix()) {		  
		  dir('build') {
			sh "./build.sh --target Test-CI"
		  }
		}
		else {
		  dir('build') {
			bat "powershell -ExecutionPolicy Bypass -File Build.ps1 -target Test-CI"
		  }
		}
		}
	  }
	}
  }
}