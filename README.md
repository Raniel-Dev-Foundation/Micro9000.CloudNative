# Backend services encounter errors docs

This is just a list of all errors I encountered during development


## Docker in Visual Studio: Unable to configure HTTPS endpoint

#### Create a new developer certificate (replace the old one)

To get rid of the old developer certificate use the following command:

    dotnet dev-certs https --clean

To generate and new one:

    dotnet dev-certs https -t -p "yourpassword"

_This command allows you to set a password and to trust the certificate_
**Don’t forget to restart visual studio after running this**

#### Link your certificate

In order to debug a docker container on HTTPS with Visual Studio you need to configure the password of the developer certificate. These things should be auto-configured for you when you generate your  **dockerfile** in visual studio, but here are the steps to do it manually:

    {
      "Kestrel:Certificates:Development:Password": "d12f4b8f-53a5-4190-af34-58e59c99cffe"
    }
Right click on your project and select -> manage user secrets.
Alternatively you may edit the **secrets.json** file in your appdata directory. To get there take a look at your projects’ csproj file.

Remember the UserSecretsId.

Now find the user userssecrets file by navigating to  **%appdata%/microsoft/usersecrets**.

Open the folder that matches your user secret GUID.
You can edit the secrets by modifying the **secrets.json** file. In this file you need to configure the password of the developer certificate:

# docker-compose commands

Detached mode
    
    docker-compose up -d

Build images for us if they do not already exists, once iamges exists, it will keep using those images
    
    docker-compose up

Only build the images

    docker-compose build

Build and then start all of the containers

    docker-compose up --build

This will force no caching of the image layers to will recreate each layer again. I’ve found this useful when testing a docker-compose file change to ensure all steps are working as expected.

    --no-cache

Stop all of the containers defined in the docker-compose file

    docker-compose stop

Cleanup the stopped containers

    docker-compose rm

Stop and then remove the containers

    docker-compose down