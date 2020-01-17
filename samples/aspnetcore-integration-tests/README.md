# AspNetCore application with integration tests

### Overview

This is a sample AspNetCore application that does CRUD operations and stores its state in MongoDB. You can run it and navigate to `/swagger`. The application uses AspNetCore configuration system to obtain the connection string which by default is `mongodb://localhost:27017` so make sure that you run local instance of MongoDB. You can do that using Docker:

```
docker run --name some-mongo -d -p "27017:27017" mongo:latest
```

### Tests

Integration tests project consists of `EnvironmentFixture` class that setups tesing environment by creating MongoDB container and hosting sample API inside `TestHost`. It overrides the configuration so the API uses the connection string to MongoDB inside a just created container. There are two inherited fixture classes creating read and write environments which are used by read only tests and tests that change the state. You can run everything just as usual tests from Visual Studio IDE or from command line:

```
dotnet test
```

Please make sure that you run Docker. If you don't have MongoDB image locally it will automatically download it for you but it might take some time to run the tests very first time.

### Debug/Release

Although, MongoDB startup shouldn't take too much time you'll still might notice some extra time spending on containers creation and disposing. That's why `EnvironmentFixture` class contains the conditionally compiled code so each time when you run your tests using `Debug` configuration it will try to cleanup and reuse existing containers and not dispose them at the end. This allows to improve local testing expirience significanlty. You should still prefer to use `Release` configuration when you run your tests inside CI/CD.
