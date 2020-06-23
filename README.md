
# Check back for the CODE PUZZLE to be announced soon !!!







![.NET Core](https://github.com/alperhankendi/event-sourcing-demo/workflows/.NET%20Core/badge.svg)![.NET Core](https://github.com/alperhankendi/event-sourcing-demo/workflows/.NET%20Core/badge.svg?event=push)

presentation [here](https://www.slideshare.net/AlperHankendi1/building-time-machine-with-net-core) 

docker-compose.yaml
```
version: '3.5'

services:

  eventstore:
    container_name: demo-eventstore
    image: eventstore/eventstore
    ports:
        - 2113:2113
        - 1113:1113
    environment:
      - EVENTSTORE_EXT_HTTP_PORT=2113
      - EVENTSTORE_EXT_TCP_PORT=1113

  ravendb:
    container_name: demo-ravendb
    image: ravendb/ravendb
    ports:
      - 8080:8080
      - 38888:38888
    environment:
      - RAVEN_Security_UnsecuredAccessAllowed=PublicNetwork
      - RAVEN_ARGS="--Setup.Mode=None"

```

### Using Docker Compose

You can run the required infrastructure components by issuing a simple command:

```
$ docker-compose up
```

form your terminal command line, whilst being inside the repository rot directory.

It might be a good idea to run the services in detached mode, so you don't accidentally stop them. To do this, execute:

```
$ docker-compose up -d
```
## What to do
Start Docker Compose as described above while at home and check that the images are downloaded and everything starts properly. 
Check if EventStore and RavenDb respond via http by visiting the administration consoles:

- [EventStore](http://localhost:2113), user name is "admin" and password is "changeit"
- [RavenDb](http://localhost:8080)
