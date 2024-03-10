# Exercises

## Exercise 1: Docker Image Preparation
In this exercise, you will optimize your application's Docker image by organizing it into multiple layers. This involves separating dependencies (libraries) that rarely change into lower layers, while placing your application code in higher layers. You'll then set up continuous integration (CI) using GitHub Actions or another preferred service to automate the build process. This CI/CD pipeline should compile and package all code from the main branch, create a new Docker image with the appropriate tag, and publish the image to a Docker Hub repository.

## Kubernetes
Securely storing sensitive information like database connection passwords is crucial in Kubernetes. In this exercise, you'll learn how to manage secrets by storing the database connection password in a secret object within Kubernetes. This ensures that sensitive information is kept safe and can be accessed securely by your microservices.

## Exercise 2: Configuration Management
Effective configuration management is essential for modern microservice architectures. You'll explore concepts such as using environment variables for configuration and separating configuration from code. By defining configuration settings that can be easily changed without recompiling or reinstalling the microservice, you'll ensure flexibility and maintainability in your application.

## Exercise 3: Health Check API and Kubernetes Liveness Probes
Ensuring the health of your microservices is critical for maintaining system reliability and availability. You'll implement a Health Check API within your services to monitor their health status. Additionally, you'll leverage Kubernetes Liveness Probes to automatically detect and restart unhealthy service instances. This exercise will help you build robust, self-healing microservices that can recover from failures gracefully.

## Exercise 4: Metrics Collection
Collecting and analyzing metrics is essential for gaining insights into the performance and behavior of your microservices. You'll add endpoints to expose metrics such as request latency, error rates, and resource utilization. By utilizing tools like KumuluzEE Metrics or Datadog, you'll be able to collect and visualize these metrics, enabling you to monitor and optimize your Kubernetes environment effectively.

## Exercise 5: Kubernetes Logs
Logging is crucial for troubleshooting and debugging microservices deployed in Kubernetes. You'll explore Kubernetes logging mechanisms and implement centralized logging for your microservices. By configuring your microservices to send logs to a centralized logging system, such as logit.io, you'll be able to aggregate and analyze logs from multiple service instances, making it easier to identify and resolve issues.

## Exercise 6: API Documentation with OpenAPI
Documenting your REST APIs is essential for ensuring interoperability and facilitating collaboration between teams. You'll use OpenAPI specifications to document your APIs, including details such as endpoints, request/response formats, and authentication mechanisms. Additionally, you'll provide a graphical interface for exploring and interacting with your APIs, making it easier for developers to understand and use your services.

## External API Integration
Incorporating external APIs into your application can extend its functionality and provide access to valuable third-party services. You'll integrate an external API into your microservice application, leveraging tools like RapidAPI to discover and consume APIs. This exercise will give you hands-on experience with integrating external services and handling API requests and responses within your application.

## Advanced Communication Protocol (Optional)
Implementing asynchronous communication protocols can improve the scalability and resilience of your microservice architecture. In this optional exercise, you'll explore asynchronous REST calls and integrate them into your microservice application. By decoupling services and leveraging asynchronous messaging patterns, you'll be able to handle high loads and improve fault tolerance in your application.
Don't forget to prepare a simple user interface (web and/or mobile) for your application. The user interface should include a few simple screens that demonstrate the functionality of your application (e.g., uploading images, viewing all user images, displaying one image along with comments, etc.). The user interface should be straightforward, with a focus on showcasing the implemented functionalities of your microservice application.

## Exercise 7: Ingress

Configure an ingress proxy on Kubernetes so that all your microservices are publicly accessible through a single address. You can use the ingress installed on your cloud provider or install any ingress controller, such as NGINX Ingress Controller.

Useful resources:
- [Ingress Controllers](https://kubernetes.io/docs/concepts/services-networking/ingress-controllers/)
- [Ingress on Azure](https://docs.microsoft.com/en-us/azure/aks/ingress-basic)
- [NGINX Ingress Controller](https://kubernetes.github.io/ingress-nginx/)

## Exercise 8: Fault Isolation and Tolerance

Simulate an error in one of the calls between two microservices (e.g., the microservice registers at the wrong address or port, the microservice returns improperly formatted data, etc.). The goal of this task is to limit the impact of errors in individual microservices on the entire application.

You can use KumuluzEE Fault Tolerance to assist in implementing fault resilience. Prepare an appropriate fallback mechanism that will trigger in case of an error in the called microservice. You can use a timer (timeout) in implementing fault resilience. Implement a circuit breaker. Prepare a demonstration showing different states of the circuit breaker (open, half-open, closed). 

Simulate an error again in one of the microservices and observe the application's behavior.

## Exercise 9: Advanced Communication Protocol

Incorporate the GraphQL protocol into your microservice application.

## Optional Task (Exercise 9)

### Stream Processing Systems

Apache Kafka is a stream-processing platform that enables the processing, storing, creation, and reading of events. Support event-driven interaction in your project and sensibly incorporate a stream processing system that allows publishing and subscribing to event streams. Examples of events include IoT device sensor readings, orders, and transactions. Create a single topic, where the producer inserts records into the topic, and the consumer reads records from partitions. You can use KumuluzEE Event Streaming, which supports the Apache Kafka platform. This task earns additional points.

You can find resources on:
- [KumuluzEE Event Streaming](https://docs.kumuluz.com/event-streaming/master/)
- [Apache Kafka](https://kafka.apache.org/)

## Optional Task (Exercise 10)

### Advanced Communication Protocol

Incorporate the gRPC protocol into your micr
