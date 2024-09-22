pipeline {
    agent any
    stages {
        stage('Build') {
            steps {
                echo "The process by which the code is compiled, built, and packaged in a deployable format. It enters into a process of resolving all dependencies to have the application in a deployable state."
                echo 'Maven – A popular build automation tool for Java projects that performs project dependency management, the build of your application, and packages the code according to the output format set.'
            }
        }
        stage('Test') {
            steps {
                echo 'The process of unit tests by which done in isolation from others to verify the functionality of the constituent units or components of software, and ensure that defects are identified and corrected at an early development cycle. JUnit – Open-source framework that acts as a unit testing tool in Java programming.'
                echo 'The process of integration tests by which done to ensure that different software components actually inter-operate together and check that interfaces work between modules. Katalon Platform – An all-inclusive integration testing tool that is widely used for web, mobile, desktop, and API testing.'
            }
            post {
                success {
                    mail to: "nnbson98@gmail.com",
                    subject: "Test Status Email",
                    body: "Test was successful!"
                }
                failure {
                    mail to: "nnbson98@gmail.com",
                    subject: "Test Status Email",
                    body: "Test failed!"
                }
            }
        }
        stage('Code Analysis') {
            steps {
                echo 'The process by which analyzing the code to ensure it conforms to industry norms related to quality, maintainability, and efficiency. It helps to identify potential quality issues and security risks.'
                echo 'SonarQube – An open-source platform that continuously inspects the quality of code, providing detail in duplications, complexity, potential bugs, and other code quality metrics. It supports more than 30 programming languages and has many plugins available for augmenting functionality.'
            }
        }
        stage('Security Scan') {
            steps {
                echo 'The process by which using for security testing to ensure there are no vulnerabilities and no possible risks in a software application that could be exploited by malicious attackers. Generally, it includes Vulnerability Scanning, Penetration Testing, Risk Assessment, Security Auditing, and Authentication and Authorization Testing.'
                echo 'OpenVAS – The well-known open-source tool detects, assesses, and reports on various security vulnerabilities.'
            }
            post {
                success {
                    mail to: "nnbson98@gmail.com",
                    subject: "Security Scan Status Email",
                    body: "Security Scan was successful!"
                }
                failure {
                    mail to: "nnbson98@gmail.com",
                    subject: "Security Scan Status Email",
                    body: "Security Scan failed!"
                }
            }
        }
        stage('Deploy to Staging') {
            steps {
                echo 'The process by which deploying the application to a staging environment as close as possible to the production environment, allowing testing of the application in conditions as close as possible to those in production before a final deployment.'
                echo 'AWS EC2 (Amazon Web Services Elastic Compute Cloud) – Offers scalable virtual servers in the cloud, giving the developer a flexible and reliable environment to simulate production-like conditions for testing purposes.'
            }
        }
        stage('Integration Tests on Staging') {
            steps {
                echo "The process by which running integration testing on staging to ensure that the application is working with expected results under near-production conditions. This is the stage that will point out issues that may not be visible within the development environment."
                echo 'Selenium – It is one of the most popular open-source tools used for web application testing. Testing can be automated on different browsers and operating systems.'
            }
        }
        stage('Deploy to Production') {
            steps {
                echo "The process by which the application is deployed to the production environment and is accessible to its end-users. This phase is actually the transition of the application from the test environment to a live environment, which can be accessed by the public."
                echo 'Docker – An open-source platform for developers to build, package, deploy, and execute applications in isolated containers. It simplifies the development and deployment of distributed applications while maintaining consistency and scalability across varied environments.'
            }
            post {
                success {
                    mail to: "nnbson98@gmail.com",
                    subject: "Deploy to Production Status Email",
                    body: "Deploy to Production was successful!"
                }
                failure {
                    mail to: "nnbson98@gmail.com",
                    subject: "Deploy to Production Status Email",
                    body: "Deploy to Production failed!"
                }
            }
        }
    }
}
